# 03 — Exception Policy

**Priority P0.** No swallowed exceptions is a stated rule for this control.

## Current behaviour

### Four bare catches

```csharp
// Helpers/TabFontHelpers.cs
catch { return DpiScalingHelper.ScaleValue(16, ownerControl); }              // line 47
catch { return TextRenderer.MeasureText(text, SystemFonts.DefaultFont).Width; } // line 56
catch { return false; }                                                      // line 66

// Hosts/BeepTabHeaderHost.Touch.cs
catch { return minWidth; }                                                   // line 70
```

Three of the four are in the code that **measures text**. If font resolution or DPI scaling throws,
the tab silently sizes to a hard-coded 16px or measures with `SystemFonts.DefaultFont` instead of the
themed font — so tabs come out the wrong size, labels clip, and nothing anywhere records why. That is
strictly worse than crashing: the failure becomes a rendering mystery.

`catch { return false; }` is worse still — the caller cannot distinguish "no" from "the question
could not be answered".

### Errors reported only to the debugger

`BeepTabs.Actions.ReportError` stores `_lastError` and writes to `Debug.WriteLine`. In a Release
build `Debug.WriteLine` compiles away, so the only trace is a private field.

**RESOLVED.** `BeepTabs` now raises a public `TabError` event carrying `BeepTabErrorEventArgs`
(context + exception), in every build configuration. It is a diagnostic channel, not error handling:
the failing operation still throws. Handlers must not throw, and the XML doc says why — the event is
raised from inside a `catch` that is about to rethrow, so a throwing handler would replace the
original failure with its own.

**The `void` operations were lying — RESOLVED.** `AddPage`, `ClearPages` and `InsertPageAt` reported
a failure and then returned normally, and `RemovePage`/`MovePage` returned `false`, which is also the
legitimate answer for "no such page" — so a caller could not distinguish "not found" from "threw".
All five now report and rethrow, matching `CreatePage`, which already did.

This mattered more than it looks at design time: `BeepTabsDesigner.ExecuteTabsAction` wraps every
call in a `DesignerTransaction` with its own catch. Because `AddPage` swallowed the exception, the
transaction **committed as though the page had been added** and the designer displayed nothing. It
now cancels the transaction and shows the error.

Six `catch (Exception ex)` blocks in `BeepTabs.HostedContent` call it and then continue:

| Method | Behaviour after failure |
|---|---|
| `CreatePage` | reports, then **rethrows** — correct |
| `AddPage` | reports, returns `void` — caller believes the page was added |
| `RemovePage` | reports, returns `false` — caller can detect |
| `ClearPages` | reports, returns `void` — caller believes pages were cleared |
| `MovePage` | reports, returns `false` — caller can detect |
| `InsertPageAt` | reports, returns `void` — caller believes insertion happened |

`AddPage`, `ClearPages` and `InsertPageAt` are the problem: they are `void`, so a failure is
indistinguishable from success at the call site, and the control is left in a state the caller does
not know about.

## What mature controls do

- Catch only what can genuinely be handled at that level; let programming errors propagate.
- Never let an operation report success it did not achieve — either return a result the caller must
  observe, or throw.
- Surface failures through a real diagnostic channel (an event, a logger, a tracing hook) that exists
  in Release, not only under a debugger.

## Work

1. **Remove all four bare catches.** For the font/measure paths, either the operation cannot throw —
   in which case the `try` goes away entirely — or the exception is a real failure and must surface.
   If a fallback is genuinely wanted for robustness, it must be a *narrow* catch of the specific
   exception with the reason recorded, not `catch { }`.
2. **Make the void operations honest.** `AddPage`, `ClearPages` and `InsertPageAt` either return a
   result the caller can check or propagate the exception. A method that "fails silently and keeps
   going" cannot be built on.
3. **Give `ReportError` a real channel.** A public `TabError` event (or an injected logger) so hosts
   can log, telemeter or surface failures. Keep `Debug.WriteLine` as an addition, not the mechanism.
4. **Audit `_lastError`'s presentation.** `ReportError` calls `Invalidate()`, implying the error is
   painted somewhere. Confirm it is, and that it clears — an error banner that never clears is its
   own defect.
5. **Sweep the rest of the folder** for `catch` blocks added later; the harness should count bare
   catches and fail if any exist.

## Verification

- Static check in the harness: **zero** bare `catch` blocks under `Tabs/`, failing the run if one
  appears.
- Inject a failing font resolution and assert the failure surfaces (event raised / exception
  propagated) rather than producing a silently mis-sized tab.
- Force `AddPage` to fail and assert the caller can detect it.
