# Beep ToolTip Enhancement Plan
**Created:** 2026-02-28  
**Scope:** `TheTechIdea.Beep.Winform.Controls\ToolTips\**`

---

## 1. Current State Audit

| Area | Current State |
|------|---------------|
| Types (`ToolTipType`) | 17 types defined (Default → Custom) |
| Placement | 13 positions + Auto |
| Animation | Fade / Scale / Slide / Bounce (enum only) |
| Painter | Single `BeepStyledToolTipPainter` via `ToolTipPainterBase` |
| Manager | Singleton `ToolTipManager` with `ConcurrentDictionary` lifecycle |
| Config | Rich `ToolTipConfig` — title, icon, steps, timing, arrow, shadow |
| Interactive | `ToolTipType.Interactive` declared, **not fully painted** |
| Rich Content | `Html` property declared, **not rendered** |
| Popover | No dedicated popover host |
| Tour/Walkthrough | `ToolTipType.Tutorial` declared, **no step-navigator UI** |
| Preview | `ToolTipType.Preview` declared, **no image/file preview renderer** |
| Accessibility | `ToolTipAccessibilityHelpers.cs` exists, **extent unknown** |
| DPI | Mentioned in helpers, unknown completeness |

---

## 2. Competitive Reference Analysis

| Feature | DevExpress | Telerik | Syncfusion | Ant Design | Figma Comps |
|---------|-----------|---------|------------|------------|-------------|
| Balloon / Arrow tooltip | ✅ | ✅ | ✅ | ✅ | ✅ |
| Rich / HTML content | ✅ | ✅ | ✅ | ✅ | ✅ |
| Popover (persistent panel) | ✅ | ✅ | ✅ | ✅ | ✅ |
| Confirmation popover | ✅ | ✅ | ✅ | ✅ | ✅ |
| Image / media preview | ✅ | ✅ | ✅ | ✅ | ✅ |
| Keyboard shortcut badge | ✅ | ❌ | ❌ | ✅ | ✅ |
| Guided tour / walkthrough | ✅ | ✅ | ✅ | ✅ | ✅ |
| Pinned / sticky tooltip | ✅ | ✅ | ❌ | ❌ | ✅ |
| Caret arrow with offset | ✅ | ✅ | ✅ | ✅ | ✅ |
| Action buttons inside | ✅ | ✅ | ✅ | ✅ | ✅ |
| Smart screen collision | ✅ | ✅ | ✅ | ✅ | ✅ |
| Spring/elastic animation | ✅ | ✅ | ✅ | ✅ | ✅ |
| Glassmorphism / acrylic | ✅ | ✅ | ❌ | ❌ | ✅ |
| Virtual-host (no form) | ✅ | ✅ | ✅ | ✅ | N/A |
| Right-click context hint | ✅ | ✅ | ✅ | ❌ | ✅ |
| Dark/light auto-switch | ✅ | ✅ | ✅ | ✅ | ✅ |
| Accessibility (ARIA equiv) | ✅ | ✅ | ✅ | ✅ | ✅ |

---

## 3. Enhancement Sprints

---

### Sprint 1 — Arrow & Caret Quality (Priority: HIGH)

**Goal:** Make the arrow/caret pixel-perfect across all 13 placements.

**Problems today:**
- Arrow is drawn inline in `PaintArrow()` — no dedicated shape path
- Arrow size not DPI-scaled
- No arrow offset control (caret doesn't align to target mid-point)
- No flip when tooltip hits screen edge

**Deliverables:**
1. `ToolTipArrowPainter` static class  
   - `DrawArrow(Graphics g, Rectangle tooltipBounds, ToolTipPlacement, int arrowSize, int arrowOffset, Color fillColor, Color borderColor)`
   - Draws crisp anti-aliased triangle with optional soft shadow under arrow
2. `ArrowOffset` property on `ToolTipConfig` — pixel offset from center of chosen edge
3. DPI-aware `ArrowSize`: `arrowSize = (int)(8 * DpiScalingHelper.GetDpiScaleFactor(ownerControl))`
4. Auto-flip: if placement puts tooltip off-screen, mirror placement AND flip arrow
5. Arrow style variants per `ToolTipType`:
   - `Rounded` — rounded triangle tips (iOS/Figma style)
   - `Sharp` — flat triangle (Material)
   - `Hidden` — no arrow (popover mode)

---

### Sprint 2 — Rich Content Rendering (Priority: HIGH)

**Goal:** Actually render the `Html` property and support multi-section layouts.

**DevExpress reference:** `XtraToolTipController` supports header, body, footer + HTML-like tags.

**Deliverables:**
1. `ToolTipContentLayout` class:
   ```
   ┌─────────────────────────────┐
   │ [Icon] Title            [✕] │  ← Header row
   ├─────────────────────────────┤
   │  Body text (multi-line,     │
   │  word-wrapped, max width)   │  ← Content area
   ├─────────────────────────────┤
   │ [Shortcut badge]  [Btn][Btn]│  ← Footer row
   └─────────────────────────────┘
   ```
2. `ToolTipSection` enum: `Header | Body | Footer | Divider`
3. Simple markup parser (`ToolTipMarkupParser`):
   - `**bold**`, `*italic*` → `Font` weight toggle
   - `` `code` `` → monospace + tinted background span
   - `[link text]` → underlined clickable text that raises `LinkClicked` event
4. `List<ToolTipContentItem>` on `ToolTipConfig` — ordered content blocks  
   Each item: `{ Section, Text, Icon, IsCode, IsBold, IsItalic }`
5. `ToolTipLayoutHelpers.MeasureContent(config, g, maxWidth)` — returns total `Size` needed

---

### Sprint 3 — Popover Component (Priority: HIGH)

**Goal:** Persistent popover that stays open until explicitly dismissed — distinct from ephemeral tooltip.

**Ant Design reference:** `Popover` component with title + content + trigger control.

**Deliverables:**
1. `BeepPopover` class (inherits `CustomToolTip`)
   - Stays visible until user clicks outside or calls `Dismiss()`
   - `TriggerMode`: `Hover | Click | Focus | Manual`
   - `StayOnHover` — keeps open when mouse moves onto the popover itself
2. `BeepConfirmPopover` — extends `BeepPopover`  
   ```
   ┌─────────────────────────────┐
   │ ⚠  Are you sure?           │
   │ This action cannot be undone│
   │              [Cancel] [Yes] │
   └─────────────────────────────┘
   ```  
   Events: `Confirmed`, `Cancelled`
3. `ToolTipManager.ShowPopover(control, config)` and `DismissPopover(control)`
4. `PopoverConfig : ToolTipConfig` adds `TriggerMode`, `ConfirmText`, `CancelText`, `ConfirmType` (Danger/Primary)

---

### Sprint 4 — Preview Tooltip (Priority: HIGH)

**Goal:** GitHub hover-card / VS Code peek style image and content preview.

**Deliverables:**
1. `PreviewToolTipConfig : ToolTipConfig` adds:
   - `PreviewImagePath` — full image path
   - `PreviewSize` — `Size` (default 280×160)
   - `PreviewTitle`, `PreviewSubtitle`
   - `PreviewFooterText`
   - `LoadPreviewAsync` — `Func<Task<Image>>` delegate for lazy load
2. `PreviewToolTipPainter : ToolTipPainterBase`
   - Top: rounded image area with `ObjectFit = Cover/Contain`
   - Below: title + subtitle + footer text
   - Skeleton loading placeholder (animated gray bars) while `LoadPreviewAsync` resolves
3. `ToolTipManager.ShowPreview(control, previewConfig)`
4. File preview variant:
   - Icon lookup by file extension
   - File name + size + modified date

---

### Sprint 5 — Keyboard Shortcut Badge (Priority: MEDIUM)

**Goal:** Show keyboard shortcuts elegantly, VSCode / Figma style.

**Deliverables:**
1. `ShortcutKeyItem` model: `{ ModifierKeys Modifiers, Keys Key, string DisplayText }`
2. `List<ShortcutKeyItem> Shortcuts` on `ToolTipConfig`
3. `ShortcutBadgePainter` — draws `Ctrl + Shift + P` as individual key-cap badges:
   ```
   ┌─────────────────────────────┐
   │  Save document              │
   │                 [Ctrl][S]   │
   └─────────────────────────────┘
   ```
   - Key cap: rounded rectangle, slightly raised (light top border + dark bottom border)
   - Font: monospace, smaller than body
   - Colors auto-derived from current theme
4. Shortcut rendering position: always footer-right, below body text
5. `ToolTipManager.RegisterShortcut(control, keys, tooltipText)` convenience overload

---

### Sprint 6 — Guided Tour / Walkthrough Engine (Priority: MEDIUM)

**Goal:** Full onboarding/guided tour system, DevExpress TutorialControl level.

**Deliverables:**
1. `BeepTourStep` model:
   ```csharp
   public class BeepTourStep {
       public Control TargetControl { get; set; }
       public string Title { get; set; }
       public string Body { get; set; }
       public string ImagePath { get; set; }
       public ToolTipPlacement Placement { get; set; }
       public Action OnEnter { get; set; }
       public Action OnLeave { get; set; }
   }
   ```
2. `BeepTourManager` — singleton:
   - `BeepTourManager.Instance.StartTour(List<BeepTourStep>)`
   - `Next()`, `Previous()`, `Skip()`, `End()`
   - Events: `TourStarted`, `StepChanged(int current, int total)`, `TourCompleted`, `TourSkipped`
3. `TourToolTipPainter : ToolTipPainterBase`:
   ```
   ┌─────────────────────────────────┐
   │  Step 2 of 5                    │   ← Step indicator
   │  ─────────────────────────────  │
   │  [Image optional]               │
   │  Title                          │
   │  Body text                      │
   │  ● ● ○ ○ ○              Dots   │   ← Progress dots
   │  [Skip]      [← Back] [Next →] │   ← Nav buttons
   └─────────────────────────────────┘
   ```
4. Overlay/spotlight effect: dim background, highlight target control region
5. `BeepTourBuilder` fluent API:
   ```csharp
   BeepTourManager.Instance
       .CreateTour()
       .AddStep(btnSave, "Save your work", "Click here to save")
       .AddStep(tbxSearch, "Search", "Type to filter the list")
       .Build()
       .Start();
   ```

---

### Sprint 7 — Animation System Overhaul (Priority: MEDIUM)

**Goal:** Replace timer-based opacity fade with a proper easing/spring system.

**Current gap:** `ToolTipAnimation` enum has 4 values but only `Fade` is likely implemented; no easing.

**Deliverables:**
1. `EasingFunction` enum: `Linear | EaseIn | EaseOut | EaseInOut | Spring | Bounce`
2. `ToolTipAnimator` class:
   - `Animate(float from, float to, int durationMs, EasingFunction, Action<float> onTick, Action onComplete)`
   - Uses `System.Windows.Forms.Timer` + easing formula per tick
3. Animation types implemented:
   | Type | Show | Hide |
   |------|------|------|
   | `Fade` | Opacity 0→1 | Opacity 1→0 |
   | `Scale` | Scale (0.8→1.0) + Fade | Scale (1.0→0.8) + Fade |
   | `Slide` | Translate from edge + Fade | Translate to edge + Fade |
   | `Bounce` | Scale with overshoot (1.1 → 0.95 → 1.0) | Scale + Fade |
   | `Spring` | Spring physics from edge | Fade out |
4. `AnimationDuration` honored for all types
5. `ToolTipConfig.AnimationEasing` new property → `EasingFunction`

---

### Sprint 8 — Smart Collision & Screen-Edge Avoidance (Priority: MEDIUM)

**Goal:** Tooltip never clips off-screen; auto-repositions with preference cascade.

**Current gap:** Basic position calculation exists but no multi-monitor or collision cascade.

**Deliverables:**
1. `ToolTipPositionResolver` static class:
   - `Resolve(Rectangle targetBounds, Size tooltipSize, ToolTipPlacement preferred, Screen screen)` → `ResolvedPosition`
   - Placement preference cascade: `Preferred → Opposite → CW90 → CCW90 → BestFit`
2. Multi-monitor aware: checks `Screen.FromControl(target)` working area
3. `ResolvedPosition` struct: `{ Point Location, ToolTipPlacement ActualPlacement, bool WasFlipped }`
4. Arrow direction updates to reflect `ActualPlacement` after flip
5. DPI-per-monitor: scale tooltip size by target screen DPI

---

### Sprint 9 — Sticky / Pinned Tooltip (Priority: MEDIUM)

**Goal:** User can pin a tooltip open for reference (DevExpress behavior).

**Deliverables:**
1. `Pinnable = true` on `ToolTipConfig`
2. When `Pinnable` is true, a 📌 pin icon appears in tooltip header
3. Clicking pin → tooltip detaches from auto-dismiss timer, gets drag handle
4. Pinned tooltip can be dragged anywhere on screen
5. ToolTipManager tracks pinned instances separately; `DismissAllPinned()` API
6. Pinned tooltip gets a slightly different visual treatment: thicker border, drop shadow, titlebar accent

---

### Sprint 10 — Glassmorphism & Acrylic Variants (Priority: LOW-MEDIUM)

**Goal:** Modern frosted-glass tooltip style per `BeepControlStyle.GlassAcrylic / Glassmorphism`.

**Deliverables:**
1. `GlassToolTipPainter : ToolTipPainterBase`
   - Semi-transparent background (alpha 180–200)
   - Backdrop blur simulation: capture background bitmap, apply `FastBlur`, draw below fill
   - Subtle noise texture overlay 
   - Thin 1px white-alpha border
2. Applies automatically when `config.Style == BeepControlStyle.GlassAcrylic || Glassmorphism || iOS15`
3. `ToolTipPainterFactory` updated to return `GlassToolTipPainter` for glass styles
4. Works over any background (captures parent form area under tooltip)

---

### Sprint 11 — Accessibility & NVDA/Narrator Support (Priority: HIGH)

**Goal:** Tooltips meet WCAG 2.1 AA — perceivable, keyboard navigable, screen-reader compatible.

**Deliverables:**
1. **Keyboard trigger:** Tooltips activated by `Tab` focus, not only hover
2. **Dismiss with Escape:** Any focused/visible tooltip closes on `Escape`
3. **Persist-on-hover:** Tooltip stays open when mouse moves onto it (WCAG 1.4.13)
4. **Contrast checker:** `ToolTipThemeHelpers.EnsureContrast(foreColor, backColor, minRatio: 4.5)` — auto-adjusts if below threshold
5. **ARIA annotations (WinForms equivalent):**
   - Set `AccessibleName` = title
   - Set `AccessibleDescription` = body text
   - Set `AccessibleRole` = `Tooltip`
6. `ToolTipAccessibilityHelpers` fully implemented:
   - `ConfigureAccessibility(CustomToolTip, ToolTipConfig)` 
   - `AnnounceToScreenReader(text)` using `NotifyClients` / `AccessibleObject`
7. High-contrast theme variant: uses `ToolTipType.HighContrast` with `SystemColors`

---

### Sprint 12 — Virtual Tooltip Host (No Form) (Priority: LOW)

**Goal:** Render tooltips directly onto a `Graphics` context (for grid cells, custom draw, etc.)

**DevExpress reference:** `ToolTipController` renders without `.Show()` into grid cell paint.

**Deliverables:**
1. `IToolTipHost` interface:
   ```csharp
   public interface IToolTipHost {
       void RenderToGraphics(Graphics g, Rectangle targetRect, ToolTipConfig config, IBeepTheme theme);
       Size MeasureTooltip(Graphics g, ToolTipConfig config);
   }
   ```
2. `VirtualToolTipHost : IToolTipHost` — stateless rendering, no `Form` created
3. Use case: `BeepGridPro` cell hover renders inline tooltip via `VirtualToolTipHost`
4. `ToolTipManager.GetVirtualHost()` factory method

---

## 4. New File Map

```
ToolTips/
├── BeepPopover.cs                          (Sprint 3)
├── BeepConfirmPopover.cs                   (Sprint 3)
├── BeepTourManager.cs                      (Sprint 6)
├── BeepTourStep.cs                         (Sprint 6)
├── BeepTourBuilder.cs                      (Sprint 6 - fluent)
├── PopoverConfig.cs                        (Sprint 3)
├── PreviewToolTipConfig.cs                 (Sprint 4)
├── ShortcutKeyItem.cs                      (Sprint 5)
│
├── Painters/
│   ├── BeepStyledToolTipPainter.cs         (existing — extend)
│   ├── TourToolTipPainter.cs               (Sprint 6)
│   ├── PreviewToolTipPainter.cs            (Sprint 4)
│   ├── GlassToolTipPainter.cs              (Sprint 10)
│   ├── ToolTipPainterFactory.cs            (new — replaces inline if-chains)
│   └── IToolTipPainter.cs                  (existing — extend)
│
├── Helpers/
│   ├── ToolTipArrowPainter.cs              (Sprint 1)
│   ├── ToolTipMarkupParser.cs              (Sprint 2)
│   ├── ToolTipContentLayout.cs             (Sprint 2)
│   ├── ShortcutBadgePainter.cs             (Sprint 5)
│   ├── ToolTipAnimator.cs                  (Sprint 7)
│   ├── ToolTipPositionResolver.cs          (Sprint 8)
│   ├── ToolTipAccessibilityHelpers.cs      (existing — Sprint 11 complete)
│   └── VirtualToolTipHost.cs              (Sprint 12)
│
└── Models/
    ├── ToolTipContentItem.cs               (Sprint 2)
    ├── ToolTipSection.cs                   (Sprint 2)
    ├── ResolvedPosition.cs                 (Sprint 8)
    └── EasingFunction.cs                  (Sprint 7)
```

---

## 5. `ToolTipConfig` Property Additions

```csharp
// Sprint 1 — Arrow
public int ArrowOffset { get; set; } = 0;
public ToolTipArrowStyle ArrowStyle { get; set; } = ToolTipArrowStyle.Sharp;

// Sprint 2 — Rich Content  
public List<ToolTipContentItem> ContentItems { get; set; }
public bool UseMarkup { get; set; } = false;

// Sprint 4 — Preview
public string PreviewImagePath { get; set; }
public Size PreviewImageSize { get; set; } = new Size(280, 160);
public Func<Task<Image>> LoadPreviewAsync { get; set; }

// Sprint 5 — Shortcuts
public List<ShortcutKeyItem> Shortcuts { get; set; }

// Sprint 7 — Animation
public EasingFunction AnimationEasing { get; set; } = EasingFunction.EaseOut;

// Sprint 9 — Sticky
public bool Pinnable { get; set; } = false;
public bool IsPinned { get; set; } = false;

// Sprint 11 — Accessibility
public bool PersistOnHover { get; set; } = true;
public bool KeyboardTriggerable { get; set; } = true;
public float MinContrastRatio { get; set; } = 4.5f;
```

---

## 6. `ToolTipManager` API Additions

```csharp
// Popover (Sprint 3)
Task ShowPopoverAsync(Control target, PopoverConfig config);
void DismissPopover(Control target);

// Preview (Sprint 4)  
Task ShowPreviewAsync(Control target, PreviewToolTipConfig config);

// Tour (Sprint 6)
BeepTourBuilder CreateTour();

// Pinned (Sprint 9)
void DismissAllPinned();
IReadOnlyList<ToolTipInstance> GetPinnedTooltips();

// Virtual (Sprint 12)
IToolTipHost GetVirtualHost();
```

---

## 7. Priority & Effort Matrix

| Sprint | Feature | Priority | Effort | Visual Impact | Completeness Gap |
|--------|---------|---------|--------|---------------|-----------------|
| 1 | Arrow & Caret Quality | 🔴 HIGH | S | High | Medium |
| 2 | Rich Content Rendering | 🔴 HIGH | M | High | High |
| 3 | Popover Component | 🔴 HIGH | M | High | High |
| 4 | Preview Tooltip | 🔴 HIGH | M | High | High |
| 11 | Accessibility | 🔴 HIGH | S | Low | High |
| 5 | Keyboard Shortcut Badge | 🟡 MED | S | Medium | High |
| 6 | Guided Tour Engine | 🟡 MED | L | Very High | High |
| 7 | Animation Overhaul | 🟡 MED | M | High | Medium |
| 8 | Smart Collision | 🟡 MED | M | Medium | Medium |
| 9 | Sticky / Pinned | 🟡 MED | M | Medium | High |
| 10 | Glass / Acrylic | 🟢 LOW | S | High | Medium |
| 12 | Virtual Host | 🟢 LOW | M | Low | High |

> S = 1–2 days · M = 3–5 days · L = 1–2 weeks

---

## 8. Recommended Implementation Order

```
Sprint 1 (Arrow)  →  Sprint 11 (A11y)  →  Sprint 7 (Animation)
    ↓
Sprint 2 (Rich)  →  Sprint 5 (Shortcuts)  →  Sprint 3 (Popover)
    ↓
Sprint 4 (Preview)  →  Sprint 8 (Collision)  →  Sprint 9 (Sticky)
    ↓
Sprint 6 (Tour)  →  Sprint 10 (Glass)  →  Sprint 12 (Virtual)
```

---

## 9. Design Reference Examples

### Figma layout tokens for tooltip
```
background:    theme.surface-overlay  (rgba white/black 0.9 + blur)
border-radius: 8px (Material) / 12px (iOS) / 4px (Fluent)
padding:       8px 12px
arrow-size:    8×8px  (point) 
min-width:     80px
max-width:     320px
shadow:        0 4px 12px rgba(0,0,0,0.15)
title-font:    theme.caption + Bold
body-font:     theme.body2
```

### DevExpress SuperTip structure
```
[Icon 32×32] | Title (bold, 13px)
             | Separator line
             | Body text (wrapped, 11px)  
             | [Footer text (italic, 10px)]
```

### Ant Design Popover
```
Title (bold header)
─────────────────
Content area (any React node)
— Trigger: hover | click | focus | contextMenu
— Arrow: offset-able caret, 8px
— Placement: 12 positions + auto-flip
```
