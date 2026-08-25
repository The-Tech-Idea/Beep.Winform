# BeepMenuBar

A commercial-grade menu bar control (`BeepMenuBar : BaseControl`) that renders a
list of top-level `SimpleItem`s, opens popup submenus through `ContextMenuManager`,
and supports keyboard activation (Alt, Alt+letter, arrows, Esc), accessibility
(UIA/MSAA accessible-object tree), high-contrast mode, and method-bound menu items.

The control was split into focused partials during Phase 02:

| File | Responsibility |
|---|---|
| `BeepMenuBar.cs` | Class header, constructor, default sizing |
| `BeepMenuBar.Properties.cs` | Public properties + shared fields |
| `BeepMenuBar.Layout.cs` | Hit areas, rect math, font-height probes, `GetPreferredSize` |
| `BeepMenuBar.Drawing.cs` | `DrawContent` + per-item paint pipeline (BeepStyling chrome) |
| `BeepMenuBar.Input.cs` | `OnMouse*` + `HandleMenuItemClick` dispatcher |
| `BeepMenuBar.Popup.cs` | Cool-down + toggle + `ShowMenuItemPopup` (blocking and non-blocking) |
| `BeepMenuBar.Keyboard.cs` | `ProcessCmdKey` / `ProcessMnemonic` — Alt/Esc/arrow handling |
| `BeepMenuBar.Activation.cs` | Keyboard-activation state machine (`MenubarActivation`) |
| `BeepMenuBar.Accessibility.cs` | Accessible-object tree + UIA event helpers |
| `BeepMenuBar.HighContrast.cs` | High-contrast palette + `SystemEvents` subscription |
| `BeepMenuBar.Theming.cs` | `ApplyTheme` |
| `BeepMenuBar.Lifecycle.cs` | Resize / parent / `SafeInvoke` / `Dispose` |
| `BeepMenuBar.Utility.cs` | `LoadSampleMenuItems` + method-bound item dispatch |

Helpers (`Helpers/`) cover fonts, icons, theme colours, and the submenu triangle
tracker; models/enums live in `ModelsandEnums/`.

## Method-bound menu items

A `SimpleItem.MethodName` names a global function to run when the item is selected.
The menubar does not resolve the method itself — it dispatches through the
host-wired `RunFunctionFromExtensionsHandler` delegate:

```csharp
menuBar.RunFunctionFromExtensionsHandler = (item, methodName) =>
    MyApp.RunMethod(methodName);
```

When no handler is wired, a method-bound item reports `Errors.Failed` via the
returned `IErrorsInfo` rather than silently succeeding.

## Recent Fixes (2026-08)

- **Swallowed exceptions reported** — all 20 bare `catch { }` sites across the
  partials now capture the exception and report through `BeepLog` (`Fallback` for
  degraded/defensive paths, `FallbackOnce` for per-paint/per-keystroke paths,
  `Failure` before rethrow).
- **Error reporting moved off `Debug.WriteLine`** — the two `catch (Exception)` sites
  in `Drawing.cs` and `Theming.cs` that wrote to `System.Diagnostics.Debug` now route
  through `BeepLog` (paint fallback via `FallbackOnce`, font failure via `Fallback`).
- **GDI disposal in the paint fallback** — `DrawMenuItemFallback`'s `SolidBrush` and
  `Pen` now use `using` so a draw failure cannot leak them.
- **Method dispatch wired (double-fire removed)** — `RunMethodFromGlobalFunctions`
  no longer re-raises `OnMenuItemSelected` (the three call sites already raise it,
  so selection fired twice whenever an item carried a `MethodName`). It now delegates
  to `RunFunctionFromExtensionsHandler`, making `MethodName` a real consumer instead
  of an accepted-then-ignored parameter, and reports failures through `BeepLog` plus
  the returned `IErrorsInfo`.
