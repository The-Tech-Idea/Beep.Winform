using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(246,245,244);
            this.NavigationForeColor = Color.FromArgb(46,52,54);
            this.NavigationHoverBackColor = Color.FromArgb(246,245,244);
            this.NavigationHoverForeColor = Color.FromArgb(46,52,54);
            this.NavigationSelectedBackColor = Color.FromArgb(246,245,244);
            this.NavigationSelectedForeColor = Color.FromArgb(46,52,54);
        }
    }
}