using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(242,242,247);
            this.NavigationForeColor = Color.FromArgb(28,28,30);
            this.NavigationHoverBackColor = Color.FromArgb(242,242,247);
            this.NavigationHoverForeColor = Color.FromArgb(28,28,30);
            this.NavigationSelectedBackColor = Color.FromArgb(242,242,247);
            this.NavigationSelectedForeColor = Color.FromArgb(28,28,30);
        }
    }
}