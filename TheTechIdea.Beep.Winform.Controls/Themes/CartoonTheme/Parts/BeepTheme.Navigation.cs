using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(255,251,235);
            this.NavigationForeColor = Color.FromArgb(33,37,41);
            this.NavigationHoverBackColor = Color.FromArgb(255,251,235);
            this.NavigationHoverForeColor = Color.FromArgb(33,37,41);
            this.NavigationSelectedBackColor = Color.FromArgb(255,251,235);
            this.NavigationSelectedForeColor = Color.FromArgb(33,37,41);
        }
    }
}