# BeepLabelDesigner Plan

Status: partially completed in this pass.

Implemented:
- Moved `BeepLabelDesigner` onto `BaseBeepControlDesigner` so label smart tags include the shared Beep style/theme surface.
- Added a dedicated label smart-tag action list for image/text presets, multiline control, alignment, and theme-image tint.
- Kept the image picker host and verb menu in sync with the new smart-tag actions.

Remaining follow-up:
- Add optional presets for status labels, metric callouts, and caption/subcaption combinations if those patterns recur in samples.