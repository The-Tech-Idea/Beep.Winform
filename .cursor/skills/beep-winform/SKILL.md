---
name: beep-winform
description: Mandatory development rules for Beep.Winform controls, forms, painters, dialogs, theming, DPI, and commercial UI/UX implementation patterns.
---

# Beep.Winform Skill (Strict Rules)

Use this skill when creating or updating any Beep.Winform control, form, painter, helper, or DialogManager feature.

## 1) Non-Negotiable Rules

1. Always use Beep controls for business UI surfaces when available (not plain WinForms controls).
2. Any new custom control must inherit from `BaseControl`.
3. Keep painter pattern strict:
   - painters only render,
   - state changes live in control/layout/hit-test helpers.
4. Always use `StyledImagePainter` for image/svg/icon rendering.
5. Always use `BackgroundPainterFactory`, `BorderPainterFactory`, and `ShadowPainterFactory` for surface rendering.
6. Always use `BeepStyling` path/style helpers.
7. Always use DPI helpers (`DpiScalingHelper`) for pixel constants.
8. Never cache a DPI scale float; always scale via control reference at usage time.
9. For custom controls, inherit from `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.cs`.
10. For form/control visual identity, always align `FormStyle` and `BeepControlStyle` through `BeepStyling`.

## 2) Font and Theme Rules (Critical)

1. Never assign `Font` directly on controls.
2. Never create inline fonts with `new Font(...)`.
3. Never use `this.Font`, `Control.Font`, or painter-owned ad-hoc fonts for text rendering.
4. Fonts must come from theme typography via:
   - `BeepThemesManager.ToFont(theme.XxxStyle)` (preferred in current codebase), or
   - equivalent theme-to-font conversion utility used by the target module.
5. Resolve fonts in `ApplyTheme()` and store in `_textFont` (or typed font fields) and pass down to painters/helpers.
6. When theme changes, re-resolve fonts and invalidate layout/paint.

## 3) Beep Control Replacement Rules

When replacing standard controls, use:

- `Button` -> `BeepButton`
- `TextBox` -> `BeepTextBox`
- `Label` -> `BeepLabel`
- `ComboBox` -> `BeepComboBox`
- `ListBox` -> `BeepListBox`
- `ProgressBar` -> `BeepProgressBar`
- `CheckBox` -> `BeepCheckBox`

Set `UseThemeColors = true` on Beep controls where applicable.

## 4) Image and Icon Rules

1. Use `StyledImagePainter.Paint`, `PaintWithTint`, `PaintDisabled`.
2. Use icon resources from:
   - `TheTechIdea.Beep.Winform.Controls/IconsManagement/SvgsUI.cs`
   - `TheTechIdea.Beep.Winform.Controls/IconsManagement/Svgs.cs`
3. Prefer string-based icon paths (resource path pattern), not raw `Image` object painting.

## 5) DPI and Layout Rules

1. Scale all hardcoded dimensions:
   - `DpiScalingHelper.ScaleValue`
   - `ScaleSize`, `ScalePoint`, `ScaleRectangle`, `ScalePadding`
2. Keep rendering and hit-test geometry in the same coordinate system and same scaled metrics.
3. Do not mix absolute and margin-relative coordinate spaces.
4. On resize/theme/DPI changes: invalidate layout cache and hit areas.

## 6) Painter and Helper Design

1. Prefer partial-class split for complex controls:
   - main/properties/events,
   - painters,
   - drawing,
   - animation,
   - layout/hit-test helpers.
2. Distinct style painters are required; avoid style logic monoliths.
3. Every painter should support hover/focus/pressed/selected visuals where relevant.
4. Layout helper owns geometry + interaction detection; painter consumes geometry.

## 6.1) Inherit From BaseControl (How-To)

Use this baseline pattern for every new Beep custom control:

```csharp
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Helpers;

public class MyBeepControl : BaseControl
{
    private Font _textFont = SystemFonts.DefaultFont;

    public MyBeepControl()
    {
        IsChild = true;
        UseThemeColors = true;
        ApplyTheme();
    }

    public override void ApplyTheme()
    {
        base.ApplyTheme();
        if (CurrentTheme == null) return;

        // Theme font only (no direct Font assignment)
        _textFont = BeepThemesManager.ToFont(CurrentTheme.BodyMedium) ?? SystemFonts.DefaultFont;

        // Map form style to control style when needed
        ControlStyle = BeepStyling.GetControlStyle(FormStyle);
        Invalidate();
    }
}
```

Mandatory notes:
- Do not assign `Font` property in constructor or runtime.
- Do not create inline fonts.
- Keep text rendering on `_textFont` resolved from theme.

## 6.1.1) BeepButton Gold Standard Pattern

Reference:
- `TheTechIdea.Beep.Winform.Controls/Buttons/BeepButton.cs`
- `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.cs`
- `TheTechIdea.Beep.Vis.Modules2.0/IBeepUIComponent.cs`

Observed architecture to follow:
1. `BeepButton` inherits `BaseControl`.
2. `BaseControl` implements `IBeepUIComponent` (so derived controls inherit the interface contract automatically).
3. Rendering uses `DrawContent(Graphics g)` override pattern and calls `base.DrawContent(g)` first.
4. Theme application is explicit via `BeepButton.ApplyTheme()` and visual rendering stays in `DrawContent(...)`.

Mandatory rendering rule:
- For Beep controls, implement custom drawing in `DrawContent(...)`, not ad-hoc paint-only logic.
- Keep base pipeline intact by calling `base.DrawContent(g)` before control-specific rendering.

Actual code references:
- `TheTechIdea.Beep.Winform.Controls/Buttons/BeepButton.cs`
  - `public class BeepButton : BaseControl`
  - `public override void ApplyTheme()`
  - `protected override void DrawContent(Graphics g)`
- `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.cs`
  - `public partial class BaseControl : ContainerControl, IBeepUIComponent, ...`

Template:

```csharp
public class MyBeepControl : BaseControl
{
    protected override void DrawContent(Graphics g)
    {
        base.DrawContent(g); // keep BaseControl pipeline

        // custom drawing here using:
        // - theme fonts from BeepThemesManager.ToFont(...)
        // - StyledImagePainter for images/icons
        // - DpiScalingHelper for dimensions
    }
}
```

## 6.1.2) IBeepUIComponent Embedding Pattern (BeepCheckBox Example)

Reference:
- `TheTechIdea.Beep.Winform.Controls/CheckBoxes/BeepCheckBox.cs`
- `TheTechIdea.Beep.Winform.Controls/CheckBoxes/BeepCheckBox.IBeepComponent.cs`

Why this matters:
- Controls that follow this pattern can be embedded and orchestrated by host controls (for example, data-host surfaces like `BeepGridPro`) using a consistent value/filter/binding contract.

Pattern to follow:
1. Inherit from `BaseControl` (inherits `IBeepUIComponent` contract).
2. Keep `BoundProperty` explicit and meaningful for host binding.
3. Override value APIs so hosts can set/get/clear values generically.
4. Override filter APIs (`HasFilterValue`, `ToFilter`) so host containers can consume control state in query/filter flows.

Actual code references:
- `TheTechIdea.Beep.Winform.Controls/CheckBoxes/BeepCheckBox.IBeepComponent.cs`
  - `public override string BoundProperty { get; set; } = "State";`
  - `public override void SetValue(object value)`
  - `public override object GetValue()`
  - `public override void ClearValue()`
  - `public override bool HasFilterValue()`
  - `public override AppFilter ToFilter()`
- `TheTechIdea.Beep.Vis.Modules2.0/IBeepUIComponent.cs`
  - `void SetValue(object value)`
  - `object GetValue()`
  - `AppFilter ToFilter()`
  - `void ReceiveMouseEvent(HitTestEventArgs eventArgs)`
  - `void SendMouseEvent(IBeepUIComponent targetControl, MouseEventType eventType, Point screenLocation)`

Why this enables embedding in hosts like grids:
- Host controls can treat embedded editors uniformly through `IBeepUIComponent`.
- Value extraction/sync and filter extraction are standardized (`GetValue`/`ToFilter`).
- Host-level mouse dispatch can forward interaction through `SendMouseEvent` / `ReceiveMouseEvent`.

Template (matching checkbox pattern):

```csharp
public partial class MyControl : BaseControl
{
    public override string BoundProperty { get; set; } = "MyState";

    public override void SetValue(object value)
    {
        if (value != null)
        {
            // map incoming object to control state
        }
    }

    public override object GetValue()
    {
        // return current control value for host containers
        return /* current value */;
    }

    public override void ClearValue()
    {
        // reset control value
    }

    public override bool HasFilterValue()
    {
        return /* true when filterable value exists */;
    }

    public override AppFilter ToFilter()
    {
        return new AppFilter
        {
            FieldName = BoundProperty,
            FilterValue = GetValue()?.ToString(),
            Operator = "="
        };
    }
}
```

Required alongside this pattern:
- Keep rendering in `DrawContent(Graphics g)` and call `base.DrawContent(g)`.
- Keep fonts/theme/DPI/image rules from this skill (theme fonts only, `StyledImagePainter`, `DpiScalingHelper`).

## 6.2) FormStyle + ControlStyle + BeepStyling Usage

Reference files:
- `TheTechIdea.Beep.Winform.Controls/Styling/BeepStyling.cs`
- `TheTechIdea.Beep.Winform.Controls/Styling/FORM_STYLE_PAINTER_PLAN.md`
- `TheTechIdea.Beep.Winform.Controls/Styling/PAINTER_ARCHITECTURE.md`

Rules:
1. Form-level style drives control-level style mapping through:
   - `BeepStyling.GetControlStyle(FormStyle formStyle)`
2. Build shape paths via:
   - `BeepStyling.CreateControlStylePath(...)` or `CreateControlPath(...)`
3. Keep style painters factory-based and style-specific (do not collapse styles into one generic painter).
4. Respect painter architecture split:
   - Background / Border / Text / Button / Shadow / Path painters.
5. Use theme-aware color retrieval through style/theme helpers, not ad-hoc hardcoded colors.

Minimal pattern:

```csharp
var controlStyle = BeepStyling.GetControlStyle(FormStyle);
using var path = BeepStyling.CreateControlStylePath(DrawingRect, controlStyle);
// Then paint using BackgroundPainterFactory + BorderPainterFactory + ShadowPainterFactory
```

## 6.3) Theme Integration With BeepThemesManager

Reference file:
- `TheTechIdea.Beep.Winform.Controls/ThemeManagement/BeepThemesManager.cs`

Rules:
1. Resolve current theme through `BeepThemesManager`/`CurrentTheme`.
2. Use theme change flow to re-apply style + fonts:
   - update local `_textFont` from theme typography,
   - update mapped `ControlStyle`,
   - invalidate layout and paint.
3. All rendered text must come from theme typography conversion, not designer/static font values.

## 6.4) Child-Control Interactivity Helpers (Use This)

Reference:
- `TheTechIdea.Beep.Winform.Controls/BaseControl/Helpers/ControlInputHelper.cs`
- `TheTechIdea.Beep.Winform.Controls/BaseControl/Helpers/ControlHitTestHelper.cs`

Use these helpers for embedded interactive parts (including child components hosted inside complex controls like grids, cards, custom editors).

Rules:
1. Register interactive regions/components via `ControlHitTestHelper.AddHitArea(...)`.
2. Route mouse lifecycle through `ControlInputHelper` (`OnMouseEnter/Move/Down/Up/Click/...`) instead of ad-hoc event duplication.
3. For embedded `IBeepUIComponent` children, use hit-test helper event routing:
   - `SendMouseEvent(...)`
   - `ReceiveMouseEvent(...)`
4. Keep hit detection + interaction state in helpers (`IsHovered`, `IsPressed`, `IsHit`), not painters.
5. Trigger actions only on click flow (`HandleClick`) to avoid duplicate execution on hover/move.

Actual code references:
- `TheTechIdea.Beep.Winform.Controls/BaseControl/Helpers/ControlInputHelper.cs`
  - `OnMouseEnter`, `OnMouseMove`, `OnMouseDown`, `OnMouseUp`, `OnClick`
  - `ProcessDialogKey(Keys keyData)`
  - `ReceiveMouseEvent(HitTestEventArgs eventArgs)`
- `TheTechIdea.Beep.Winform.Controls/BaseControl/Helpers/ControlHitTestHelper.cs`
  - `AddHitArea(string name, Rectangle rect, IBeepUIComponent component, Action hitAction)`
  - `SendMouseEvent(IBeepUIComponent targetControl, MouseEventType eventType, Point screenLocation)`
  - `HandleMouseMove`, `HandleMouseDown`, `HandleMouseUp`, `HandleClick`
  - Internal dispatch of `MouseEnter/Move/Leave/Down/Up/Click/Hover` to embedded component

Step-by-step embedding workflow (recommended):
1. During layout refresh, register each embedded interactive region with `_hitTest.AddHitArea(...)`.
2. In the parent control input overrides, forward mouse events to `_input` methods.
3. Let `_input` call `_hitTest` to detect active region and dispatch to embedded `IBeepUIComponent`.
4. Keep painter pure: painter reads state only (`IsHovered`, `IsPressed`, `IsHit`) and paints visuals.
5. Keep business actions in control/helper callbacks (`hitAction`), not inside painter.

Common mistake to avoid:
- Do not call embedded actions from hover/move branches; only trigger command actions in click path.

Minimal pattern:

```csharp
// During layout refresh:
_hitTest.AddHitArea("CellEditor", editorRect, embeddedBeepComponent, () => OnEditorClick());

// In input flow:
_input.OnMouseMove(e.Location);
_input.OnMouseDown(e);
_input.OnMouseUp(e);
_input.OnClick();
```

This pattern enables reusable controls to be embedded in host controls (e.g., grid-like surfaces) with consistent interaction semantics.

## 7) DialogManager-Specific Rules

1. `IDialogManager` public surface should be async-first.
2. Do not use `Application.DoEvents()` pump-based compatibility wrappers unless explicitly requested.
3. For long-running dialogs (busy/progress), use async handles + cancellation support.
4. Dialog UI should follow Beep-controls compliance, DPI scaling, and theme-font rules in this skill.

## 8) Commercial UI/UX Quality Bar

Implement interactions at a commercial standard (DevExpress/Figma-inspired):

- smooth hover/press transitions,
- focus visibility and keyboard navigation,
- consistent backdrop/elevation behavior,
- clear visual hierarchy and spacing,
- responsive layout under compact/comfortable density.

## 9) Explicit Anti-Patterns (Do Not Use)

- `new Font(...)`
- setting `Font = ...` directly
- using unscaled pixel constants in layout/render paths
- using plain WinForms controls where Beep equivalent exists
- mutating control state inside painter rendering methods
- direct `g.DrawImage(...)` for themed icons/images instead of `StyledImagePainter`

## 10) Implementation Checklist

- [ ] Inherits from `BaseControl` (for new controls)
- [ ] Theme fonts resolved via `BeepThemesManager.ToFont(...)`
- [ ] No direct `Font` assignments
- [ ] No inline `new Font(...)`
- [ ] All size/spacing values use `DpiScalingHelper`
- [ ] Images/icons rendered via `StyledImagePainter`
- [ ] Beep controls used instead of standard WinForms controls
- [ ] Painters are render-only; helpers/control manage state
- [ ] Hover/focus/pressed/selected states visually supported

## 11) Key Paths

- `TheTechIdea.Beep.Winform.Controls/ThemeManagement/BeepThemesManager.cs`
- `TheTechIdea.Beep.Winform.Controls/Styling/ImagePainters/StyledImagePainter.cs`
- `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.cs`
- `TheTechIdea.Beep.Winform.Controls/Buttons/BeepButton.cs`
- `TheTechIdea.Beep.Winform.Controls/CheckBoxes/BeepCheckBox.IBeepComponent.cs`
- `TheTechIdea.Beep.Vis.Modules2.0/IBeepUIComponent.cs`
- `TheTechIdea.Beep.Winform.Controls/BaseControl/Helpers/ControlInputHelper.cs`
- `TheTechIdea.Beep.Winform.Controls/BaseControl/Helpers/ControlHitTestHelper.cs`
- `TheTechIdea.Beep.Winform.Controls/Styling/BeepStyling.cs`
- `TheTechIdea.Beep.Winform.Controls/Styling/FORM_STYLE_PAINTER_PLAN.md`
- `TheTechIdea.Beep.Winform.Controls/Styling/PAINTER_ARCHITECTURE.md`
- `TheTechIdea.Beep.Winform.Controls/IconsManagement/`
- `TheTechIdea.Beep.Winform.Controls/Helpers/DpiScalingHelper.cs`
- `.cursor/skills/beep-controls-usage/SKILL.md`
- `.cursor/skills/beep-dpi-fonts/SKILL.md`