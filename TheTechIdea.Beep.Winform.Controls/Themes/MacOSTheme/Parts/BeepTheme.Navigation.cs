using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = BackgroundColor;
            this.NavigationForeColor = ForeColor;
            this.NavigationHoverBackColor = PanelGradiantMiddleColor;
            this.NavigationHoverForeColor = ForeColor;
            this.NavigationSelectedBackColor = PanelBackColor;
            this.NavigationSelectedForeColor = ForeColor;
        }
    }
}