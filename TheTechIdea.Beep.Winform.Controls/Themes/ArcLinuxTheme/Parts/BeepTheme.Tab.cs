using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = SurfaceColor;
            this.TabForeColor = ForeColor;
            this.TabBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.TabHoverBackColor = SurfaceColor;
            this.TabHoverForeColor = ForeColor;
            this.TabSelectedBackColor = SurfaceColor;
            this.TabSelectedForeColor = OnPrimaryColor;
            this.TabSelectedBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.TabHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
        }
    }
}
