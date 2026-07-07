---
name: beep-winform-design
description: Beep WinForms design reference — pick the right BeepControlStyle, alignment grid, sizing tokens, padding/border/shadow per style, and per-control DefaultSize templates. Use when designing, laying out, or restyling any Beep WinForms surface (forms, dialogs, cards, buttons, fields, lists, charts, navs). Covers all 60+ BeepControlStyle values, the StyleBorders/StyleShadows/StyleSpacing tables, theme tokens, accessibility rules, and DPI scaling.
---

# Beep WinForms Design System

Use this skill when designing Beep.Winform surfaces — choosing a style, aligning controls, sizing controls, picking paddings/borders/shadows, or restyling existing layouts.

## Use this skill when

- Picking a `BeepControlStyle` for a new form/dialog/control (Material3, Fluent2, iOS15, Neumorphism, Brutalist, etc.)
- Laying out controls in a 2-column form, dialog, card grid, or toolbar
- Choosing `BorderRadius`, `BorderThickness`, `ShadowOffset`, `ShadowOpacity`, or `CustomPadding`
- Migrating from hardcoded pixel sizes to DPI-aware `BeepLayoutMetrics` tokens
- Reviewing a layout for WCAG touch-target / contrast compliance
- Adding a `DefaultSize` override to a Beep control that lacks one
- Restyling existing controls to match a different design system

## Do not use this skill when

- Working on BeepDM data-source plumbing → use `beepdm` / `beep-data-connection` skills
- Wiring `BeepBlock`/`BeepForms` smart-tag editors → use `beep-block` / `beep-forms-integrated`
- Pure runtime WinForms mechanics (WndProc, painting pipeline) → use `beep-winform`
- Picking an icon SVG path → use `beep-icons` or `BeepIconsManagement` skill

## Source of Truth

All numeric values come from these helpers. Never hardcode them — read from the helpers so the system updates in one place:

| Property | File | API |
|---|---|---|
| Border radius per style | `TheTechIdea.Beep.Winform.Controls/Styling/Borders/StyleBorders.cs` | `StyleBorders.GetRadius(BeepControlStyle)` |
| Border width per style | same | `StyleBorders.GetBorderWidth(style)` |
| Selection radius per style | same | `StyleBorders.GetSelectionRadius(style)` |
| Shadow blur/spread/offset/color | `TheTechIdea.Beep.Winform.Controls/Styling/Shadows/StyleShadows.cs` | `StyleShadows.GetShadowBlur/Offset/Color(style)` |
| Padding per style | `TheTechIdea.Beep.Winform.Controls/Styling/Spacing/StyleSpacing.cs` | `StyleSpacing.GetPadding(style)` |
| Item gap per style | same | `StyleSpacing.GetItemSpacing(style)` |
| Aggregator | `TheTechIdea.Beep.Winform.Controls/Styling/BeepStyling.cs` | `BeepStyling.GetRadius/Width/Padding/...` |
| Enum itself | `TheTechIdea.Beep.Winform.Controls/Common/BeepControlStyle.cs` | 60+ values |

## Style Classes (decision matrix)

The 60+ `BeepControlStyle` values group into ~10 visual families. Pick by the look you want first, then refine with the specific token.

| Family | Use when | Typical radius | Shadow | Border width |
|---|---|---|---|---|
| **Modern web** (Material3, iOS15, Fluent2, AntDesign, MaterialYou, ChakraUI, TailwindCard, Apple) | Most modern desktop apps | 4–16 px | subtle (2–12 px blur, 1–4 px offset Y) | 0–2 px |
| **Flat minimal** (Minimal, NotionMinimal, VercelClean, Office, Gnome, Elementary) | Content-focused UIs, dense forms | 0–6 px | none or tiny | 2 px |
| **Bold / sharp** (Metro, NeoBrutalist, Brutalist, HighContrast, Retro, Gaming) | Aggressive UI, dashboards with strong hierarchy | 0 px (sharp) | none or hard offset (no blur) | 2–3 px |
| **Glow / colored** (DarkGlow, Neon, NeonGlow, Kde, Cyberpunk, Holographic, Gaming) | Dark themes, gaming/entertainment | 0–12 px | **glow** (large blur, zero offset, colored) | 0–2 px |
| **Glass / soft** (GlassAcrylic, Glassmorphism, GradientModern, MacOSBigSur, Windows11Mica) | Modern desktop, hero sections | 10–14 px | subtle / no shadow | 0–2 px |
| **Neumorphic** (Neumorphism) | Soft extruded UI | 4 px (small) | **dual** (dark below-right + light above-left) | 0 px |
| **Card / dashboard** (StripeDashboard, FigmaCard, Bootstrap, WebFramework) | SaaS dashboards | 4–8 px | subtle elevation (6–12 px blur, 2–6 px offset Y) | 2 px |
| **Pill / rail** (PillRail) | Pill navigation rails | 6 px (rail) / 100 px (selected) | none | 0 px |
| **OS desktop** (ArcLinux, Ubuntu, Kde, Cinnamon, Gnome, Elementary) | Native desktop look | 4–8 px | OS-typical | 2 px |
| **Editorial / palette** (Dracula, Nord, Nord, OneDark, Solarized, GruvBox, Tokyo, Paper) | Code editor / themed IDE look | 4–8 px | subtle | 2 px |
| **Novel** (Cartoon, ChatBubble, Brutalist, Effect, Terminal) | Stylized UI | 0–4 px | varies | 2–3 px |
| **Next-gen (2024-2025)** (Shadcn, RadixUI, NextJS, Linear) | Modern web component aesthetic | 6–8 px | very subtle (1–5 px blur, 1–2 px offset Y) | 2 px |

Full per-style table (every value, every style) is in `reference.md`.

## Sizing Tokens (`BeepLayoutMetrics`)

Use these instead of hardcoded pixel values. They live in `TheTechIdea.Beep.Winform.Controls/Layouts/Helpers/BeepLayoutMetrics.cs`.

### Dialogs & forms
| Token | Value | When |
|---|---|---|
| `DialogSmall` | 420×320 | Login, simple prompts, single-action confirmations |
| `DialogMedium` | 600×460 | Property editors, multi-field forms |
| `DialogLarge` | 840×560 | Data grids, wizards with multiple panels |
| `DialogPadding` | 12 px | Outer form/dialog padding |
| `ContainerPadding` | 8 px | Inner panel/group padding |
| `ButtonStripPd` | 10 px | Footer button bar padding |
| `HeaderPadding` | (16,8,16,8) | Page header bar |
| `LabelColumnWidth` | 130 px | Label column in 2-col forms |
| `TextRowHeight` | 35 px | Single-line field rows |
| `InterRowSpacing` | 5 px | Vertical gap between form rows |
| `ButtonGap` | 8 px | Horizontal gap between buttons in a bar |
| `SmallGap` | 4 px | Tight gap (chip rows, segmented controls) |
| `CornerRadius` | 4 px | Generic corner radius fallback |

### Buttons
| Token | Value | When |
|---|---|---|
| `ButtonSmall` | 80×28 | Icon buttons, toolbar actions |
| `ButtonStandard` | 100×32 | Default button (Cancel, secondary) |
| `ButtonToolbar` | 110×32 | Toolbar buttons |
| `ButtonLarge` | 130×36 | Primary CTA (OK, Save, Login) |
| `MinTouchTarget` | 44 px | WCAG minimum touch size |

### Typography
| Token | Value | When |
|---|---|---|
| `TitleFontSize` | 16 px | Dialog title |
| `SubtitleFontSize` | 12 px | Section subtitle |
| `BodyFontSize` | 9 px | Field labels and body |

### DPI scaling
Always scale tokens per control. Helper methods already exist:
```csharp
Size sz = BeepLayoutMetrics.ButtonStandard.ScaleSize(this);     // scales both dimensions
int pad = BeepLayoutMetrics.DialogPadding.Left.ScaleValue(this); // scales a single int
Padding pd = BeepLayoutMetrics.DialogPadding.ScalePadding(this); // scales all 4 sides
```

## Alignment Rules

### Layout container decision (read first)
Microsoft's official guidance: **"In general, you should not use a TableLayoutPanel control as a container for the whole layout. Use TableLayoutPanel controls to provide proportional resizing capabilities to parts of the layout."** ([Microsoft Learn — Arranging Controls Using a TableLayoutPanel](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/walkthrough-arranging-controls-on-windows-forms-using-a-tablelayoutpanel)). Nest layout panels — don't try to express a whole form with one.

| When you need | Use | Why |
|---|---|---|
| Top-level form scaffold (nav top/bottom, content fills the rest) | **`Dock`** chain | Z-order matters: dock `Top`, then `Top`, then `Fill` |
| Resizable two-pane (master-detail, nav + content, properties + preview) | **`SplitContainer`** | Built-in splitter bar, splitter distance in DPI-aware pixels |
| Form fields in a label-input grid (2-col / N-col) | **`TableLayoutPanel`** with mixed Absolute/Percent column styles | Localizable, proportional resize, rows can `AutoSize` |
| Items that wrap or flow (button bars, tag lists, breadcrumbs) | **`FlowLayoutPanel`** with `FlowDirection` + `WrapContents` | Wraps naturally when resized; respects RTL |
| Free-form absolute positioning | **`Anchor`** | Only when the other patterns don't fit (rare) |
| Whole-form proportional grid (dashboard tiles) | Nested **`TableLayoutPanel`** inside a Dock-Fill parent | Top panel = Dock Fill, internal = TableLayoutPanel |

### 2-column form layout (the Beep default — the most common case)
```
TableLayoutPanel { ColumnCount=2, Padding=DialogPadding.ScalePadding() }
  Column 0: Absolute LabelColumnWidth.Scale()        // left label column, fixed
  Column 1: Percent 100F                              // right input column, stretches
  Rows: AutoSize or Absolute TextRowHeight.Scale()
```
- Row count = 1 header + N fields + 1 button bar
- Between rows: `InterRowSpacing.Scale()`
- Left column holds `BeepLabel` (Right-to-Left text alignment optional for Arabic)
- Right column holds the input control sized to fill (`Dock = Fill`)

### Button bar (footer)
```
FlowLayoutPanel { FlowDirection=RightToLeft, Padding=ButtonStripPd.ScalePadding() }
  Primary CTA (rightmost): ButtonLarge   — OK, Save, Login
  Secondary:                ButtonStandard — Cancel, Close
  Tertiary:                 ButtonSmall   — Help, About
  Gap between buttons: ButtonGap.Scale()
```

### Card grid (dashboard tiles)
```
TableLayoutPanel (Dock = Fill)                // top-level
  Padding: ContainerPadding.ScalePadding()
  ColumnCount: N (one per tile)
  ColumnStyle: Percent 100F / N                // equal widths
  RowStyle:    AutoSize                        // row fits tallest tile
  Cell contents: FlowLayoutPanel { FlowDirection=TopDown } holding a BeepCard
                                                // so cards are centered in their cell
```

### Toolbar
```
FlowLayoutPanel (LeftToRight)
  Items: ButtonToolbar each
  Padding: ButtonStripPd.ScalePadding()
  Gap: SmallGap.Scale() between items
```

### Sidebar nav
- Width: 240–280 px expanded, 60–72 px collapsed (BeepSideBar manages this internally)
- Item height: 44 px (touch-target compliant)
- Item gap: 4 px

### Three SizeType rules for TableLayoutPanel rows/columns
| SizeType | Behavior | Use for |
|---|---|---|
| **Absolute** | Fixed pixel size, never changes | Label column widths, button heights, fixed chrome |
| **AutoSize** | Sized to fit the largest child | Field rows that follow content height |
| **Percent** | Takes remaining space as share | Stretchable input columns, filler rows |

**Golden rule:** in a vertical form, use **Absolute** for label columns + **Percent** for input columns. Use **AutoSize** for rows. Avoid mixing all three in the same dimension — it confuses both designer and runtime.

### One control per cell
TableLayoutPanel **only allows one child per cell**. If you need multiple controls in a region, wrap them in:
- A nested `TableLayoutPanel` (best for grids)
- A `FlowLayoutPanel` (best for buttons / chips)
- A `Panel` (best for arbitrary overlap or visual grouping)
- A `BeepCard` or other Beep container (if you want theming)

### Set `GrowStyle` deliberately
`TableLayoutPanel.GrowStyle` defaults to `AddRows` (when overflowing) but you can choose:
- `AddRows` (default) — grows downward
- `AddColumns` — grows rightward
- `FixedSize` — refuses overflow with an error (use for fixed-size dialogs)

For forms that build rows dynamically (e.g. add a field per iteration), prefer `AddRows`.

### Anchor / Dock inside TableLayoutPanel
This is the **#1 cause of layout bugs**:
- **`Dock = Fill`** inside a cell → fills the cell. ✅ Default for input controls in a 2-col form.
- **`Anchor`** inside a cell behaves differently than in a `Panel` — the control sizes/positions itself relative to the cell edges, but the cell may not be the same size as the parent. For predictable layout, prefer **`Dock = Fill`** in cells.
- Setting **both** Anchor and Dock on a child of TableLayoutPanel can give surprising results — pick one.

### AutoSizeMode on rows/columns
A `TableLayoutPanel` cell always shrinks the child to fit until the child's `MinimumSize` — **except** when the row or column is `AutoSize`. So:
- If you want child controls to size to the cell: use `Percent` or `Absolute` rows/cols.
- If you want the row/col to size to the largest child: use `AutoSize`.

### Sizing tokens for layout panels (use these instead of hardcoding)
```csharp
new TableLayoutPanel {
    Padding     = BeepLayoutMetrics.DialogPadding.ScalePadding(this),
    ColumnStyles = {
        new ColumnStyle(SizeType.Absolute, BeepLayoutMetrics.LabelColumnWidth.ScaleValue(this)),
        new ColumnStyle(SizeType.Percent, 100F),
    },
    RowStyles = {
        new RowStyle(SizeType.Absolute, BeepLayoutMetrics.TextRowHeight.ScaleValue(this)),
        new RowStyle(SizeType.AutoSize),  // button bar grows with content
    },
};
```

## Padding, Border & Shadow Rules

The 3 visuals compose — set them in this order to avoid mismatched chrome:

1. **Padding** = `StyleSpacing.GetPadding(style)` from the chosen style + any `CustomPadding` for asymmetric gaps
2. **Border radius** = `StyleBorders.GetRadius(style)` (default `BorderRadius` on each control already wires to style)
3. **Border width** = `StyleBorders.GetBorderWidth(style)` (set on `BorderThickness` property)
4. **Shadow** — only paint if `StyleShadows.HasShadow(style)` returns true; otherwise the shadow token is wasted space
   - `ShadowOffset` ← `GetShadowOffsetY(style)` (Y-only; X is set separately via the helper)
   - Shadow color via `GetShadowColor(style)`
   - Blur via `GetShadowBlur(style)` — already wired into the painter for style-based shadow

**Rule of thumb:** the sum of `padding + border + shadow` is the control's chrome overhead. When sizing controls, leave room for it on every side. `BaseControl.CalculateStyleChrome(style)` returns this for a given style.

### Override patterns
- To keep the style padding but add extra top spacing → set `CustomPadding = new Padding(0, 8, 0, 0)` (asymmetric works)
- To force a sharp border on a Material3 control → set `BorderRadius = 0` after setting style
- To disable shadow on a normally-shadowed style → set `ShowShadow = false`
- To override border width → set `BorderThickness` after style is applied

## DefaultSize Policy

Every Beep control that derives from a WinForms UI base **should** override `DefaultSize`. This:
- Gives Visual Studio a sensible size when the control is first dropped onto a form
- Matches what `GetPreferredSize` returns for an empty control
- Avoids 0×0 controls in the designer until the user resizes

The reference.md has a per-control template list. If you're adding a control and don't see a recommended size, use this heuristic:

| Control type | Recommended default | Examples |
|---|---|---|
| Action button | 100×32 | BeepButton = 100×36 |
| Icon-only button | 32×32 or 28×28 | toolbar icon buttons |
| Input field (text/combo/date) | 200×34 | BeepTextBox = 200×34 |
| Multi-line input | 200×80 | textareas, multiline textboxes |
| Label | 120×24 | inline labels |
| Checkbox / Toggle / Switch | 44×24 or 180×24 (with label) | WCAG touch-target height |
| Card | 280×160 | general-purpose card |
| Metric tile | 200×120 | dashboard KPI tile |
| Panel / container | 300×200 | generic grouping surface |
| Sidebar | 240×600 | navigation rail |
| NavBar | 800×48 | top navigation |
| Breadcrumb | 300×36 | already set |
| Notification | 320×80 | toast |
| Chart | 400×300 | already set |
| Grid | 600×300 | data grid |
| Tab strip | 400×32 | tab headers |
| Calendar | 280×320 | already set |
| Stepper | 320×32 | multi-step indicator |
| Rating | 120×24 | 5-star rating |
| Badge / Chip | 24×24 (dot) or 60×24 (text) | small adornments |

DPI scaling reminder: the default is set in design units (96 DPI); framework + `DpiScalingHelper` handles runtime scaling. For DPI-aware defaults, wrap with `DpiScalingHelper.ScaleValue(w, this)` as `BeepBreadcrump` does.

## Controls Belong in designer.cs (Always)

**Rule:** Every control placed on a host (form or composite control) must be created at **design time** by the Visual Studio designer and serialized into the host's `*.Designer.cs` file. The user must be able to see, select, move, resize, and edit every child control in the designer without writing code.

**Never** instantiate child controls in a constructor or `Load` handler — those controls are invisible to the designer and will vanish on every designer reload.

### Sizing and location are part of the contract

The same rule applies to control geometry:
- `new Size(...)` and `new Point(...)` for every child control belongs in `*.Designer.cs`.
- The usercontrol's own `Size = new Size(...)` and `AutoScaleDimensions` likewise.
- `Padding` and `Margin` literals on every control likewise.
- Do **not** set `Dock`/`Size`/`Location`/`Padding`/`Margin`/`AutoSize` from code-behind after `InitializeComponent()` unless the value is **DPI-derived** (`BeepLayoutMetrics.*.Scale*()`).

The Designer is the single source of truth for layout, geometry, and child instantiation. Code-behind may only adjust runtime state (data binding, event wiring, view-model assignment) and DPI-scaled dimensions.

**Never** bulk-edit designer files to add or remove child controls via `Edit`; use the Visual Studio designer or `IDesignerHost.CreateComponent` pattern. The pattern below is what an IDE round-trip looks like.

### How to make a custom control auto-add a child at design time

When you ship a Beep control that *must* contain another control (e.g. `BeepComboBox` has a built-in search textbox; `BeepTabbedView` has a default tab), use the **`IDesignerHost.CreateComponent`** pattern:

```csharp
// 1. Apply [Designer] to the host control
[Designer(typeof(BeepComboBoxDesigner))]
public class BeepComboBox : BaseControl
{
    // 2. Expose the child via a property marked Content so VS serializes it
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public BeepTextBox SearchBox { get; set; }

    public BeepComboBox()
    {
        // 3. Constructor stays EMPTY — designer will create the child
    }
}

// 4. In the Designer, call host.CreateComponent
public class BeepComboBoxDesigner : ControlDesigner
{
    public override void Initialize(IComponent component)
    {
        base.Initialize(component);
        var host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host == null || component is not BeepComboBox combo || combo.SearchBox != null) return;

        var change = (IComponentChangeService)GetService(typeof(IComponentChangeService));
        var prop = TypeDescriptor.GetProperties(combo)["SearchBox"];

        change?.OnComponentChanging(combo, prop);
        var tb = (BeepTextBox)host.CreateComponent(typeof(BeepTextBox));
        tb.Dock = DockStyle.Top;
        combo.Controls.Add(tb);
        combo.SearchBox = tb;
        change?.OnComponentChanged(combo, prop, null, tb);
    }
}
```

This pattern is already used by `BeepBlock`, `BeepForms`, and `BeepDock` — read their `*.Designer.cs` files in `Controls.Design.Server/Designers/` for working examples.

### When runtime creation **is** allowed (and required)

| Scenario | Why runtime is OK | Example |
|---|---|---|
| User explicitly requested a new dialog | User action, not layout | `BeepDialogManager.ShowDialog(...)` |
| User clicked "+ Add" on a list | User added data, not layout | `BeepListBox.AddItem(...)` |
| Plugin / extension loaded at startup | External config | plugin host |
| Notification triggered by app event | User/system event | `BeepNotificationManager.Push(...)` |
| Tab content added at runtime via API | API surface, not design surface | `BeepTabs.AddTab(...)` |
| Data-driven rows in a grid | Grid renders data, not layout | `BeepGridPro.DataSource = ...` |

The rule: **if a control's *existence* is a design-time concern (layout, structure), it goes in designer.cs. If a control's existence is a runtime event (user action, plugin, data), it's OK to create it at runtime.**

### Anti-patterns

❌ `new BeepButton()` in a constructor — designer can't see it, reload erases it
❌ `this.Controls.Add(new BeepLabel())` in `Load` or `OnLoad` — same problem
❌ `Form designer.cs new BeepXxx()` in code-behind without designer support — will be wiped on next designer save
❌ Instantiating inside a property getter — control appears once, dies, appears again
❌ `InitializeComponent()` calling code that creates controls in another control — breaks round-trip

✅ **Right way:** design-time placement in `*.Designer.cs` only; runtime creation only behind explicit user actions or plugin/data events.

## How to Add a Control at Design Time (the right way)

When you drag a `BeepButton` (or any other control) from the Visual Studio toolbox onto a Form or UserControl surface, the designer serializes it into the host's `*.Designer.cs` file inside `InitializeComponent()`. You almost never write this code by hand — the IDE generates it for you. The pattern below is what the IDE produces and what your code should match when adding controls programmatically through a `Custom Designer`.

### The 3 files per form/control

| File | Purpose | Edit by hand? |
|---|---|---|
| `MyForm.cs` | Event handlers, business logic, partial declarations | Yes |
| `MyForm.Designer.cs` | Auto-generated layout (control instances, properties, event wiring) | **Rarely** — IDE regenerates it |
| `MyForm.resx` | Localized strings, embedded resources | Yes (via IDE editor) |

### Anatomy of a designer.cs file (Microsoft convention)

```csharp
// MyForm.Designer.cs
namespace TheTechIdea.Beep.MyApp
{
    partial class MyForm                       // ← only the partial declaration
    {
        private IContainer components = null;   // ← required designer variable

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) { components.Dispose(); }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()      // ← IDE regenerates the body
        {
            // 1. Resource manager (for resx strings)
            var resources = new ComponentResourceManager(typeof(MyForm));

            // 2. Create all child control instances first
            this.beepPanel1   = new BeepPanel();
            this.beepTextBox1 = new BeepTextBox();
            this.beepButton1  = new BeepButton();

            // 3. For each container, bracket its setup with SuspendLayout/ResumeLayout(false)
            this.beepPanel1.SuspendLayout();
            this.SuspendLayout();

            // 4. Set properties for each control
            this.beepPanel1.Dock   = DockStyle.Fill;
            this.beepPanel1.Padding = new Padding(8);

            // 5. Add children to their parents (inside the parent’s ResumeLayout block)
            this.beepPanel1.Controls.Add(this.beepTextBox1);
            this.beepPanel1.Controls.Add(this.beepButton1);

            // 6. Form-level setup
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode       = AutoScaleMode.Font;
            this.ClientSize          = new Size(600, 400);
            this.Controls.Add(this.beepPanel1);
            this.Name = "MyForm";
            this.Text = "My Form";

            // 7. Close all brackets — inner to outer
            this.beepPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        // Fields the designer creates (private!) — listed at the BOTTOM of the file
        private BeepPanel   beepPanel1;
        private BeepTextBox  beepTextBox1;
        private BeepButton   beepButton1;
    }
}
```

### The 7-rule quick check for a healthy designer.cs

1. **Partial class only** — never put `public class MyForm` in `Designer.cs`; use `partial class`.
2. **`IContainer components` field** — required for `Dispose` and component tray support.
3. **Override `Dispose(bool)`** — disposes the components container; auto-generated by IDE.
4. **Brackets every container with `SuspendLayout()` / `ResumeLayout(false)`** — one pair per container + one for `this` at the outer level. `false` = don't perform layout immediately (batched).
5. **Create instances first, then set properties** — the IDE follows this order; respect it.
6. **`Controls.Add()` happens INSIDE the parent’s `SuspendLayout` block** — otherwise you get a layout pass per child.
7. **Fields are `private`** at the BOTTOM of the file — never `public`; never `protected`; code-behind accesses them via the `designer` field or through a property wrapper.

### The critical anti-patterns (these break the IDE round-trip)

❌ **Reordering or hand-editing** code inside `#region Windows Form Designer generated code` — the IDE will throw it away on the next regen. If you need to do something tricky, do it in `MyForm.cs`, not in `Designer.cs`.
❌ **Calling designer fields from the constructor** — fields are `null` until `InitializeComponent()` runs. Use `Load` / `Shown` events.
❌ **Setting `BeepLayoutMetrics.X` directly in designer.cs** — IDE will overwrite the literal value on every regen. Set it in code-behind after the designer constructor call, or use a custom designer verb.
❌ **Adding new fields to designer.cs manually** — IDE will delete them on the next save. Use a Custom Designer to register them properly.
❌ **Making designer fields `public`** — breaks encapsulation; will be flagged in code review. Use a property:

```csharp
// In MyForm.cs (code-behind)
public BeepButton SubmitButton => beepButton1;   // private field, public read-only access
```

### The IDesignerHost pattern (for custom Beep controls)

When you ship a Beep control that *must* contain another control (e.g. `BeepComboBox` with a built-in `SearchBox`), the right way is a **Custom Designer** that calls `IDesignerHost.CreateComponent` to formally register the child. See the full pattern in `reference.md` § 9.2 and § 10.

Reference implementations in this repo:
- `TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/BeepiFormProDesigner.cs`
- `TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/BeepBlockDesigner.cs`
- `TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/BeepCardDesigner.cs`
- `TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/BaseBeepParentControlDesigner.cs` (shared base)

## Theme Tokens

The `IBeepTheme` (`BeepThemesManager`) provides colors via reflection: pass a suffix-less key like `"Primary"`, `"Background"`, `"Border"`, `"Shadow"`, `"Accent"` — the helper appends `"Color"`. Common keys:

```
Primary, Secondary, Accent, Background, Foreground, Border,
HoverBackground, PressedBackground, DisabledBackground,
SelectedBackground, FocusBorder, HoverBorder, PressedBorder,
Shadow, ErrorColor, WarningColor, SuccessColor, InfoColor
```

Always set `UseThemeColors = true` on Beep controls so they pick up the current theme without manual binding. Apply the active theme to a form with `BeepThemesManager.ApplyTheme(form)` or assign `form.CurrentTheme = BeepThemesManager.CurrentTheme`.

## Accessibility Checklist

- [ ] TabIndex follows logical left-to-right, top-to-bottom
- [ ] All icon-only controls have `AccessibleName` + tooltip
- [ ] Interactive height ≥ `MinTouchTarget.ScaleValue(this)` (44 px @ 96 DPI)
- [ ] Text contrast ≥ `MinContrast` (4.5 for body, 3.0 for large)
- [ ] `CancelButton` set on modal dialogs
- [ ] `AcceptButton` set on data-entry dialogs
- [ ] Focus indicator visible against the background
- [ ] Color is not the sole state indicator (combine with icon or text)
- [ ] All pixel values flow through `BeepLayoutMetrics` or `DpiScalingHelper`

## Anti-Patterns

❌ **Hardcoded pixel values** without `DpiScalingHelper` or `.Scale()`
❌ **Plain WinForms `Button`, `TextBox`, `Label`** where a `Beep*` equivalent exists
❌ **Inconsistent padding** within the same dialog (mixing 8/12/16 randomly)
❌ **Missing `CancelButton`/`AcceptButton`** on modal dialogs
❌ **Buttons without tooltips** when only icons show
❌ **Mixing style border radius and custom `BorderRadius`** without resetting `IsRounded`
❌ **`BorderRadius > min(Width, Height) / 2`** — produces a pill on its own, which is fine, but the painter may misbehave at extreme values
❌ **Setting `BackColor = Color.Transparent`** on a `Control` (not `Panel`) → design-time crash in out-of-process designer (see AGENTS.md)
❌ **`async void` event handlers** without try-catch in UI code
❌ **`Task.Run` wrapping** of `new Form()` + `ShowDialog()` → causes blank forms (AGENTS.md rule)
❌ **One giant `TableLayoutPanel` for the whole form** — Microsoft explicitly says don't. Nest: Dock chain for top-level, TableLayoutPanel for fields, SplitContainer for panes.
❌ **Anchor + Dock on the same child of a `TableLayoutPanel`** — pick one (use `Dock = Fill` for inputs)
❌ **Two controls in one TableLayoutPanel cell** — silently reparented; wrap in nested panel
❌ **Setting `Location` inside a `TableLayoutPanel`** — engine ignores it; use `Cell`/`Column`/`Row`/`ColumnSpan`/`RowSpan`
❌ **Pixel sizes for `ColumnStyles`/`RowStyles`/`Padding`/`SplitterDistance`** — always scale via `BeepLayoutMetrics` tokens
❌ **Absolute positioning with `Location`/`Size` on a non-design-time form** — use layout containers instead
❌ **Mixing Absolute/Percent/AutoSize in the same dimension without a clear pattern** — pick one rule per dimension

## Typical Workflow

1. **Pick a style** from the family table for the project mood
2. **Set the form's `ControlStyle`** (or call `BeepThemesManager.SetControlStyle(...)`)
3. **Lay out the surface** with `BeepLayoutMetrics` tokens, scaled via `.ScaleSize(this)`
4. **Place Beep controls** (not WinForms primitives) with the right `DefaultSize`
5. **Override individual style tokens** (`BorderRadius`, `CustomPadding`) only when needed
6. **Verify DPI scaling** by changing Windows display scale and re-checking layout
7. **Run accessibility checklist** before merge

## File Locations

- Style enum: `TheTechIdea.Beep.Winform.Controls/Common/BeepControlStyle.cs`
- Style tables: `TheTechIdea.Beep.Winform.Controls/Styling/{Borders,Shadows,Spacing}/`
- Aggregator: `TheTechIdea.Beep.Winform.Controls/Styling/BeepStyling.cs`
- Theme manager: `TheTechIdea.Beep.Winform.Controls/ThemeManagement/BeepThemesManager.cs`
- Metrics tokens: `TheTechIdea.Beep.Winform.Controls/Layouts/Helpers/BeepLayoutMetrics.cs`
- DPI helpers: `TheTechIdea.Beep.Winform.Controls/Helpers/DpiScalingHelper.cs`
- Base styling properties: `TheTechIdea.Beep.Winform.Controls/BaseControl/BaseControl.Properties.cs`
- Theme folder: `TheTechIdea.Beep.Winform.Controls/Themes/`

## Related Skills

- `beep-controls-usage` — list of every Beep control + mapping from WinForms primitives
- `beep-ux-design` — higher-level dialog/form composition patterns (reuses these tokens)
- `beep-winform` — core development rules (designer hosting, async-dialog anti-patterns)
- `beep-dpi-fonts` — DPI scaling and font management
- `beep-icons` — picking icon SVG paths

## Detailed Reference

See [`reference.md`](./reference.md) for:
- **Full style catalogue** — every `BeepControlStyle` value with its radius / border-width / padding / shadow blur / shadow offset / shadow color
- **Per-control DefaultSize templates** — recommended size for every Beep control type
- **Per-style recommended controls** — which controls pair best with which style
- **Examples** — Material3 dashboard, Neumorphic login, Brutalist card, Glass hero