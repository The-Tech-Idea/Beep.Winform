# BeepMaterialTextField - Read.md (What the screenshot shows and what’s missing)

Screenshot legend
- Red (circled): the floating label “Text field”.
  - Filled: label rests inside the filled container (top-left) at full size; when focused or when there’s content, it shrinks (~0.75x) and floats above the input line.
  - Outlined: label shrinks (~0.75x) and floats into a notch that is cut out of the outline’s top border.
- Blue (underlined): the supporting text (helper text) shown below the field. It’s left-aligned under the input. A character counter (when enabled) sits on the bottom-right.

Current status in code
- Label
  - We already compute label rectangles and animate scale/position.
  - Outlined notch is not fully implemented. We draw a standard outline and briefly clear a thin strip; this does not produce a proper notch dimensioned to the label width and border thickness.
  - Resting vs floating positions aren’t tied tightly to variant/density paddings (Filled vs Outlined specifics).
- Helper (supporting) text
  - We compute a helper rect and draw it, but vertical spacing is reserved only when HelperText/ ErrorText has content.
  - This can cause layout jumps; Material spec often reserves space (configurable) and also shows the right-aligned character counter simultaneously.
- Character counter
  - Drawing is present, but its layout and reserved space need to be consistent with helper text so they don’t overlap and do not clip.
- Suffix/Prefix text
  - Now placed inside the input zone (prefix after leading icon, suffix before trailing icon) and the text rect is clipped accordingly. This matches the Material inline suffix/prefix behavior.

Why the red (floating label notch) is not implemented
- The Outlined variant needs a notched outline: the top border is split and a notch gap created whose width equals the measured floating label width plus extra horizontal padding (to avoid touching the glyphs), and vertically centered on the top border thickness.
- Our current DrawOutlinedBackground draws one continuous path with a thin clear strip; that is not sufficient and does not handle rounded corners and border thickness.

Why the blue (supporting text) is not implemented perfectly
- Space for supporting text below the input is not always reserved; the control height may change when HelperText toggles.
- The character counter and supporting text require a shared baseline area with left/right alignment. This needs a “reserve space” option and coordinated measurement in height calculation and layout.

What to add (high-level)
1) Proper notched outline for the Outlined variant
   - Measure floating label width at the current animation progress: use TextRenderer with NoPadding and apply scale (~0.75f). Add NotchPadding (e.g., 8 px total) and border thickness.
   - Build the outline path in three segments for the top edge: left segment up to notch start, skip the notch gap, right segment from notch end, keeping rounded corners. Use the same radius and thickness as the rest of the outline so it looks integral.
2) Filled label resting/floating paddings
   - Resting label should sit inside the filled container (top-left) with density dependent padding. Floating should be aligned above the text rect baseline.
3) Supporting text and counter area
   - Add an option ReserveSupportingSpace: Off, WhenHasText (current), Always.
   - Compute a dedicated bottom area height: max(helperHeight, counterHeight) + vertical padding.
   - Draw supporting (left) and counter (right) inside that area.
   - Include this area in AutoAdjustHeight so height doesn’t jump.
4) Preset defaults
   - MaterialOutlined: Notched outline enabled, ReserveSupportingSpace=WhenHasText.
   - MaterialFilled: Underline animation enabled, ReserveSupportingSpace=WhenHasText.
   - Comfortable/Dense variants adjust paddings and label offsets.

Acceptance criteria
- Outlined: label floats into a true notch (gap in the top border sized to the label width + padding). No visual overlap with the border.
- Filled: label rests inside the fill and floats on focus/when not empty with smooth animation.
- Supporting text is left-aligned below; the counter is right-aligned; layout does not jump when toggling helper visibility if reserve is enabled.
- Suffix/prefix remain inline, text does not overlap them, and caret/selection rendering stay centered vertically.
