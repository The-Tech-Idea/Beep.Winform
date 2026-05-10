# BeepiFormPro Todo Tracker

Last updated: 2026-05-10

## Status Summary

| Item | Status | Depends On | Notes |
|---|---|---|---|
| Audit baseline captured | Done | None | Source review completed and converted into the phase plan set. |
| Phase 1 - Baseline and contract reset | Not started | Audit baseline | Must finish before broad implementation work. |
| Phase 2 - Theme, cache, and state | In progress | Phase 1 | Baseline theme-state contract repair is complete; broader invalidation and ownership cleanup remains. |
| Phase 3 - Layout, hit-testing, and input | In progress | Phase 1, Phase 2 | Hit-area normalization, shared caption helper families, stale-layout repairs, keyboard-caption baseline, profile slot support, centralized search-box metrics, and the custom-caption-region interaction contract are complete; the remaining work is validation follow-through plus a follow-up review of painter-specific button geometry. |
| Phase 4 - Painter consolidation | Not started | Phase 3 | Extract shared helpers without flattening painter identity. |
| Phase 5 - Win32, backdrop, and design-time hardening | In progress | Phase 1, Phase 2, Phase 3 | Maximize and restore repaint repair landed; broader non-client and design-time hardening remains. |
| Phase 6 - Validation, docs, and samples | In progress | Phase 1-5 | Runtime and design-time matrices now exist, and the sample app now includes a diagnostics host for theme, backdrop, and multi-form checks, but those scenarios still need execution and later automation coverage. |

## Phase Checklist

- [ ] Phase 1 started
- [ ] Phase 1 completed
- [x] Phase 2 started
- [ ] Phase 2 completed
- [x] Phase 3 started
- [ ] Phase 3 completed
- [ ] Phase 4 started
- [ ] Phase 4 completed
- [x] Phase 5 started
- [ ] Phase 5 completed
- [x] Phase 6 started
- [ ] Phase 6 completed

## Immediate Implementation Queue

1. Run through the new runtime and design-time validation matrices and capture any failures or gaps.
2. Execute the expanded runtime validation coverage for theme transitions, backdrops, and multi-form scenarios using the new diagnostics host.
3. Use `CAPTION-BUTTON-WIDTH-REFACTOR-PROPOSAL.md` plus the diagnostics host metrics/layout/hit-area view before touching any more remaining manual caption width literals.

## Progress Notes

- The older `ModernForm/plan.md` is now treated as legacy context, not the active implementation tracker.
- The remaining manual right-side width review now has a concrete follow-up doc in `CAPTION-BUTTON-WIDTH-REFACTOR-PROPOSAL.md`, and the diagnostics host exposes metrics, layout rectangles, and actual registered hit areas together so those decisions can be validated against live runtime state instead of only source reads.
- Painter documentation currently overstates completion. The new tracker should be treated as the working source of truth.
- Investigated defect: maximize and restore likely miss `RefreshChromeGeometryAfterBoundsSettled()`. The current flow updates only the window region in `WM_SIZE`, while full layout and managed-region refresh is deferred to `OnResizeEnd` and `WM_EXITSIZEMOVE`.
- External benchmark pass completed. The most relevant additions are palette-token architecture, explicit chrome redraw vs recalc hooks, shared animation infrastructure, accessibility validation, and a live diagnostics and preview host.
- Targeted repair applied: `BeepiFormPro.Win32.cs` now refreshes full chrome geometry from `WM_SIZE` only for window-state transitions and non-normal bounds changes, which addresses maximize and restore repaint lag without reintroducing heavy work during live resize.
- Targeted repair applied: `Theme`, `ThemeName`, and `CurrentTheme` now flow through one helper path, and theme changes now invalidate painter cache, metrics cache, layout cache, and form background state consistently.
- Targeted repair applied: hit-area names are now normalized at the `BeepiFormProHitAreaManager` registration boundary and reused in click dispatch, reducing the mixed `Style`/`system:*`/`region:*` alias contract across painters.
- Targeted repair applied: built-in hit-area keys now have shared constants in `BeepiFormPro.Models.cs`, and `OnRegionClicked` has been repaired to dispatch normalized custom action, theme, style, search, and caption hits to the correct behavior paths.
- Targeted repair applied: painter hover lookups across the ModernForm painter set now use `FormHitAreaNames` constants instead of raw built-in string literals, so the shared hit-area contract now covers both the core form path and the painter lookup surface.
- Targeted repair applied: representative painters now use `FormHitAreaNames` inside helper button-type flows and registration calls as well, and the Forms and Painters readmes now document the canonical built-in hit-area contract for new painter work.
- Targeted repair applied: additional helper-based painters now use `FormHitAreaNames` through helper arguments, switch labels, and registrations, including ChatBubble, Custom, Glass, Material, Material You, Cartoon, Cyberpunk, GNOME, Linear, and Metro2.
- Targeted repair applied: a second cluster of registration-only painters now registers built-in caption actions with canonical constants as well, including ArcLinux, Dracula, GruvBox, Holographic, iOS, NeoMorphism, and OneDark.
- Targeted repair applied: the remaining ModernForm painter families now also use `FormHitAreaNames` for built-in helper strings and registration calls, and the only leftover raw examples in the painter folder were updated in `skinplan.md`.
- The painter-side built-in hit-area naming migration is now effectively complete, so the remaining Phase 3 work has shifted from naming cleanup to shared layout builders, stale-layout prevention, and accessibility behavior.
- Targeted repair applied: `FormPainterLayoutHelper` now owns the shared right-aligned caption skeleton for compatible painters, and Modern, Material, Material You, NextJS, RadixUI, Shadcn, Linear, Minimal, Metro, and Metro2 now use that helper while keeping painter-specific icon and title placement local.
- The helper contract now carries explicit optional-button policy for the custom-action slot, which lets Metro and Metro2 opt out without cloning the whole caption layout again.
- Targeted repair applied: `FormPainterLayoutHelper` now also owns the shared traffic-light caption skeleton for MacOS and iOS, including caption drag registration, left-side traffic-light button placement, and the shared right-side optional button cluster.
- Ubuntu is no longer treated as part of the traffic-light family because its live layout path is still a standard right-side caption model.
- Targeted repair applied: the core layout cache now tracks the previously missed `ShowSearchBox` and `ShowProfileButton` visibility flags, and direct `FormPainterMetrics` replacement now invalidates layout instead of repainting stale geometry.
- Targeted repair applied: caption keyboard traversal and activation now use one shared focus model. `Ctrl+F` focuses the caption search box, `F6` and `Shift+F6` cycle visible caption actions, `Tab` continues cycling while the caption search is active, and `Enter` or `Space` activates the focused caption action.
- Targeted repair applied: caption keyboard focus now paints a central focus-ring overlay and updates the form accessibility surface with screen-reader-friendly names and descriptions when `ScreenReaderSupport` is enabled.
- Targeted repair applied: `OnRegionClicked` has been rebuilt around canonical hit-area names, so minimize, maximize, close, theme, style, search, custom action, and caption drag now all route through one readable dispatch path again instead of the previously corrupted merge state.
- Targeted repair applied: the shared right-aligned and traffic-light layout helpers now reserve a profile button slot, the shared keyboard/accessibility model recognizes that slot, and the main shared-family painters now render the built-in profile button surface instead of leaving the new hit area invisible.
- Targeted repair applied: the remaining manual search-paint outlier painters now also reserve `ProfileButtonRect` and paint the built-in profile region, so the profile caption action is no longer limited to helper-based families.
- Targeted repair applied: `FormPainterMetrics` now owns `SearchBoxWidth` and `SearchBoxPadding`, the DPI-aware metrics path scales those values centrally, and all remaining painter layout paths now consume shared search-box metrics instead of hardcoded `200/8` caption constants.
- Targeted repair applied: the Glass painter now uses `metrics.ButtonWidth` for its caption button cluster because that style already matched the shared metric, while the remaining hardcoded caption button widths were left in place after audit because they currently diverge from the style metrics table and need deliberate review rather than blind normalization.
- Targeted repair applied: caption accessibility focus now snapshots and restores role and default action metadata in addition to name and description, runtime changes to `HighContrastMode`, `ScreenReaderSupport`, and `FocusIndicatorStyle` now react immediately, and the caption focus ring automatically switches to a stronger high-contrast treatment when accessibility mode or system high contrast is active.
- Targeted repair applied: successful layout recalculation now clears keyboard-caption focus when the focused caption target is no longer visible, so hiding search, profile, theme, style, custom action, min/max, close, or the entire caption bar no longer leaves stale invisible focus or stale screen-reader metadata behind.
- Targeted repair applied: disabling `ScreenReaderSupport` now force-restores the base accessibility metadata instead of leaving the last caption-action role or default action behind, and toggling `HighContrastMode` now repaints the full form because that accessibility mode affects the whole custom paint pipeline rather than only the caption focus ring.
- Targeted repair applied: `FormRegion` now exposes an opt-in interaction and accessibility contract, layout recalculation registers interactive regions into `_hits`, caption-docked interactive custom regions now join keyboard caption traversal and caption accessibility metadata, and `AddRegion` or `ClearRegions` now invalidate layout instead of leaving stale custom-region hit targets behind.
- Targeted repair applied: `ResolveRegionRect` now preserves the full-caption fallback only for empty caption-region bounds; custom caption regions with explicit bounds now resolve relative to `CurrentLayout.CaptionRect`, which lets them paint, hit-test, focus, and expose accessibility metadata inside their own caption slot instead of spanning the whole title bar.
- Targeted repair applied: `WinFormsApp.UI.Test` now has a standalone `--demo custom-caption-region` launch path with a dedicated `BeepiFormPro` sample form that exercises one custom caption region across right-aligned, traffic-light, and high-effects styles while exposing screen-reader and high-contrast toggles for manual validation.
- Targeted repair applied: Phase 6 now has a dedicated `RUNTIME-VALIDATION-MATRIX.md` document, and the tracker has been updated to reflect that validation, docs, and samples are in progress rather than still untouched.
- Targeted repair applied: Phase 6 now also has a dedicated `DESIGNTIME-VALIDATION-MATRIX.md` document grounded in the live designer hooks and deferred invalidation path, so design-surface repaint expectations are captured alongside runtime checks.
- Targeted audit applied: `CAPTION-BUTTON-WIDTH-AUDIT.md` now classifies the remaining painter-owned caption-width outliers instead of treating them as one cleanup bucket; the next step is a metrics-model choice, not a blind literal replacement.
- Targeted repair applied: the MacOS and iOS traffic-light painters now use `FormPainterMetrics.AuxiliaryButtonWidth` for the wider right-side optional-button cluster, which removes that family from the remaining hardcoded-width bucket.
- Targeted repair applied: GNOME now uses `FormPainterMetrics.VisualButtonWidth` for its narrower painted pill buttons while keeping its wider hit targets unchanged, which removes the last style-token width literal from the remaining audit bucket.
- Targeted validation expansion applied: the runtime matrix now covers one-form theme switching, global multi-form theme or style synchronization, and backdrop startup or transition checks, so those scenarios are no longer only implicit Phase 6 goals.
- Targeted sample expansion applied: `WinFormsApp.UI.Test` now has a `--demo diagnostics` host that exposes current painter, theme, style, current metrics, paint backdrop, Win32 backdrop, actual registered hit areas, live layout rectangles, DPI and window-state reporting, and a companion-form path for multi-form synchronization checks.
- The remaining major work is now validation of the new custom-caption-region contract, plus a deliberate follow-up review of whether painter-specific caption button widths should stay local or move into shared metrics.