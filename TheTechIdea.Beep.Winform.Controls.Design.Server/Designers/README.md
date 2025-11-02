# Designers Overview

This directory contains the design-time support for Beep WinForms controls. Designers register via `DesignRegistration` and expose image picking verbs through `BeepImagePickerDialog`.

## Latest Updates

- Added `BeepImageDesigner` with Select/Clear image verbs and registration via `DesignRegistration`.
- `BeepImagePathEditor` is now registered on the shared `BaseControl`/`BeepControl` types so every derived control with an `ImagePath` (or `EmbeddedImagePath`) automatically uses the picker.
- Introduced shared smart-tag support (`ImagePathDesignerActionList`) so `BeepButton`, `BeepLabel`, `BeepPanel`, and `BeepImage` expose Select/Clear actions directly in the designer actions panel.
