using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(245,246,247);
            this.TabForeColor = Color.FromArgb(43,45,48);
            this.TabBorderColor = Color.FromArgb(220,223,230);
            this.TabHoverBackColor = Color.FromArgb(245,246,247);
            this.TabHoverForeColor = Color.FromArgb(43,45,48);
            this.TabSelectedBackColor = Color.FromArgb(245,246,247);
            this.TabSelectedForeColor = Color.FromArgb(43,45,48);
            this.TabSelectedBorderColor = Color.FromArgb(220,223,230);
            this.TabHoverBorderColor = Color.FromArgb(220,223,230);
        }
    }
}