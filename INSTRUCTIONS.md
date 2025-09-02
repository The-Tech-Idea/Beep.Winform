# Development Instructions

## Material sizing for derived controls

All controls inheriting from `BaseControl` must account for Material spacing (padding, focus ring, icons, elevation). BaseControl provides helpers and a default policy so derived controls don’t need to reinvent this.

- If your control’s intrinsic minimum content size is W×H (e.g., 300×30), the effective minimum must include BaseControl’s spacing:
  - EffectiveMinWidth = W + MaterialPadding.Horizontal + EffectsSpace.Width + IconSpace.Width
  - EffectiveMinHeight = max(H + MaterialPadding.Vertical + EffectsSpace.Height,
    IconSpace.Height + MaterialPadding.Vertical + EffectsSpace.Height)
- EffectsSpace = 4 (focus ring both sides) + 2 × MaterialElevationLevel when elevation is enabled.
- IconSpace is the total horizontal space for leading/trailing icons and optional clear button.

### Defaults implemented in BaseControl

- During Material size compensation, BaseControl automatically raises `MinimumSize` to the required Material minimum (never shrinking an explicit larger `MinimumSize`).
- `GetMaterialStylePadding()`, `GetMaterialEffectsSpace()`, and `GetMaterialIconSpace()` compute padding/effects/icon deltas consistently across variants.
- `CalculateMinimumSizeForMaterial(Size baseContentSize)` returns the required size including the above.
- `AdjustSizeForMaterial(Size baseContentSize)` scales for DPI and applies the result to both `MinimumSize` and the control `Size` when needed.

### What derived controls should do

- Provide your intrinsic content minimums and let BaseControl compute the effective size:

  ```csharp
  // Inside your derived control
  protected override void OnCreateControl()
  {
      base.OnCreateControl();
      var baseMin = new Size(300, 30); // your control’s content minimum
      var effective = GetEffectiveMaterialMinimum(baseMin); // includes padding/effects/icons + DPI
      MinimumSize = effective; // optional, BaseControl also raises MinimumSize during compensation
  }
  ```

- If your control has a dynamic content minimum (e.g., based on font/label), compute `baseMin` first, then call `AdjustSizeForMaterial(baseMin)`. This updates both `MinimumSize` and `Size` as needed.
- Optionally override `GetMaterialMinimumWidth/Height()` if your control should enforce a higher baseline than the default 120×[48|56] rules.

### Quick reference

- Padding: `GetMaterialStylePadding()`
- Effects (focus + elevation): `GetMaterialEffectsSpace()`
- Icons: `GetMaterialIconSpace()`
- Compute required size: `CalculateMinimumSizeForMaterial(baseContentSize)`
- Enforce: `AdjustSizeForMaterial(baseContentSize)` or `GetEffectiveMaterialMinimum(baseContentSize)`

## DPI and scaling

`BaseControl.DisableDpiAndScaling` is true by default to avoid autoscaling surprises. If you enable DPI helpers, calculations still account for DPI via `ScaleSize`.

## Theming

Call `ApplyTheme()` or set `Theme` to propagate theme values. `ApplyThemeToChilds` pushes the theme to child controls.
