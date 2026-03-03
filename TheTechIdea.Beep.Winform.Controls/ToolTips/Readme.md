# ToolTips — Beep Winform Controls

This folder contains the complete enhanced tooltip system for Beep Winform Controls.  
All components follow the `BaseControl` architecture and use `BeepThemesManager`/`BeepStyling` for consistent theming.

---

## Architecture Overview

```
ToolTips/
├── ToolTipEnums.cs               — All tooltip-related enumerations
├── ToolTipConfig.cs              — Configuration POCO (all options)
├── IToolTipHost.cs               — Abstract host interface (Sprint 12)
├── CustomToolTip.cs              — Main form (partial class root)
├── CustomToolTip.Core.cs         — Core fields, properties, constructor
├── CustomToolTip.Methods.cs      — ApplyConfig, ShowAsync, HideAsync, UpdatePosition
├── CustomToolTip.Animation.cs    — AnimateInAsync, AnimateOutAsync, easing pipeline
├── CustomToolTip.Positioning.cs  — CalculatePlacement, AdjustForPlacement, ConstrainToScreen
├── CustomToolTip.Drawing.cs      — OnPaint, OnPaintBackground, Dispose
├── CustomToolTip.Accessibility.cs— Sprint 11: keyboard dismiss, WCAG contrast, UIA accessible name
├── ToolTipManager.cs             — Singleton lifecycle manager
├── BeepPopover.cs                — Sprint 3: action-button popover
├── BeepPinnedTooltip.cs          — Sprint 9: pinnable / draggable tooltip
├── BeepTourStep.cs               — Sprint 6: tour step model
├── BeepTourBuilder.cs            — Sprint 6: fluent tour builder
├── BeepTourManager.cs            — Sprint 6: singleton tour orchestrator
├── PopoverConfig.cs              — Sprint 3: popover-specific config
│
├── Painters/
│   ├── ToolTipPainterBase.cs             — Abstract base (6 abstract members)
│   ├── BeepStyledToolTipPainter.cs       — Default styled painter (arrow + shortcut badges)
│   ├── PreviewToolTipPainter.cs          — Sprint 4: hover-card with image + skeleton
│   ├── TourToolTipPainter.cs             — Sprint 6: tour step painter with nav controls
│   ├── GlassToolTipPainter.cs            — Sprint 10: glassmorphism / frosted-glass
│   └── ToolTipPainterFactory.cs          — Routes LayoutVariant → painter instance
│
└── Helpers/
    ├── ToolTipArrowPainter.cs            — Sprint 1: DPI-aware arrow geometry
    ├── ToolTipMarkupParser.cs            — Sprint 2: inline markup (bold/italic/code/link)
    ├── ShortcutBadgePainter.cs           — Sprint 5: keyboard shortcut key-cap badges
    ├── ToolTipAnimator.cs                — Sprint 7: timer-driven easing animator
    ├── ToolTipPositionResolver.cs        — Sprint 8: multi-monitor placement cascade
    ├── VirtualToolTipHost.cs             — Sprint 12: IToolTipHost WinForms implementation
    ├── ToolTipAccessibilityHelpers.cs    — WCAG contrast, high-contrast detection
    ├── ToolTipAnimationHelpers.cs        — Static easing math functions
    ├── ToolTipLayoutHelpers.cs           — CalculateLayout, CalculateOptimalSize
    ├── ToolTipPositioningHelpers.cs      — Score-based auto-placement, screen clamp
    ├── ToolTipStyleAdapter.cs            — Resolves (background, foreground, border) tuple
    ├── ToolTipThemeHelpers.cs            — Theme color extraction
    └── ToolTipExtensions.cs             — Fluent extension methods
```

---

## Key Enumerations (`ToolTipEnums.cs`)

| Enum | Values | Purpose |
|---|---|---|
| `ToolTipLayoutVariant` | Simple, Rich, Card, Preview, Tour, Shortcut, **Glass** | Selects painter |
| `ToolTipArrowStyle` | Sharp, Rounded, Hidden | Arrow geometry style |
| `ToolTipTriggerMode` | Hover, Focus, Click, Manual | How tooltip is triggered |
| `EasingFunction` | Linear, EaseIn, EaseOut, EaseInOut, Spring, Bounce, BackOut | Animation easing |
| `ToolTipSection` | Header, Body, Divider, Footer | Rich content sections |

---

## Quick Usage Examples

### Simple tooltip (default)
```csharp
var config = new ToolTipConfig
{
    Title   = "Save File",
    Message = "Saves the current document to disk.",
};
await ToolTipManager.Instance.ShowTooltipAsync(saveButton, config);
```

### Rich card tooltip
```csharp
var config = new ToolTipConfig
{
    LayoutVariant = ToolTipLayoutVariant.Card,
    Title         = "Pro Feature",
    Message       = "Upgrade your plan to unlock this.",
};
```

### Glassmorphism style
```csharp
var config = new ToolTipConfig { LayoutVariant = ToolTipLayoutVariant.Glass, Title = "Hello" };
```

### Shortcut badge tooltip
```csharp
var config = new ToolTipConfig
{
    LayoutVariant = ToolTipLayoutVariant.Shortcut,
    Message       = "Undo last action",
    Shortcuts     = new List<ShortcutKeyItem> { new ShortcutKeyItem("Ctrl+Z") }
};
```

### Popover with action buttons
```csharp
var popoverCfg = new PopoverConfig
{
    Title            = "Delete item?",
    Message          = "This cannot be undone.",
    PrimaryButtonText   = "Delete",
    SecondaryButtonText = "Cancel",
    OnPrimaryClick   = () => itemService.Delete(item),
};
await ToolTipManager.Instance.ShowPopoverAsync(deleteButton, popoverCfg);
```

### Pinned / draggable tooltip
```csharp
var cfg = new ToolTipConfig { Title = "Notes", Message = "…", Pinnable = true, IsPinned = true };
var pinned = new BeepPinnedTooltip();
pinned.ApplyPinnedConfig(cfg, Control.MousePosition);
```

### Guided tour
```csharp
BeepTourManager.Instance
    .CreateTour()
    .AddStep(fileMenuButton, "File Menu", "Open, save, and print documents.")
    .AddStep(toolbar,        "Toolbar",   "Quick access to common actions.")
    .Build();
await BeepTourManager.Instance.StartAsync();
```

### Virtual host (headless / testable)
```csharp
IToolTipHost host = new VirtualToolTipHost();
await host.ShowAsync(config, new Point(100, 200));
```

---

## Accessibility (Sprint 11)

- **Escape key** dismisses any tooltip when `ToolTipConfig.KeyboardTriggerable = true`.  
- **Contrast enforcement**: painting code calls `EnforceContrastIfNeeded()` before rendering text.  
- **Screen reader**: `AccessibleName` and `AccessibleRole = ToolTip` are set automatically by `SyncAccessibility()`.  
- **High-contrast mode**: check `CustomToolTip.IsHighContrastActive` to disable gradients and translucency.

---

## Creating a Custom Painter

1. Inherit from `ToolTipPainterBase`.
2. Implement all 6 abstract members (`Paint`, `PaintBackground`, `PaintBorder`, `PaintShadow`, `PaintArrow`, `PaintContent`).
3. Register in `ToolTipPainterFactory` by adding a new `ToolTipLayoutVariant` case.

```csharp
public class MyCustomPainter : ToolTipPainterBase
{
    public override void Paint(Graphics g, Rectangle bounds, ToolTipConfig config,
                               ToolTipPlacement placement, IBeepTheme theme) { … }
    public override void PaintBackground(…) { … }
    public override void PaintBorder(…) { … }
    public override void PaintShadow(…) { … }
    public override void PaintArrow(…) { … }
    public override void PaintContent(…) { … }
}
```
