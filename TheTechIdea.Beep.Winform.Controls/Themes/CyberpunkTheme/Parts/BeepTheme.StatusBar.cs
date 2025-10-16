using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(10,8,20);
            this.StatusBarForeColor = Color.FromArgb(228,244,255);
            this.StatusBarBorderColor = Color.FromArgb(90,20,110);
            this.StatusBarHoverBackColor = Color.FromArgb(10,8,20);
            this.StatusBarHoverForeColor = Color.FromArgb(228,244,255);
            this.StatusBarHoverBorderColor = Color.FromArgb(90,20,110);
        }
    }
}