using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = SurfaceColor;
            this.NavigationForeColor = ForeColor;
            this.NavigationHoverBackColor = SurfaceColor;
            this.NavigationHoverForeColor = ForeColor;
            this.NavigationSelectedBackColor = SurfaceColor;
            this.NavigationSelectedForeColor = ForeColor;
        }
    }
}
