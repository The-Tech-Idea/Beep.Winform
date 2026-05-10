# Phase 6 - Validation, Docs, And Samples

## Goal

Lock the previous phases in place with repeatable validation, accurate documentation, and sample coverage before more painter expansion resumes.

## Why This Is A Separate Phase

`BeepiFormPro` already has multiple optimistic completion documents. Without a validation and documentation phase, new fixes will drift the same way.

## Scope

- Build a manual regression matrix for runtime and designer scenarios.
- Add or update sample coverage for representative painter families.
- Update control and painter documentation to match the final implementation.
- Capture a maintenance checklist for future painter additions.

## Deliverables

- A validation checklist covering theme changes, style changes, resize, search box, optional caption buttons, custom caption-region bounds, backdrop transitions, and design-time editing.
- Sample scenarios for at least one right-button painter family, one left-traffic-light family, and one high-effects painter.
- Updated docs for `ModernForm/plan.md`, painter docs, and any relevant readme files.
- A future-change checklist for new painters.
- A benchmark-driven sample or diagnostics host for live preview of painter, theme, metrics, hit areas, DPI, and window-state transitions.

## Task Breakdown

- [x] Create a manual runtime test matrix.
- [x] Create a manual design-time test matrix.
- [x] Add or update a sample host form for representative styles.
- [ ] Update documentation to reflect the actual final contract and supported painter set.
- [ ] Record known limits or deferred items explicitly instead of leaving them implicit.
- [ ] Add accessibility, localization, and UI-automation checks to the regression checklist.
- [ ] Validate custom caption regions with explicit caption-relative bounds, keyboard traversal, and accessibility metadata.
- [x] Add a developer diagnostics surface that can display hit areas, layout rectangles, current painter, theme, DPI, and window state.

## Suggested Validation Matrix

1. Theme switch at runtime with one form.
2. Theme switch across multiple open forms.
3. Style switch between right-button and left-button painters.
4. Search box focus, typing, and click-away behavior.
5. Live resize drag followed by repaint correctness.
6. Maximize and restore from caption button, double-click caption, and taskbar/system menu.
7. Backdrop transitions across supported Windows versions.
8. Designer add/move/remove child controls.
9. High-contrast and DPI scaling behavior.
10. Interactive custom caption region with non-empty bounds: pointer hit test, `F6` traversal, `Enter` activation, and screen-reader metadata.

## Current Progress

- `WinFormsApp.UI.Test` now includes a standalone `--demo custom-caption-region` mode that opens a dedicated `BeepiFormPro` sample instead of the full business shell.
- That demo switches among `Modern`, `MacOS`, `Neon`, and `Glass`, which covers a representative right-button style, a traffic-light family, and a higher-effects painter while keeping the validation surface small.
- The demo exposes runtime toggles for `ScreenReaderSupport`, `HighContrastMode`, `ShowSearchBox`, and `ShowProfileButton`, so manual validation of the custom caption-region contract no longer depends on ad hoc local setup.
- `WinFormsApp.UI.Test` now also includes a standalone `--demo diagnostics` mode that opens a dedicated diagnostics host for current painter/theme/style/backdrop inspection, current metrics, actual registered hit areas, live layout rectangles, DPI reporting, companion-form synchronization checks, and remaining manual-width review.
- `RUNTIME-VALIDATION-MATRIX.md` now captures the repeatable runtime checklist for the custom caption-region demo and the diagnostics host, including pointer hit testing, keyboard traversal, style-family switches, maximize or restore, accessibility toggles, global theme or style synchronization, and both paint and native backdrop transitions.
- `DESIGNTIME-VALIDATION-MATRIX.md` now captures the corresponding designer-side checklist for child add/remove/move/resize/select repaint behavior, recursive styling of added containers, and property-grid updates to caption options and form style.

## Risks

- If validation remains manual-only, regressions can return silently.
- If docs are not updated at the end of each implementation phase, the project will repeat the current mismatch between docs and code.

## Exit Criteria

- A future maintainer can verify the control with a documented checklist.
- Docs match the real code surface.
- New painter work can resume on top of a stable, validated baseline.