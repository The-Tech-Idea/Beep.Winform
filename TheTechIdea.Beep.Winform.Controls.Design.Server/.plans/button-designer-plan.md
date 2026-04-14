# BeepButtonDesigner Plan

Status: completed in this pass.

Implemented:
- Moved `BeepButtonDesigner` onto `BaseBeepControlDesigner` so button smart tags now include the shared Beep style/theme surface.
- Kept the existing image picker host and designer verbs.
- Reduced duplicated property-change plumbing by delegating `ImagePath` writes through the shared designer property helpers.

Remaining follow-up:
- Consider a button-specific action list for icon/text layout presets if authors keep using the Properties grid for those settings.