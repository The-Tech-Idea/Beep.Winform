# DesignSystemTable Layout Plan (Table UI / Design System)

Intent
- Match the clean design-system table with checkbox selection, subtle separators, optional filter icons, status chips (text-only for now), avatars in contact column (text-only for now), and an action column.

Grid Tweaks
- RowHeight: 40
- ColumnHeaderHeight: 48
- ShowGridLines: true (solid, light)
- ShowRowStripes: false
- UseHeaderGradient: false
- UseHeaderHoverEffects: true
- UseBoldHeaderText: false
- HeaderCellPadding: 8

Behavior
- Ensure selection checkbox column is present and visible (ShowCheckBox = true).
- Show filter icons on headers for quick filtering.

Alignment Heuristics
- Checkbox column: center
- First content column: left
- Columns containing "status": center
- Columns containing "type": center
- Columns containing "sku" or "id": center
- Columns containing "price" or currency: right
- Columns containing "contact" or "company": left
- Columns containing "action": center
- Others: left

Notes
- Chip/Avatar rendering can be added later with a small drawer extension in GridRenderHelper.
