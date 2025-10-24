using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(250,250,251);
            this.NavigationForeColor = Color.FromArgb(31,41,55);
            this.NavigationHoverBackColor = Color.FromArgb(250,250,251);
            this.NavigationHoverForeColor = Color.FromArgb(31,41,55);
            this.NavigationSelectedBackColor = Color.FromArgb(250,250,251);
            this.NavigationSelectedForeColor = Color.FromArgb(31,41,55);
        }
    }
}