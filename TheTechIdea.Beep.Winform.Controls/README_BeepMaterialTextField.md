# BeepMaterialTextField – Material Design 3 text field for WinForms

BeepMaterialTextField is a full-featured Material Design 3 (Material You) text field for WinForms, implemented on top of BeepControl’s DrawContent pipeline. It supports the three MD3 variants (Standard, Filled, Outlined), floating labels, helper/validation text, prefix/suffix, character counter, curved/search-box styling, and dual clickable icons.

## Highlights
- MD3-compliant container, label, input, supporting text, and interaction states.
- Three variants: Standard, Filled, Outlined; three densities: Standard, Dense, Comfortable.
- Floating label animation and focus underline/outline animation.
- Prefix and suffix text with automatic text-rect adjustment.
- Dual icons (leading and trailing) with independent click events; optional clear button.
- Curved borders and search-box style (pill); background fill for iOS/Android/Win11 look.
- Selection, caret drawing, line numbers (multiline), and character counter.
- Theme integration through IBeepTheme and BeepSvgPaths icon library.
- High DPI aware; all drawing aligned to BeepControl.DrawingRect.

## Architecture
The control follows the BeepControl + helper pattern for clean separation of responsibilities.

- TextFields/BeepMaterialTextField.cs – Core control, BeepControl integration, DrawContent.
- TextFields/BeepMaterialTextField.Properties.cs – Public properties (Text, Material, Icons, etc.).
- TextFields/BeepMaterialTextField.Methods.cs – Public methods and helpers.
- TextFields/Helpers/BeepMaterialTextFieldHelper.cs – Layout, animations, input hit-testing.
- TextFields/Helpers/MaterialTextFieldDrawingHelper.cs – All rendering in DrawAll().
- TextFields/Helpers/MaterialTextFieldInputHelper.cs – Keyboard and mouse handling.
- TextFields/Helpers/MaterialTextFieldCaretHelper.cs – Caret and selection.
- TextFields/Helpers/TextBoxValidationHelper.cs – Masking and validation.
- TextFields/Helpers/SmartAutoCompleteHelper.cs – Autocomplete (optional).
- TextFields/Helpers/TextBoxAdvancedEditingHelper.cs – Advanced editing features.

## Using the control
Minimal usage

```csharp
var field = new BeepMaterialTextField
{
    Variant = MaterialTextFieldVariant.Outlined,
    LabelText = "Enter text",
    PlaceholderText = "Type here",
    Width = 300
};
```

Search box with dual icons

```csharp
var search = new BeepMaterialTextField();
search.ConfigureAsSearchBox(BeepSvgPaths.Search, showClearButton: true);
search.TrailingIconPath = BeepSvgPaths.Microphone;
search.LeadingIconClicked += (s,e) => PerformSearch();
search.TrailingIconClicked += (s,e) => StartVoiceInput();
```

Curved look

```csharp
var curved = new BeepMaterialTextField
{
    Variant = MaterialTextFieldVariant.Standard,
    CurvedBorderRadius = 20,
    ShowFill = true,
    FillColor = Color.FromArgb(245,245,245)
};
```

Password with visibility toggle

```csharp
var pwd = new BeepMaterialTextField
{
    LabelText = "Password",
    LeadingIconPath = BeepSvgPaths.Shield,
    TrailingIconPath = BeepSvgPaths.Eye,
    UseSystemPasswordChar = true
};
pwd.TrailingIconClicked += (s,e) => pwd.TogglePasswordVisibility();
```

## Important: alignment and drawing
Starting with this version, every part of the control is measured and drawn relative to BeepControl.DrawingRect. This fixes alignment issues (icons, text, placeholder, label, helper/counter, selection, caret) when the host control uses custom borders, padding, or DPI scaling.

- BeepMaterialTextFieldHelper.UpdateLayout computes rectangles in DrawingRect space.
- MaterialTextFieldDrawingHelper offsets all draw calls by DrawingRect.X/Y.
- Mouse hit-testing for icons and text translates the point into DrawingRect space.

If you customize Padding or add parent borders, no extra work is needed—the layout stays aligned.

## Key properties
Text and behavior
- Text, Multiline, ReadOnly, MaxLength, UseSystemPasswordChar, PasswordChar, TextAlignment
- AcceptsReturn, AcceptsTab, WordWrap

Material
- Variant (Standard, Filled, Outlined), Density (Standard, Dense, Comfortable)
- LabelText, HelperText, ErrorText, IsRequired
- ShowCharacterCounter, SurfaceColor, OutlineColor, LabelColor, HelperTextColor, PrimaryColor, ErrorColor
- PlaceholderText, PlaceholderTextColor

Icons
- LeadingIconPath, TrailingIconPath (BeepSvgPaths) or LeadingImagePath, TrailingImagePath
- LeadingIconClickable, TrailingIconClickable, IconSize, IconPadding
- ShowClearButton (shows Close icon when HasContent)

Curved and search styles
- CurvedBorderRadius, SearchBoxStyle, FillColor, ShowFill

Prefix/Suffix
- PrefixText, SuffixText (layout adjusts automatically)

Line numbers (multiline)
- ShowLineNumbers, LineNumberMarginWidth, LineNumberForeColor, LineNumberBackColor, LineNumberFont

## Events
- TextChanged (inherited), LeadingIconClicked, TrailingIconClicked, ClearButtonClicked

## Helpful methods
- ConfigureAsSearchBox(iconPath, showClearButton)
- ConfigureDualIcons(leading, trailing, leadingClickable, trailingClickable)
- SetCurvedRadius(radius)
- TogglePasswordVisibility()
- UpdateMaterialLayout(), UpdateIcons(), ApplyVariant(variant), ApplyDensity(density)

## Theming
Call ApplyTheme on the control or set Theme on the parent BeepControl. The control maps IBeepTheme colors to Material colors and can theme SVG icons when ApplyThemeToImage is true.

## Troubleshooting
- Icons overlap or clicks don’t trigger
  Ensure LeadingIconPath/LeadingImagePath and TrailingIconPath/TrailingImagePath are set. Hit-testing is done in DrawingRect space; don’t manually position child controls over this area.
- Text/placeholder not vertically centered in search style
  Enable SearchBoxStyle or set CurvedBorderRadius; the helper centers the text inside the container height.
- Helper text/counter clipped
  Increase Height or reduce density; helper/counter are placed below the input rectangle inside DrawingRect.

## Requirements
- .NET 8+ WinForms
- GDI+ rendering; TextRenderingHint.ClearTypeGridFit

## License
Part of TheTechIdea Beep.Winform controls. See the repository license.
