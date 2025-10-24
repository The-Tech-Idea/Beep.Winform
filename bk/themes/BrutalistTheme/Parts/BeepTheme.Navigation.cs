using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(250,250,250);
            this.NavigationForeColor = Color.FromArgb(20,20,20);
            this.NavigationHoverBackColor = Color.FromArgb(250,250,250);
            this.NavigationHoverForeColor = Color.FromArgb(20,20,20);
            this.NavigationSelectedBackColor = Color.FromArgb(250,250,250);
            this.NavigationSelectedForeColor = Color.FromArgb(20,20,20);
        }
    }
}