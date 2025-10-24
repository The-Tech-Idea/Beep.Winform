using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(46,52,64);
            this.NavigationForeColor = Color.FromArgb(216,222,233);
            this.NavigationHoverBackColor = Color.FromArgb(46,52,64);
            this.NavigationHoverForeColor = Color.FromArgb(216,222,233);
            this.NavigationSelectedBackColor = Color.FromArgb(46,52,64);
            this.NavigationSelectedForeColor = Color.FromArgb(216,222,233);
        }
    }
}