# WM_NCCALCSIZE vs WinForms AutoScale - The Conflict Explained

## The Problem Timeline

```
1. Windows sends WM_NCCALCSIZE to calculate client area
   ↓
2. BeepiFormPro modifies rgrc0 (client rectangle)
   Original: (0, 0, 800, 600)
   Modified: (1, 0, 799, 599) ← Border thickness removed
   ↓
3. WinForms reads modified client area dimensions
   Client Width = 799 (instead of 800)
   Client Height = 599 (instead of 600)
   ↓
4. WinForms calculates AutoScale factor
   CurrentAutoScaleDimensions = GetAutoScaleSize(CurrentFont)
   Based on WRONG client area (799x599)
   ↓
5. AutoScale applies factor to ALL child controls
   Scale Factor = 1.5 (at 144 DPI)
   BUT client area was already reduced by WM_NCCALCSIZE
   ↓
6. RESULT: Double transformation!
   Text scaled by: Border adjustment (0.99x) × AutoScale (1.5x) = 1.485x
   But PERCEIVED as: 1.8-2.0x due to accumulated rounding errors
```

## Visual Representation

### Before Fix (BROKEN)
```
┌─────────────────────────────────────┐
│ Windows Frame (handled by OS)      │
│ ┌─────────────────────────────────┐ │
│ │ WM_NCCALCSIZE modifies HERE     │ │ ← rgrc0 adjusted
│ │ ┌─────────────────────────────┐ │ │
│ │ │ Client Area (799x599)       │ │ │ ← WRONG dimensions
│ │ │                             │ │ │
│ │ │ AutoScale uses these dims   │ │ │ ← Calculates wrong factor
│ │ │ Label Text (scaled 1.8x!!!) │ │ │ ← TOO BIG!
│ │ │ [Black Box]                 │ │ │ ← Painting outside bounds
│ │ └─────────────────────────────┘ │ │
│ └─────────────────────────────────┘ │
└─────────────────────────────────────┘
```

### After Fix (CORRECT)
```
┌─────────────────────────────────────┐
│ Windows Frame (handled by OS)      │
│ ┌─────────────────────────────────┐ │
│ │ WM_NCCALCSIZE NOT modified      │ │ ← rgrc0 unchanged
│ │ ┌─────────────────────────────┐ │ │
│ │ │ Client Area (800x600)       │ │ │ ← CORRECT dimensions
│ │ │                             │ │ │
│ │ │ AutoScale uses correct dims │ │ │ ← Correct factor (1.5x)
│ │ │ Label Text (scaled 1.5x)    │ │ │ ← Perfect size!
│ │ │ Clean rendering             │ │ │ ← No black boxes
│ │ └─────────────────────────────┘ │ │
│ └─────────────────────────────────┘ │
└─────────────────────────────────────┘
```

## Code Flow Comparison

### BROKEN Flow (DrawCustomWindowBorder = true)
```csharp
// Step 1: Windows message arrives
WndProc(ref Message m) // m.Msg == WM_NCCALCSIZE

// Step 2: Modify client rectangle
var nccsp = Marshal.PtrToStructure<NCCALCSIZE_PARAMS>(m.LParam);
nccsp.rgrc0.left += 1;   // ❌ Changes client area
nccsp.rgrc0.right -= 1;  // ❌ Changes client area
nccsp.rgrc0.bottom -= 1; // ❌ Changes client area
Marshal.StructureToPtr(nccsp, m.LParam, false);

// Step 3: WinForms reads client area (LATER in message processing)
// ClientSize now returns (799, 599) instead of (800, 600)

// Step 4: AutoScale calculates factor
CurrentAutoScaleDimensions = GetAutoScaleSize(AutoScaleFont);
// Uses wrong client area → calculates wrong font dimensions
AutoScaleFactor = CurrentAutoScaleDimensions / AutoScaleDimensions;
// Factor is wrong! (e.g., 1.52 instead of 1.5)

// Step 5: Child controls scaled
foreach (Control child in Controls)
{
    child.Scale(AutoScaleFactor); // ❌ Applies wrong factor
}
```

### FIXED Flow (DrawCustomWindowBorder = false)
```csharp
// Step 1: Windows message arrives
WndProc(ref Message m) // m.Msg == WM_NCCALCSIZE

// Step 2: DON'T modify client rectangle
// Let Windows handle it naturally
base.WndProc(ref m); // ✅ Pass through unchanged

// Step 3: WinForms reads correct client area
// ClientSize returns (800, 600) as designed

// Step 4: AutoScale calculates correct factor
CurrentAutoScaleDimensions = GetAutoScaleSize(AutoScaleFont);
AutoScaleFactor = CurrentAutoScaleDimensions / AutoScaleDimensions;
// Factor is correct! (exactly 1.5 at 144 DPI)

// Step 5: Child controls scaled correctly
foreach (Control child in Controls)
{
    child.Scale(AutoScaleFactor); // ✅ Applies correct factor
}
```

## The Black Box Mystery Solved

**Why did Labels show as black boxes?**

1. **Wrong scale factor** caused Label to think it was larger than actual space
2. **Label.OnPaint** tried to draw text at position/size calculated from wrong dimensions
3. **Graphics clipping** cut off the oversized text
4. **Partial rendering** left a "black box" artifact where text was clipped

**Fix:**
- Correct client area → correct scale → correct Label size → text fits properly

## Key Takeaway

**NEVER modify `WM_NCCALCSIZE` client rectangle (`rgrc0`) if you're using WinForms AutoScale!**

The `rgrc0` rectangle is used by Windows AND WinForms for multiple calculations:
- Window layout
- Client area coordinate system
- **AutoScale dimension calculations** ← This is what we broke!
- Child control positioning
- Hit testing

Modifying it creates a cascading chain of errors throughout the entire rendering pipeline.

## When IS It Safe to Modify WM_NCCALCSIZE?

Only modify `WM_NCCALCSIZE` when:
1. ✅ `AutoScaleMode = None` (no automatic scaling)
2. ✅ You're handling ALL layout manually
3. ✅ You understand the implications for all child controls
4. ✅ `WindowState != FormWindowState.Normal` (minimized, maximized, custom states)

**For standard forms with AutoScale: Leave it alone!**

## References

- [WM_NCCALCSIZE Message](https://learn.microsoft.com/en-us/windows/win32/winmsg/wm-nccalcsize)
- [Automatic Scaling in Windows Forms](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/forms/autoscale)
- [Custom Window Frame Using DWM](https://learn.microsoft.com/en-us/windows/win32/dwm/customframe)

## Date
October 11, 2025
