# BeepiFormPro Runtime Validation Matrix

Last updated: 2026-05-10

## Goal

Provide one repeatable manual runtime checklist for the current `BeepiFormPro` repair set, with the custom caption-region demo and the diagnostics host as the primary validation surfaces.

## Primary Demo Surfaces

- Sample app: `Beep.Winform.Sample/WinFormsApp.UI.Test`
- Launch mode: `--demo custom-caption-region`
- Demo form: `CustomCaptionRegionDemoForm`
- Launch mode: `--demo diagnostics`
- Demo form: `ModernFormDiagnosticsDemoForm`

## Preconditions

1. Launch the sample with `--demo custom-caption-region` for RT-01 through RT-12.
2. Launch the sample with `--demo diagnostics` for RT-13 through RT-17.
3. Start from the default `Modern` style.
4. Confirm the custom `?` caption region is visible near the left side of the caption bar in the caption-region demo.

## Runtime Checklist

| ID | Scenario | Steps | Expected Result |
|---|---|---|---|
| RT-01 | Demo launch | Start `WinFormsApp.UI.Test` with `--demo custom-caption-region`. | The dedicated `BeepiFormPro` demo opens instead of the full business-shell app. |
| RT-02 | Pointer hit test | Click the `?` caption region. | Status text updates with an activation timestamp and the reported region bounds. |
| RT-03 | Keyboard traversal forward | Press `F6` until the custom caption region is focused, then press `Enter` or `Space`. | The custom region activates without requiring mouse hover. |
| RT-04 | Keyboard traversal reverse | Press `Shift+F6` from a later caption target until the custom region is focused. | Reverse traversal reaches the custom region in the expected sequence. |
| RT-05 | Search box coexistence | Leave `Show search box` enabled, use `Ctrl+F`, type in the search box, then resume `F6` traversal. | Search focus still works, and the custom caption region remains reachable after the search box. |
| RT-06 | Optional target removal | Toggle `Show search box` off, then toggle `Show profile button` off. Repeat `F6` traversal. | Traversal skips hidden targets cleanly and still reaches the custom region. |
| RT-07 | Style family switch | Switch among `Modern`, `MacOS`, `Neon`, and `Glass`. After each switch, click the custom region and test `F6`. | The custom region keeps a bounded caption-relative slot and does not stretch to the full caption bar. |
| RT-08 | Resize | Resize the form wider and narrower. | The custom region repaints in the caption and remains interactive. |
| RT-09 | Maximize and restore | Maximize, restore, then activate the custom region again. | Caption repaint remains correct and the custom region still responds. |
| RT-10 | Screen reader toggle | Turn `Screen reader support` off, then on again. Traverse back to the custom region. | No stale caption accessibility state remains when disabled; focus metadata resumes when re-enabled. |
| RT-11 | High contrast toggle | Turn `High contrast mode` on, then activate the custom region again. | The form repaints correctly and the custom region remains legible and interactive. |
| RT-12 | Bounds integrity | While on `MacOS`, visually confirm the traffic-light buttons remain separate from the custom `?` region. | The custom region keeps its own caption slot and does not overlap the traffic-light cluster. |
| RT-13 | Theme switch on one form | With `--demo diagnostics` open, change the selected Beep theme locally. | The form invalidates, repaints with the new theme colors, and updates the diagnostics panel without stale painter metrics or layout rectangles. |
| RT-14 | Global theme and style sync across multiple forms | In `--demo diagnostics`, open the companion form, enable global sync, then change the selected theme and form style. | Both forms react without stale colors, stale painter metrics, or caption-layout drift because global theme/style synchronization updates theme, painter, layout, and repaint together. |
| RT-15 | Paint and Win32 backdrop transition | On a supported Windows machine, use `--demo diagnostics` to cycle `BackdropEffect` through `None`, `Mica`, `Acrylic`, `MicaAlt`, and `Blur`, then cycle `Backdrop` through `None`, `Acrylic`, `Mica`, `Tabbed`, `Transient`, and `Blur`. | Paint-time backdrop changes and native Win32 backdrop changes do not break caption painting, layout reporting, or post-resize repaint correctness. |
| RT-16 | Handle-created backdrop startup | Launch `--demo diagnostics`, choose a non-`None` Win32 `Backdrop`, close the form, then relaunch with the same selection or set it immediately after startup. | The native backdrop applies after handle creation and the form still paints, reports diagnostics, and hit-tests correctly from the first visible frame. |
| RT-17 | Remaining manual width review | In `--demo diagnostics`, switch among `Fluent`, `Cartoon`, `ChatBubble`, and `Custom`. For each style, compare the reported current metrics, `CurrentLayout` rectangles, and registered hit areas before and after resize, maximize or restore, and optional caption-button toggles. | Any remaining painter-specific width drift is visible as an explicit metrics-vs-layout-vs-hit-area decision instead of an inferred code review; follow `CAPTION-BUTTON-WIDTH-REFACTOR-PROPOSAL.md` before changing any local width literal. |

## Deferred Coverage

- Designer-time validation remains tracked in Phase 6 and is not covered by this runtime matrix.
- UI Automation tooling checks are still manual and need a later follow-up pass.
- The diagnostics host now covers RT-13 through RT-17 directly, but those checks still require a Windows runtime environment that supports the relevant native backdrops.

## Exit Signal For This Matrix

- The custom caption-region contract is considered stable when the full checklist above passes without any caption geometry, repaint, focus, or accessibility drift.