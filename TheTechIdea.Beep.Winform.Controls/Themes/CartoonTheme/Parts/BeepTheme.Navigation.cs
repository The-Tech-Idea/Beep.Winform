using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = SurfaceColor;
            this.NavigationForeColor = ForeColor;
            this.NavigationHoverBackColor = SecondaryColor;
            this.NavigationHoverForeColor = ForeColor;
            this.NavigationSelectedBackColor = AccentColor;
            this.NavigationSelectedForeColor = ForeColor;
        }
    }
}