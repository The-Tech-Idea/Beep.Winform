using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(10,12,20);
            this.StatusBarForeColor = Color.FromArgb(235,245,255);
            this.StatusBarBorderColor = Color.FromArgb(60,70,100);
            this.StatusBarHoverBackColor = Color.FromArgb(10,12,20);
            this.StatusBarHoverForeColor = Color.FromArgb(235,245,255);
            this.StatusBarHoverBorderColor = Color.FromArgb(60,70,100);
        }
    }
}