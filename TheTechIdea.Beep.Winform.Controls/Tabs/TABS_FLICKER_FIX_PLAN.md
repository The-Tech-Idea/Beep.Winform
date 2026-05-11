# Beep Tabs Stability & Flicker Fix Plan

## Executive Summary

The `BeepTabs` control and its supporting classes suffer from instability and flickering due to inheriting from `BaseControl`, which introduces a heavy, multi-layered paint pipeline, transparent background emulation (`WS_EX_TRANSPARENT`), region manipulation, and redundant invalidation cycles that are unnecessary for a tab container.

This plan outlines the architectural changes required to eliminate flicker, reduce CPU overhead, and improve rendering stability by moving the tab components away from `BaseControl` inheritance toward lighter base classes with direct `OnPaint` / `OnPaintBackground` management.

---

## Root Cause Analysis

### 1. BaseControl Paint Pipeline Overhead

`BaseControl` (`BaseControl.Events.cs`) implements a complex paint pipeline:

- `OnPaintBackground` performs parent-background caching via **BitBlt** when `IsTransparentBackground` is true.
- `OnPaint` calls `SafeDraw()`, which chains:
  1. `ClearDrawingSurface(g)` – clears to `BackColor`
  2. `SafeExternalDrawing(g, BeforeContent)` – external drawing hooks
  3. `DrawContent(g)` – strategy-based painter
  4. `DrawLabelAndHelperUniversal(g)` – label/helper text
  5. `SafeExternalDrawing(g, AfterAll)` – more external hooks
  6. `SafeDrawEffects(g)` – ripple/splash overlays
- `CreateParams` adds **`WS_EX_TRANSPARENT`** at runtime, forcing the parent to paint first and causing visible erase-before-draw flicker.
- `OnResize` calls `UpdateControlRegion()`, which rebuilds the `Region` object on every resize, causing additional paint storms.

**Impact on BeepTabs:**  
`BeepTabs` only needs to draw a header strip and host child pages. It does **not** need BitBlt transparency, external drawing hooks, badge rendering, ripple effects, or dynamic region shaping. All of this overhead is pure waste and a source of flicker.

### 2. Double-Buffering Conflicts

- `BaseControl` constructor sets `DoubleBuffered = true` and `OptimizedDoubleBuffer | AllPaintingInWmPaint | SupportsTransparentBackColor`.
- `BeepTabs.Initialization.cs` sets the **same flags again** via `SetStyle(..., true)`.
- `BeepTabPage` and `BeepTabContentHost` also inherit these settings.

Result: multiple layers of double-buffering, buffer allocation churn, and potential conflicts between the WinForms `DoubleBuffered` property and the `ControlStyles` flags.

### 3. BeepTabHeaderHost Is a BaseControl (But Never Shown)

`BeepTabHeaderHost` inherits from `BaseControl` yet it is:
- Never added to the control tree (`Controls.Add`).
- Used purely as a rendering helper (`_headerHost.RenderLegacyHeader(...)`).
- Subject to all `BaseControl` construction overhead (helpers, painters, theme subscriptions, tooltip init, etc.) for zero benefit.

### 4. Redundant Header Background Painting

`BeepTabs.Drawing.cs`:
```csharp
protected override void OnPaintBackground(PaintEventArgs e)
{
    // paints header background
}

protected override void OnPaint(PaintEventArgs e)
{
    // paints header background AGAIN
    // then draws tab headers
}
```

The header background is painted twice per frame because `BeepTabs.OnPaint` does not trust `OnPaintBackground` to have completed the work (or `BaseControl.OnPaintBackground` interferes).

### 5. Aggressive Content-Host Repaint

`BeepTabContentHost.RepaintSelectedPageImmediately`:
- Calls `PerformLayout()`
- Recursively calls `CreateVisibleChildHandles()` (forces handle creation on every tab switch)
- Calls `Invalidate(true)` **and** `Refresh()` on the page
- Calls `Invalidate(false)` on itself

This creates a cascade of `WM_PAINT` messages across the entire child tree every time the selected tab changes.

### 6. CreateGraphics() During Paint

`SyncHeaderSnapshot()` and `GetCurrentHeaderTabRects()` call `CreateGraphics()` to measure tabs. This:
- Creates a temporary device context
- Bypasses the current `PaintEventArgs.Graphics` state
- Can trigger additional paint messages

---

## Proposed Architecture Changes

### Phase 1 – Change Inheritance (High Priority)

| Class | Current Base | Proposed Base | Rationale |
|-------|-------------|---------------|-----------|
| `BeepTabs` | `BaseControl` | `ContainerControl` | Tab control is a container; needs no BaseControl styling/effects pipeline. |
| `BeepTabHeaderHost` | `BaseControl` | Plain class (`object`) | Never a visible control; should be a pure layout/render helper. |
| `BeepTabContentHost` | `BaseControl` | `ContainerControl` | Only needs to host child pages and paint a solid background. |
| `BeepTabPage` | `BaseControl` | `ContainerControl` or keep `BaseControl`* | *Evaluate: if theming of child controls is required, keep `BaseControl`; otherwise use `ContainerControl`.* |

> **Note:** If `BeepTabPage` must remain `BaseControl` to support themed child controls, isolate it so its paint overhead does not affect the tab header or content host.

### Phase 2 – Simplify Paint Pipeline (High Priority)

#### 2.1 BeepTabs.OnPaintBackground
```csharp
protected override void OnPaintBackground(PaintEventArgs e)
{
    // Only clear the non-header content area.
    // Do NOT call base.OnPaintBackground (avoid BaseControl BitBlt/transparent logic).
    Rectangle content = DisplayRectangle;
    if (content.Width > 0 && content.Height > 0)
    {
        using var brush = new SolidBrush(BackColor);
        e.Graphics.FillRectangle(brush, content);
    }
}
```

#### 2.2 BeepTabs.OnPaint
```csharp
protected override void OnPaint(PaintEventArgs e)
{
    if (IsDisposed || Disposing || !IsHandleCreated) return;

    // No base.OnPaint(e) – skip BaseControl entirely.

    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
    e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

    Rectangle headerBounds = BeepTabLayoutHelper.GetHeaderBounds(this);
    if (!headerBounds.IsEmpty && e.ClipRectangle.IntersectsWith(headerBounds))
    {
        // Paint header background once, here.
        _painter?.PaintHeaderBackground(e.Graphics, headerBounds);

        // Draw tab headers.
        DrawTabHeaders(e.Graphics);

        // Draw selection indicator (underline, etc.).
        DrawHeaderSelectionIndicator(e.Graphics);
    }

    DrawErrorOverlay(e.Graphics);
}
```

#### 2.3 BeepTabHeaderHost – Remove Control Overhead

Convert `BeepTabHeaderHost` to a plain helper class:

```csharp
public class BeepTabHeaderHost // NOT a Control
{
    public BeepTabs? TabsOwner { get; private set; }
    public BeepTabHeaderLayoutSnapshot LayoutSnapshot { get; set; } = new();
    // ... interaction state fields ...

    public void RenderHeader(Graphics graphics, BeepTabHeaderRenderRequest request)
    {
        // Existing render logic, minus Control.OnPaint baggage.
    }
}
```

- Remove `OnPaint`, `OnPaintBackground`, `CreateParams`, `AccessibleRole`, etc.
- Keep layout, hit-testing, and rendering methods.

#### 2.4 BeepTabContentHost – Minimal Container

Inherit from `ContainerControl` and override only what is needed:

```csharp
public class BeepTabContentHost : ContainerControl
{
    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // Solid fill only – no BaseControl transparency/BitBlt.
        if (BackColor.A > 0)
            e.Graphics.Clear(BackColor);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // Intentionally empty – children paint themselves.
    }
}
```

- Remove `CreateParams` override (no `WS_EX_TRANSPARENT`).
- Remove `RepaintSelectedPageImmediately` recursive handle creation; let WinForms lazy-create handles.

### Phase 3 – Fix Double-Buffering Setup (Medium Priority)

In `BeepTabs` constructor / initialization:

```csharp
public BeepTabs()
{
    // Single, authoritative double-buffer configuration.
    SetStyle(ControlStyles.OptimizedDoubleBuffer |
             ControlStyles.UserPaint |
             ControlStyles.AllPaintingInWmPaint, true);
    UpdateStyles();

    // Do NOT set DoubleBuffered = true separately (conflicts with UserPaint).
}
```

- Remove duplicate `SetStyle` calls in `BeepTabs.Initialization.cs`.
- Ensure `BeepTabPage` and `BeepTabContentHost` do **not** set `AllPaintingInWmPaint` if they rely on `OnPaintBackground` for child clearing.

### Phase 4 – Eliminate Redundant Invalidations (Medium Priority)

#### 4.1 Content Host
Replace `RepaintSelectedPageImmediately` with:

```csharp
private void ShowSelectedPage(Control? page)
{
    if (page == null || page.IsDisposed) return;

    // Layout is already set by SetSelectedPage.
    // Only invalidate the page and host; do NOT force handle creation.
    page.Invalidate(false);
    Invalidate(false);
}
```

#### 4.2 Header Invalidation
`InvalidateHeader()` currently inflates bounds and intersects with `ClientRectangle`. Ensure it uses a single `Invalidate(Rectangle)` call and does not trigger `Update()` or `Refresh()`.

#### 4.3 Remove `BeginInvoke` in MouseLeave
`BeepTabs.MouseLeave` uses `BeginInvoke(new Action(UpdateHostedContentBounds))`. This defers work to the message loop and can cause a second paint cycle. Replace with synchronous call if safe, or remove if redundant.

### Phase 5 – Remove CreateGraphics() from Paint Path (Medium Priority)

`SyncHeaderSnapshot()` and `GetCurrentHeaderTabRects()` should accept the `Graphics` instance from `PaintEventArgs` instead of creating a new one.

Current:
```csharp
private void SyncHeaderSnapshot()
{
    using Graphics graphics = CreateGraphics();
    SyncHeaderSurface(graphics);
}
```

Proposed:
```csharp
private void SyncHeaderSnapshot(Graphics? graphics = null)
{
    using var g = graphics ?? CreateGraphics();
    SyncHeaderSurface(g);
}
```

Call sites in `OnPaint` pass `e.Graphics`:
```csharp
SyncHeaderSnapshot(e.Graphics);
```

### Phase 6 – Theme & Property Propagation (Low Priority)

Because `BeepTabs` will no longer inherit `BaseControl.Theme`, ensure:
- `BeepTabs` implements `IBeepUIComponent` explicitly or keeps its own `Theme` property.
- `ApplyTheme()` propagates colors to `BeepTabPage` and `BeepTabContentHost` via direct property sets, not through `BaseControl`'s global event subscriptions.
- Remove `BeepThemesManager.ThemeChanged` subscription from `BeepTabs` if it is no longer needed (or keep it lightweight).

---

## File-by-File Change List

| File | Change |
|------|--------|
| `BeepTabs.cs` | Change `class BeepTabs : BaseControl` → `class BeepTabs : ContainerControl`. Keep `IBeepUIComponent` implementation. Remove `UseBaseMouseInputRouting` override. |
| `BeepTabs.Drawing.cs` | Rewrite `OnPaintBackground` and `OnPaint` as shown in Phase 2. Remove `base.OnPaint` / `base.OnPaintBackground` calls. |
| `BeepTabs.Initialization.cs` | Remove duplicate `SetStyle` calls. Simplify `InitializeControlDefaults`. |
| `BeepTabs.Layout.cs` | Remove `OnResize` override if only calling `RefreshHeaderLayoutState`; ensure `UpdateLayout` does not call `Invalidate` redundantly. |
| `BeepTabs.Interaction.cs` | Remove `BeginInvoke` in `BeepTabs_MouseLeave`. Pass `Graphics` to `SyncHeaderSnapshot`. |
| `Hosts/BeepTabHeaderHost.cs` | Remove `: BaseControl`. Convert to plain class. Remove all `protected override` methods. Keep fields and `RenderLegacyHeader`. |
| `Hosts/BeepTabHeaderHost.Painting.cs` | Move rendering logic into the plain class. Remove `OnPaint` override. |
| `Hosts/BeepTabContentHost.cs` | Change `: BaseControl` → `: ContainerControl`. Simplify `OnPaintBackground` / `OnPaint`. Remove `RepaintSelectedPageImmediately` recursion. |
| `BeepTabPage.cs` | Evaluate keeping `: BaseControl` vs `: ContainerControl`. If kept, ensure `OnPaintBackground` / `OnPaint` are lightweight. |
| `Painters/BaseTabPainter.cs` | No structural change; verify no dependency on `BaseControl` properties. |

---

## Validation Checklist

- [ ] Tab header renders without flicker during hover/press.
- [ ] Tab switching does not produce white flashes or background bleed.
- [ ] Resizing the control does not cause header/content tearing.
- [ ] Drag-and-drop reordering is smooth (no ghost images or flicker).
- [ ] High-DPI scaling remains correct (no blurry text or misaligned close buttons).
- [ ] Designer loads without errors (inheritance change is compatible with VS Toolbox).
- [ ] Theme changes apply correctly to header, content host, and tab pages.
- [ ] No `ObjectDisposedException` or `InvalidOperationException` during rapid add/remove tab operations.

---

## Risk Assessment

| Risk | Mitigation |
|------|------------|
| Designer breaks due to inheritance change | Test in VS immediately after `BeepTabs` change. Keep `[ToolboxItem(true)]` and `[ToolboxBitmap]`. |
| Child controls lose theming | `BeepTabPage` may need to stay `BaseControl` temporarily. Apply theme explicitly in `BeepTabs.ApplyTheme()`. |
| Accessibility tools expect `AccessibleRole` | Re-declare `AccessibleRole` and `AccessibleName` on `BeepTabs` and `BeepTabPage` even if not inheriting `BaseControl`. |
| Third-party code references `BaseControl` members | Search solution for casts to `BaseControl` on tab instances. Update to `ContainerControl` or `IBeepUIComponent`. |

---

## Recommended Implementation Order

1. **BeepTabHeaderHost** – lowest risk, removes invisible overhead.
2. **BeepTabContentHost** – simplifies container painting.
3. **BeepTabs** – core change; update paint pipeline and inheritance.
4. **BeepTabPage** – evaluate last; only change if theming can be externalized.
5. **Run validation checklist** – fix regressions before committing.
