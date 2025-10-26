using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyList()
        {
            this.ListBackColor = SurfaceColor;
            this.ListForeColor = ForeColor;
            this.ListBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.ListItemForeColor = ForeColor;
            this.ListItemHoverForeColor = ForeColor;
            this.ListItemHoverBackColor = SurfaceColor;
            this.ListItemSelectedForeColor = ForeColor;
            this.ListItemSelectedBackColor = SurfaceColor;
            this.ListItemSelectedBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.ListItemBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.ListItemHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.ListItemSpacing = 0f;
            this.ListIndentation = 0f;
        }
    }
}
