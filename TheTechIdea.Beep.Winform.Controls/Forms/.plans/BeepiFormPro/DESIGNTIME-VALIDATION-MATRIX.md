# BeepiFormPro Design-Time Validation Matrix

Last updated: 2026-05-10

## Goal

Provide a repeatable design-time checklist for the current `BeepiFormPro` designer and design-surface refresh behavior.

## Evidence Anchors

- `Designers/BeepiFormProDesigner.cs`
- `BeepiFormPro.cs`
- Design-time hooks for `ControlAdded`, `ControlRemoved`, child move or resize, focus changes, and deferred surface invalidation

## Preconditions

1. Open a WinForms project that can host `BeepiFormPro`.
2. Open the form in the Visual Studio designer.
3. Ensure the Properties grid is visible.

## Design-Time Checklist

| ID | Scenario | Steps | Expected Result |
|---|---|---|---|
| DT-01 | Designer load | Open a `BeepiFormPro`-derived form in the designer. | The design surface loads without throwing and the custom form chrome is visible. |
| DT-02 | Add child control | Drag a child control onto the form. | The form repaint completes without stale chrome artifacts and the child appears in the expected location. |
| DT-03 | Remove child control | Delete a child control from the designer. | The removed control no longer leaves stale paint behind on the form surface. |
| DT-04 | Move child control | Drag an existing child control to a new position. | The form repaints after the move and the old location is cleared. |
| DT-05 | Resize child control | Resize an existing child control. | The form repaints around the resized control without leaving stale borders or overlap artifacts. |
| DT-06 | Selection refresh | Select different child controls in sequence. | Selection changes trigger a repaint so focus adorners and chrome stay visually current. |
| DT-07 | Visibility toggle | Set a child control's `Visible` property to `false`, then back to `true`. | The form surface repaints both transitions correctly. |
| DT-08 | Enabled toggle | Set a child control's `Enabled` property to `false`, then back to `true`. | The form repaints correctly and no stale state remains on the chrome. |
| DT-09 | Child colors | Change a child control's `BackColor` and `ForeColor`. | The form surface refreshes around the control without requiring a manual reopen. |
| DT-10 | Container child add | Add a container control or `UserControl` with nested children. | Recursive style application does not block the designer transaction and the surface remains responsive. |
| DT-11 | Form style switch | Change the form's `FormStyle` in the Properties grid. | The form repaints in the new style without requiring the designer to reopen. |
| DT-12 | Caption options | Toggle caption options such as `ShowSearchBox`, `ShowProfileButton`, `ShowThemeButton`, or `ShowStyleButton`. | Caption chrome repaints cleanly and optional caption elements appear or disappear without stale bounds. |
| DT-13 | Save and reopen | Save the form, close the designer, then reopen it. | The designer reloads with the same chrome and control layout state. |

## Known Gaps

- This matrix does not validate runtime keyboard traversal; that remains in `RUNTIME-VALIDATION-MATRIX.md`.
- Theme changes driven by the running application shell are not exercised on the isolated design surface.
- UI automation and accessibility tooling checks remain outside the current designer-only scope.

## Exit Signal For This Matrix

- The design surface is considered stable when child add/remove/move/resize/select operations no longer leave stale chrome or require a manual refresh to repaint correctly.