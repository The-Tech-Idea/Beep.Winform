BeepVerticalTable
=================

Simple vertical table control (pricing-style example) implemented as a Beep `BaseControl`.

- Features
- Partial-style implementation using helpers and painters
- Uses `VerticalTableLayoutHelper` for layout and hit detection
- Painter `DefaultVerticalTablePainter` uses `BeepStyling` and `StyledImagePainter` for images and colors
- Integrates with BaseControl hit-testing and input helpers (via `AddHitArea` and `ReceiveMouseEvent`)

-Usage
- Add control to your form and populate `Items` with `SimpleItem` objects.
- Use `SubText2` to store price data (renderer example)
- Handle `ItemClicked` or `SelectedItemChanged` events

-Notes
- The control is intentionally lightweight; register a painter with the `Painter` property — there is no default painter.  Custom painters should implement `IVerticalTablePainter` and own a layout helper for hit testing and hover/selection rendering.
- For each painter you add, provide a layout helper and use hit areas for interactivity
