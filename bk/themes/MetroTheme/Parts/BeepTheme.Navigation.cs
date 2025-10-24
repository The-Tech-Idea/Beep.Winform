using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(243,242,241);
            this.NavigationForeColor = Color.FromArgb(32,31,30);
            this.NavigationHoverBackColor = Color.FromArgb(243,242,241);
            this.NavigationHoverForeColor = Color.FromArgb(32,31,30);
            this.NavigationSelectedBackColor = Color.FromArgb(243,242,241);
            this.NavigationSelectedForeColor = Color.FromArgb(32,31,30);
        }
    }
}