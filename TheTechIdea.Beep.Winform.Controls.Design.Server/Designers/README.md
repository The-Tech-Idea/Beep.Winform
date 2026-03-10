# Designers Overview

This directory contains the design-time support for Beep WinForms controls. Designers register via `DesignRegistration` and expose image picking verbs through `BeepImagePickerDialog`.

## Latest Updates

- `BeepImagePathEditor` now launches `BeepImagePickerDialog` as the unified image-selection entry point.
- `BeepImagePickerDialog` now supports explicit source modes (embedded resources and icon catalog), search/filter, explicit embed action, and status feedback.
- Picker output is standardized as a path-string selection contract, preserving designer serialization compatibility.
- `BeepImageDesigner` smart-tags now include source-focused actions (Select image, Clear image, embedded context quick actions) in addition to clipping/effects controls.
- `BeepImagePickerDialog` designer initialization was trimmed to reduce generated noise and keep required preview wiring only.
- `DesignRegistration` now explicitly registers image path editor coverage for `BeepImage` in addition to base control types.
