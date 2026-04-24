using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
 

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
                var cellFont = BeepThemesManager.ToFont(t.GridCellFont);

                _grid.ScrollBars.ApplyTheme(_grid.Theme);

                ApplyToolbarTheme(t);
            }
        }

        private void ApplyToolbarTheme(IBeepTheme theme)
        {
            _grid.ToolbarBackColor = theme.GridHeaderBackColor;
            _grid.ToolbarForeColor = theme.GridHeaderForeColor;
            _grid.ToolbarBorderColor = theme.GridLineColor;
            _grid.ToolbarButtonHoverBackColor = theme.GridRowHoverBackColor;
            _grid.ToolbarButtonPressedBackColor = theme.GridRowSelectedBackColor;
            _grid.ToolbarSeparatorColor = theme.GridLineColor;
            _grid.ToolbarSearchBackColor = theme.GridBackColor;
            _grid.ToolbarPlaceholderColor = Color.FromArgb(
                Math.Min(180, theme.GridForeColor.A),
                theme.GridForeColor.R,
                theme.GridForeColor.G,
                theme.GridForeColor.B);

            bool isDark = theme.GridBackColor.GetBrightness() < 0.5;
            _grid.ToolbarSearchFocusBackColor = isDark
                ? Color.FromArgb(40, 50, 65)
                : Color.FromArgb(240, 245, 255);
        }
    }
}
