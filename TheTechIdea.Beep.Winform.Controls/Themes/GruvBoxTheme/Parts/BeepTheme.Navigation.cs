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
            this.NavigationBackColor = Color.FromArgb(40,40,40);
            this.NavigationForeColor = Color.FromArgb(235,219,178);
            this.NavigationHoverBackColor = Color.FromArgb(40,40,40);
            this.NavigationHoverForeColor = Color.FromArgb(235,219,178);
            this.NavigationSelectedBackColor = Color.FromArgb(40,40,40);
            this.NavigationSelectedForeColor = Color.FromArgb(235,219,178);
        }
    }
}