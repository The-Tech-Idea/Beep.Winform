using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(250,250,252);
            this.StatusBarForeColor = Color.FromArgb(28,28,30);
            this.StatusBarBorderColor = Color.FromArgb(229,229,234);
            this.StatusBarHoverBackColor = Color.FromArgb(250,250,252);
            this.StatusBarHoverForeColor = Color.FromArgb(28,28,30);
            this.StatusBarHoverBorderColor = Color.FromArgb(229,229,234);
        }
    }
}