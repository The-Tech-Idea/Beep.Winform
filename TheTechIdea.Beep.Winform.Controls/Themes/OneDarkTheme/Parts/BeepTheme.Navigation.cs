using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(40,44,52);
            this.NavigationForeColor = Color.FromArgb(171,178,191);
            this.NavigationHoverBackColor = Color.FromArgb(40,44,52);
            this.NavigationHoverForeColor = Color.FromArgb(171,178,191);
            this.NavigationSelectedBackColor = Color.FromArgb(40,44,52);
            this.NavigationSelectedForeColor = Color.FromArgb(171,178,191);
        }
    }
}