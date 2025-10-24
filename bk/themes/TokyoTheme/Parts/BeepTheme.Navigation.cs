using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(26,27,38);
            this.NavigationForeColor = Color.FromArgb(192,202,245);
            this.NavigationHoverBackColor = Color.FromArgb(26,27,38);
            this.NavigationHoverForeColor = Color.FromArgb(192,202,245);
            this.NavigationSelectedBackColor = Color.FromArgb(26,27,38);
            this.NavigationSelectedForeColor = Color.FromArgb(192,202,245);
        }
    }
}