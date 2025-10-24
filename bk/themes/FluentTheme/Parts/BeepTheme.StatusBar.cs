using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(245,246,248);
            this.StatusBarForeColor = Color.FromArgb(32,32,32);
            this.StatusBarBorderColor = Color.FromArgb(218,223,230);
            this.StatusBarHoverBackColor = Color.FromArgb(245,246,248);
            this.StatusBarHoverForeColor = Color.FromArgb(32,32,32);
            this.StatusBarHoverBorderColor = Color.FromArgb(218,223,230);
        }
    }
}