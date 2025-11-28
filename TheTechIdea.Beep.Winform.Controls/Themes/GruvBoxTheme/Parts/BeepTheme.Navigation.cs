using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = PanelBackColor;
            this.NavigationForeColor = ForeColor;
            this.NavigationHoverBackColor = PanelBackColor;
            this.NavigationHoverForeColor = ForeColor;
            this.NavigationSelectedBackColor = PanelBackColor;
            this.NavigationSelectedForeColor = ForeColor;
        }
    }
}