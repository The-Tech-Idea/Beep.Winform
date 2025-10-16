using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(245,246,248);
            this.NavigationForeColor = Color.FromArgb(32,32,32);
            this.NavigationHoverBackColor = Color.FromArgb(245,246,248);
            this.NavigationHoverForeColor = Color.FromArgb(32,32,32);
            this.NavigationSelectedBackColor = Color.FromArgb(245,246,248);
            this.NavigationSelectedForeColor = Color.FromArgb(32,32,32);
        }
    }
}