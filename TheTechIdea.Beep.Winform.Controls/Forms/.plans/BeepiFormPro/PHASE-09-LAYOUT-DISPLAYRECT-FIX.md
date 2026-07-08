# Phase 9 — DisplayRectangle & Padding Fix

**Status:** Not started | **Priority:** CRITICAL | **Depends on:** Phase 3

## Problem

`BeepiFormPro` does NOT override `DisplayRectangle` and does NOT set `Padding`. The painters compute `CurrentLayout.ContentRect` correctly (caption bar + borders accounted for), but this information is never propagated to the form's layout system.

**Result:** Child controls placed on a `BeepiFormPro` form overlap the caption bar because `DisplayRectangle` returns the full `ClientRectangle`.

## Root Cause

- `BeepiFormPro` inherits from `Form` which computes `DisplayRectangle = ClientRectangle - Padding`
- `Padding` is never set by any painter (0 occurrences across all 33 painters)
- `DisplayRectangle` is never overridden
- `CurrentLayout.ContentRect` is computed but never used to adjust the form's layout insets

## Fix

### DL-01: Set form Padding in every painter's `CalculateLayoutAndHitAreas`

After computing `CurrentLayout.ContentRect`, set the form's `Padding` to match:

```csharp
var contentRect = layout.ContentRect;
var clientRect = owner.ClientRectangle;
owner.Padding = new Padding(
    contentRect.Left - clientRect.Left,
    contentRect.Top - clientRect.Top,
    clientRect.Right - contentRect.Right,
    clientRect.Bottom - contentRect.Bottom
);
```

This makes `DisplayRectangle` return exactly the content area, so child controls automatically respect the caption bar and borders.

### DL-02: Verify all 33 painters apply this fix

Each painter's `CalculateLayoutAndHitAreas` method must set `owner.Padding` after computing layout.

### DL-03: Test with child controls

Place a `BeepButton` or `BeepTextBox` at `Dock = Fill` on a `BeepiFormPro` form — verify it does NOT overlap the caption bar.

### DL-04: Verify SafeContentInsets is consistent with Padding

`PainterLayoutInfo.SafeContentInsets` provides rounded-corner safe margins. Ensure this is additive to (not replacing) the base `Padding` from caption/border.

## Files

| File | Change |
|---|---|
| All 33 `Painters/*FormPainter.cs` | Add `owner.Padding = ...` in `CalculateLayoutAndHitAreas` |
| `BeepiFormPro.Core.cs` | Add `override DisplayRectangle` that returns `base.DisplayRectangle` (ensures Padding is used) |

## Verification

- [ ] Child control at `Dock = Fill` does NOT overlap caption bar
- [ ] Child control at `Dock = Fill` respects rounded corner safe insets
- [ ] All 33 painters produce correct content area
- [ ] `dotnet build` — 0 errors
