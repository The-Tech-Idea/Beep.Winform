using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(10,8,20);
            this.NavigationForeColor = Color.FromArgb(228,244,255);
            this.NavigationHoverBackColor = Color.FromArgb(10,8,20);
            this.NavigationHoverForeColor = Color.FromArgb(228,244,255);
            this.NavigationSelectedBackColor = Color.FromArgb(10,8,20);
            this.NavigationSelectedForeColor = Color.FromArgb(228,244,255);
        }
    }
}