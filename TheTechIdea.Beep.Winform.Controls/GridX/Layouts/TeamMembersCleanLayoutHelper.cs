using System;
using System.Linq;
using System.Text.RegularExpressions;
using TheTechIdea.Beep.Winform.Controls.GridX.Layouts;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Clean, airy team members grid preset (matches Image 2 style).
    /// </summary>
    public sealed class TeamMembersCleanLayoutHelper : IGridLayoutPreset
    {
        public void Apply(BeepGridPro grid)
        {
            if (grid == null) return;

            // Sizing
            grid.RowHeight = 36;              // airy rows
            grid.ColumnHeaderHeight = 42;     // soft large header
            grid.ShowColumnHeaders = true;

            // Render flags
            grid.Render.ShowGridLines = true;                 // subtle lines
            grid.Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            grid.Render.ShowRowStripes = false;               // no stripes
            grid.Render.UseHeaderGradient = false;            // flat header
            grid.Render.UseHeaderHoverEffects = true;         // hover
            grid.Render.UseBoldHeaderText = false;            // not bold
            grid.Render.HeaderCellPadding = 6;                // generous padding
            grid.Render.UseElevation = false;                 // no elevation in rows
            grid.Render.CardStyle = false;                    // no card borders per row

            var cols = grid?.Columns?.ToList();
            if (cols == null || cols.Count == 0) return;

            // Alignment heuristics
            var firstVisible = cols.FirstOrDefault(c => c.Visible);
            if (firstVisible != null)
            {
                firstVisible.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                firstVisible.CellTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            }

            foreach (var c in cols.Where(x => x.Visible))
            {
                var name = (c.ColumnCaption ?? c.ColumnName ?? string.Empty).ToLowerInvariant();
                if (name.Contains("status"))
                {
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                }
                else if (name.Contains("phone") || name.Contains("mobile") || name.Contains("contact"))
                {
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                }
                else if (c != firstVisible)
                {
                    // default left for the rest
                    c.HeaderTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                    c.CellTextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
                }
            }
        }
    }
}
