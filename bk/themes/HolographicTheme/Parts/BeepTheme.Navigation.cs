using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(15,16,32);
            this.NavigationForeColor = Color.FromArgb(245,247,255);
            this.NavigationHoverBackColor = Color.FromArgb(15,16,32);
            this.NavigationHoverForeColor = Color.FromArgb(245,247,255);
            this.NavigationSelectedBackColor = Color.FromArgb(15,16,32);
            this.NavigationSelectedForeColor = Color.FromArgb(245,247,255);
        }
    }
}