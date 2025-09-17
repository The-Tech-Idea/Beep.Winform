# DepartmentsRecordBold Layout Plan (Image 3)

Intent
- Recreate the bold header + compact body from image 3: strong header background, small separators, small row height, and rightmost action column.

Grid Tweaks
- RowHeight: 30
- ColumnHeaderHeight: 44
- ShowGridLines: true (solid)
- ShowRowStripes: false
- UseHeaderGradient: false (rely on theme background)
- UseHeaderHoverEffects: false (headers stable)
- UseBoldHeaderText: true
- HeaderCellPadding: 6–8
- Optional: CardStyle false, UseElevation false

Alignment Heuristics
- First column (row # or checkbox): center
- Name-like columns: left
- Status column: center
- Action column: center

Notes
- We keep theme colors; the bold header feel should come from UseBoldHeaderText + theme GridHeaderBackColor.

Steps
1) Implement DepartmentsRecordBoldLayoutHelper.
2) Apply sizes and render flags.
3) Apply alignment heuristics.
4) Recalculate + invalidate.

Usage
```csharp
using TheTechIdea.Beep.Winform.Controls.GridX.Layouts;

grid.ApplyLayoutPreset(new DepartmentsRecordBoldLayoutHelper());
```