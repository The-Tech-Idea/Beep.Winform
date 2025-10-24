using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(242,242,245);
            this.NavigationForeColor = Color.FromArgb(44,44,44);
            this.NavigationHoverBackColor = Color.FromArgb(242,242,245);
            this.NavigationHoverForeColor = Color.FromArgb(44,44,44);
            this.NavigationSelectedBackColor = Color.FromArgb(242,242,245);
            this.NavigationSelectedForeColor = Color.FromArgb(44,44,44);
        }
    }
}