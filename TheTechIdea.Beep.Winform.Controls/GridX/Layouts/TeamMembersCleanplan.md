# TeamMembersClean Layout Plan (Image 2)

Intent
- Recreate the clean, airy table from image 2: subtle separators, large row height, soft header, rounded status chips (rendered as plain text for now), and aligned content.

Grid Tweaks
- RowHeight: 36
- ColumnHeaderHeight: 42
- ShowGridLines: true (solid, very light)
- ShowRowStripes: false
- UseHeaderGradient: false
- UseHeaderHoverEffects: true
- UseBoldHeaderText: false
- HeaderCellPadding: 6

Alignment Heuristics
- First visible column: Left (usually User)
- Columns containing "Status" (name or caption): Center
- Phone-like columns (name/caption contains "Phone"): Left
- Everything else: Left

Notes
- Chips/badges for statuses will be text-only in this pass. A later pass can add a chip renderer.
- Header/stroke colors come from the theme; we rely on GridRenderHelper flags and theme colors.

Steps
1) Implement TeamMembersCleanLayoutHelper.
2) Apply row/header sizes and render flags.
3) Apply simple alignment heuristics.
4) Recalculate + invalidate.

Usage
```csharp
using TheTechIdea.Beep.Winform.Controls.GridX.Layouts;

grid.ApplyLayoutPreset(new TeamMembersCleanLayoutHelper());
```