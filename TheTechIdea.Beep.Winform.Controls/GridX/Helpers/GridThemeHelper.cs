using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    internal class GridThemeHelper
    {
        private readonly BeepGridPro _grid;
        public GridThemeHelper(BeepGridPro grid) { _grid = grid; }

        public void ApplyTheme()
        {
            var t = BeepThemesManager.GetTheme(_grid.Theme);
            if (t != null)
            {
                _grid.BackColor = t.GridBackColor;
                _grid.ForeColor = t.GridForeColor;
                // Align font sizing with BeepSimpleGrid: use theme grid cell font
                var cellFont = BeepThemesManager.ToFont(t.GridCellFont);
                //if (cellFont != null)
                //{
                //    _grid.Font = cellFont;
                //}
            }
        }
    }
}
