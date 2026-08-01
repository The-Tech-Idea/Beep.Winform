# 08 — Notifications & Toasts

**Priority P2. Phase 5.** Not yet audited — this document states what to establish, not conclusions.

## What exists

`BeepDialogManager.Notifications.cs` (503 lines) provides `Banner`, `ToastLoading`, `Toast`,
`ToastSuccess`, `ToastDeduped`, `NotifyFeaturePending` and a `NotificationPolicy` model.

`NOTIFICATIONS.md` sits in the directory root and predates this program. I approached it as
stale-until-proven, on the basis of the nine stale documents deleted at the start of the `BeepTabs`
program — **and that suspicion was wrong**: every method it documents exists in the code with a
matching signature. It is a usage cookbook, not a design document, and it should be kept and updated
alongside this feature rather than deleted.

Worth noting how it was found: the notification API is *larger* than the public-surface listing in
this program's README, because the grep that built that listing matched only certain signature
shapes. `Toast`, `ToastSuccess`, `ToastDeduped` and `NotifyFeaturePending` are all public and were
missed by it. Any completeness claim about the API surface in these documents should be re-derived
by reflection, not by grep.

Two facts already established elsewhere in this program:

- `BeepDialogManager.Notifications.cs:498` contains `public void Dispose() { }` — see
  [04](04-dead-scaffolding.md).
- Toasts and dialogs are different surfaces with different rules, and this file sits inside the
  *dialog* manager. Whether that is the right home is an open question, not a defect yet.

## What the reference products do

| System | Model |
|---|---|
| Sonner / react-hot-toast | a queue with a max visible count; newest replaces oldest; each toast is dismissible and hoverable-to-persist |
| Material 3 snackbar | one at a time, bottom-anchored, single optional action, auto-dismiss with a documented minimum |
| Fluent 2 / Ant Design | stacked, positioned, with a promise-driven loading→success→error transition |
| VS Code notifications | severity-ordered, collapsible, with a notification centre for history |

The behaviours that separate a real implementation from a wrapper: **queueing and a max visible
count**, **hover-to-persist** (WCAG 1.4.13), **auto-dismiss timing tied to message length**, and a
**loading → resolved transition** that does not flash.

## To establish

1. Does `Banner`/`ToastLoading` queue, or can toasts overlap?
2. Is there a max visible count, and what happens past it?
3. Does hovering a toast pause its dismissal timer? (WCAG 1.4.13 — the `ToolTips` program had to
   implement dismissible/hoverable/persistent explicitly.)
4. Does `ToastLoading` transition to success/error, or only disappear?
5. Is `NotificationPolicy` read anywhere, or is it another declared-and-never-read surface?
6. Does `NOTIFICATIONS.md` describe the code as it is?

## Verification

- ⬜ Probe: raise 10 toasts and assert the visible count respects the policy.
- ⬜ Probe: hover a toast and assert its timer pauses.
- ⬜ Harness: every `NotificationPolicy` property is read somewhere.

---

## Outcome

Four of this plan's five suspicions were not supported by the code. Recorded rather than acted on,
because the suspicion list was drawn from patterns found elsewhere in the codebase and applying it
here would have meant changing things that were already right:

| Plan question | Finding |
|---|---|
| Toast queueing missing? | Present — `_notificationQueue`, max 5 visible |
| Hover-to-persist (WCAG 1.4.13) missing? | Already default: `PauseOnHover = true`, wired `MouseEnter`/`Leave` -> `Pause`/`Resume` |
| `NotificationPolicy` has dead surface? | None — all 13 properties are read |
| `NOTIFICATIONS.md` stale? | Accurate |

### The one real defect, fixed

`Banner` returned a no-op `EmptyDisposable`, so `using (dm.Banner(...))` read as scoped and was not —
while its sibling `ToastLoading` returned a handle whose disposal genuinely dismissed. Two methods
with the same signature and opposite behaviour, indistinguishable at the call site. `Banner` now
returns `BannerHandle`.

**Stated limitation:** it dismisses by notification *type*, matching the existing `LoadingToastHandle`
precedent, because `BeepNotificationManager.Show` returns `void` and there is no handle to the
instance it created. Dismissing exactly one banner requires `Show` to return the notification — a
signature change in the Notifications subsystem, deliberately not made here.
