# Beep.Winform

Advanced WinForms controls with Material Design support and performance-focused helpers.

## Material sizing policy (important)

Controls inheriting from `BaseControl` render Material padding, focus rings, icons, and optional elevation around their content. This means their “effective minimum size” must include BaseControl’s spacing. By default, BaseControl enforces this for you when Material is enabled.

- EffectiveMinWidth = BaseMinWidth + MaterialPadding.Horizontal + EffectsSpace.Width + IconSpace.Width
- EffectiveMinHeight = max(BaseMinHeight + MaterialPadding.Vertical + EffectsSpace.Height,
  IconSpace.Height + MaterialPadding.Vertical + EffectsSpace.Height)

Notes:
- EffectsSpace includes a 4px focus ring (both sides) and 2 × elevation level when elevation is enabled.
- BaseControl now automatically raises `MinimumSize` to the required Material size during size compensation, so derived controls don’t need to duplicate this logic.
- For custom controls that have a known intrinsic minimum (e.g., 300×30), use `GetEffectiveMaterialMinimum(new Size(300,30))` or `CalculateMinimumSizeForMaterial` to compute the final minimum. See INSTRUCTIONS.md for details and examples.

See INSTRUCTIONS.md for guidance on building new controls, sizing, DPI, and theming.