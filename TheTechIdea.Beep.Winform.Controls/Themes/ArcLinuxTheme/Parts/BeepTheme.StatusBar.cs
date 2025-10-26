using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = SurfaceColor;
            this.StatusBarForeColor = ForeColor;
            this.StatusBarBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.StatusBarHoverBackColor = SurfaceColor;
            this.StatusBarHoverForeColor = ForeColor;
            this.StatusBarHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
        }
    }
}
