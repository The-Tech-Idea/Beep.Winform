# Plan: Fix duplicates and finalize suffix/prefix layout inside text zone

Goals
- Remove duplicated and malformed methods in BeepMaterialTextFieldHelper (caused by earlier merges).
- Keep all features (ripple, animations, label, helper text, icons, prefix/suffix, counters).
- Implement Material-spec suffix/prefix layout: both are inside the input text zone, before trailing icon and after leading icon.
- Keep GetPrefixTextRectangle/GetSuffixTextRectangle returning cached inside-zone rects; GetAdjustedTextRectangle returns text rect already excluding prefix/suffix.
- Do not rebuild solution.

Changes
1) Clean BeepMaterialTextFieldHelper.cs
   - Add fields: _prefixTextRect, _suffixTextRect (already present).
   - Rewrite UpdateLayout to:
     a. Measure font via TextRenderer.NoPadding.
     b. Compute inputRect (excluding helper area if any).
     c. Place icons (leading/trailing) to define text zone (zoneLeft..zoneRight).
     d. Create textRect tall (top=2, height=inputHeight-4) to allow TextRenderer vertical centering.
     e. Place PrefixText at zoneLeft, shrink textRect from left.
     f. Place SuffixText before trailing icon (zoneRight - width), shrink textRect from right.
     g. Compute labelRect and helperTextRect.
   - Keep only one set of: StartLabelAnimation, StartFocusAnimation, StartRippleAnimation, UpdateAnimations, ApplyVariant, GetCharacterCounterRectangle, GetPrefixTextRectangle, GetSuffixTextRectangle, GetAdjustedTextRectangle.
   - Remove all duplicated/malformed method fragments.

2) Ensure MaterialTextFieldDrawingHelper continues using:
   - GetAdjustedTextRectangle for text/placeholder
   - GetPrefixTextRectangle/GetSuffixTextRectangle for prefix/suffix drawing.

Outcome
- No duplicate methods.
- Suffix sits inline before trailing icon; prefix inline after leading icon.
- Text never overlaps prefix/suffix.
