# BeepComboBox Dropdown Enhancement - Master Plan

## Overview

This plan addresses critical sizing, positioning, and behavioral issues in the BeepComboBox dropdown popup to match commercial-grade controls like DevExpress, Telerik, and native WinForms ComboBox.

## Current Issues Summary

1. **Popup sizing incorrect** - Height calculation ignores borders, padding, search/footer heights. No `IntegralHeight` support.
2. **Popup positioning broken** - Placement helper computes proper location but result is discarded. No screen edge detection.
3. **Page Up/Down hardcoded** - Always moves by 6 items instead of calculating from viewport.
4. **Content width mismatch** - Doesn't account for scrollbar or auto-size to content.
5. **Scrollbar math incorrect** - LargeChange calculation is wrong.
6. **Animation sluggish** - 150ms fade feels slow.

## Phase Documents

| Phase | Document | Priority | Status |
|-------|----------|----------|--------|
| 1 | [01-popup-sizing.md](01-popup-sizing.md) | High | Pending |
| 2 | [02-popup-positioning.md](02-popup-positioning.md) | High | Pending |
| 3 | [03-page-navigation.md](03-page-navigation.md) | High | Pending |
| 4 | [04-content-width.md](04-content-width.md) | Medium | Pending |
| 5 | [05-scrollbar-fixes.md](05-scrollbar-fixes.md) | Medium | Pending |
| 6 | [06-animation-polish.md](06-animation-polish.md) | Low | Pending |

## Implementation Order

1. Phase 1: Fix sizing (most visible issue)
2. Phase 2: Fix positioning (prevents off-screen popups)
3. Phase 3: Fix Page Up/Down (keyboard navigation)
4. Phase 5: Fix scrollbar (related to sizing)
5. Phase 4: Content width (nice to have)
6. Phase 6: Polish (animation, focus)

## Files to Modify

- `ComboBoxes/Popup/ComboBoxPopupHostForm.cs`
- `ComboBoxes/Popup/ComboBoxPopupPlacementHelper.cs`
- `ComboBoxes/Popup/ComboBoxPopupContent.cs`
- `ComboBoxes/Popup/ComboBoxPopupHostProfile.cs`
- `ComboBoxes/Popup/ComboBoxListBoxPopupContent.cs`
- `Forms/BeepPopupForm.cs`
- `ComboBoxes/BeepComboBox.Properties.cs`

## Research & References

### Commercial Documentation
- [DevExpress PopupFormSize](https://docs.devexpress.com/WindowsForms/DevExpress.XtraEditors.Repository.RepositoryItemPopupBase.PopupFormSize)
- [DevExpress DropDownItemHeight](https://docs.devexpress.com/WindowsForms/DevExpress.XtraEditors.Repository.RepositoryItemComboBox.DropDownItemHeight)
- [WinForms ComboBox.IntegralHeight](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox.integralheight)
- [WinForms ComboBox.DropDownHeight](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox.dropdownheight)
- [WinForms ComboBox.DropDownWidth](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox.dropdownwidth)

### GitHub Implementations Studied

1. **EWSoftware/ListControls** ([BaseComboBox.cs](https://github.com/EWSoftware/ListControls/blob/master/Source/BaseComboBox.cs))
   - Uses `MaxDropDownItems` for page up/down size (not hardcoded)
   - Proper `CommitSelection` pattern with `SelectionChangeCommitted`/`SelectionChangeCanceled` events
   - `ToolStripDropDown`-like approach with `IDropDown` interface
   - **Key pattern:** `PageUp`/`PageDown` use `maxDropDownItems` value

2. **sgissinger/CheckBoxComboBox** ([Popup.cs](https://github.com/sgissinger/CheckBoxComboBox/blob/master/CheckBoxComboBox/Popup.cs))
   - Inherits from `ToolStripDropDown` (not Form)
   - Auto-flips when not enough space below screen
   - Uses `Screen.FromControl(control).WorkingArea` for positioning
   - Fade animation via `Opacity` property
   - **Key pattern:** `Show(Control, Rectangle)` method handles screen edge detection

3. **dotnet/winforms** ([ComboBox.cs](https://github.com/dotnet/winforms/blob/main/src/System.Windows.Forms/System/Windows/Forms/Controls/ComboBox/ComboBox.cs))
   - Official WinForms ComboBox source
   - `IntegralHeight` property with `RecreateHandle()` on change
   - `DropDownHeight` sets `IntegralHeight = false` automatically
   - `MeasureItem` event for owner-draw variable height
   - **Key pattern:** `DefaultDropDownHeight = 106` pixels

4. **CodeProject Custom ComboBox**
   - Uses `ToolStripDropDown` with `ToolStripControlHost`
   - Calculates dropdown height: `min(MaxDropDownItems, itemCount) * itemHeight + 3`
   - **Key pattern:** Height calculation accounts for border (3px)
