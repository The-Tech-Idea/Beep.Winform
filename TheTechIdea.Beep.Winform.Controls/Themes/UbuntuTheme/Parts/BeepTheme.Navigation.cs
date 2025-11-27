using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = PanelBackColor;
            this.NavigationForeColor = ForeColor;
            this.NavigationHoverBackColor = PanelGradiantMiddleColor;
            this.NavigationHoverForeColor = ForeColor;
            this.NavigationSelectedBackColor = PanelGradiantMiddleColor;
            this.NavigationSelectedForeColor = ForeColor;
        }
    }
}