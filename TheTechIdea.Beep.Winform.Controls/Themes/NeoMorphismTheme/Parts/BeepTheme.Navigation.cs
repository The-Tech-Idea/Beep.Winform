using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(236,240,243);
            this.NavigationForeColor = Color.FromArgb(58,66,86);
            this.NavigationHoverBackColor = Color.FromArgb(236,240,243);
            this.NavigationHoverForeColor = Color.FromArgb(58,66,86);
            this.NavigationSelectedBackColor = Color.FromArgb(236,240,243);
            this.NavigationSelectedForeColor = Color.FromArgb(58,66,86);
        }
    }
}