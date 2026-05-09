# Changelog — TheTechIdea.Beep.Winform.Controls

All notable changes to this library are recorded here.
Format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

---

## [Unreleased]

### Added — BeepTabs (Phases 3–5)

#### Phase 3: Mode System + Workspace Commands
- `BeepTabMode` enum (`Navigation`, `Documents`, `Workspace`) drives per-mode policy across keyboard, mouse, context menu, and close guards.
- `BeepTabWorkspaceState` sealed data bag: `IsDirty`, `IsPinned`, `IsPreview`, `ReusePreviewSlot`, `GroupKey`. Forwarded as properties on `BeepTabItem`.
- `BeepTabCommandRouter` centralises `CloseTab`, `CloseOthers`, `CloseAll`, `PinTab`, `MarkDirty`, `PromotePreview`, `ReopenLast` commands behind a single dispatch surface.
- `BeepTabContextMenu` builds a mode-aware context-menu at runtime; suppresses destructive items (Close/Pin) in Navigation mode.
- `BeepTabKeyboardRouter` maps `Ctrl+W`, `Ctrl+Tab`, `Ctrl+Shift+Tab`, `Ctrl+1–9`, `Ctrl+Shift+T` per mode; delegates to `BeepTabCommandRouter`.
- `BeepTabMruTracker` records the MRU access sequence; `BeepTabQuickSwitcher` shows a popup list (`Ctrl+\``) for fast tab switching.
- `BeepTabClosedHistory` (partial `BeepTabs.ClosedTabHistory.cs`) tracks a bounded ring of closed tabs; exposes `MaxClosedHistory` designer property (`[DefaultValue(10)]`).
- Dirty-close guard in Documents/Workspace modes: `TabCloseRequested` event carries `BeepTabCloseRequestedEventArgs { Page, Metadata, Cancel }`. Callers set `e.Cancel = true` to abort.
- Preview-slot promotion: when `ReusePreviewSlot = true`, the next opened tab occupies the current preview slot instead of appending.

#### Phase 4: Accessibility, RTL, High-Contrast, Touch, Design-Time
- `BeepTabAccessibilityContract` ensures every `BeepTabHeaderItemLayout` exposes an `AccessibleObject` with name, role, state, and bounds. `SyncAccessibility()` updates the host's child-accessible list after each snapshot rebuild.
- `BeepTabInputPolicy` sealed class: evaluated once per interaction via `BeepTabInputPolicy.For(item, mode)`; encapsulates `CanClose`, `CanPin`, `CanDrag`, `CanReorder`, `CanFocus`, `AcceptsPointerDown` per item per mode.
- `BeepTabRtlLayoutHelper` static class: `MirrorSnapshot(snapshot, containerWidth)` mirrors all bounds (body, text, close button, icon, badge, dirty marker, busy indicator) for `RightToLeft.Yes`. `FlipPoint(pt, width)` mirrors mouse hit-test coordinates so all existing hit-test code remains unchanged.
- `BeepTabHeaderHost.HighContrast.cs` partial: paints tabs using `SystemColors` when `SystemInformation.HighContrast` is active; short-circuited in `OnPaint` before the standard rendering path runs.
- `BeepTabHeaderHost.Touch.cs` partial: enforces `MinTouchTargetWidth`/`MinTouchTargetHeight` per `BeepTabRenderContext`; expands hit-test zones without visually enlarging tabs.
- `BeepTabsDesigner` (`Design.Server/Designers/`): `DesignerActionList` smart-tag with **Behavior** section (`ShowCloseButtons`, `HeaderOverflowPolicy`, `MaxClosedHistory`) and **Mode Presets** section (Apply Navigation / Documents / Workspace preset in one click).

#### Phase 5: Release Readiness
- Public API additions on `BeepTabs` (`BeepTabs.HostedContent.cs`):
  - `AddTab(TabPage page, bool select = false)` — public wrapper over internal `AddHostedSourcePage`.
  - `RemoveTab(TabPage page)` — public wrapper over internal `RemoveHostedSourcePage`.
  - `GetTabAt(int index)` — public wrapper over internal `GetHostedSourcePageAt`.
  - `SelectedTab` (`TabPage?`) — read-only property returning the currently selected hosted page.
- All helper and model classes confirmed sealed or static: `BeepTabWorkspaceState`, `BeepTabInputPolicy`, `BeepTabHeaderLayoutSnapshot`, `BeepTabHeaderMetricsCache`, `BeepTabRtlLayoutHelper`, `BeepTabLayoutHelper`.
- `BeepTabLayoutHelper.CreateSnapshot` allocation improvements:
  - Replaced `items.Where(tab => tab.IsVisible)` LINQ chain with a plain `foreach`+`if` guard — eliminates one enumerator object per snapshot rebuild.
  - Pre-sizes `snapshot.Items.Capacity` from `tabRects.Count` — avoids backing-array reallocations when the list grows.
  - Removed unused `System.Linq` import.

### Added — Sample Project (`Beep.Sample.Winform.Features`)
- `BeepTabsDemoForm`: standalone `Form` covering all three tab modes; wired into `ControlGalleryView` via "Open Full Demo..." button when BeepTabs is selected.
- `REGRESSION_CHECKLIST.md`: 55 checkboxes across 8 groups covering Navigation mode, Documents mode, Workspace mode, High-Contrast/Accessibility, RTL Layout, Touch Targets, Input Policy, Design-Time, and Demo Form behaviour.

---

## Notes
- No breaking changes to existing `BeepTabs` public surface; all additions are new members.
- `BeepTabsRuntimeBridge` internal `n` property (layout snapshot) is cached on the host; snapshot rebuilds are triggered only by explicit `SyncHeaderSnapshot()` calls, not on every paint.
