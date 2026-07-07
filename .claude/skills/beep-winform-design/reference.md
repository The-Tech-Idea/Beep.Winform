# Beep WinForms Design Reference

Detailed catalogue backing `SKILL.md`. Source of truth is always the helpers in `TheTechIdea.Beep.Winform.Controls/Styling/`.

---

## 1. Full Style Catalogue

Every `BeepControlStyle` value with its geometry and shadow parameters. Use `StyleBorders.GetRadius/Width`, `StyleShadows.GetShadowBlur/Offset/Color`, `StyleSpacing.GetPadding` to read these at runtime — don't hardcode.

| Style | Radius (px) | Border (px) | Padding (px) | Item Gap | Has Shadow | Blur | Offset Y | Shadow Color | Filled | Family |
|---|---:|---:|---:|---:|---|---:|---:|---|:---:|---|
| `Material3` | 12 | 2.0 | 16 | 4 | ✅ | 12 | 4 | rgba(60,0,0,0) | No | Modern web |
| `iOS15` | 16 | 2.0 | 12 | 4 | ✅ | — | — | — | Yes | Modern web |
| `Fluent2` | 4 | 2.0 | 8 | 4 | ✅ | 8 | 2 | rgba(40,0,0,0) | Yes | Modern web |
| `Minimal` | 0 | 2.0 | 8 | 2 | ❌ | — | — | — | No | Flat minimal |
| `AntDesign` | 6 | 2.0 | 16 | 8 | ✅ | 6 | 2 | rgba(45,0,0,0) | No | Modern web |
| `MaterialYou` | 16 | 0.0 | 16 | 4 | ✅ | 12 | 4 | rgba(60,0,0,0) | Yes | Modern web |
| `Windows11Mica` | 4 | 2.0 | 12 | 4 | ❌ (mica instead) | — | — | — | Yes | Glass / soft |
| `MacOSBigSur` | 10 | 2.0 | 12 | 2 | ✅ | 10 | 3 | rgba(30,0,0,0) | Yes | Glass / soft |
| `ChakraUI` | 6 | 2.0 | 16 | 8 | ✅ | 10 | 4 | rgba(50,0,0,0) | No | Modern web |
| `TailwindCard` | 8 | 2.0 | 16 | 4 | ✅ | 10 | 4 | rgba(40,0,0,0) | No | Modern web |
| `FinSet` | 8 | 2.0 | — | — | ✅ | — | — | — | No | Card / dashboard |
| `NotionMinimal` | 3 | 2.0 | 12 | 2 | ❌ | — | — | — | No | Flat minimal |
| `VercelClean` | 4 | 2.0 | 16 | 8 | ❌ | — | — | — | No | Flat minimal |
| `StripeDashboard` | 6 | 2.0 | 20 | 8 | ✅ | 12 | 6 | rgba(50,0,0,0) | Yes | Card / dashboard |
| `DarkGlow` | 2 | 2.0 | 16 | 4 | ✅ (glow) | 14 | 0 | purple glow | Yes | Glow / colored |
| `DiscordStyle` | 8 | 0.0 | 12 | 2 | ❌ | — | — | — | Yes | Flat minimal |
| `GradientModern` | 14 | 0.0 | 20 | 8 | ✅ | 12 | 8 | rgba(80,0,0,0) | Yes | Glass / soft |
| `GlassAcrylic` | 12 | 2.0 | 16 | 8 | ❌ (glass) | — | — | — | Yes | Glass / soft |
| `Neumorphism` | 4 | 0.0 | 20 | 12 | ✅ (dual) | 12 | 10 | soft blue-gray | Yes | Neumorphic |
| `Bootstrap` | 4 | 2.0 | 12 | 8 | ✅ | 8 | 2 | rgba(40,0,0,0) | No | Card / dashboard |
| `FigmaCard` | 8 | 2.0 | 16 | 8 | ✅ | 10 | 4 | rgba(40,0,0,0) | No | Card / dashboard |
| `PillRail` | 6 | 0.0 | 8 | 4 | ❌ | — | — | — | Yes | Pill / rail |
| `Metro` | 0 | 2.0 | 8 | 2 | ❌ | — | — | — | Yes | Bold / sharp |
| `Office` | 3 | 2.0 | 12 | 4 | ✅ | 8 | 2 | rgba(30,0,0,0) | Yes | Card / dashboard |
| `Gnome` | 6 | 2.0 | 12 | 4 | ✅ | 6 | 2 | rgba(35,0,0,0) | Yes | OS desktop |
| `Kde` | 5 | 2.0 | 16 | 6 | ✅ (glow) | 8 | 0 | blue glow | Yes | Glow / colored |
| `Cinnamon` | 8 | 2.0 | 16 | 8 | ✅ | 8 | 3 | rgba(40,0,0,0) | Yes | OS desktop |
| `Elementary` | 6 | 2.0 | 12 | 4 | ✅ | 8 | 2 | rgba(30,0,0,0) | Yes | OS desktop |
| `NeoBrutalist` | 0 | 3.0 | 20 | 8 | ✅ (hard) | 0 | 4 | rgba(255,0,0,0) | Yes | Bold / sharp |
| `Gaming` | 0 | 2.0 | 12 | 6 | ✅ (glow) | 12 | 0 | green glow | Yes | Bold / sharp |
| `HighContrast` | 0 | 2.0 | 16 | 8 | ❌ | — | — | — | No | Bold / sharp |
| `Neon` | 2 | 2.0 | 16 | 8 | ✅ (glow) | 8 | 0 | cyan glow | Yes | Glow / colored |
| `NeonGlow` | 12 | 2.0 | — | — | ✅ (glow) | — | — | — | Yes | Glow / colored |
| `Retro` | 0 | 2.0 | — | — | ✅ | 4 | 2 | rgba(80,0,0,0) | Yes | Bold / sharp |
| `ArcLinux` | 4 | 2.0 | — | — | ✅ | 6 | 3 | rgba(40,0,0,0) | Yes | OS desktop |
| `Brutalist` | 0 | 3.0 | — | — | ✅ | 6 | 6 | rgba(255,0,0,0) | Yes | Bold / sharp |
| `Cartoon` | 2 | 3.0 | — | — | ✅ | 10 | 8 | rgba(60,0,0,0) | Yes | Novel |
| `ChatBubble` | 4 | 2.0 | — | — | ✅ | 8 | 6 | blue | Yes | Novel |
| `Cyberpunk` | 4 | 2.0 | — | — | ✅ (glow) | 14 | 8 | cyan glow | Yes | Glow / colored |
| `Dracula` | 7 | 2.0 | — | — | ✅ | 10 | 6 | purple | Yes | Editorial |
| `Glassmorphism` | 2 | 2.0 | — | — | ✅ | 10 | 6 | rgba(50,0,0,0) | Yes | Glass / soft |
| `Holographic` | 4 | 2.0 | — | — | ✅ (glow) | 12 | 8 | blue glow | Yes | Glow / colored |
| `GruvBox` | 6 | 2.0 | — | — | ✅ | 8 | 4 | warm brown | Yes | Editorial |
| `Metro2` | 0 | 2.0 | — | — | ✅ | 10 | 3 | rgba(70,0,0,0) | Yes | Bold / sharp |
| `Modern` | 8 | 2.0 | — | — | ✅ | 10 | 4 | rgba(70,0,0,0) | Yes | Modern web |
| `Nord` | 6 | 2.0 | — | — | ✅ | 6 | 3 | blue-gray | Yes | Editorial |
| `Nordic` | 8 | 2.0 | — | — | ✅ | 10 | 4 | soft blue | Yes | Editorial |
| `OneDark` | 6 | 2.0 | — | — | ✅ | 10 | 4 | rgba(90,0,0,0) | Yes | Editorial |
| `Paper` | 8 | 2.0 | — | — | ✅ | 12 | 6 | rgba(70,0,0,0) | Yes | Editorial |
| `Solarized` | 4 | 2.0 | — | — | ✅ | 7 | 3 | warm dark | Yes | Editorial |
| `Terminal` | 0 | 2.0 | — | — | ❌ | 0 | 0 | — | Yes | Novel |
| `Tokyo` | 8 | 2.0 | — | — | ✅ (glow) | 14 | 7 | purple glow | Yes | Editorial |
| `Ubuntu` | 7 | 2.0 | — | — | ✅ | 6 | 2 | rgba(40,0,0,0) | Yes | OS desktop |
| `Apple` | 12 | 2.0 | 12 | 2 | ✅ | 5 | 1 | rgba(25,0,0,0) | Yes | Modern web |
| `Fluent` | 4 | 2.0 | 8 | 4 | ✅ | 8 | 2 | rgba(30,0,0,0) | Yes | Modern web |
| `Material` | 4 | 2.0 | 16 | 4 | ✅ | 10 | 4 | rgba(60,0,0,0) | Yes | Modern web |
| `WebFramework` | 4 | 2.0 | 16 | 8 | ✅ | 8 | 2 | rgba(30,0,0,0) | No | Card / dashboard |
| `Effect` | 4 | 0.0 | 20 | 8 | ✅ | 12 | 8 | blue-purple glow | Yes | Novel |
| `Shadcn` | 6 | 2.0 | — | — | ✅ | 2 | 1 | — | No | Next-gen (2024+) |
| `RadixUI` | 6 | 2.0 | — | — | ✅ | 3 | 2 | — | No | Next-gen (2024+) |
| `NextJS` | 8 | 2.0 | — | — | ✅ | 5 | 2 | — | No | Next-gen (2024+) |
| `Linear` | 8 | 2.0 | — | — | ✅ | 2 | 1 | — | No | Next-gen (2024+) |

> "—" = not yet wired in `StyleSpacing` (default 12/4). Always read from helpers at runtime.

### Selection-radius overrides (for pill / selected rows)

`StyleBorders.GetSelectionRadius(style)` returns the per-style radius for the "selected" pill of a row. Highlights:
- Material3 / MaterialYou / PillRail / GradientModern / Neumorphism use 16–28 (full pill)
- Metro / NeoBrutalist / Brutalist / Gaming / HighContrast use 0 (square)
- Default = 4

### Accent bar (Fluent-style left indicator)

`StyleBorders.GetAccentBarWidth(style)`:
- Fluent2, Windows11Mica, Office = 3 px
- StripeDashboard, Bootstrap = 4 px
- Default = 0

### Glow / ring widths

`StyleBorders.GetGlowWidth(style)` — Neon = 3, DarkGlow / Dracula / Holographic / Tokyo = 2.0–2.5, default = 0.
`StyleBorders.GetRingWidth(style)` — Material3 / Material / MaterialYou = 2.5, Fluent / Fluent2 / iOS15 / Apple / MacOSBigSur / Modern = 2.0–2.5, TailwindCard / HighContrast = 3.0, default = 0.

---

## 2. Per-Control `DefaultSize` Templates

After applying `DefaultSize` to all Beep controls, here is the complete reference. All sizes are in design units (96 DPI); framework `DpiScalingHelper` handles runtime scaling.

### Input fields
| Control | Size (W×H) | Token |
|---|---:|---|
| `BeepTextBox` | 200×34 | — (hardcoded original) |
| `BeepDateDropDown` | 200×34 | — (hardcoded original) |
| `BeepComboBox` | 200×34 | `ComboBox` |
| `BeepDropDownCheckBoxSelect` | 200×34 | `DropDownSelect` |
| `BeepDatePicker` | 200×36 | — (hardcoded original) |
| `BeepTimePicker` | 150×36 | — (hardcoded original) |
| `BeepDateTimePicker` | 200×34 | `FieldDateTime` |
| `BeepDateRangePicker` | 240×34 | `FieldDateRange` |
| `BeepDatePickerView` | 200×200 | `FieldDateView` |
| `BeepNumericUpDown` | 100×34 | `FieldNumeric` |
| `BeepFilter` | 400×32 | `FilterBar` |
| `BeepFilterRow` | 300×32 | `FilterRow` |
| `BeepQuickFilterBar` | 400×32 | `QuickFilterBar` |
| `BeepQueryandFilter` | 400×36 | `QueryAndFilter` |

### Inline / small
| Control | Size (W×H) | Token |
|---|---:|---|
| `BeepLabel` | 120×24 | `LabelStandard` |
| `BeepCheckboxStandard` | 140×24 | `CheckboxStandard` |
| `BeepToggle` | 50×26 | `ToggleStandard` |
| `BeepSwitch` | 50×26 | `SwitchStandard` |
| `BeepStarRating` | 120×24 | `RatingStandard` |

### Buttons
| Control | Size (W×H) | Token |
|---|---:|---|
| `BeepButton` | 100×36 | — (hardcoded original) |
| `BeepAdvancedButton` | 110×32 | `ButtonToolbar` |
| `BeepCircularButton` | 100×32 | `ButtonStandard` |
| `BeepChevronButton` | 80×28 | `ButtonSmall` |

### Cards
| Control | Size (W×H) | Token |
|---|---:|---|
| `BeepCard` | 280×160 | `Card` |
| `BeepFeatureCard` | 320×240 | `CardFeature` |
| `BeepStatCard` | 200×100 | `CardStat` |
| `BeepTaskCard` | 300×100 | `CardTask` |
| `BeepTestimonial` | 300×200 | `CardTestimonial` |
| `BeepMetricTile` | 200×120 | `CardMetric` |
| `BeepProjectCard` | 320×200 | `CardProject` |
| `BeepTaskListItemControl` | 300×100 | `CardTask` |
| `BeepCompanyProfile` | 300×150 | `CompanyProfile` |

### Containers / panels
| Control | Size (W×H) | Token |
|---|---:|---|
| `BeepPanel` | 300×200 | `Panel` |
| `BeepMultiSplitter` | 300×200 | `MultiSplitter` |
| `BeepLayoutControl` | 300×200 | `LayoutControl` |
| `BeepScrollList` | 280×200 | `ScrollList` |
| `BeepDisplayContainer` | 400×300 | `DisplayContainer` |
| `BeepDisplayContainer2` | 400×300 | `DisplayContainer` |
| `BeepFlyoutMenu` | 240×300 | `FlyoutPanel` |

### Lists / Trees
| Control | Size (W×H) | Token |
|---|---:|---|
| `BeepListBox` | 280×200 | `ListBox` |
| `BeepListofValuesBox` | 300×240 | `ListOfValues` |
| `BeepChipListBox` | 200×200 | `ChipList` |
| `BeepRadioListBox` | 200×200 | `RadioList` |
| `BeepTree` | 250×300 | `Tree` |
| `BeepHierarchicalRadioGroup` | 200×200 | `HierarchicalRadio` |
| `BeepRadioGroup` | 200×100 | `RadioGroup` |
| `BeepMultiChipGroup` | 300×32 | `MultiChipGroup` |
| `BeepDropDownCheckBoxSelect` | 200×34 | `DropDownSelect` |

### Tabs
| Control | Size (W×H) | Token |
|---|---:|---|
| `BeepTabs` | 400×32 | `TabsStrip` |
| `BeepTabPage` | 400×200 | `TabPage` |
| `BeepTabContentHost` | 400×200 | `TabHost` |

### Data / Grid
| Control | Size (W×H) | Token |
|---|---:|---|
| `BeepGridPro` | 600×300 | `Grid` |
| `BeepChart` | 400×300 | — (hardcoded original) |
| `BeepVerticalTable` | 300×200 | `VerticalTable` |
| `BeepDataRecord` | 400×32 | `DataRecord` |
| `BeepDataNavigator` | 400×36 | `DataNavigator` |
| `BeepBindingNavigator` | 400×32 | `BindingNavigator` |
| `BeepProgressBar` | 200×16 | `ProgressBar` |
| `BeepNumericUpDown` | 100×34 | `FieldNumeric` |
| `BeepDualPercentageControl` | 200×60 | `DualPercentage` |

### Navigation
| Control | Size (W×H) | Token |
|---|---:|---|
| `BeepNavBar` | 800×56 | `NavBar` |
| `BeepWebHeaderAppBar` | 800×56 | `WebHeaderAppBar` |
| `BeepBreadcrump` | 300×36 | — (DPI-scaled) |
| `BeepMenuBar` | 600×32 | `MenuBar` |
| `BeepSideBar` | 240×400 | `Sidebar` |
| `BeepSideMenu` | 240×400 | `SideMenu` |
| `BeepAccordionMenuItem` | 240×36 | `AccordionItem` |
| `BeepAccordionMenu` | (DefaultExpandedWidth, 200) | — (computed) |
| `BeepStepperBar` | 320×32 | `Stepper` |
| `BeepStepperBreadCrumb` | 300×36 | `StepperBreadcrumb` |

### Date / time
| Control | Size (W×H) | Token |
|---|---:|---|
| `BeepCalendar` | 280×320 | `Calendar` |

### Notifications & Feedback
| Control | Size (W×H) | Token |
|---|---:|---|
| `BeepNotification` | (form-managed) | — |
| `BeepNotificationGroup` | 320×200 | `NotificationGroup` |
| `BeepNotificationHistory` | 400×300 | `NotificationHistory` |
| `BeepNotificationWidget` | 320×80 | `Notification` |
| `BeepFloatingBadge` | 24×24 | `Badge` |

### Misc
| Control | Size (W×H) | Token |
|---|---:|---|
| `BeepShape` | 100×100 | `Shape` |
| `BeepImage` | 100×100 | `Image` |
| `BeepMarquee` | 400×32 | `Marquee` |
| `BeepLogin` | 400×500 | `Login` |
| `BeepDock` | 200×200 | `Dock` |
| `BeepDockSplitter` | 8×100 | `DockSplitter` |
| `BeepRibbonControl` | 800×56 | `NavBar` |
| `BeepScrollBar` | (computed) | — (dynamic) |

### Widgets
| Control | Size (W×H) | Token |
|---|---:|---|
| `BeepCalendarWidget` | 280×160 | `Card` |
| `BeepChartWidget` | 400×300 | `Chart` |
| `BeepControlWidget` | 280×160 | `Card` |
| `BeepDashboardWidget` | 280×160 | `Card` |
| `BeepFinanceWidget` | 280×160 | `Card` |
| `BeepFormWidget` | 280×160 | `Card` |
| `BeepListWidget` | 280×200 | `ListBox` |
| `BeepMapWidget` | 280×160 | `Card` |
| `BeepMediaWidget` | 280×160 | `Card` |
| `BeepMetricWidget` | 200×120 | `CardMetric` |
| `BeepNavigationWidget` | 280×160 | `Card` |
| `BeepSocialWidget` | 280×160 | `Card` |

### Forms / Dialogs (size managed by `BeepiFormPro` / `DialogConfig`)
`BeepiFormPro`, `BeepDialogModal`, `BeepMessageDialog`, `BeepQuestionDialog`, `BeepInputDialog`, `BeepListDialog`, `BeepFileDialog`, `BeepProgressDialog`, `BeepMultiSelectDialog`, `BeepModelessDialog`, `BeepWait`, `BeepNotification`, `BeepDockPopup`, `BeepGridFilterPopup`, `BeepTreeFilterPopup`, `BeepAdvancedFilterDialog`, `BeepCommandPaletteDialog`, `BeepPopupForm`, `BeepPopupModalForm`, `BeepPrintPreviewForm`, `BeepContextMenu`, `BeepDockTooltip`, `BeepLovPopup` — these are `Form`-derived. They manage their own size through their respective config (`DialogConfig`, `MenuConfig`, etc.) and do **not** need `DefaultSize`.

---

## 3. Per-Style Recommended Controls

Not every style fits every control. Use this when choosing style:

| Family / Style | Best for | Avoid on |
|---|---|---|
| Material3 / Material | Buttons, text fields, cards, dialogs, nav bars | Sidebars (use Material You for surface-heavy panels) |
| iOS15 / Apple | Buttons, cards, toggles, switches | Dense data grids (use Fluent2) |
| Fluent2 / Fluent | Windows desktop apps, native-feel UI | Marketing-style hero cards (use Glass) |
| Minimal / VercelClean / NotionMinimal | Content-focused forms, dense tables | Hero sections, marketing surfaces |
| AntDesign / ChakraUI / TailwindCard | SaaS dashboards, web-app aesthetic | Desktop-only LOB apps |
| StripeDashboard / FigmaCard | KPI dashboards, metric tiles, stat cards | Small buttons (over-padded) |
| MaterialYou | Filled containers, sidebar nav, big surfaces | Outlined inputs (style is filled-first) |
| Windows11Mica / MacOSBigSur | Native desktop shells | Web-only controls (Date picker etc.) |
| GradientModern / GlassAcrylic / Glassmorphism | Hero sections, splash surfaces, modals | Dense data grids (visual noise) |
| Neumorphism | Soft UI, login screens | Dense forms (heavy shadows) |
| NeoBrutalist / Brutalist | Bold marketing, design portfolios | LOB apps, accessibility-critical surfaces |
| DarkGlow / Neon / NeonGlow / Cyberpunk | Dark-mode entertainment, gaming | Business apps, accessibility-critical |
| Gaming | RGB / gaming / streaming UIs | Anything serious |
| HighContrast | Accessibility-first apps, government | Decorative surfaces |
| Metro / Metro2 | Win8/Win10 tile-style apps | Card-heavy SaaS |
| Office | Ribbon-style productivity apps | Modern flat apps |
| Gnome / Elementary / Ubuntu / ArcLinux / Kde / Cinnamon | Native Linux desktop apps | Cross-platform apps |
| Dracula / Nord / Nord / OneDark / Solarized / GruvBox / Tokyo | Code-editor aesthetic, themed apps | Business apps |
| Shadcn / RadixUI / NextJS / Linear | Modern web component aesthetic | Legacy WinForms LOB |
| DiscordStyle | Chat / community / gaming | Forms / data entry |
| PillRail | Sidebar navigation rails | Buttons / dialogs |
| Cartoon / ChatBubble / Effect | Novel / whimsical UIs | Enterprise |
| WebFramework | Generic web-app L&F | Branded surfaces |
| Terminal | CLI / console aesthetic | Mouse-driven UI |

---

## 4. Examples

### 4.1 Material 3 dashboard
```csharp
public partial class DashboardForm : BeepiFormPro
{
    public DashboardForm()
    {
        InitializeComponent();
        ControlStyle = BeepControlStyle.Material3;
        BeepThemesManager.SetControlStyle(BeepControlStyle.Material3);

        // Top nav
        var navBar = new BeepNavBar
        {
            Dock = DockStyle.Top,
            ControlStyle = BeepControlStyle.Material3
        };

        // 4 KPI tiles
        var tilePanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            Padding = BeepLayoutMetrics.ContainerPadding.ScalePadding(this)
        };
        tilePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tilePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tilePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tilePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

        foreach (var kpi in kpis)
        {
            var tile = new BeepStatCard
            {
                Size = BeepLayoutMetrics.CardStat.ScaleSize(this),
                Dock = DockStyle.Fill,
                Margin = new Padding(BeepLayoutMetrics.ButtonGap),
                ControlStyle = BeepControlStyle.Material3
            };
            tilePanel.Controls.Add(tile);
        }

        Controls.Add(tilePanel);
        Controls.Add(navBar);
    }
}
```

### 4.2 Neumorphic login
```csharp
public partial class LoginForm : BeepiFormPro
{
    public LoginForm()
    {
        InitializeComponent();
        Size = BeepLayoutMetrics.Login.ScaleSize(this);
        ControlStyle = BeepControlStyle.Neumorphism;
        BeepThemesManager.SetControlStyle(BeepControlStyle.Neumorphism);

        // Form layout (2-column)
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = BeepLayoutMetrics.DialogPadding.ScalePadding(this)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,
            BeepLayoutMetrics.LabelColumnWidth.ScaleValue(this)));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.RowCount = 3;

        var lblUser = new BeepLabel { Text = "User:", TextAlign = ContentAlignment.MiddleRight };
        var txtUser = new BeepTextBox { Dock = DockStyle.Fill };
        var lblPass = new BeepLabel { Text = "Password:", TextAlign = ContentAlignment.MiddleRight };
        var txtPass = new BeepTextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };

        var btnBar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft
        };
        var btnLogin = new BeepButton { Text = "Login", Size = BeepLayoutMetrics.ButtonLarge };
        var btnCancel = new BeepButton { Text = "Cancel", Size = BeepLayoutMetrics.ButtonStandard };
        btnBar.Controls.AddRange(new Control[] { btnLogin, btnCancel });
    }
}
```

### 4.3 Brutalist card
```csharp
var card = new BeepCard
{
    Size = BeepLayoutMetrics.Card.ScaleSize(this),
    ControlStyle = BeepControlStyle.Brutalist, // 0 radius, 3 px black border, hard shadow
    Padding = new Padding(BeepLayoutMetrics.FieldStandard.Height.ScaleValue(this))
};
card.HeaderText = "Big number";
card.ParagraphText = "A bold take on metrics.";
```

### 4.4 Glass hero card
```csharp
var hero = new BeepFeatureCard
{
    Size = BeepLayoutMetrics.CardFeature.ScaleSize(this),
    ControlStyle = BeepControlStyle.GlassAcrylic,
    UseGlassmorphism = true,
    GlassmorphismBlur = 12f,
    GlassmorphismOpacity = 0.15f,
    Padding = BeepLayoutMetrics.DialogPadding.ScalePadding(this)
};
```

---

## 5. Layout Recipes — Beep + WinForms Layout Containers

WinForms gives you five layout primitives. Source: [Microsoft Learn — Control layout options](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/layout) and [Microsoft Learn — Arranging Controls Using a TableLayoutPanel](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/walkthrough-arranging-controls-on-windows-forms-using-a-tablelayoutpanel).

| Primitive | What it does | Best for |
|---|---|---|
| `Dock` | Snaps a control to one side of its parent and resizes to fit that side's available space | Top-level form scaffold (navbar, status bar, fill-content) |
| `Anchor` | Maintains fixed distance to one or more parent edges as the parent resizes | Free-form, edge-aligned controls |
| `TableLayoutPanel` | Arranges children in a grid; supports `Absolute` / `Percent` / `AutoSize` rows and columns | Form fields, localizable layouts, proportional grids |
| `FlowLayoutPanel` | Lays children in a flow direction with optional wrapping | Button bars, chip/tag rows, breadcrumbs |
| `SplitContainer` | Two panels with a movable splitter bar | Master-detail, resizable panes, navigation + content |

**Microsoft's explicit guidance:** *"In general, you should not use a TableLayoutPanel control as a container for the whole layout. Use TableLayoutPanel controls to provide proportional resizing capabilities to parts of the layout."* — that is, **nest** layout panels. The form is scaffolded with Dock; regions inside use TableLayoutPanel or SplitContainer.

### Row/column SizeType cheat sheet

| SizeType | Behavior | Typical use |
|---|---|---|
| `Absolute` | Fixed pixel size; never changes | Label columns (130 px), icon sizes, fixed chrome |
| `AutoSize` | Sized to fit the largest child | Field rows (grows with content), spacer rows |
| `Percent` | Takes a proportional share of remaining space | Input columns (100%), content panels |

`RowCount` or `ColumnCount = 0` means the panel is **unbounded** in that direction (cells keep adding until you stop). This pairs with `GrowStyle = AddRows / AddColumns` to decide which direction grows on overflow.

### 5.1 Dock-driven form scaffold (the most common top-level)
```csharp
public partial class MainForm : BeepiFormPro
{
    public MainForm()
    {
        InitializeComponent();
        ClientSize = BeepLayoutMetrics.DialogLarge.ScaleSize(this);
        ControlStyle = BeepControlStyle.Material3;

        // Z-order matters — controls are docked in the order added.
        // First add fills the top, last fills the remaining space.
        var status = new BeepLabel { Text = "Ready", Dock = DockStyle.Bottom,
            Height = BeepLayoutMetrics.TextRowHeight.ScaleValue(this) };
        var nav = new BeepNavBar { Dock = DockStyle.Top,
            Height = BeepLayoutMetrics.NavBar.Height };
        var content = new Panel { Dock = DockStyle.Fill };   // everything else

        // Add in this order: status first, then nav, then content last.
        // Content is added last → it gets the remaining Fill space.
        Controls.Add(status);
        Controls.Add(nav);
        Controls.Add(content);
    }
}
```

### 5.2 Two-column form (TableLayoutPanel — the canonical Beep pattern)
```csharp
var form = new TableLayoutPanel
{
    Dock = DockStyle.Fill,
    ColumnCount = 2,
    RowCount = 4,                     // header + 2 fields + button bar
    Padding = BeepLayoutMetrics.DialogPadding.ScalePadding(this),
};

form.ColumnStyles.Add(new ColumnStyle(
    SizeType.Absolute,
    BeepLayoutMetrics.LabelColumnWidth.ScaleValue(this)));     // left labels — fixed
form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));  // right inputs — stretch

form.RowStyles.Add(new RowStyle(SizeType.Absolute,
    BeepLayoutMetrics.TitleFontSize + 16));                       // header
form.RowStyles.Add(new RowStyle(SizeType.Absolute,
    BeepLayoutMetrics.TextRowHeight.ScaleValue(this)));           // field 1
form.RowStyles.Add(new RowStyle(SizeType.Absolute,
    BeepLayoutMetrics.TextRowHeight.ScaleValue(this)));           // field 2
form.RowStyles.Add(new RowStyle(SizeType.AutoSize));              // button bar — grows

// Add controls; remember one per cell. Use Dock = Fill on the input side.
form.Controls.Add(new BeepLabel { Text = "Username:",
    TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 1);
form.Controls.Add(new BeepTextBox { Dock = DockStyle.Fill }, 1, 1);

form.Controls.Add(new BeepLabel { Text = "Password:",
    TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 2);
var pwd = new BeepTextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
form.Controls.Add(pwd, 1, 2);

// Button bar — FlowLayoutPanel inside the last row
var btnBar = new FlowLayoutPanel
{
    Dock = DockStyle.Fill,
    FlowDirection = FlowDirection.RightToLeft,
    Padding = BeepLayoutMetrics.ButtonStripPd.ScalePadding(this),
};
btnBar.Controls.Add(new BeepButton { Text = "Login",
    Size = BeepLayoutMetrics.ButtonLarge });
btnBar.Controls.Add(new BeepButton { Text = "Cancel",
    Size = BeepLayoutMetrics.ButtonStandard });
form.Controls.Add(btnBar, 0, 3);
form.SetColumnSpan(btnBar, 2);    // span both columns for the bar
```

### 5.3 Master-detail (SplitContainer — resizable two-pane)
```csharp
var split = new SplitContainer
{
    Dock = DockStyle.Fill,
    Orientation = Orientation.Vertical,           // left | right
    SplitterDistance = 240,                       // initial left pane width
    FixedPanel = FixedPanel.Panel1,               // left pane stays fixed when form resizes
    Panel1MinSize = 180,
    Panel2MinSize = 320,
};
split.Panel1.Controls.Add(new BeepListBox
{
    Dock = DockStyle.Fill,
    ControlStyle = BeepControlStyle.MaterialYou
});
split.Panel2.Controls.Add(new BeepCard
{
    Dock = DockStyle.Fill,
    ControlStyle = BeepControlStyle.Fluent2
});
```
**Tip:** Set `SplitterDistance` to a DPI-scaled value (`BeepLayoutMetrics.Sidebar.Width`). Use `FixedPanel` to lock one side when needed; use `Panel1MinSize` and `Panel2MinSize` to prevent the splitter from collapsing a pane entirely.

### 5.4 Toolbar with wrapping (FlowLayoutPanel)
```csharp
var toolbar = new FlowLayoutPanel
{
    Dock = DockStyle.Top,
    Height = BeepLayoutMetrics.ButtonToolbar.Height + 16,
    FlowDirection = FlowDirection.LeftToRight,
    WrapContents = true,                          // wrap to next line when too narrow
    Padding = BeepLayoutMetrics.ButtonStripPd.ScalePadding(this),
};
int gap = BeepLayoutMetrics.SmallGap.ScaleValue(this);
toolbar.Margin = new Padding(0);

foreach (var item in items)
{
    var btn = new BeepButton { Text = item.Text, Size = BeepLayoutMetrics.ButtonToolbar };
    toolbar.Controls.Add(btn);
    // AutoSize the flow panel around its contents in width when needed
}
```
**RTL:** set `RightToLeft = RightToLeft.Yes` on the panel and the flow direction auto-flips — buttons reverse order and padding mirrors. Works correctly for Arabic / Hebrew apps.

### 5.5 Dashboard tile grid (nested TableLayoutPanel)
```csharp
// Top-level: Dock=Fill so it expands with the form
var grid = new TableLayoutPanel
{
    Dock = DockStyle.Fill,
    ColumnCount = 4,
    RowCount = 2,
    Padding = BeepLayoutMetrics.ContainerPadding.ScalePadding(this),
};
// 4 equal columns
for (int c = 0; c < 4; c++)
    grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

for (int r = 0; r < 2; r++)
{
    for (int c = 0; c < 4; c++)
    {
        // One control per cell → wrap in FlowLayoutPanel if you want to add label+button
        var tile = new BeepStatCard
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(BeepLayoutMetrics.ButtonGap),
            ControlStyle = BeepControlStyle.Material3
        };
        grid.Controls.Add(tile, c, r);
    }
}
```

### 5.6 Localizable form (the killer feature of TableLayoutPanel)
When text grows (German "Benutzername" vs English "Username"), `AutoSize` rows grow with the content; `Percent` columns redistribute width. No `Layout` event handler needed.

```csharp
var form = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = true };
form.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      // label column grows with text
form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));  // input column stretches
form.RowStyles.Add(new RowStyle(SizeType.AutoSize));
```
With `ColumnStyles[0] = AutoSize`, the label column auto-expands to fit translated text; `Percent, 100F` keeps the input column filling the remainder. The form reflows itself when the user changes language at runtime.

### 5.7 Wizard with progress stepper (top + content + nav)
```csharp
ClientSize = BeepLayoutMetrics.DialogLarge.ScaleSize(this);

var stepper = new BeepStepperBar { Dock = DockStyle.Top,
    Height = BeepLayoutMetrics.Stepper.Height + 16 };
var content = new Panel { Dock = DockStyle.Fill };     // current step's controls go here
var navBar = new FlowLayoutPanel
{
    Dock = DockStyle.Bottom,
    FlowDirection = FlowDirection.RightToLeft,
    Padding = BeepLayoutMetrics.ButtonStripPd.ScalePadding(this),
};
navBar.Controls.Add(new BeepButton { Text = "Back",
    Size = BeepLayoutMetrics.ButtonStandard });
navBar.Controls.Add(new BeepButton { Text = "Next",
    Size = BeepLayoutMetrics.ButtonLarge });

// Add in this order: top → fill → bottom
Controls.Add(stepper);
Controls.Add(navBar);   // docked Bottom, sized to its content
Controls.Add(content);   // added last → fills the remaining middle
```

### 5.8 Nested grid (form fields + side panel)
A common pattern: 2-col form on the left, info card on the right.

```csharp
var outer = new TableLayoutPanel
{
    Dock = DockStyle.Fill,
    ColumnCount = 2,
    Padding = BeepLayoutMetrics.ContainerPadding.ScalePadding(this),
};
outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));   // form
outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));   // side panel

// Column 0: another TableLayoutPanel with label/input pairs
var form = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,
    BeepLayoutMetrics.LabelColumnWidth.ScaleValue(this)));
form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
// ... add fields ...

// Column 1: a single card that fills its cell
var helpCard = new BeepFeatureCard
{
    Dock = DockStyle.Fill,
    Margin = new Padding(BeepLayoutMetrics.ButtonGap),
    ControlStyle = BeepControlStyle.GlassAcrylic
};

outer.Controls.Add(form, 0, 0);
outer.Controls.Add(helpCard, 1, 0);
```

### 5.9 Decision matrix — which container for which job

| You need | Use | Example |
|---|---|---|
| Form nav + content + status bar at top/bottom | `Dock` chain (Top, Bottom, Fill) | Main shell |
| Resizable master-detail | `SplitContainer` (Vertical) | Email list + reading pane |
| Form fields (label + input pairs) | `TableLayoutPanel` 2-col | Settings, login, data entry |
| Localizable form that grows with text | `TableLayoutPanel` with `AutoSize` rows | All translatable forms |
| Proportional tile grid | `TableLayoutPanel` N-col Percent | Dashboard, gallery |
| Toolbar / breadcrumb / chip row | `FlowLayoutPanel` (Horizontal, Wrap) | App toolbar |
| Vertical filter rail | `FlowLayoutPanel` (Vertical) | Sidebar filter list |
| Wizard step nav at bottom | `FlowLayoutPanel` (RightToLeft) | Back / Next buttons |
| Resizable two-pane with splitter | `SplitContainer` | Code editor, properties panel |
| Status bar with separators | `StatusStrip` (special FlowLayoutPanel) | App status bar |
| Single child that resizes with parent | `Dock = Fill` (no container needed) | Content area |
| Edge-pinned child | `Anchor` (Top/Bottom/Left/Right) | Pin button to corner |

### 5.10 Common pitfalls (the ones that always bite)

- **Anchor + Dock on the same child of TableLayoutPanel** — pick one. Use `Dock = Fill` for inputs, `Anchor` only when the cell shouldn't be filled.
- **Setting `Location` inside a TableLayoutPanel** — the layout engine ignores it. Use `Cell`, `Column`, `Row`, `ColumnSpan`, `RowSpan` instead.
- **`Percent` rows/cols summing to 0 or all 100** — fine, but if some are `0` you lose stretching. Use explicit Percent values for visible stretch.
- **Forgetting `Dock = Fill` on inputs** — the input will size to its default and leave whitespace.
- **Adding a second control to a cell** — silently rejected at design time; at runtime it gets reparented to the panel. Wrap in nested panel.
- **`GrowStyle = FixedSize` with a `RowCount = 0`** — works, but new cells can't be added. Choose deliberately.
- **`AutoSize = true` on the TableLayoutPanel itself** — useful when you want the table to size to contents. Disable when the table should fill its parent.
- **Mixing Anchor styles across rows/columns** — design-time looks fine but runtime resizing is unpredictable. Use Dock or AutoSize, not both.
- **Not setting `MinimumSize` on inputs** — they shrink past usability when window is narrow. Use `MinTouchTarget.ScaleValue(this)` (44 px) as the floor.
- **Hardcoding pixel sizes for layout** — never. Use `BeepLayoutMetrics` tokens + `.ScaleSize(this)` / `.ScalePadding(this)`.
- **Forgetting `Padding` vs `Margin`** — `Padding` is interior space on the panel; `Margin` is exterior space around a child. TableLayoutPanel respects both. Keep them in sync with `BeepLayoutMetrics`.
- **Designer view vs runtime** — the designer shows 96-DPI pixels; runtime scales. Always test on a high-DPI display.

### 5.11 DPI-aware layout patterns

Every `BeepLayoutMetrics` value is in 96-DPI design units. Convert before assigning to a layout container:

```csharp
// Right way — every dimension scaled
panel.Padding   = BeepLayoutMetrics.DialogPadding.ScalePadding(this);
col.Width       = BeepLayoutMetrics.LabelColumnWidth.ScaleValue(this);
row.Height      = BeepLayoutMetrics.TextRowHeight.ScaleValue(this);
control.Size    = BeepLayoutMetrics.ButtonStandard.ScaleSize(this);
split.SplitterDistance = BeepLayoutMetrics.Sidebar.Width.ScaleValue(this);

// Wrong way — pixels stay 96-DPI and look wrong at 150%
panel.Padding   = new Padding(12);
col.Width       = 130;
row.Height      = 35;
control.Size    = new Size(100, 32);
```

`AutoScaleMode = Inherit` (the default for `BeepiFormPro`) + `BeepLayoutMetrics` + `DpiScalingHelper` is the safe combination. WinForms will additionally scale your controls, but the values you typed in code need to be scaled manually.

### 5.12 Localizability checklist

- [ ] All visible text comes from a resource (`.resx`), not a string literal
- [ ] Form uses `TableLayoutPanel` for fields, not absolute positions
- [ ] Label column is `AutoSize`, input column is `Percent`
- [ ] Row styles are `AutoSize` so they grow with text
- [ ] `AutoSize = true` is set on the table if it doesn't fill its parent
- [ ] Minimum form size is set so labels in German/Finnish don't push inputs off-screen
- [ ] Test with longer strings (German, French) before shipping

---

### Per-control override examples

```csharp
// Force sharp corners on Material3
btn.ControlStyle = BeepControlStyle.Material3;
btn.BorderRadius = 0;
btn.IsRounded = true;

// Add asymmetric top padding to a title label
lbl.CustomPadding = new Padding(0, 8, 0, 0);

// Disable shadow on a normally-shadowed style
card.ShowShadow = false;

// Increase border width above style default (e.g., for a focused state)
btn.BorderThickness = 3;

// Style-driven override (most common):
// Just set ControlStyle — the painter reads StyleBorders/StyleShadows automatically.
// Override individual properties only when you need to deviate from the style.
```

### Inheriting chrome overhead

When sizing a control manually, leave room for chrome:
```csharp
int chrome = BaseControl.CalculateStyleChrome(style);  // not public — replicate if needed:
// border*2 + max(blur, abs(offsetX), abs(offsetY))*2
```

---

## 6. Adding a New Control — DefaultSize Checklist

When adding a new Beep control:

1. Pick the closest existing `BeepLayoutMetrics` token (or add a new one to `BeepLayoutMetrics.cs` with an XML doc-comment explaining its use).
2. Add the override to the main class file (the one with the class declaration — not partial files):
   ```csharp
   protected override Size DefaultSize => BeepLayoutMetrics.YourNewToken;
   ```
3. Ensure the file has `using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;` (PowerShell Edit / IDE will add it on first reference).
4. If you create a new style, also add it to `BeepControlStyle.cs` AND extend all four tables in `Styling/` (Borders, Shadows, Spacing) — otherwise the new style falls through to default values (12 padding / 4 radius / 8 blur) which will look generic.
5. If the control is in the visual designer (`[ToolboxItem(true)]`), the new `DefaultSize` shows up immediately the next time the control is dragged from the toolbox.

---

## 7. Theme Token Reference

`IBeepTheme` (`BeepThemesManager`) provides colors via reflection. Pass a key (no `Color` suffix) to `BeepStyling.GetThemeColor(key)`.

Standard keys:
```
Primary, PrimaryColor, Secondary, Accent, Background, Foreground, Border,
HoverBackground, PressedBackground, DisabledBackground,
SelectedBackground, FocusBorder, HoverBorder, PressedBorder, DisabledBorder,
Shadow, ErrorColor, WarningColor, SuccessColor, InfoColor
```

Always set `UseThemeColors = true` on Beep controls so they pick up the active theme automatically.

Apply a theme to a form:
```csharp
form.CurrentTheme = BeepThemesManager.GetTheme("Material3Dark");
```

Switch at runtime:
```csharp
BeepThemesManager.SetTheme("DarkGlow");
BeepThemesManager.SetControlStyle(BeepControlStyle.DarkGlow);
```

---

## 8. Quick Reference Card

| I want to… | Use |
|---|---|
| Pick a family of styles | `Style` families table in `SKILL.md` |
| Read radius / border / padding per style | `StyleBorders.GetRadius` / `GetBorderWidth` / `StyleSpacing.GetPadding` |
| Read shadow per style | `StyleShadows.HasShadow` / `GetShadowBlur` / `GetShadowOffsetY` / `GetShadowColor` |
| Get a default size for a control type | `BeepLayoutMetrics.Card` / `ButtonStandard` / `FieldStandard` / `Panel` / … |
| Scale a size for DPI | `BeepLayoutMetrics.X.ScaleSize(this)` |
| Scale a Padding | `BeepLayoutMetrics.DialogPadding.ScalePadding(this)` |
| Switch theme at runtime | `BeepThemesManager.SetTheme(name)` |
| Get a theme color | `BeepStyling.GetThemeColor("Primary")` |
| 2-col form layout | `TableLayoutPanel` with `LabelColumnWidth` col + Percent col |
| Button bar | `FlowLayoutPanel { FlowDirection = RightToLeft }` with `ButtonLarge` + `ButtonStandard` |
| Card grid | `TableLayoutPanel` with `ContainerPadding` and `StyleSpacing.GetItemSpacing(style)` gap |

---

## 9. Controls Belong in designer.cs

**The rule:** every child control placed on a host (a `Form` or a composite Beep control) must be created at **design time** by Visual Studio and serialized into the host's `*.Designer.cs` file. The user must be able to see, select, move, resize, and edit every child control in the designer without writing code.

**Why this rule exists:** WinForms design-time serialization writes the layout of the host into `InitializeComponent()` inside `*.Designer.cs`. If a control is instantiated in a constructor, a `Load` handler, or any other runtime path, it is invisible to the serializer. On every designer reload, the IDE re-runs `InitializeComponent()`, and the runtime-created control vanishes.

### 9.1 Default rule: design-time only

| Place control here? | Why |
|---|---|
| `*.Designer.cs` inside `InitializeComponent()` | ✅ Designer sees it, user can edit it, survives reload |
| Constructor (`new BeepButton()`) | ❌ Not serialized, vanishes on reload |
| `Form_Load` / `OnLoad` / `Shown` | ❌ Runtime, not designer-visible |
| Property getter (`if (_btn == null) _btn = new BeepButton()`) | ❌ Created and disposed on every access |
| Code-behind `Form` partial (outside `InitializeComponent`) | ❌ Lost when designer re-generates |
| `InitializeComponent()` BUT via `new SomeControl()` not dragged from toolbox | ⚠️ Works only if the property holding it has `[DesignerSerializationVisibility(Content)]` |

### 9.2 The IDesignerHost pattern (for composite Beep controls)

When you ship a Beep control that *must* contain another control (e.g. `BeepComboBox` must contain a `BeepTextBox` for search), Visual Studio's serializer needs help — controls added via `this.Controls.Add(new Child())` in the constructor are not serialized. Use a **Custom Control Designer** + `IDesignerHost.CreateComponent`:

```csharp
// 1. The host control — constructor stays empty
[Designer(typeof(BeepComboBoxDesigner))]
public partial class BeepComboBox : BaseControl
{
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public BeepTextBox SearchBox { get; set; }

    public BeepComboBox()
    {
        // Empty — the designer creates SearchBox at design time.
        // SearchBox will be null at runtime in some test/early paths;
        // guard reads with `if (SearchBox != null)`.
    }
}

// 2. The designer — adds SearchBox the first time the host is dropped on a form
public class BeepComboBoxDesigner : ControlDesigner
{
    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        var host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host == null || component is not BeepComboBox combo) return;
        if (combo.SearchBox != null) return;          // already exists

        var changeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
        var prop = TypeDescriptor.GetProperties(combo)["SearchBox"];

        changeSvc?.OnComponentChanging(combo, prop);

        var tb = (BeepTextBox)host.CreateComponent(typeof(BeepTextBox));
        tb.Dock = DockStyle.Top;
        tb.PlaceholderText = "Search…";
        combo.Controls.Add(tb);
        combo.SearchBox = tb;

        changeSvc?.OnComponentChanged(combo, prop, null, tb);
    }
}
```

**Reference implementations** in the repo:
- `TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/BeepiFormProDesigner.cs`
- `TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/BeepBlockDesigner.cs`
- `TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/BeepFormsDesigner.cs`
- `TheTechIdea.Beep.Winform.Controls.Design.Server/Designers/BeepDataConnectionDesigner.cs` (component, no surface, just smart-tag)

### 9.3 Designer base-class choice

| Host type | Designer base | Why |
|---|---|---|
| Plain `Component` (no UI, e.g. `BeepDataConnection`) | `ComponentDesigner` | No surface; smart-tag only |
| `Control` / `Panel` (e.g. `BeepForms`) | `BaseBeepParentControlDesigner` (extends `ControlDesigner`) | Inherits common Beep smart-tag + change-service plumbing |
| `Control` with inner panels and child composition (e.g. `BeepBlock`) | `ParentControlDesigner` (out-of-process equivalent) | Lets the designer offer parent/child surface verbs |

### 9.4 Properties that hold child controls

Three attribute knobs control serialization:

```csharp
// Force the child to be serialized into designer.cs
[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
public BeepTextBox SearchBox { get; set; }

// Or: serialize as code (the child is created in the parent's InitializeComponent)
[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
public BeepTextBox SearchBox { get; set; }   // designer writes: this.searchBox = new BeepTextBox()

// Hide the property from designer entirely (rare)
[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
```

For collections of children (tabs, fields, list items):
```csharp
[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
public BindingList<BeepTabPage> Pages { get; } = new();
```

### 9.5 The 4-step recipe for any new composite Beep control

1. **Reference** — add `System.Windows.Forms.Design` (NuGet) or `System.Design` (.NET Framework).
2. **Custom Designer class** — extend `ControlDesigner`/`ComponentDesigner`. In `Initialize`, call `host.CreateComponent(typeof(ChildControl))`, wrap with `OnComponentChanging`/`OnComponentChanged`.
3. **Bind to host** — `[Designer(typeof(YourDesigner))]` on the host class.
4. **Expose children** — `[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]` on the property holding each child.

Then `Rebuild` the solution (the IDE caches old designer assemblies) and the child appears in the host automatically.

### 9.6 When runtime creation is allowed

| Runtime-created controls are OK when | Example |
|---|---|
| User explicitly clicked a button that creates a dialog | `BeepDialogManager.ShowDialog(...)` |
| User added a new item via an "Add" action | `BeepListBox.AddItem(...)` |
| A plugin loaded at startup populates content | plugin manager attaching panels to a host |
| A notification was triggered by an app event | `BeepNotificationManager.Push(...)` |
| Tab content added at runtime via API | `BeepTabs.AddTab(...)` |
| Data-driven rows in a grid | `BeepGridPro.DataSource = ...` (the grid renders rows; it doesn't add controls) |

**Mnemonic:** *if the control's **existence** is a design-time concern → designer.cs. If its existence is a runtime event → runtime creation is fine.*

### 9.7 Common pitfalls

- ❌ `this.Controls.Add(new BeepButton())` inside a constructor — designer doesn't see it, the next reload erases it
- ❌ `InitializeComponent()` calling a method that creates controls — same problem unless you use `host.CreateComponent` from a designer
- ❌ Property getter creates control on first read — non-deterministic lifetime
- ❌ Adding controls to a `Form` from another form's `Load` — not designer-visible
- ❌ Using `Application.OpenForms[0].Controls.Add(...)` — bypasses designer entirely
- ❌ Creating controls in `Dispose`/`Finalize` path — wrong direction
- ✅ Drag from toolbox to designer surface — designer.cs records it
- ✅ Smart-tag verb → opens dialog → verb writes via `designer.SetProperty(name, value)` — designer's change service handles serialization
- ✅ `host.CreateComponent(typeof(...))` inside a Custom Designer → designer tracks and serializes

### 9.8 Out-of-process designer gotcha

The WinForms out-of-process designer refuses to instantiate any `Control` that sets `BackColor = Color.Transparent` without first enabling `ControlStyles.SupportsTransparentBackColor`. Two safe options:

1. **Derive from `Panel`** instead of `Control` — `Panel` enables `SupportsTransparentBackColor` in its constructor. Used by `BeepForms`.
2. **Set the style flag in `InitializeComponent`**: `SetStyle(ControlStyles.SupportsTransparentBackColor, true); BackColor = Color.Transparent;`. Used by `BeepBlock`.

This is also why `BeepForms` derives from `Panel`: it needs transparency at design time.

### 9.9 Verifying a host follows the rule

After implementing a composite Beep control:

1. Drop it on a test form in the Visual Studio designer.
2. Open the form's `*.Designer.cs` — every child must be present in `InitializeComponent()`.
3. Edit a child property (e.g. `Text`) in the property grid — designer.cs must update.
4. Move a child on the design surface — designer.cs must update.
5. Close and reopen the form — children must reappear unchanged.
6. Delete the host from the form — children must be removed from designer.cs cleanly.

If any of these fail, the composite control is breaking the rule. Add a Custom Designer to fix it.

---

## 10. designer.cs File Structure — The Complete Pattern

When you drag a control from the Visual Studio toolbox onto a Form or UserControl, the IDE serializes it into the host's `*.Designer.cs` file. You rarely write this code by hand — but you must understand the structure so you can:
- Read existing designer.cs files
- Add controls programmatically through a Custom Designer
- Fix broken designer.cs files
- Write proper test code that simulates the designer

Source: [Microsoft Learn — Create custom controls overview](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls-design/overview) and [Microsoft Learn — User control overview](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls-design/usercontrol-overview).

### 10.1 The 3 files per form/control

| File | Purpose | Edit by hand? |
|---|---|---|
| `MyForm.cs` | Event handlers, business logic, partial declaration | Yes |
| `MyForm.Designer.cs` | Auto-generated layout (control instances, properties, event wiring) | **Rarely** — IDE regenerates it |
| `MyForm.resx` | Localized strings, embedded resources | Yes (via IDE editor) |

The `partial class` keyword splits the same class across `MyForm.cs` + `MyForm.Designer.cs` (and optionally `MyForm.Core.cs`, `MyForm.Properties.cs`, etc.). The compiler merges them at build time.

### 10.2 The required skeleton in every designer.cs

```csharp
// MyForm.Designer.cs
namespace MyApp
{
    partial class MyForm                    // ← only the partial declaration
    {
        // 1. Required designer variable — IDE adds this automatically
        private IContainer components = null;

        // 2. Required Dispose override — IDE generates this automatically
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        // 3. The method the designer fills with control-creation code
        private void InitializeComponent()
        {
            // ... IDE generates body here ...
        }

        #endregion

        // 4. Private fields for every control created in InitializeComponent
        //    (IDE puts these at the BOTTOM of the file)
        private BeepButton  submitButton;
        private BeepTextBox  nameTextBox;
        private BeepPanel    buttonPanel;
    }
}
```

### 10.3 The 7-step `InitializeComponent` order

This is the exact order the IDE follows. Replicating it in your own code (especially in Custom Designers and code generation) is essential — the IDE can choke on out-of-order statements.

```csharp
private void InitializeComponent()
{
    // STEP 1: Resource manager (for resx strings)
    var resources = new ComponentResourceManager(typeof(MyForm));

    // STEP 2: Create all control instances (just `new`, no properties yet)
    this.buttonPanel   = new BeepPanel();
    this.nameTextBox   = new BeepTextBox();
    this.submitButton  = new BeepButton();
    this.cancelButton  = new BeepButton();

    // STEP 3: For each CONTAINER, call SuspendLayout before touching its children
    this.buttonPanel.SuspendLayout();
    this.SuspendLayout();

    // STEP 4: Set properties on each control (in the order the IDE will)
    this.buttonPanel.Dock     = DockStyle.Bottom;
    this.buttonPanel.Padding  = new Padding(8);
    this.buttonPanel.AutoSize = true;

    this.nameTextBox.Dock    = DockStyle.Fill;
    this.nameTextBox.Margin  = new Padding(4);
    this.nameTextBox.PlaceholderText = resources.GetString("nameTextBox.PlaceholderText");

    this.submitButton.Text  = resources.GetString("submitButton.Text");
    this.submitButton.Size  = new Size(100, 32);
    this.cancelButton.Text  = resources.GetString("cancelButton.Text");
    this.cancelButton.Size  = new Size(100, 32);

    // STEP 5: Add children to their parents (INSIDE the parent's SuspendLayout block)
    this.buttonPanel.Controls.Add(this.submitButton);
    this.buttonPanel.Controls.Add(this.cancelButton);

    // STEP 6: Form-level properties + Controls.Add for top-level children
    this.AutoScaleDimensions = new SizeF(7F, 15F);
    this.AutoScaleMode       = AutoScaleMode.Font;
    this.ClientSize          = new Size(600, 400);
    this.Controls.Add(this.nameTextBox);
    this.Controls.Add(this.buttonPanel);
    this.Name = "MyForm";
    this.Text = "My Form";

    // STEP 7: Resume layout, INNER to OUTER
    this.buttonPanel.ResumeLayout(false);
    this.ResumeLayout(false);
}
```

### 10.4 Field declarations — always at the bottom, always private

```csharp
// ❌ Wrong
public BeepButton SubmitButton;        // IDE will move + mark private
private BeepButton submitButton;       // But in the middle of the class

// ✅ Right — at the BOTTOM of designer.cs, all private
private BeepButton submitButton;
private BeepTextBox nameTextBox;
private BeepPanel buttonPanel;
```

If code-behind needs to read a designer field, expose it through a property:

```csharp
// In MyForm.cs (NOT in designer.cs)
public BeepButton SubmitButton => submitButton;
public BeepTextBox NameField    => nameTextBox;
```

### 10.5 The `SuspendLayout` / `ResumeLayout(false)` rule

`SuspendLayout` tells the layout engine to **batch** layout changes. `ResumeLayout(false)` re-enables layout **without forcing an immediate layout pass** (the `false` argument = don't perform layout). This avoids one layout pass per `Controls.Add()`.

- **One `SuspendLayout` per container, including `this`.**
- **`ResumeLayout(false)` order is INNER → OUTER.** If you have a `buttonPanel` and a `form`, you call `buttonPanel.ResumeLayout(false)` first, then `form.ResumeLayout(false)`.
- **All `Controls.Add()` calls happen INSIDE the parent's `SuspendLayout` block.** If you `Add` outside, you get a separate layout pass per child.
- **Set properties before adding to parent** — the IDE follows this; runtime behavior is more predictable.

```csharp
// ❌ Wrong — Add outside SuspendLayout
this.panel.ResumeLayout(false);  // resumed too early
this.panel.Controls.Add(btn);    // add after resume → extra layout pass

// ✅ Right
this.panel.SuspendLayout();
this.panel.Controls.Add(btn);    // add during suspend
// ... more setup ...
this.panel.ResumeLayout(false);  // batched layout pass
```

### 10.6 Adding event handlers in designer.cs

The IDE wires events in the same file. Format:

```csharp
this.submitButton.Click += new System.EventHandler(this.SubmitButton_Click);

// Event handler in MyForm.cs:
private void SubmitButton_Click(object sender, EventArgs e) { /* ... */ }
```

Always use the `new System.EventHandler(...)` form — not a method group (`+= SubmitButton_Click`) — for designer-generated code. The IDE writes it this way.

### 10.7 The complete Form designer.cs template (copy-paste ready)

```csharp
// MyForm.Designer.cs
namespace TheTechIdea.Beep.MyApp
{
    partial class MyForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(600, 400);
            this.Name                = "MyForm";
            this.Text                = "My Form";
            this.ResumeLayout(false);
        }

        #endregion
    }
}
```

This is the *empty* template the IDE produces when you `Add > New Form`. Add controls by dragging from the toolbox — the IDE fills in the body of `InitializeComponent` following the 7-step order above.

### 10.8 The complete UserControl designer.cs template

For a `UserControl` (composite), the pattern is the same but the namespace and base class differ:

```csharp
// MyUserControl.Designer.cs
namespace TheTechIdea.Beep.MyApp
{
    partial class MyUserControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support — do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // UserControl body
            this.Name     = "MyUserControl";
            this.Size     = new System.Drawing.Size(300, 200);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
```

Note the region name is `Component Designer generated code` for UserControl (vs `Windows Form Designer generated code` for Form). Same pattern.

### 10.9 Adding controls to a custom Beep control (composite) — the right way

If your Beep control must contain a child control (e.g. `BeepComboBox` must contain a `BeepTextBox` for search), the design-time serialization needs `IDesignerHost.CreateComponent`. **The child MUST be created by the host designer, not the host's constructor.**

#### Pattern 1 — Standard Custom Designer for Beep controls

```csharp
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Base;

[Designer(typeof(BeepMyWidgetDesigner))]
public partial class BeepMyWidget : BaseControl
{
    [Browsable(true)]
    [Category("Layout")]
    [Description("The embedded search textbox.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public BeepTextBox SearchBox { get; set; }

    public BeepMyWidget()
    {
        // Constructor stays empty — designer creates the child.
        // SearchBox may be null in test/early paths; always null-check.
    }
}

public class BeepMyWidgetDesigner : ControlDesigner
{
    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        var host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host == null) return;
        if (component is not BeepMyWidget widget) return;
        if (widget.SearchBox != null) return;          // already exists; do not duplicate

        var changeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
        var prop = TypeDescriptor.GetProperties(widget)["SearchBox"];

        // 1. Notify designer that the property is about to change
        changeSvc?.OnComponentChanging(widget, prop);

        // 2. Use host.CreateComponent — this is the KEY line
        //    It registers the child with the designer host so it gets serialized.
        var tb = (BeepTextBox)host.CreateComponent(typeof(BeepTextBox));
        tb.Name   = "searchBox";
        tb.Dock   = DockStyle.Top;
        tb.PlaceholderText = "Search…";

        // 3. Add to parent's Controls (will be serialized too)
        widget.Controls.Add(tb);
        widget.SearchBox = tb;

        // 4. Notify designer that the property has changed
        changeSvc?.OnComponentChanged(widget, prop, null, tb);
    }
}
```

When the user drags `BeepMyWidget` from the toolbox onto a form, the IDE runs the designer, which calls `host.CreateComponent(typeof(BeepTextBox))`. The TextBox gets registered with the IDE's design surface and is serialized into the form's `*.Designer.cs`.

#### Pattern 2 — Adding a collection of children (e.g. tabs, fields, list items)

```csharp
[Designer(typeof(BeepMyTabControlDesigner))]
public partial class BeepMyTabControl : BaseControl
{
    [Browsable(true)]
    [Category("Layout")]
    [Description("The tabs in this control.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public BindingList<BeepTabPage> Pages { get; } = new();

    public BeepMyTabControl()
    {
        // Empty — designer creates initial tabs
    }
}

public class BeepMyTabControlDesigner : ParentControlDesigner
{
    public override void Initialize(IComponent component)
    {
        base.Initialize(component);
        var host = (IDesignerHost)GetService(typeof(IDesignerHost));
        if (host == null || component is not BeepMyTabControl tab) return;
        if (tab.Pages.Count > 0) return;

        var changeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
        var prop = TypeDescriptor.GetProperties(tab)["Pages"];

        for (int i = 0; i < 2; i++)   // start with 2 tabs
        {
            changeSvc?.OnComponentChanging(tab, prop);
            var page = (BeepTabPage)host.CreateComponent(typeof(BeepTabPage));
            page.Text = $"Tab {i + 1}";
            tab.Controls.Add(page);
            tab.Pages.Add(page);
            changeSvc?.OnComponentChanged(tab, prop, null, page);
        }
    }
}
```

#### Pattern 3 — Programmatic addition via a DesignerAction verb

```csharp
// In your DesignerActionList
public class BeepMyWidgetActionList : DesignerActionList
{
    private readonly BeepMyWidgetDesigner _designer;
    public BeepMyWidgetActionList(BeepMyWidgetDesigner designer) : base(designer.Component)
    {
        _designer = designer;
    }

    public void AddChildButton()
    {
        var host = (IDesignerHost)_designer.GetService(typeof(IDesignerHost));
        var widget = (BeepMyWidget)_designer.Component;
        var changeSvc = (IComponentChangeService)_designer.GetService(typeof(IComponentChangeService));
        var prop = TypeDescriptor.GetProperties(widget)["Buttons"];

        changeSvc?.OnComponentChanging(widget, prop);
        var btn = (BeepButton)host.CreateComponent(typeof(BeepButton));
        btn.Text = $"Button {widget.Buttons.Count + 1}";
        widget.Buttons.Add(btn);
        changeSvc?.OnComponentChanged(widget, prop, null, btn);
    }

    public override DesignerActionItemCollection GetSortedActionItems()
    {
        return new DesignerActionItemCollection
        {
            new DesignerActionMethodItem(this, nameof(AddChildButton), "Add Button", "Layout", "Add a button", true)
        };
    }
}
```

### 10.10 The forbidden patterns (these will break the IDE round-trip)

| Pattern | Why it fails |
|---|---|
| `this.Controls.Add(new BeepButton())` in constructor | Not in `InitializeComponent`; designer doesn't know about it; vanishes on reload |
| `InitializeComponent()` calling code that creates controls | Same as above; the child isn't tracked |
| `new BeepButton()` inside a property getter | Control created on every access; non-deterministic lifetime |
| Setting `BeepLayoutMetrics.X.ScaleSize(this)` in `InitializeComponent` | IDE regenerates `InitializeComponent`; your scaling call is lost. Set in `Form_Load` or a custom designer verb. |
| Calling `host.CreateComponent` from runtime code | `IDesignerHost` is `null` outside the designer |
| Adding to `Controls` before `SuspendLayout` | Triggers a separate layout pass per child |
| Setting properties on a child after `ResumeLayout` | Property change triggers a layout pass |
| Adding a second control to a TableLayoutPanel cell | Designer rejects at design time; runtime silently reparents |

### 10.11 The "Clear and Rebuild" rule

If you modify a Custom Designer or the host's `InitializeComponent`, you must **Clean** the solution before **Rebuilding**. Visual Studio caches designer assemblies per control; stale caches cause confusing errors ("the type `BeepMyWidget` does not contain a constructor that takes 0 arguments" when it clearly does).

```
Build → Clean Solution
Build → Rebuild Solution
File → Close All
```

Then reopen the form.

### 10.12 Out-of-process designer crash recipe: `BackColor = Color.Transparent`

The WinForms out-of-process designer refuses to instantiate any `Control` that sets `BackColor = Color.Transparent` without first enabling `ControlStyles.SupportsTransparentBackColor`. Two safe options:

1. **Derive from `Panel`** instead of `Control` — `Panel` enables `SupportsTransparentBackColor` in its constructor. Used by `BeepForms`.
2. **Set the style flag in `InitializeComponent`**: `SetStyle(ControlStyles.SupportsTransparentBackColor, true); BackColor = Color.Transparent;`. Used by `BeepBlock`.

This is also why `BeepForms` derives from `Panel`: it needs transparency at design time.

### 10.13 Verifying a custom control follows the pattern

1. **Build Clean + Rebuild** the assembly that contains your custom control.
2. **Drag** your custom control from the toolbox onto a fresh form in the designer.
3. **Open the form's `*.Designer.cs`** — the child control must be in `InitializeComponent()`, with its own `SuspendLayout`/`ResumeLayout` block.
4. **Edit the child's property** in the property grid — designer.cs must update.
5. **Move the child** on the design surface — designer.cs must update.
6. **Close + reopen** the form — children reappear unchanged.
7. **Delete the host** from the form — children must be removed from designer.cs cleanly.

If any of these fail, your custom control is breaking the pattern. Re-read § 10.9 and check each step.

### 10.14 Useful breakpoints for debugging designer code

- `ControlDesigner.Initialize(IComponent)` — entry point when the IDE drops your control
- `IDesignerHost.CreateComponent(Type)` — confirms a child is being created
- `IComponentChangeService.OnComponentChanging` / `OnComponentChanged` — confirms the change-service bridge is firing
- The host's `InitializeComponent()` — confirms `SuspendLayout` is bracketing
- The form's `Load` event — designer fields are populated here; safe to access them

### 10.15 Designer serialization attributes cheat sheet

| Attribute | Where | What it does |
|---|---|---|
| `[Designer(typeof(MyDesigner))]` | Class | Associates custom designer with the class |
| `[DesignerSerializationVisibility(Content)]` | Property | Serializes property value as a `Content` block (children appear in their own region) |
| `[DesignerSerializationVisibility(Visible)]` | Property | Serializes property value as a code statement in `InitializeComponent` |
| `[DesignerSerializationVisibility(Hidden)]` | Property | Don't serialize |
| `[Browsable(true/false)]` | Property | Show/hide in property grid |
| `[Category("Layout")]` | Property | Group in property grid |
| `[Description("…")]` | Property | Tooltip in property grid |
| `[DefaultValue(...)]` | Property | IDE only writes the line if value differs from default (smaller designer.cs) |
| `[ToolboxItem(true/false)]` | Class | Show/hide in toolbox |
| `[DesignTimeVisible(true/false)]` | Class | Show/hide in designer when on a non-visual container |
| `[ToolboxBitmap(typeof(...), "name.png")]` | Class | Icon in toolbox |
| `[TypeDescriptionProvider(typeof(...))]` | Class | Custom type description (e.g. to swap properties in property grid) |

### 10.16 Modern .NET (6+) designer behavior

In .NET 6 and later, the Windows Forms designer is **out-of-process**. This means:

- The designer runs in a separate process from the form
- All designer-related types must be in a **separate design-time assembly** (e.g. `TheTechIdea.Beep.Winform.Controls.Design.Server`)
- The `DesignerAttribute` points to the design-time type, not the runtime type
- `[DesignTimeVisible]` and `SupportsTransparentBackColor` matter more than ever (see § 10.12)
- The designer uses `Microsoft.DotNet.DesignTools.Designers.ControlDesigner` instead of `System.Windows.Forms.Design.ControlDesigner`

The Beep codebase already follows this pattern — see `TheTechIdea.Beep.Winform.Controls.Design.Server/` for the design-time assembly.