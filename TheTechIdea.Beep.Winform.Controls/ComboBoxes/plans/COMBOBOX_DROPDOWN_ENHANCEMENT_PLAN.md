# BeepComboBox Dropdown Enhancement Plan

## Executive Summary

The BeepComboBox dropdown popup suffers from several sizing, positioning, and behavioral issues that degrade the user experience compared to commercial products like DevExpress, Telerik, and native WinForms ComboBox. This plan outlines a comprehensive set of fixes and enhancements.

---

## Issues Identified

### 1. Popup Sizing Problems

**Issue:** The dropdown height is calculated incorrectly, causing:
- Popup too tall or too short for the item count
- Single-item popups showing excessive whitespace or clipped content
- No concept of `IntegralHeight` (showing only whole items)
- Borders and padding not accounted for in size calculations

**Root Cause:**
```csharp
// ComboBoxPopupHostForm.cs:188
int contentHeight = count * profile.BaseRowHeight;
```

This simplistic calculation ignores:
- Popup form borders and chrome
- Content panel padding (`ListHorizontalPadding`, `ListVerticalPadding`)
- Search box height when visible
- Footer height when visible
- The fact that partial items should not be shown (IntegralHeight)

**Expected Behavior (DevExpress/native):**
- Dropdown shows only whole items (no partial item at bottom)
- Height is exactly: `min(MaxHeight, itemCount * itemHeight + searchHeight + footerHeight + padding)`
- For single item: shows exactly one row with minimal padding
- Respects `DropDownRows` or `MaxDropDownItems` property

### 2. Popup Positioning Problems

**Issue:** The popup often appears in the wrong location or is clipped by screen edges.

**Root Cause:**
```csharp
// ComboBoxPopupHostForm.cs:88
_form.ShowPopup(owner, BeepPopupFormPosition.Bottom, _form.Width, _form.Height);

// BeepPopupForm.cs:456
Point location = CalculatePopupLocation(triggerControl, position);
// CalculatePopupLocation simply does: triggerScreenLocation.Y + triggerSize.Height
```

The `ComboBoxPopupPlacementHelper.Calculate` computes proper screen-aware placement, but the result is **never used**. `ShowPopup` recalculates position from scratch ignoring:
- Screen working area bounds
- Auto-flip when not enough space below
- Horizontal alignment (popup should match combobox width, not just use trigger left edge)
- DPI scaling

**Expected Behavior:**
- Popup aligns left edge with combobox left edge
- Popup width matches combobox width (or content width, whichever is larger)
- If not enough space below, flips to show above
- Ensures popup is fully visible within screen bounds
- Handles multi-monitor scenarios

### 3. Page Up / Page Down Behavior

**Issue:** Page Up and Page Down keys move by a hardcoded 6 items, which is incorrect when:
- There is only 1 item (should go to first/last)
- Viewport shows fewer than 6 items (should move by visible count)
- Viewport shows more than 6 items (should move by visible count)

**Root Cause:**
```csharp
// ComboBoxPopupContent.cs:273-278
case Keys.PageDown:
    MoveFocus(6);
    e.Handled = true;
    break;
case Keys.PageUp:
    MoveFocus(-6);
    e.Handled = true;
    break;
```

**Expected Behavior (native ComboBox):**
- PageDown = move focus by `viewportHeight / itemHeight` items
- PageUp = move focus by `-viewportHeight / itemHeight` items
- If at last page, PageDown goes to last item
- If at first page, PageUp goes to first item

### 4. Content Width Mismatch

**Issue:** The popup content doesn't always match the popup width, causing:
- Horizontal scrollbar appearing unnecessarily
- Content clipped on the right
- Empty space on the right when content is narrower

**Root Cause:**
- Content panel width is calculated from `triggerBounds.Width` but doesn't account for popup borders
- List items may be wider than the popup due to long text
- No auto-width to fit content feature

**Expected Behavior:**
- Popup width = max(combobox width, widest item width + scrollbar width + padding)
- Content fills popup width exactly
- Long items show ellipsis or tooltip

### 5. Scrollbar Issues

**Issue:** Scrollbar appears when not needed or doesn't appear when needed.

**Root Cause:**
```csharp
// ComboBoxPopupContent.cs:447
_vScrollBar.LargeChange = Math.Max(1, visibleHeight);
```

`LargeChange` is set to pixel height, but `Maximum` is set to total pixel height. The WinForms scrollbar expects `LargeChange` to represent the viewport size in the same units as `Maximum`, but the value calculation is off.

**Expected Behavior:**
- Scrollbar only appears when `totalHeight > viewportHeight`
- Thumb size proportional to visible / total ratio
- Scrolls smoothly by item height increments

### 6. Animation and Performance

**Issue:** The fade animation can feel sluggish and causes perceived delay.

**Expected Behavior:**
- Optional animation (default: enabled but fast, ~100ms)
- No animation when closing via item selection (instant close)
- Hardware-accelerated if possible

---

## Enhancement Plan

### Phase 1: Fix Popup Sizing (High Priority)

#### 1.1 Implement IntegralHeight
- Add `IntegralHeight` property to `ComboBoxPopupHostProfile`
- When true, calculate dropdown height to show only whole items
- Formula: `((itemCount * itemHeight) / itemHeight) * itemHeight` = exact multiple
- Max height constraint still applies

#### 1.2 Fix Height Calculation
```csharp
// New CalculatePopupHeight implementation:
int CalculatePopupHeight(ComboBoxPopupModel model, ComboBoxPopupHostProfile profile)
{
    int count = model.FilteredRows?.Count ?? 0;
    if (count == 0) return profile.BaseRowHeight * 2; // Minimum height for empty state
    
    int searchHeight = (model.ShowSearchBox || profile.ForceSearchVisible) ? profile.SearchBoxHeight : 0;
    int footerHeight = (model.ShowFooter || profile.ForceFooterVisible) ? profile.FooterHeight : 0;
    int padding = profile.ListVerticalPadding * 2;
    int borders = profile.PopupBorderThickness * 2; // Account for border
    
    int contentHeight = count * profile.BaseRowHeight;
    int totalHeight = contentHeight + searchHeight + footerHeight + padding + borders;
    
    // Apply integral height - round down to nearest item multiple
    if (profile.IntegralHeight && count > 0)
    {
        int maxContentHeight = profile.MaxHeight - searchHeight - footerHeight - padding - borders;
        int visibleItems = Math.Max(1, maxContentHeight / profile.BaseRowHeight);
        int actualVisibleItems = Math.Min(count, visibleItems);
        contentHeight = actualVisibleItems * profile.BaseRowHeight;
        totalHeight = contentHeight + searchHeight + footerHeight + padding + borders;
    }
    
    return Math.Min(totalHeight, profile.MaxHeight);
}
```

#### 1.3 Add DropDownRows Property
- Add `DropDownRows` property to `BeepComboBox` (default: 8)
- Use it to calculate `MaxHeight` in profile: `MaxHeight = DropDownRows * BaseRowHeight + extras`

### Phase 2: Fix Popup Positioning (High Priority)

#### 2.1 Use Placement Helper Result
- Modify `ComboBoxPopupHostForm.ShowPopup` to use the location from `ComboBoxPopupPlacementHelper.Calculate`
- Remove redundant `CalculatePopupLocation` call in `BeepPopupForm`

#### 2.2 Implement Screen Edge Detection
```csharp
// Enhanced placement calculation:
var placement = ComboBoxPopupPlacementHelper.Calculate(owner, desiredWidth, targetHeight, autoFlip);

// Ensure popup is fully on screen:
Rectangle popupRect = new Rectangle(placement.Location, new Size(desiredWidth, placement.Height));
Rectangle workingArea = Screen.FromControl(owner).WorkingArea;

// Adjust if off-screen:
if (popupRect.Right > workingArea.Right)
    popupRect.X = workingArea.Right - popupRect.Width - 4;
if (popupRect.Left < workingArea.Left)
    popupRect.X = workingArea.Left + 4;
if (popupRect.Bottom > workingArea.Bottom)
    popupRect.Y = workingArea.Bottom - popupRect.Height - 4;
```

#### 2.3 Horizontal Alignment
- Popup left edge should align with combobox left edge
- Popup width should be at least combobox width
- Support `DropDownWidth` property for wider popups

### Phase 3: Fix Page Up/Down (High Priority)

#### 3.1 Calculate Page Size Dynamically
```csharp
// In ComboBoxPopupContent:
private int GetPageSize()
{
    int viewportHeight = _scrollContainer.ClientSize.Height;
    if (viewportHeight <= 0 || _rows.Count == 0) return 1;
    int avgItemHeight = _rows[0].Height;
    return Math.Max(1, viewportHeight / avgItemHeight);
}

// Usage:
case Keys.PageDown:
    MoveFocus(GetPageSize());
    break;
case Keys.PageUp:
    MoveFocus(-GetPageSize());
    break;
```

#### 3.2 Handle Edge Cases
- If focus is on last item and PageDown pressed: stay at last item
- If focus is on first item and PageUp pressed: stay at first item
- If only 1 item: PageUp/PageDown both select that item

### Phase 4: Fix Content Width (Medium Priority)

#### 4.1 Auto-Size to Content
- Measure widest item text using `Graphics.MeasureString`
- Popup width = max(combobox width, widestItem + scrollbarWidth + padding)
- Add `AutoSizeDropDown` property (default: true)

#### 4.2 Handle Long Items
- Show ellipsis for text that doesn't fit
- Show tooltip on hover for truncated text
- Respect `DropDownWidth` if explicitly set

### Phase 5: Fix Scrollbar (Medium Priority)

#### 5.1 Correct Scrollbar Math
```csharp
// Proper scrollbar configuration:
int totalHeight = _rows.Count * _profile.BaseRowHeight;
int viewportHeight = _scrollContainer.ClientSize.Height;
bool needsScroll = totalHeight > viewportHeight;

_vScrollBar.Visible = needsScroll;
if (needsScroll)
{
    // Maximum = total content height
    _vScrollBar.Maximum = totalHeight;
    // LargeChange = viewport height (thumb size)
    _vScrollBar.LargeChange = viewportHeight;
    // SmallChange = item height (one item scroll)
    _vScrollBar.SmallChange = _profile.BaseRowHeight;
}
```

#### 5.2 Smooth Scrolling
- Scroll by item height increments (not pixel-by-pixel)
- Ensure focused item is always fully visible
- Mouse wheel scrolls by 3 items per notch

### Phase 6: Animation & Polish (Low Priority)

#### 6.1 Optimize Animation
- Default duration: 100ms (faster than current 150ms)
- Disable animation when `AnimationDurationMs <= 0`
- Skip animation when closing via item selection

#### 6.2 Focus Handling
- Focus first selectable item when popup opens
- Restore focus to combobox when popup closes
- Keyboard navigation wraps around (optional)

---

## Implementation Order

1. **Phase 1** - Fix sizing (most visible issue)
2. **Phase 2** - Fix positioning (prevents popup going off-screen)
3. **Phase 3** - Fix Page Up/Down (keyboard navigation)
4. **Phase 5** - Fix scrollbar (related to sizing)
5. **Phase 4** - Content width (nice to have)
6. **Phase 6** - Polish (animation, focus)

---

## Files to Modify

| File | Changes |
|------|---------|
| `ComboBoxPopupHostForm.cs` | Fix `CalculatePopupHeight`, use placement helper location, add screen edge detection |
| `ComboBoxPopupPlacementHelper.cs` | Add horizontal alignment, screen edge constraints |
| `ComboBoxPopupContent.cs` | Fix PageUp/Down, scrollbar math, scrolling |
| `ComboBoxPopupHostProfile.cs` | Add `IntegralHeight`, `PopupBorderThickness` |
| `BeepPopupForm.cs` | Accept external location, skip animation on selection |
| `BeepComboBox.Properties.cs` | Add `DropDownRows`, `DropDownWidth`, `AutoSizeDropDown` |
| `ComboBoxListBoxPopupContent.cs` | Width measurement, ellipsis handling |

---

## Validation Checklist

- [ ] Single-item popup shows exactly one row with minimal padding
- [ ] Multi-item popup shows only whole items (no partial item at bottom)
- [ ] Popup width matches combobox width
- [ ] Popup flips to show above when not enough space below
- [ ] Popup stays within screen bounds on all edges
- [ ] PageDown moves by visible item count, not hardcoded 6
- [ ] PageUp moves by visible item count
- [ ] Scrollbar thumb size is proportional to visible/total ratio
- [ ] Scrollbar only appears when content exceeds viewport
- [ ] Focused item is always visible after keyboard navigation
- [ ] Animation is smooth and fast (<=100ms)
- [ ] No animation when selecting with mouse/Enter
- [ ] Long items show ellipsis, not horizontal scrollbar

---

## References

- [DevExpress PopupFormSize](https://docs.devexpress.com/WindowsForms/DevExpress.XtraEditors.Repository.RepositoryItemPopupBase.PopupFormSize)
- [WinForms ComboBox.IntegralHeight](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox.integralheight)
- [WinForms ComboBox.DropDownHeight](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox.dropdownheight)
- [WinForms ComboBox.DropDownWidth](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox.dropdownwidth)
