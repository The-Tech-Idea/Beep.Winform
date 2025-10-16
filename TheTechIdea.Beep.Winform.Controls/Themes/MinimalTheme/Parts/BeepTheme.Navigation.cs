using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(255,255,255);
            this.NavigationForeColor = Color.FromArgb(31,41,55);
            this.NavigationHoverBackColor = Color.FromArgb(255,255,255);
            this.NavigationHoverForeColor = Color.FromArgb(31,41,55);
            this.NavigationSelectedBackColor = Color.FromArgb(255,255,255);
            this.NavigationSelectedForeColor = Color.FromArgb(31,41,55);
        }
    }
}