using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = BackgroundColor;
            this.NavigationForeColor = ForeColor;
            this.NavigationHoverBackColor = SurfaceColor;
            this.NavigationHoverForeColor = ForeColor;
            this.NavigationSelectedBackColor = ThemeUtil.Lighten(SurfaceColor, 0.05);
            this.NavigationSelectedForeColor = ForeColor;
        }
    }
}
