# Caption Bar Fix Plan (fixbar.md)

This plan tracks targeted fixes to caption bar rendering across painters.

Scope:
- Ensure caption elements (icon, title, system buttons, theme/style/custom) appear correctly without overlap, honoring style intent.

Tasks:
1) Icon and Title overlap
   - Make all painters render title text within `owner.CurrentLayout.TitleRect` (not full caption).
   - Keep icon drawn via `PaintBuiltInCaptionElements` using `IconRect` to avoid overlap.

2) macOS traffic lights duplication
   - Skip drawing default system buttons in `PaintBuiltInCaptionElements` when `FormStyle.MacOS`.
   - macOS painter renders traffic lights; hit areas remain painter-defined.

3) macOS title and right-side buttons arrangement
   - In `MacOSFormPainter.CalculateLayoutAndHitAreas`, set `TitleRect` to span between the traffic lights on the left and the right-side buttons.
   - Place `IconRect` just after the traffic lights to avoid title/icon overlap.

4) ChatBubble "..." overlap
   - Move chat "typing dots" away from the title by placing them left of `TitleRect`.
   - Keep visibility subtle (no occlusion of buttons or title).

5) Classic style hides caption bar
   - Do not paint caption for `FormStyle.Classic`.
   - Skip built-in caption element rendering for Classic as a safety.

Validation:
- Switch styles and confirm caption elements are visible, non-overlapping, and interactive.
- macOS shows only traffic lights (no default system buttons), and title aligns between lights and right-side buttons.
- Classic shows no caption bar.

Rollback:
- Revert individual painter changes if a style requires different layout semantics.
