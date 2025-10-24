using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(40,42,54);
            this.NavigationForeColor = Color.FromArgb(248,248,242);
            this.NavigationHoverBackColor = Color.FromArgb(40,42,54);
            this.NavigationHoverForeColor = Color.FromArgb(248,248,242);
            this.NavigationSelectedBackColor = Color.FromArgb(40,42,54);
            this.NavigationSelectedForeColor = Color.FromArgb(248,248,242);
        }
    }
}