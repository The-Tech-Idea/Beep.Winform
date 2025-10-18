using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(248,249,250);
            this.NavigationForeColor = Color.FromArgb(33,37,41);
            this.NavigationHoverBackColor = Color.FromArgb(248,249,250);
            this.NavigationHoverForeColor = Color.FromArgb(33,37,41);
            this.NavigationSelectedBackColor = Color.FromArgb(248,249,250);
            this.NavigationSelectedForeColor = Color.FromArgb(33,37,41);
        }
    }
}