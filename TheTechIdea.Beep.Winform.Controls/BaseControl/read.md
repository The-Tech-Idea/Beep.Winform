# Material style in BaseControl

What’s added
- Optional Material border and fill rendering on BaseControl using DrawCustomBorder.
- Leading and trailing icons inside the border, centered vertically.

Properties
- EnableMaterialStyle (bool): turn Material painting on/off.
- MaterialVariant (MaterialTextFieldVariant): Outlined, Filled, Standard.
- CurvedBorderRadius (int)
- ShowFill (bool), FillColor (Color)
- OutlineColor (Color), PrimaryColor (Color), ErrorColor (Color)
- LeadingIconPath / TrailingIconPath (string), LeadingImagePath / TrailingImagePath (string)
- IconSize (int), IconPadding (int)

Mouse
- Icon hit-testing and click events: LeadingIconClicked, TrailingIconClicked.

Notes
- No notch (no floating label in BaseControl).
- Does not change layout; purely decorative unless you use icons.
