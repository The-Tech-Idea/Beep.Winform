using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(0,43,54);
            this.NavigationForeColor = Color.FromArgb(147,161,161);
            this.NavigationHoverBackColor = Color.FromArgb(0,43,54);
            this.NavigationHoverForeColor = Color.FromArgb(147,161,161);
            this.NavigationSelectedBackColor = Color.FromArgb(0,43,54);
            this.NavigationSelectedForeColor = Color.FromArgb(147,161,161);
        }
    }
}