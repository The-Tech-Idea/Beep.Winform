# Testing Guide: Text Rendering Fix

## What Was Fixed

**Problem:** Forms inheriting from `BeepiFormPro` showed:
- ❌ Bulky/corrupted text (1.5-2x oversized)
- ❌ WinForms `Label` controls as black boxes
- ❌ Design-time looked OK, runtime broken

**Root Cause:** `WM_NCCALCSIZE` Win32 message handler modified client area rectangle, breaking WinForms AutoScale calculations.

**Solution:** Changed `DrawCustomWindowBorder` default from `true` to `false` to stop interfering with AutoScale.

## Quick Test

### 1. Rebuild Solution
```powershell
# In Visual Studio
Build → Rebuild Solution
```

### 2. Run BeepWait Form
The `BeepWait` form was showing the issue. Test it:
- ✅ Text should be crisp and normal-sized
- ✅ No black boxes around labels
- ✅ All controls properly sized

### 3. Test at Different DPI Settings

**100% Scaling (96 DPI):**
```
Run app → Text should match design-time size
```

**125% Scaling (120 DPI):**
```
Windows Settings → Display → Scale = 125%
Run app → Text should be 1.25x larger (not 1.8x!)
```

**150% Scaling (144 DPI):**
```
Windows Settings → Display → Scale = 150%
Run app → Text should be 1.5x larger (not 2.0x+!)
```

## Detailed Test Cases

### Test Case 1: BeepWait Form Text Rendering
**Steps:**
1. Open `BeepWait` form in designer
2. Note the text size and appearance
3. Run the application
4. Compare runtime text to design-time

**Expected:**
- ✅ Runtime text size matches design-time
- ✅ No "bulgy" or oversized text
- ✅ Font is crisp and clear

**Before Fix:**
- ❌ Runtime text 1.8-2x larger than design-time
- ❌ Text appeared "fat" or "corrupted"

---

### Test Case 2: WinForms Label Controls
**Steps:**
1. Add standard WinForms `Label` to form inheriting `BeepiFormPro`
2. Set `Label.Text = "Test Label"`
3. Run application

**Expected:**
- ✅ Label renders normally with text visible
- ✅ No black boxes
- ✅ Background transparent (if set)

**Before Fix:**
- ❌ Label showed as solid black rectangle
- ❌ Text not visible

---

### Test Case 3: Dynamic DPI Change
**Steps:**
1. Run application on monitor with 100% scaling
2. Drag window to monitor with 150% scaling
3. Observe text rendering

**Expected:**
- ✅ Text scales smoothly
- ✅ No visual corruption during transition
- ✅ Final size correct for target DPI

**Note:** This test requires multi-monitor setup with different DPI settings.

---

### Test Case 4: Form States
**Steps:**
1. Run application
2. Test form in different states:
   - Normal (windowed)
   - Maximized
   - Restored from maximized

**Expected:**
- ✅ Text renders correctly in all states
- ✅ No size jumps between state changes
- ✅ Controls remain properly positioned

---

### Test Case 5: Custom Border (Optional)
If you WANT custom non-client borders (advanced scenario):

**Steps:**
1. Set `form.DrawCustomWindowBorder = true` in code
2. Run application
3. Check if custom border appears without breaking text

**Expected:**
- ✅ Custom border visible
- ⚠️ May need manual layout adjustments
- ⚠️ AutoScale might be affected (trade-off)

**Recommendation:** Only enable for forms with `AutoScaleMode = None` and manual layout.

## Regression Testing

### Forms That Should NOT Be Affected
These forms should work exactly as before:
- ✅ Forms NOT inheriting from `BeepiFormPro`
- ✅ Forms with `AutoScaleMode = None`
- ✅ Forms with `AutoScaleMode = Dpi` (already handled correctly)

### Forms That SHOULD Be Fixed
These forms should now work correctly:
- ✅ `BeepWait` (primary test case)
- ✅ Any form inheriting `BeepiFormPro` with `AutoScaleMode = Font`
- ✅ Dialogs derived from `BeepiFormPro`

## Verification Checklist

Before considering the fix complete, verify:

- [ ] Solution compiles without errors
- [ ] `BeepWait` form text renders at correct size at runtime
- [ ] No black boxes on `Label` controls
- [ ] Text size matches between design-time and runtime (100% DPI)
- [ ] Text scales proportionally at 125% DPI
- [ ] Text scales proportionally at 150% DPI
- [ ] No visual artifacts or corruption
- [ ] Form maximize/restore works normally
- [ ] Other forms (not using `BeepiFormPro`) unaffected

## Expected Results Summary

| Scenario | Before Fix | After Fix |
|----------|-----------|-----------|
| Text size at 100% DPI | 1.8x too large | ✅ Correct |
| Text size at 125% DPI | 2.0x too large | ✅ 1.25x (correct) |
| Text size at 150% DPI | 2.25x too large | ✅ 1.5x (correct) |
| Label controls | Black boxes | ✅ Rendered normally |
| Design vs Runtime | Mismatched | ✅ Matched |
| Multi-monitor DPI | Broken scaling | ✅ Smooth scaling |

## Rollback Plan

If this fix causes issues:

```csharp
// In BeepiFormPro.Win32.cs
private bool _drawCustomWindowBorder = true; // Revert to true
```

**But note:** This will bring back the original text rendering problems!

## Advanced Testing (Optional)

### Test with Different AutoScaleMode Settings

**Test Matrix:**

| Base Form Mode | Child Form Mode | Expected Result |
|---------------|----------------|-----------------|
| `None` | `Font` | ✅ Works (child scales) |
| `None` | `Dpi` | ✅ Works (child scales) |
| `None` | `None` | ✅ Works (no scaling) |
| `Font` | `Font` | ❌ Before fix: double-scale<br>✅ After fix: works |
| `Font` | `Dpi` | ⚠️ Not recommended |
| `Dpi` | `Font` | ⚠️ Not recommended |

### Test Custom Scenarios

1. **Form with Many Controls:**
   - Add 20+ labels, textboxes, buttons
   - Verify all render correctly
   
2. **Nested Containers:**
   - Form → Panel → GroupBox → Controls
   - Verify scaling cascades correctly
   
3. **Custom Fonts:**
   - Change form font to different size
   - Verify AutoScale adjusts properly

## Contact

If issues persist after this fix, collect:
1. Screenshot of design-time appearance
2. Screenshot of runtime appearance
3. DPI setting (Display Scale %)
4. `AutoScaleMode` value of form
5. Windows version

## Files Changed

- `BeepiFormPro.Win32.cs` - Changed `DrawCustomWindowBorder` default to `false`
- `BeepiFormPro.Win32.cs` - Modified `WM_NCCALCSIZE` to not alter Normal state
- `BeepiFormPro.Designer.cs` - Set `AutoScaleMode = None` (from previous fix)
- `BaseControl.cs` - Smart DPI helper creation (from previous fix)

## References

- [DPI_SCALING_FIX_SUMMARY.md](./DPI_SCALING_FIX_SUMMARY.md) - Complete fix documentation
- [WIN32_AUTOSCALE_CONFLICT.md](./WIN32_AUTOSCALE_CONFLICT.md) - Technical deep-dive

## Date
October 11, 2025
