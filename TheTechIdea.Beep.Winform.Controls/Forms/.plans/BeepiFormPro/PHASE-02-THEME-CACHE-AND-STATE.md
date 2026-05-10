# Phase 2 - Theme, Cache, And State

## Goal

Make theme and style changes deterministic so `BeepiFormPro` stops depending on shared stale state, implicit cache resets, and multi-step reapplication flows.

## Evidence From Current Code

- `BeepiFormPro.Theme.cs` clears `PaintersFactory` globally when `CurrentTheme` changes.
- `PaintersFactory.cs` caches painter instances by `FormStyle` only, which is risky if painters keep theme-derived or owner-derived state.
- `FormPainterMetrics.cs` uses a static metrics cache tied to style, theme hash, and theme-color usage, which creates another independent invalidation surface.
- `ApplyGlobalThemeAndStyle` in `BeepiFormPro.Theme.cs` can apply theme, form style, layout invalidation, and repaint in one call chain.
- `ApplyTheme()` currently propagates the string theme to child components, but not a clearly staged refresh contract for painter metrics, layout, and region geometry.

## Scope

- Unify the meaning of `Theme`, `ThemeName`, and `CurrentTheme`.
- Replace broad cache clearing with deterministic invalidation or instance ownership.
- Ensure theme/style changes are single-pass operations rather than layered side effects.
- Make painter and metrics caching safe for multiple form instances.

## Deliverables

- A documented theme/style state model.
- A cache strategy decision: stateless painters, per-form painter ownership, or versioned cache entries.
- A single apply sequence for theme/style changes.
- Safe child-theme propagation rules.

## Current Progress

- A baseline repair has already been applied so `Theme`, `ThemeName`, and `CurrentTheme` no longer drift through separate code paths.
- Theme changes now clear painter and metrics caches and refresh layout-derived theme state through one helper.
- The remaining work in this phase is architectural: ownership model, cache scope, and event semantics still need to be formalized.

## Task Breakdown

- [ ] Choose the canonical source of truth for the active theme object and name.
- [ ] Decide whether `PaintersFactory` remains global or becomes per-form/per-request.
- [ ] Add explicit invalidation for metrics, layout, window region, and visual refresh after theme changes.
- [ ] Prevent repeated style/theme application cascades from menus or global events.
- [ ] Verify child controls receive both styling and theme refresh in a predictable order.

## Recommended Direction

- Treat painter instances as stateless or form-owned, not global mutable state.
- Use one change transaction for theme/style updates.
- Make cache invalidation version-based or owner-scoped instead of broad global clears.
- Introduce a palette-style token model so painters consume resolved metrics and colors rather than privately caching theme-derived decisions.
- Consider a managed registration model for live themed forms and child surfaces.

## Risks

- Shared caches can hide bugs until multiple `BeepiFormPro` instances use different themes or styles.
- Per-form painter instances can increase allocations if layout and drawing helpers are not also shared.

## Exit Criteria

- Switching theme or form style produces one deterministic refresh path.
- Painter and metrics state can no longer go stale across forms.
- Global theme events do not cause layered or conflicting state transitions.