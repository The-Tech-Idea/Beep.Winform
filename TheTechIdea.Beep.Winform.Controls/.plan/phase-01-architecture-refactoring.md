# Phase 1: Architecture Refactoring & Beep Compliance

> **Sprint:** 19-20 · **Priority:** Critical · **Complexity:** High
> **Dependency:** None (first phase) · **Estimated Effort:** 3-4 weeks

---

## Objective

Refactor the entire `BeepDocumentHost` component family to comply with `.cursor/rules/mycontrolsonly.mdc` and `.cursor/skills/beep-winform/SKILL.md`. Every control must inherit `BaseControl`, use the painter architecture, be DPI-aware, and theme-driven. This is the foundation upon which all subsequent phases depend.

---

## Current Architecture Violations

### Inheritance Hierarchy

| Control | Current Base | Required Base | Status |
|---------|-------------|---------------|--------|
| `BeepDocumentHost` | `Panel` | `BaseControl` | Violation |
| `BeepDocumentTabStrip` | `Control` | `BaseControl` | Violation |
| `BeepDocumentPanel` | `Panel` | `BaseControl` | Violation |
| `BeepDocumentGroup` | `object` | `BaseControl` | Violation |
| `BeepDocumentFloatWindow` | `BeepiFormPro` | `BeepiFormPro` | OK |
| `BeepDocumentDockOverlay` | `Panel` | `BaseControl` | Violation |
| `BeepDocumentQuickSwitch` | `Form` | `BaseControl`-derived | Violation |
| `BeepDocumentRichTooltip` | `Form` | `BaseControl`-derived | Violation |

### Painting Violations

| Location | Current Pattern | Required Pattern |
|----------|----------------|------------------|
| `BeepDocumentTabStrip.Painting.cs` | Direct `g.FillRectangle`, `g.DrawString`, `g.DrawPath` | Painter classes via `BackgroundPainterFactory`, `BorderPainterFactory` |
| `BeepDocumentHost.cs` (empty state) | Direct `g.DrawLines`, `g.DrawString` | `EmptyStatePainter` using `StyledImagePainter` |
| `BeepDocumentTabStrip.Badges.cs` | Direct `g.FillEllipse`, `g.DrawString` | Badge painter via factory |
| `BeepDocumentDockOverlay.cs` | Direct `g.FillRectangle`, `g.DrawPath` | `DockOverlayPainter` |

### Font Violations

| Location | Current Pattern | Required Pattern |
|----------|----------------|------------------|
| `BeepDocumentTabStrip.Painting.cs` | `new Font(Font.FontFamily, 13f, ...)` | `BeepFontManager.ToFont(_theme.TypographyStyle.TabTitle)` |
| `BeepDocumentHost.cs` (empty state) | `new Font(Font.FontFamily, 9f, ...)` | `BeepFontManager.ToFont(_theme.TypographyStyle.BodySmall)` |

### DPI Scaling Violations

| Location | Issue |
|----------|-------|
| `BeepDocumentTabStrip.Layout.cs` | Hardcoded pixel values (32, 120, etc.) |
| `BeepDocumentHost.Layout.cs` | Unscaled splitter bar width |
| `BeepDocumentTabStrip.Painting.cs` | Unscaled badge dimensions |
| `Tokens/DocumentHostTokens.cs` | Token values not DPI-scaled at runtime |

### Hit-Testing Violations

| Location | Current Pattern | Required Pattern |
|----------|----------------|------------------|
| `BeepDocumentTabStrip.Mouse.cs` | Manual `Rectangle.Contains(pt)` | `DocumentHitTestHelper` with registered hit areas |
| `BeepDocumentHost.Layout.cs` | Manual splitter hit detection | `DocumentHitTestHelper` |

---

## Tasks

### Task 1.1: Create Painter Infrastructure

**Files to Create:**

```
DocumentHost/Painters/
├── ITabStripPainter.cs
├── ChromeTabPainter.cs
├── VSCodeTabPainter.cs
├── UnderlineTabPainter.cs
├── PillTabPainter.cs
├── FlatTabPainter.cs
├── RoundedTabPainter.cs
├── TrapezoidTabPainter.cs
├── OfficeTabPainter.cs
├── FluentTabPainter.cs
├── DockOverlayPainter.cs
├── EmptyStatePainter.cs
├── BadgePainter.cs
├── SplitterBarPainter.cs
└── PainterFactory.cs
```

**Requirements per Painter:**

- Implement `ITabStripPainter` interface
- Accept `Control ownerControl` reference (never cache DPI scale)
- Accept `_textFont` from owner (never create fonts)
- Use `StyledImagePainter` for all icon rendering
- Support hover/focus/pressed/selected states
- Use `BackgroundPainterFactory` for backgrounds
- Use `BorderPainterFactory` for borders
- Use `ShadowPainterFactory` for shadows (float windows, tooltips)
- All pixel values scaled via `DpiScalingHelper.ScaleValue(value, ownerControl)`

### Task 1.2: Create Helper Infrastructure

**Files to Create:**

```
DocumentHost/Helpers/
├── DocumentHitTestHelper.cs
├── DocumentLayoutManager.cs
├── DocumentAnimationHelper.cs
├── DocumentDragHelper.cs
└── DocumentStyleResolver.cs
```

**`DocumentHitTestHelper` Requirements:**

- Register hit areas: `registerHitArea(string name, Rectangle rect)`
- Test hits: `HitTestResult HitTest(Point pt)`
- Support dynamic resize: `UpdateHitArea(string name, Rectangle rect)`
- Clear all: `ClearHitAreas()`
- Return structured result: `{ AreaName, TabIndex, IsCloseButton, IsAddButton, IsSplitter }`

**`DocumentLayoutManager` Requirements:**

- Calculate tab geometry based on available width
- Handle overflow detection
- Calculate splitter positions
- Calculate group boundaries
- Return layout results (never mutate control state directly)
- DPI-aware: all calculations use scaled values

### Task 1.3: Refactor `BeepDocumentTabStrip`

**Changes:**

1. Change inheritance: `Control` to `BaseControl`
2. Implement `IBeepUIComponent` interface
3. Add `ApplyTheme()` override:
   ```csharp
   public override void ApplyTheme()
   {
       _textFont = BeepFontManager.ToFont(CurrentTheme.TypographyStyle.TabTitle);
       _iconFont = BeepFontManager.ToFont(CurrentTheme.TypographyStyle.IconMedium);
       UseThemeColors = true;
       InvalidateLayout();
   }
   ```
4. Replace `OnPaint` with `DrawContent(Graphics g)` pattern
5. Move all painting logic to painter classes
6. Replace manual hit-testing with `DocumentHitTestHelper`
7. Scale all pixel constants via `DpiScalingHelper`
8. Replace `new Font(...)` with `BeepFontManager.ToFont(...)`
9. Replace `g.DrawImage(...)` with `StyledImagePainter.Paint(...)`

**Partial Class Reorganization:**

```
BeepDocumentTabStrip.cs              ← Constructor, DrawContent, ApplyTheme
BeepDocumentTabStrip.Properties.cs   ← Properties (unchanged)
BeepDocumentTabStrip.Events.cs       ← Event handlers
BeepDocumentTabStrip.Mouse.cs        ← Mouse events (use HitTestHelper)
BeepDocumentTabStrip.Keyboard.cs     ← Keyboard handling
BeepDocumentTabStrip.Animations.cs   ← Animation logic
BeepDocumentTabStrip.Accessibility.cs← Accessibility
BeepDocumentTabStrip.HighContrast.cs ← High contrast mode
BeepDocumentTabStrip.Touch.cs        ← Touch handling
```

### Task 1.4: Refactor `BeepDocumentHost`

**Changes:**

1. Change inheritance: `Panel` to `BaseControl`
2. Implement `IBeepUIComponent` interface
3. Add `ApplyTheme()` override
4. Replace `OnContentAreaPaint` with `EmptyStatePainter`
5. Replace manual theme color application with `BeepStyling`
6. Scale all pixel constants via `DpiScalingHelper`
7. Replace `new Font(...)` with `BeepFontManager.ToFont(...)`

**Partial Class Reorganization:**

```
BeepDocumentHost.cs                  ← Constructor, DrawContent, ApplyTheme
BeepDocumentHost.Properties.cs       ← Properties
BeepDocumentHost.Documents.cs        ← Document management
BeepDocumentHost.Layout.cs           ← Layout (use LayoutManager)
BeepDocumentHost.Events.cs           ← Event handlers
BeepDocumentHost.AutoHide.cs         ← Auto-hide logic
BeepDocumentHost.Serialisation.cs    ← Save/Restore layout
BeepDocumentHost.MVVM.cs             ← MVVM binding
BeepDocumentHost.DataBinding.cs      ← Data source binding
BeepDocumentHost.Preview.cs          ← Thumbnail preview
```

### Task 1.5: Refactor `BeepDocumentPanel`

**Changes:**

1. Change inheritance: `Panel` to `BaseControl`
2. Add `ApplyTheme()` override
3. Use `BackgroundPainterFactory` for status bar
4. Use `BorderPainterFactory` for borders
5. Scale all pixel constants

### Task 1.6: Refactor `BeepDocumentGroup`

**Changes:**

1. Convert from plain class to `BaseControl`-derived (or keep as wrapper with BaseControl children)
2. Use `BeepStyling` for style resolution
3. DPI-aware layout calculations

### Task 1.7: Refactor Supporting Controls

**Controls to Refactor:**

- `BeepDocumentDockOverlay` to `BaseControl`
- `BeepDocumentQuickSwitch` to `BaseControl`-derived form
- `BeepDocumentRichTooltip` to `BaseControl`-derived form
- `BeepTabOverflowPopup` to `BaseControl`-derived popup
- `BeepDocumentFloatWindow` already uses `BeepiFormPro` (OK)

### Task 1.8: Update Design Tokens

**File:** `Tokens/DocumentHostTokens.cs`

**Changes:**

- All token values should be base (unscaled) values
- Runtime scaling via `DpiScalingHelper.ScaleValue(token, control)`
- Add new tokens for:
  - `TabMinWidth`, `TabMaxWidth`, `TabPreferredWidth`
  - `PinnedTabWidth`, `PinnedTabIconSize`
  - `SplitterWidth`, `SplitterHotspotWidth`
  - `DockZoneSize`, `DockGuideThickness`
  - `BadgeMinSize`, `BadgePadding`
  - `TooltipMaxWidth`, `ThumbnailWidth`, `ThumbnailHeight`
  - `AnimationDurationMs` (various animations)

### Task 1.9: Update SVG Icons

**Reference:** `SvgsUI.cs` / `Svgs.cs` for all icons

**Icons Needed:**

- `icon_close` — close button
- `icon_add` — add new document
- `icon_pin` / `icon_unpin` — pin toggle
- `icon_float` — float window
- `icon_split_horizontal` / `icon_split_vertical` — split icons
- `icon_chevron_left` / `icon_chevron_right` — scroll overflow
- `icon_chevron_down` — overflow dropdown
- `icon_document` — document icon (empty state)
- `icon_modified` — dirty dot indicator
- `icon_maximize` / `icon_restore` — window state
- `icon_dock_left` / `icon_dock_right` / `icon_dock_top` / `icon_dock_bottom` / `icon_dock_center` — docking guides
- `icon_search` — command palette / quick switch
- `icon_keyboard` — keyboard shortcuts hint

### Task 1.10: Migration & Backward Compatibility

**Requirements:**

- All existing public APIs preserved
- Existing layout JSON files must still load
- Existing event signatures unchanged
- Designer verbs and smart-tags still work
- `BeepDocumentDragManager` still functions
- MVVM interface unchanged

---

## Acceptance Criteria

- [ ] All controls inherit `BaseControl` (or `BeepiFormPro` for windows)
- [ ] All controls implement `IBeepUIComponent`
- [ ] All controls override `ApplyTheme()` with proper font resolution
- [ ] All painting done via painter classes (no direct `g.FillRectangle` etc.)
- [ ] All fonts come from `BeepFontManager.ToFont()`
- [ ] All icons use `StyledImagePainter`
- [ ] All backgrounds use `BackgroundPainterFactory`
- [ ] All borders use `BorderPainterFactory`
- [ ] All shadows use `ShadowPainterFactory`
- [ ] All hit-testing via `DocumentHitTestHelper`
- [ ] All layout calculations via `DocumentLayoutManager`
- [ ] All pixel values DPI-scaled via `DpiScalingHelper`
- [ ] No cached DPI scale factors
- [ ] No inline `new Font(...)` anywhere
- [ ] No direct `g.DrawImage(...)` anywhere
- [ ] Existing public APIs unchanged
- [ ] Existing layouts still load correctly
- [ ] Designer integration still works
- [ ] All controls support 16 style presets
- [ ] High contrast mode works
- [ ] Touch input works
- [ ] Accessibility works

---

## Risk Mitigation

| Risk | Mitigation |
|------|------------|
| `BaseControl` inheritance breaks WndProc | Test thoroughly with `BeepiFormPro`; use `DrawContent` pattern |
| Painter refactoring introduces visual regressions | Side-by-side visual comparison tests |
| DPI scaling changes break existing layouts | Test at 100%, 125%, 150%, 200% DPI |
| Performance regression from painter overhead | Benchmark paint times; optimize hot paths |
| Designer integration breaks | Test all verbs, smart-tags, property grid |

---

## Files Modified

| File | Change Type |
|------|-------------|
| `BeepDocumentHost.cs` | Refactor (inheritance, ApplyTheme, DrawContent) |
| `BeepDocumentHost.*.cs` | Refactor (use helpers, painters) |
| `BeepDocumentTabStrip.cs` | Refactor (inheritance, ApplyTheme, DrawContent) |
| `BeepDocumentTabStrip.*.cs` | Refactor (use helpers, painters) |
| `BeepDocumentPanel.cs` | Refactor (inheritance, ApplyTheme) |
| `BeepDocumentGroup.cs` | Refactor (use BaseControl children) |
| `BeepDocumentDockOverlay.cs` | Refactor (inheritance, painter) |
| `BeepDocumentQuickSwitch.cs` | Refactor (inheritance, painter) |
| `BeepDocumentRichTooltip.cs` | Refactor (inheritance, painter) |
| `BeepTabOverflowPopup.cs` | Refactor (inheritance, painter) |
| `Tokens/DocumentHostTokens.cs` | Update (add new tokens) |

## Files Created

| File | Purpose |
|------|---------|
| `Painters/ITabStripPainter.cs` | Painter interface |
| `Painters/ChromeTabPainter.cs` | Chrome style |
| `Painters/VSCodeTabPainter.cs` | VS Code style |
| `Painters/UnderlineTabPainter.cs` | Underline style |
| `Painters/PillTabPainter.cs` | Pill style |
| `Painters/FlatTabPainter.cs` | Flat style |
| `Painters/RoundedTabPainter.cs` | Rounded style |
| `Painters/TrapezoidTabPainter.cs` | Trapezoid style |
| `Painters/OfficeTabPainter.cs` | Office style |
| `Painters/FluentTabPainter.cs` | NEW Fluent style |
| `Painters/DockOverlayPainter.cs` | Dock overlay |
| `Painters/EmptyStatePainter.cs` | Empty state |
| `Painters/BadgePainter.cs` | Badges |
| `Painters/SplitterBarPainter.cs` | Splitter bar |
| `Painters/PainterFactory.cs` | Painter factory |
| `Helpers/DocumentHitTestHelper.cs` | Hit testing |
| `Helpers/DocumentLayoutManager.cs` | Layout calculations |
| `Helpers/DocumentAnimationHelper.cs` | Animations |
| `Helpers/DocumentDragHelper.cs` | Drag state |
| `Helpers/DocumentStyleResolver.cs` | Style resolution |
