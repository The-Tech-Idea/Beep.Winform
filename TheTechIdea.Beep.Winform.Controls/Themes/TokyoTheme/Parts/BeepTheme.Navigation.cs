using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = BackgroundColor;
            this.NavigationForeColor = ForeColor;
            this.NavigationHoverBackColor = BackgroundColor;
            this.NavigationHoverForeColor = ForeColor;
            this.NavigationSelectedBackColor = BackgroundColor;
            this.NavigationSelectedForeColor = ForeColor;
        }
    }
}