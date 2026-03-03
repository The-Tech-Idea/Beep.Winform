# BeepComboBox — Painting, Sizing & Scaling Fix Plan

> **Created**: 2026-03-02  
> **Updated**: 2026-03-02  
> **Scope**: `ComboBoxes/` partial files, `Helpers/BeepComboBoxHelper.cs`, `Painters/BaseComboBoxPainter.cs`  
> **Problem**: Drop-down arrow sizing wrong, inline editor drifts downward, layout shifts on form style / focus / theme changes  

---

## Why BeepComboBox Breaks But BeepButton Does Not

BeepButton and BeepComboBox both inherit from `BaseControl` and both receive the same
`DrawingRect` shifts when `ControlStyle` / `FormStyle` / focus state changes.
The critical difference is their **architecture**:

### BeepButton — Paint-Only (no persistent child layout)

```
DrawContent(g)
  └── base.DrawContent(g)       ← ClassicBaseControlPainter computes DrawingRect
  └── contentRect = DrawingRect  ← grab the CURRENT rect…
  └── CalculateLayout(contentRect, …, out imageRect, out textRect)
  └── DrawImageAndText(g)        ← paint imageRect / textRect into g
```

**Key:** `contentRect`, `imageRect`, `textRect` are **local variables**, recomputed from
scratch every single paint frame. There is no cached layout, no child WinForms control
whose `.Bounds` must match, and no hit-test rects persisted between paints. When
`DrawingRect` shifts 3 px because the border style changed, the next `DrawContent` call
picks up the new rect automatically and everything draws in the right place.

### BeepComboBox — Cached Layout + Live Child Control

```
DrawContent(g)
  └── base.DrawContent(g)       ← ClassicBaseControlPainter computes DrawingRect
  └── Paint(g, DrawingRect)
        └── UpdateLayout()       ← BUT only runs when _layoutCacheValid == false
              └── _helper.CalculateLayout(DrawingRect, out _textAreaRect, …)
              └── cache _textAreaRect, _dropdownButtonRect, _imageRect
        └── _comboBoxPainter.Paint(g, this, bounds)  ← paints using cached rects
```

Plus a **real WinForms child**: `_inlineEditor.Bounds = _textAreaRect`

**Three architectural problems that BeepButton does NOT have:**

| Problem | BeepButton | BeepComboBox |
|---------|-----------|--------------|
| **1. Stale cached rects** | N/A — rects are locals | `_textAreaRect`, `_dropdownButtonRect` are fields cached across frames. When `DrawingRect` shifts but the cache-invalidation conditions don't all fire, stale rects persist. |
| **2. Real child control positioning** | N/A — no child controls | `_inlineEditor` (BeepTextBox) is a real WinForms child whose `.Bounds` is set to `_textAreaRect` only in `ShowInlineEditor()` and `OnResize()`. Focus / style / theme changes shift `DrawingRect` but **don't** resync the editor's `.Bounds` → it stays at old Y → **drifts down**. |
| **3. Hit-test areas must match painted areas** | N/A — click is on whole control | `OnMouseDown` uses `_dropdownButtonRect.Contains(e.Location)` and `_textAreaRect.Contains(e.Location)` for click routing. If cached rects are stale, clicks hit the wrong zone. |

### Why the arrow area resizes wrong

The arrow icon size is computed as `Min(buttonRect.Width, buttonRect.Height) - ScaleX(8)`.
`buttonRect.Height` = `DrawingRect.Height − InnerPadding.Vertical`. When the form style
changes, `DrawingRect` may shrink (thicker border / shadow insets eat space), so the arrow
shrinks too. BeepButton has no such "sub-area that must maintain a fixed visual size" — its
entire content just reflows.

### Summary: the fix is to make BeepComboBox behave like BeepButton where possible

1. **Eliminate stale caches** — recalculate every paint from the current `DrawingRect`
   (cheap: just 3 rect computations) instead of caching across frames.
2. **Resync child control on every layout** — whenever `_textAreaRect` is recalculated,
   immediately update `_inlineEditor.Bounds`.
3. **Use fixed-logical sizes for the arrow** — don't derive icon size from the variable
   `buttonRect.Height`.

---

## Progress Tracker

| Phase | Title | Priority | Status |
|-------|-------|----------|--------|
| 1 | Stabilize DrawingRect consumption | P0 | ✅ Done |
| 2 | Reposition inline editor on every layout change | P0 | ✅ Done |
| 3 | Enforce height via SetBoundsCore | P0 | ✅ Done |
| 4 | Stabilize arrow/button sizing | P0 | ✅ Done |
| 5 | Re-enable OnDpiScaleChanged | P1 | ✅ Done |
| 6 | Fix font initialization | P1 | ✅ Done |
| 7 | Guard against paint-time BackColor thrash | P2 | ✅ Done |

**Status legend:** ⬜ Not started · 🔄 In progress · ✅ Done · ❌ Blocked

---

## Root-Cause Analysis

### RC-1: `DrawingRect` is unstable across style/focus changes
`ClassicBaseControlPainter.UpdateLayout()` (ClassicBaseControlPainter.cs L42–175) computes `DrawingRect` from `ContentShape.GetBounds()`. This depends on `ControlStyle`, `UseFormStylePaint`, `BorderThickness`, `ShadowOffset`, and focus-state border thickness. When any of these change, `DrawingRect.X`/`.Y` shift — every internal rect (`_textAreaRect`, `_dropdownButtonRect`) shifts with it.

**Why BeepButton is immune:** It reads `DrawingRect` into a local `contentRect` on every
`DrawContent` call and computes `imageRect` / `textRect` as locals — nothing persists.

### RC-2: Inline editor not repositioned after focus/theme/style changes
`_inlineEditor.Bounds = _textAreaRect` is only set in two places:
- `ShowInlineEditor()` (BeepComboBox.Methods.cs L455–482)
- `OnResize()` (BeepComboBox.Events.cs L338–349)

But `DrawingRect` also changes on focus gain/loss, theme swap, and `ControlStyle` change. The editor is never repositioned in those scenarios → it stays at old coordinates → **drifts downward**.

**Why BeepButton is immune:** It has no child control to position.

### RC-3: No height enforcement
No `SetBoundsCore` override exists. Parent layout managers, dock/anchor, and designer can freely change the control height, breaking the carefully-calculated internal geometry.

### RC-4: Arrow icon depends on a shrinking `buttonRect.Height`
Arrow size = `Min(buttonRect.Width, buttonRect.Height) - ScaleX(8)` (BaseComboBoxPainter.cs L195). When `DrawingRect` shrinks (style change), `buttonRect.Height` shrinks, and the arrow becomes tiny or disappears.

### RC-5: `OnDpiScaleChanged` is commented out
(BeepComboBox.Methods.cs L707–722). DPI changes on multi-monitor setups don't trigger layout re-initialization.

### RC-6: Constructor sets `Size(200,40)` then immediately overrides height to 32
(BeepComboBox.Core.cs L192). The 40px flash causes a resize event before initialization completes.

---

## Phase 1 — Stabilize DrawingRect Consumption (fixes RC-1)

**Goal**: Make the combo's internal layout immune to `DrawingRect` position shifts while still painting inside `DrawingRect`.

**File**: `BeepComboBoxHelper.cs` — `CalculateLayout()`

1. Change `CalculateLayout` to work with a **zero-origin** copy of `drawingRect`, then offset back at the end:
   ```csharp
   // Before computing sub-rects:
   int offsetX = drawingRect.X;
   int offsetY = drawingRect.Y;
   var localRect = new Rectangle(0, 0, drawingRect.Width, drawingRect.Height);
   // ... all calculations use localRect ...
   // After computing:
   textAreaRect.Offset(offsetX, offsetY);
   dropdownButtonRect.Offset(offsetX, offsetY);
   imageRect.Offset(offsetX, offsetY);
   ```
   This ensures the **shape** of internal rects depends only on the **size** of `DrawingRect`, not its absolute position. The offset is applied once at the end, making it immune to cumulative rounding errors.

2. Add a `_lastDrawingRectOrigin` field to `BeepComboBox.Core.cs`. After `UpdateLayout()`, if the origin changed but size didn't, just offset all cached rects instead of a full recalculation. This prevents re-layout on pure position shifts.

**Files changed**: `BeepComboBoxHelper.cs`, `BeepComboBox.Core.cs`

---

## Phase 2 — Reposition Inline Editor on Every Layout Change (fixes RC-2)

**Goal**: Ensure `_inlineEditor.Bounds` is always in sync with `_textAreaRect`.

**File**: `BeepComboBox.Core.cs` — `UpdateLayout()`

1. At the end of `UpdateLayout()`, after all rect calculations and RTL mirroring, add:
   ```csharp
   // Always keep inline editor positioned correctly
   if (_inlineEditor != null && _inlineEditor.Visible)
   {
       if (_inlineEditor.Bounds != _textAreaRect)
           _inlineEditor.Bounds = _textAreaRect;
   }
   ```

2. In `OnControlGotFocus` (BeepComboBox.Core.cs L307–314), after invalidating layout cache, also call:
   ```csharp
   if (_inlineEditor != null && _inlineEditor.Visible)
   {
       UpdateLayout();
       _inlineEditor.Bounds = _textAreaRect;
   }
   ```

3. In `OnControlLostFocus`, do the same.

4. In `ApplyTheme()` (BeepComboBox.Methods.cs L660–705), after `_layoutCacheValid = false`, add inline editor resync.

**Files changed**: `BeepComboBox.Core.cs`, `BeepComboBox.Methods.cs`

---

## Phase 3 — Enforce Height via SetBoundsCore (fixes RC-3)

**Goal**: Lock the control to a single-line height based on `SizeVariant`.

**File**: `BeepComboBox.Core.cs` (new override)

1. Add `SetBoundsCore` override:
   ```csharp
   protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
   {
       // Enforce single-line height from SizeVariant
       int enforcedHeight = SizeVariant switch
       {
           BeepComboBoxSize.Small  => ScaleLogicalY(24),
           BeepComboBoxSize.Large  => ScaleLogicalY(40),
           _                       => ScaleLogicalY(32)
       };
       base.SetBoundsCore(x, y, width, enforcedHeight, specified);
   }
   ```

2. Remove the height assignment from the constructor (currently `Size = new Size(200, 40)`) and replace with:
   ```csharp
   Size = new Size(200, ScaleLogicalY(32));
   ```
   This avoids the wasted 40→32 resize cycle (RC-6).

3. In the `SizeVariant` property setter (BeepComboBox.Properties.cs L530+), simplify — `SetBoundsCore` now handles height enforcement:
   ```csharp
   set
   {
       if (_sizeVariant == value) return;
       _sizeVariant = value;
       InvalidateLayout();
       Size = new Size(Width, 0); // triggers SetBoundsCore which enforces correct height
   }
   ```

**Files changed**: `BeepComboBox.Core.cs`, `BeepComboBox.Properties.cs`

---

## Phase 4 — Stabilize Arrow/Button Sizing (fixes RC-4)

**Goal**: Arrow icon should have a consistent visual size regardless of `DrawingRect` fluctuations.

**File**: `BaseComboBoxPainter.cs` — `DrawDropdownButton()`

1. Compute arrow icon size from a fixed logical value instead of deriving from `buttonRect.Height`:
   ```csharp
   int iconSize = _owner?.ScaleLogicalY(12) ?? 12;
   iconSize = Math.Min(iconSize, Math.Min(buttonRect.Width, buttonRect.Height) - ScaleX(4));
   ```
   This gives a **stable** icon size that only changes with DPI, not with border theme fluctuations.

2. Keep the fallback arrow path (`DrawDropdownArrow`) using the same logic for consistency.

3. In `BeepComboBoxHelper.CalculateLayout`, ensure `dropdownButtonRect` minimum height is `ScaleLogicalY(20)`:
   ```csharp
   int minButtonHeight = _owner.ScaleLogicalY(20);
   dropdownButtonRect = new Rectangle(
       workingRect.Right - buttonWidth,
       workingRect.Y + Math.Max(0, (workingRect.Height - Math.Max(minButtonHeight, workingRect.Height)) / 2),
       buttonWidth,
       Math.Max(minButtonHeight, workingRect.Height)
   );
   ```

**Files changed**: `BaseComboBoxPainter.cs`, `BeepComboBoxHelper.cs`

---

## Phase 5 — Re-enable OnDpiScaleChanged (fixes RC-5)

**Goal**: Correctly re-layout when moving between monitors with different DPIs.

**File**: `BeepComboBox.Methods.cs`

1. Uncomment the `OnDpiScaleChanged` override (lines 707–722).

2. Add inline editor repositioning inside it:
   ```csharp
   protected override void OnDpiScaleChanged(float oldScaleX, float oldScaleY,
                                              float newScaleX, float newScaleY)
   {
       base.OnDpiScaleChanged(oldScaleX, oldScaleY, newScaleX, newScaleY);
       if (!_dropdownButtonWidthSetExplicitly || !_innerPaddingSetExplicitly)
       {
           _layoutDefaultsInitialized = false;
           ApplyLayoutDefaultsFromPainter(force: true, applyHeight: true);
       }
       InvalidateLayout();
       // Force immediate resync
       UpdateLayout();
       if (_inlineEditor != null && _inlineEditor.Visible)
           _inlineEditor.Bounds = _textAreaRect;
   }
   ```

**Files changed**: `BeepComboBox.Methods.cs`

---

## Phase 6 — Fix Font Initialization

**Goal**: Comply with SKILL.md: "Never create inline fonts with `new Font(...)`"

**File**: `BeepComboBox.Core.cs` (line ~170)

1. Change field initializer from:
   ```csharp
   private Font _textFont = new Font("Segoe UI", 9f);
   ```
   To:
   ```csharp
   private Font _textFont;
   ```

2. In the constructor, after BeepThemesManager is accessible, resolve from theme:
   ```csharp
   _textFont = BeepThemesManager.ToFont(_currentTheme?.ComboBoxItemFont) ?? Font;
   ```

3. In `Draw()` fallback (BeepComboBox.Drawing.cs L68), change `?? new Font("Segoe UI", 9f, ...)` to `?? Font` (uses inherited `Control.Font`).

**Files changed**: `BeepComboBox.Core.cs`, `BeepComboBox.Drawing.cs`

---

## Phase 7 — Guard Against Paint-Time BackColor Thrash

**Goal**: Prevent the `BaseControl.DrawContent` BackColor assignment from causing invalidation loops.

**File**: `BeepComboBox.Drawing.cs` — `DrawContent()`

1. Before calling `base.DrawContent(g)`, store current `BackColor`. After the call, if it changed, suppress re-invalidation:
   ```csharp
   protected override void DrawContent(Graphics g)
   {
       var prevBackColor = BackColor;
       base.DrawContent(g);
       // base may have changed BackColor → suppress cascading invalidate
       if (BackColor != prevBackColor)
           _layoutCacheValid = false; // picked up in next paint, doesn't cause a new one
       Paint(g, DrawingRect);
   }
   ```

This is a lightweight guard — the real fix belongs in `BaseControl` but is out of scope here.

**Files changed**: `BeepComboBox.Drawing.cs`

---

## Summary of Changes by File

| File | Phases | Changes |
|------|--------|---------|
| `BeepComboBox.Core.cs` | 1,2,3,6 | Add `_lastDrawingRectOrigin`, offset optimization in `UpdateLayout()`, inline editor resync in `UpdateLayout()/OnGotFocus/OnLostFocus`, `SetBoundsCore` override, fix font initialization |
| `BeepComboBox.Drawing.cs` | 6,7 | Remove inline `new Font(...)`, add BackColor guard in `DrawContent()` |
| `BeepComboBox.Events.cs` | — | No changes (OnResize already handled) |
| `BeepComboBox.Methods.cs` | 2,5 | Inline editor resync in `ApplyTheme()`, uncomment `OnDpiScaleChanged` |
| `BeepComboBox.Properties.cs` | 3 | Simplify `SizeVariant` setter (delegate height to `SetBoundsCore`) |
| `BeepComboBoxHelper.cs` | 1,4 | Zero-origin rect computation, minimum button height |
| `BaseComboBoxPainter.cs` | 4 | Fixed-logical arrow icon size instead of derived-from-height |

---

## Verification Checklist

- [ ] **Style change test**: Switch `ControlStyle` between `Shadcn`, `Radix`, `Linear`, `None` while combo is visible — inline editor must not move, arrow must stay same size.
- [ ] **Focus cycle test**: Click in/out of the combo 20 times rapidly — editor must not drift.
- [ ] **Resize test**: Dock combo in a `Fill` panel and resize the form — layout must scale proportionally.
- [ ] **DPI test**: Move form between 100% and 150% DPI monitors — all elements must rescale correctly.
- [ ] **SizeVariant test**: Toggle Small/Medium/Large — height must lock to 24/32/40 (at 100% DPI), no external override possible.
- [ ] **Theme change test**: Swap theme at runtime — arrow, editor, and padding must recompute without drift.
- [ ] **Grid embedding test**: `BeepGridPro` calls `Draw(g, rect)` — verify arrow and text render correctly in the passed rectangle.

---

## Design Decisions

- **Chose zero-origin approach over fixed-inset approach**: Computing rects relative to `(0,0)` then offsetting is cheaper and more robust than trying to normalize `DrawingRect` insets across all styles.
- **Chose `SetBoundsCore` over `OnResize` guard**: `SetBoundsCore` intercepts height changes at the framework level before any layout occurs, preventing flicker.
- **Chose fixed-logical arrow size over proportional**: `ScaleLogicalY(12)` gives a predictable icon across all styles. Proportional sizing caused the arrow to shrink when borders ate into the available height.
- **Kept `UpdateLayout` pattern for inline editor** (sync in `UpdateLayout()`) rather than a separate timer — avoids additional timer overhead and keeps positioning deterministic.
