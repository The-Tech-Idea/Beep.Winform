# Form Helpers: Border, Region, and Layout

This folder contains helper components that BeepiForm composes to render a modern, professional window similar to frameworks like DevExpress or Syncfusion.

This document explains how borders are created and drawn, what code paths are involved, and how to reason about changes.

## Overview of the rendering pipeline

- FormLayoutHelper: Provides unified paint/content rectangles respecting Padding.
- FormRegionHelper: Maintains the window Region for rounded corners. Keeps the shape in sync with BorderRadius and window state. When maximized, Region is cleared for full-screen compatibility.
- FormBackgroundPainter: Fills the client background (theme-aware), supports gradients/images.
- FormBorderPainter: Single source of truth for border stroke rendering. Draws both client-path borders and non-client window borders with correct geometry.
- FormCaptionBarHelper: Paints caption UI (title, icon/logo, system buttons) inside the client area as an overlay so hit-testing is reliable.
- FormShadowGlowPainter: Adds soft drop shadow and outer glow around the rounded window path.

## Border creation vs drawing

There are two separate responsibilities to achieve professional-looking borders:

1) Shape/Region creation (rounded corners)
- FormRegionHelper computes and applies a rounded Region when BorderRadius > 0 and the form is not maximimized.
- For maximized windows, Region is removed to avoid clipping and to respect work-area insets.

2) Border drawing (stroke)
- Border stroke is painted in the non-client area (WM_NCPAINT) via BeepiForm, but the drawing logic is delegated to FormBorderPainter.
- FormBorderPainter draws using PenAlignment.Inset on a safe rectangle/path to avoid 1px bleed and to keep all sides crisp on HiDPI.

This split mirrors what pro UI frameworks do: let Windows own the non-client frame space while you take over actual rendering.

## How BeepiForm uses the helpers

- During OnPaint
  - Build the rounded form GraphicsPath (client coords).
  - Paint shadow/glow (if enabled) and fill the path with BackColor.
  - Do NOT draw the border here; it is drawn in the non-client pass for consistent metrics and crisp alignment.

- During WM_NCCALCSIZE
  - If DrawCustomWindowBorder is true and the window is not maximized, reserve BorderThickness on all sides in the non-client band. This gives a dedicated area for the border stroke and prevents caption seams.

- During WM_NCPAINT
  - Fill the reserved non-client band to avoid background showing through.
  - Ask FormBorderPainter to draw the border on window coordinates: PaintWindowBorder(graphics, windowRect, BorderRadius, BorderThickness).

- Region sync
  - On changes to BorderRadius/BorderThickness, or size/state changes, the Region is invalidated and recomputed via FormRegionHelper to keep hit-testing and painting aligned.

## FormBorderPainter details

- PaintWindowBorder(Graphics g, Rectangle windowBounds, int borderRadius, int borderThickness)
  - Skips when thickness <= 0 or the form is maximized.
  - Uses PenAlignment.Inset to keep the stroke fully inside the window image, avoiding off-by-one artifacts.
  - If radius > 0, builds a rounded rectangle path over windowBounds; otherwise draws a rectangle.

- GetBorderColor()
  - Prefers form.BorderColor when set.
  - Falls back to theme BorderColor, then to SystemColors.ControlDark.

These choices match common pro UI frameworks: consistent stroke alignment, HiDPI-safe coordinates, and predictable fallback colors.

## Professional results tips

- Prefer non-client border drawing via DrawCustomWindowBorder = true (default), so the stroke lives outside your content.
- Use PenAlignment.Inset and a -1px safe width/height where appropriate to avoid anti-alias bleed.
- Skip rounded corners when maximized to avoid clipped edges by the OS work area.
- Always invalidate Region and non-client frame (SWP_FRAMECHANGED + RedrawWindow) when thickness/radius changes.

## Key classes and responsibilities

- IBeepModernFormHost: Minimal contract BeepiForm implements so helpers can interact without tight coupling.
- FormRegionHelper: Computes and applies window Region for rounded corners.
- FormBorderPainter: Draws border strokes, both client-path (legacy) and non-client window border (preferred).
- FormLayoutHelper: Centralizes layout rectangles for consistent child and paint layout.

## Change safety checklist

- After editing border/radius logic, verify: normal, maximized, and restore transitions.
- Confirm that WM_NCCALCSIZE reserves the correct non-client band when custom borders are enabled.
- Confirm that Region is null when maximized, and rounded when normal.
- Confirm no 1px seams at the top edge near the caption bar on light backgrounds.

---

If you add a new border style, route stroke drawing through FormBorderPainter to keep one source of truth and avoid drift.
