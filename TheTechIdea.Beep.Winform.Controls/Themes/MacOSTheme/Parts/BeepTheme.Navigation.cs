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
            this.NavigationBackColor = Color.FromArgb(250,250,252);
            this.NavigationForeColor = Color.FromArgb(28,28,30);
            this.NavigationHoverBackColor = Color.FromArgb(250,250,252);
            this.NavigationHoverForeColor = Color.FromArgb(28,28,30);
            this.NavigationSelectedBackColor = Color.FromArgb(250,250,252);
            this.NavigationSelectedForeColor = Color.FromArgb(28,28,30);
        }
    }
}