using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridRenderHelper
    {
        private readonly BeepGridPro _grid;
        private IBeepTheme Theme => BeepThemesManager.GetTheme(_grid.Theme);
        public GridRenderHelper(BeepGridPro grid) { _grid = grid; }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            DrawBackground(g);
            if (_grid.Layout.ShowColumnHeaders)
                DrawHeaders(g);
            DrawRows(g);
            DrawSelection(g);
        }

        private void DrawBackground(Graphics g)
        {
            using var b = new SolidBrush(_grid.BackColor);
            g.FillRectangle(b, _grid.ClientRectangle);
        }

        private void DrawHeaders(Graphics g)
        {
            var rect = _grid.Layout.HeaderRect;
            using var bg = new SolidBrush(Theme?.GridHeaderBackColor ?? SystemColors.Control);
            using var fg = new SolidBrush(Theme?.GridHeaderForeColor ?? SystemColors.ControlText);
            using var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark);
            g.FillRectangle(bg, rect);

            int x = rect.Left;
            foreach (var col in _grid.Data.Columns)
            {
                var cellRect = new Rectangle(x, rect.Top, col.Width, rect.Height);
                g.DrawRectangle(pen, cellRect);
                TextRenderer.DrawText(g, col.ColumnCaption ?? col.ColumnName, _grid.Font, cellRect, fg.Color,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                x += col.Width;
            }
        }

        private void DrawRows(Graphics g)
        {
            using var fg = new SolidBrush(Theme?.GridForeColor ?? SystemColors.ControlText);
            using var alt = new SolidBrush(Color.FromArgb(8, 0, 0, 0));
            using var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark);

            foreach (var row in _grid.Data.Rows)
            {
                foreach (var cell in row.Cells)
                {
                    var rect = cell.Rect;
                    if (row.RowIndex % 2 == 1)
                        g.FillRectangle(alt, rect);
                    g.DrawRectangle(pen, rect);

                    string text = cell.CellValue?.ToString() ?? string.Empty;
                    TextRenderer.DrawText(g, text, _grid.Font, rect, fg.Color,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }
            }
        }

        private void DrawSelection(Graphics g)
        {
            var sel = _grid.Selection;
            if (!sel.HasSelection) return;

            using var pen = new Pen(Theme?.PrimaryTextColor ?? Color.DodgerBlue, 2f);
            g.DrawRectangle(pen, sel.SelectedCellRect);
        }
    }
}
