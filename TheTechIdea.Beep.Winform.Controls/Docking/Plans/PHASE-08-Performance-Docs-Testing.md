# PHASE 08 — Performance, Docs & Testing

**Goal:** Harden `BeepDockingManager` for many panels and rapid interaction, document it, ship a sample,
and define a repeatable verification matrix.

**Depends on:** all · **Blocks:** —

---

## 8.1 Existing-code disposition (this phase)

| File | Disposition | Notes |
|------|-------------|------|
| `BeepDockingUpdate.cs` | **Reuse** | Batch-update scope for suspend/resume layout + paint. |
| `Layout/DockingLayoutController` cache | **Refactor** | Ensure single recompute per batch; `Invalidate()` coalesces. |
| Renderer glyph/icon usage | **Refactor** | Cache tinted SVGs via `StyledImagePainter` cache; avoid per-paint reload. |

---

## 8.2 Performance work

- `BeginUpdate/EndUpdate` (via `BeepDockingUpdate`) suspends layout + invalidation; one
  `ApplyLayout` + one repaint on resume.
- Layout engine caches `DockLayoutResult`; recompute only on `Invalidate`.
- Cache splitter/caption/tab geometry from layout managers; recompute on size/tree change only.
- Reuse `StyledImagePainter` tinted-image cache for caption/tab/strip/guide glyphs.
- Throttle drag (Phase 02) and splitter (Phase 03) updates to paint cadence.
- Double-buffer all custom-painted surfaces.

## 8.3 Documentation

- `Docking/README.md`: `BeepDockingManager` overview, hosting model, designer.cs persistence, API.
- `CODE-DICTIONARY.md` (from Phase 00): one-line responsibility per surviving class.
- XML doc comments on public `BeepDockingManager` API + events.
- Migration note: native-MDI hosting path removed from `BeepDockingManager`; WinForms-control hosting only.

## 8.4 Sample

- Add a sample form in the Winform sample app: tool windows on all edges, a document well
  with several documents, auto-hide, float, splitters, style/theme switcher — all arranged at
  **design time** so it demonstrates designer.cs persistence.

## 8.5 Testing matrix

| Dimension | Values |
|----------|--------|
| Styles | each `BeepControlStyle` used by docking |
| Themes | light, dark, high-contrast |
| DPI | 100, 125, 150, 200% |
| Ops | dock, float, auto-hide/peek/pin, split, drag-reorder, document split |
| Persistence | design-time arrange → reopen form → run app |
| Scale | 1, 10, 50 panels |

---

## 8.6 TODO checklist

- [x] Batch update suspends layout+paint; single recompute/repaint on resume (`BeepDockingUpdate` + depth-counted `BeginUpdate`/`EndUpdate`).
- [x] Layout result caching + coalesced `Invalidate` (`DockingLayoutController` caches `DockLayoutResult`; manager calls `InvalidateLayout()` on every tree mutation).
- [x] Geometry + tinted-glyph caching (layout managers cache geometry; icons via cached `StyledImagePainter`/`DockingCaptionPainter`).
- [x] Double-buffering everywhere (`OptimizedDoubleBuffer | AllPaintingInWmPaint` on every painted surface); shared `DockAnimator` timer.
- [x] `README.md` (this folder), `CODE-DICTIONARY.md` refreshed to final state, XML docs on the public API, migration note.
- [ ] **QA follow-up:** sample form (design-time arranged) in the Winform sample app — requires the VS designer; not authorable from the build environment.
- [ ] **QA follow-up:** run the full style/theme/DPI/scale matrix and record results.

> The remaining two items are interactive QA tasks (Visual Studio designer + manual
> multi-DPI/theme runs). All code-level performance hardening and documentation are complete.

---

## 8.7 Verification criteria

- 50-panel layout resizes/drags without visible lag.
- No flicker during batch updates, drag, or splitter resize.
- Sample demonstrates every feature and round-trips through `*.Designer.cs`.
- Matrix passes across styles, themes, and DPI.
