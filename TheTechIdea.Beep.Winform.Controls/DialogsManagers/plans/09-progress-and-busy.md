# 09 — Progress & Busy

**Priority P2. Phase 4.** Not yet audited.

## What exists

`BeepDialogManager.Progress.cs` (564 lines) provides `ShowProgress`, `ShowIndeterminate`,
`ShowBusy`, `BusyOverlay`, `RunWithProgressAsync`, `RunWithBusyAsync` and an `IProgressDialogHandle`
with a `ProgressHandle` implementation.

The async wrappers are the interesting part: `RunWithProgressAsync` and `RunWithBusyAsync` take a
delegate and own the dialog's lifetime around it. That is the right shape — it is the pattern that
makes it impossible to leak a progress dialog by forgetting to close it.

## Audited - the design is sound

Unlike the rest of this directory, progress and busy needed almost nothing. Four of the plan's five
concerns were already handled correctly:

| Concern | Finding |
|---|---|
| Does cancel reach the work? | **Yes.** A real `CancellationTokenSource`; the handle exposes `CancellationToken` and `IsCancellationRequested`; the button calls `Cancel()`, shows "Cancelling..." and disables itself |
| Exception path | **Correct.** `using var progress` closes the dialog even on a throw, and the exception is rethrown rather than swallowed |
| Thread affinity | **Correct.** Every update path - `UpdateProgress`, `UpdateStatus`, `SetIndeterminate`, `SetDeterminate`, `Complete` - checks `InvokeRequired` and marshals via `BeginInvoke` |
| `OperationCanceledException` | Distinguished from a fault and rethrown without being reported as an error |

**One asymmetry found and fixed.** `RunWithProgressAsync<T>` returned the operation's result without
calling `progress.Complete()`, while the non-generic overload called it. The two overloads therefore
ended a successful run in different states, and anything `Complete()` does beyond the visual -
settling the bar at 100%, stopping the indeterminate animation - was skipped for every caller that
happened to need a return value.

Recording this as audited-and-sound matters as much as recording a defect: it is the only feature in
this program that was already right.

## Still to establish

1. **Cancellation.** `ShowProgress(..., cancellable: true)` — does cancelling actually signal the
   operation, or only close the dialog? A cancel that does not reach the work is worse than no cancel
   button, because the user believes the work stopped.
2. **Exception paths.** If the delegate passed to `RunWithProgressAsync` throws, is the dialog closed
   before the exception propagates? A modal left open over a faulted operation locks the app.
3. **Thread affinity.** Progress reported from a background thread must marshal to the UI thread.
   Check `Invoke`/`BeginInvoke` on the handle's `Report` path.
4. **Double-dispose and double-complete.** `ProgressHandle.Complete()` then `Dispose()` — safe?
5. **Nested busy.** Two overlapping `ShowBusy` calls — does the first `Dispose` tear down the overlay
   the second still needs? This is a refcount question and a common bug in this pattern.

## What the reference products do

- **Material 3 / Fluent** — indeterminate for unknown duration, determinate only when a real
  fraction is available; never a fake progress bar.
- **VS Code `withProgress`** — scoped to a delegate, cancellable through a `CancellationToken` that
  the operation is *given*, and the notification disappears when the task completes or faults.

`RunWithProgressAsync` already matches the VS Code shape. The question is whether cancellation is
wired through to the caller's work or stops at the dialog.

## Verification

- ⬜ Probe: cancel a `cancellable: true` progress dialog and assert the operation observes it.
- ⬜ Probe: throw from inside `RunWithProgressAsync` and assert the dialog closes and the exception
  reaches the caller — not swallowed (ground rule 3).
- ⬜ Probe: report progress from a background thread without an `InvalidOperationException`.
- ⬜ Probe: nested `ShowBusy` — inner dispose must not tear down the outer overlay.
