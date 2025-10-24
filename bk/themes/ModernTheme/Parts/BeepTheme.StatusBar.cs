using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(255,255,255);
            this.StatusBarForeColor = Color.FromArgb(17,24,39);
            this.StatusBarBorderColor = Color.FromArgb(203,213,225);
            this.StatusBarHoverBackColor = Color.FromArgb(255,255,255);
            this.StatusBarHoverForeColor = Color.FromArgb(17,24,39);
            this.StatusBarHoverBorderColor = Color.FromArgb(203,213,225);
        }
    }
}