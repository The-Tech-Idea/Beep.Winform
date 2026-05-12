# BeepComboBox Dropdown - Comprehensive Revision Plan
**Date:** 2026-05-11
**Status:** Needs full reimplementation based on user feedback that all fixes are incorrect

---

## Summary of Current State

Based on code review and GitHub best practices research:
- Code has the right structure but implementation has bugs
- Properties and methods exist but may not work correctly
- User reports ALL fixes are not correct

## Reference Implementations (from GitHub research)

### 1. dotnet/winforms (Official ComboBox)
- Uses `IntegralHeight` properly - when DropDownHeight is set, IntegralHeight is disabled
- PageUp/PageDown handled natively by the control
- MaxDropDownItems property controls visible rows

### 2. sgissinger/CheckBoxComboBox
- Uses `ToolStripDropDown` for popup which has built-in auto-flip
- Proper screen bounds detection using `Screen.FromControl(control).WorkingArea`
- Popup shows below/above based on available space

### 3. BradSmith1985/DropDownControls
- Uses `ToolStripDropDown` with `ToolStripControlHost`
- Bitmap caching for smooth scrolling of large lists
- Buffered Paint API for smooth animations

### 4. CodeProject Custom DropDown
- Base class `DropDownControl` handles all dropdown logic
- Uses `Control` parameter for dropdown content sizing
- Handles click-outside to close

---

## Critical Issues to Fix

### Issue 1: Popup Sizing (Phase 1)

**Current Problem:** 
```csharp
// ComboBoxPopupHostForm.cs - lines 219-244
int count = model.FilteredRows?.Count ?? 0;
if (count == 0)
    return profile.BaseRowHeight * 2;
// ... but maxHeightOverride may not be properly used
```

**Fix Required:**
1. Verify `IntegralHeight` is being respected in all code paths
2. Ensure `CalculatePopupHeight` properly calculates visible items
3. Test single item, 5 items, 20 items with DropDownRows=8

**Correct Implementation:**
```csharp
protected virtual int CalculatePopupHeight(ComboBoxPopupModel model, ComboBoxPopupHostProfile profile, int? maxHeightOverride = null)
{
    int count = model.FilteredRows?.Count ?? 0;
    if (count == 0)
        return profile.BaseRowHeight * 2; // Minimum for empty state

    int searchHeight = (model.ShowSearchBox || profile.ForceSearchVisible) 
        ? profile.SearchBoxHeight : 0;
    int footerHeight = (model.ShowFooter || profile.ForceFooterVisible) 
        ? profile.FooterHeight : 0;
    int padding = profile.ListVerticalPadding * 2;
    int borders = profile.PopupBorderThickness * 2;
    
    // Calculate max content height (subtract non-content elements)
    int effectiveMaxHeight = maxHeightOverride ?? profile.MaxHeight;
    int maxContentHeight = effectiveMaxHeight - searchHeight - footerHeight - padding - borders;
    
    // Calculate visible items (integral height - no partial items)
    int visibleItems = Math.Min(count, Math.Max(1, maxContentHeight / profile.BaseRowHeight));
    int contentHeight = visibleItems * profile.BaseRowHeight;
    
    int totalHeight = contentHeight + searchHeight + footerHeight + padding + borders;
    return Math.Min(totalHeight, effectiveMaxHeight);
}
```

### Issue 2: Popup Positioning (Phase 2)

**Current Problem:**
Placement helper IS being used (lines 111-113), but auto-flip may not work correctly.

**Fix Required:**
1. Verify `AutoFlip` property is being passed correctly
2. Test popup near bottom of screen
3. Test popup near right edge of screen

**Correct Implementation (verify this exists):**
```csharp
// ComboBoxPopupPlacementHelper.cs should have:
bool placeBelow = !autoFlip || spaceBelow >= preferredHeight || spaceBelow >= spaceAbove;
```

### Issue 3: Page Up/Down Navigation (Phase 3)

**Current Problem:**
`GetPageSize()` is implemented but may not calculate correctly.

**Fix Required:**
1. Verify viewport height calculation
2. Ensure GetPageSize returns 1 for single-item viewport
3. Test with 3, 5, 10 visible items

**Correct Implementation:**
```csharp
private int GetPageSize()
{
    // Use viewport height from scroll container
    int viewportHeight = _scrollContainer.ClientSize.Height;
    if (viewportHeight <= 0 || _rows.Count == 0) 
        return 1;
    
    int itemHeight = _profile.BaseRowHeight; // Use profile item height
    if (itemHeight <= 0) 
        itemHeight = 32; // Fallback
    
    int pageSize = viewportHeight / itemHeight;
    return Math.Max(1, pageSize); // Always at least 1
}
```

### Issue 4: Content Width (Phase 4)

**Current Problem:**
`MeasureContentWidth` uses Graphics.FromImage which may cause issues.

**Fix Required:**
1. Use proper Graphics from Control.CreateGraphics()
2. Add proper scrollbar width consideration
3. Verify ellipsis rendering for long items

**Correct Implementation:**
```csharp
protected virtual int MeasureContentWidth(ComboBoxPopupModel model)
{
    if (model.FilteredRows == null || model.FilteredRows.Count == 0)
        return 0;

    int maxWidth = 0;
    using (var bitmap = new Bitmap(1, 1))
    using (var g = Graphics.FromImage(bitmap))
    {
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        
        foreach (var row in model.FilteredRows)
        {
            if (string.IsNullOrEmpty(row.Text)) continue;
            
            // Measure main text
            var font = _themeTokens?.LabelFont ?? SystemFonts.MessageBoxFont;
            SizeF size = g.MeasureString(row.Text, font);
            maxWidth = Math.Max(maxWidth, (int)Math.Ceiling(size.Width));
            
            // Measure subtext if present
            if (!string.IsNullOrEmpty(row.SubText))
            {
                var subFont = _themeTokens?.SubTextFont ?? font;
                SizeF subSize = g.MeasureString(row.SubText, subFont);
                maxWidth = Math.Max(maxWidth, (int)Math.Ceiling(subSize.Width));
            }
        }
    }
    
    // Add space for icon, checkbox, and padding
    int extraSpace = 48; // icon(20) + checkbox(16) + margins(12)
    
    return maxWidth + extraSpace;
}
```

### Issue 5: Scrollbar Behavior (Phase 5)

**Current Implementation:** (seems correct)
```csharp
_vScrollBar.Maximum = Math.Max(0, totalHeight - visibleHeight);
_vScrollBar.LargeChange = visibleHeight;
```

This is the correct formula. Verify it's being called properly.

### Issue 6: Animation (Phase 6)

**Current Problem:**
Animation duration not reduced to 100ms.

**Fix Required:**
1. Check BeepPopupForm.AnimationDurationMs property
2. Default should be 100ms, not 150ms
3. Verify SkipAnimationOnSelection works

---

## Files to Review and Fix

### Priority 1: Core Popup Files
1. `ComboBoxes/Popup/ComboBoxPopupHostForm.cs` - Sizing, positioning
2. `ComboBoxes/Popup/ComboBoxPopupPlacementHelper.cs` - Screen detection
3. `ComboBoxes/Popup/ComboBoxPopupContent.cs` - Navigation, scrollbar

### Priority 2: ListBox-based Popup
4. `ComboBoxes/Popup/ComboBoxListBoxPopupContent.cs` - Should delegate to BeepListBox

### Priority 3: Support Files
5. `ComboBoxes/Popup/ComboBoxPopupHostProfile.cs` - Profile settings
6. `Forms/BeepPopupForm.cs` - Animation settings

---

## Validation Checklist

After fixes, test:

1. **Sizing**
   - [ ] Single item: shows exactly 1 row, minimal padding
   - [ ] 5 items: shows exactly 5 rows
   - [ ] 20 items with DropDownRows=8: shows exactly 8 rows
   - [ ] No partial item visible at bottom

2. **Positioning**
   - [ ] Popup aligns with combobox left edge
   - [ ] Popup near bottom of screen flips to show above
   - [ ] Popup never extends beyond screen edges

3. **Navigation**
   - [ ] PageUp/PageDown moves by visible count
   - [ ] Works when only 1 item visible
   - [ ] Skips group headers and separators

4. **Scrollbar**
   - [ ] Only appears when needed
   - [ ] Thumb size is proportional
   - [ ] Mouse wheel scrolls by item increments

5. **Animation**
   - [ ] Fast animation (~100ms)
   - [ ] No animation on selection
   - [ ] Smooth open/close

---

## Implementation Steps

### Step 1: Verify Profile Settings
Check `ComboBoxPopupHostProfile.cs`:
- `IntegralHeight = true` should be default
- `PopupBorderThickness` should default to 1

### Step 2: Fix CalculatePopupHeight
Update `ComboBoxPopupHostForm.cs` to properly use:
- Integral height calculation
- maxHeightOverride parameter
- All padding and borders

### Step 3: Fix GetPageSize
Update `ComboBoxPopupContent.cs`:
- Use `_scrollContainer.ClientSize.Height`
- Use `_profile.BaseRowHeight` for item height

### Step 4: Verify Animation
Check `BeepPopupForm.cs`:
- Default animation should be 100ms
- SkipAnimationOnSelection should work

### Step 5: Test All Scenarios
Create test cases for:
- Single item
- 5 items
- 20 items with DropDownRows=8
- Popup near screen edges
- Keyboard navigation