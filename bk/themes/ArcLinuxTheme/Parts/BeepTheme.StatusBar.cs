using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(245,246,247);
            this.StatusBarForeColor = Color.FromArgb(43,45,48);
            this.StatusBarBorderColor = Color.FromArgb(220,223,230);
            this.StatusBarHoverBackColor = Color.FromArgb(245,246,247);
            this.StatusBarHoverForeColor = Color.FromArgb(43,45,48);
            this.StatusBarHoverBorderColor = Color.FromArgb(220,223,230);
        }
    }
}