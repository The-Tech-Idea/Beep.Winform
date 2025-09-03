Dark Theme Audit
=================

Summary:
- DarkTheme core colors were mostly okay but ForeColor was previously set to near-black which made text invisible on dark backgrounds.
- Updated ForeColor to White for readability.

Button tokens:
- ButtonBackColor uses dark gray; ButtonForeColor is white (good).
- Hover/Pressed/Selected tokens are consistent with accent cyan/blue.

Next steps:
- Visual verification in app to confirm contrast in real controls.
- If any controls expect an outline default button, their ApplyTheme mappings should be checked.

