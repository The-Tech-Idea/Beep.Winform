# BeepWait — Design Skill Audit

**Date:** 2026-07-07 | **Findings:** 5

## Issues

| # | Severity | File:Line | Current | Fix |
|---|---|---|---|---|
| W1 | Medium | `BeepWait.cs:98` | No `DefaultSize` override | Add `DefaultSize => BeepLayoutMetrics.Notification` |
| W2 | Low | `Designer.cs:1058` | `ClientSize = (601,400)` — not DPI-scaled | Apply `BeepLayoutMetrics.Notification.ScaleSize(this)` in ctor after InitializeComponent |
| W3 | Low | `Designer.cs:864-1053` | Orphan `beepImage1` — never referenced in code-behind, at Location(100,100) overlapping spinner | Remove from Designer or wire to code-behind |
| W4 | Low | `BeepWait.cs:307,495` | `Task.Delay(2000)` magic number | Extract to named constant `CloseDelayMs` |
| W5 | Info | Code-behind | Clean: `ApplyTheme()` correctly uses `_currentTheme` keys; `SafeInvoke` properly marshals to UI thread; `using`/`Dispose` pattern correct | No change needed |

## Verification
- ✅ All controls in Designer.cs
- ✅ `Anchor` on message text box for responsive resizing
- ✅ `Dock=Top` on Title label
- ✅ Theme applied correctly via `BeepThemesManager`
- ✅ Thread-safe with SynchronizationContext pattern
