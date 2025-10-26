using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = BackgroundColor;
            this.TabForeColor = ForeColor;
            this.TabBorderColor = BorderColor;
            this.TabHoverBackColor = SurfaceColor;
            this.TabHoverForeColor = ForeColor;
            this.TabSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.05);
            this.TabSelectedForeColor = ForeColor;
            this.TabSelectedBorderColor = ActiveBorderColor;
            this.TabHoverBorderColor = ActiveBorderColor;
        }
    }
}
