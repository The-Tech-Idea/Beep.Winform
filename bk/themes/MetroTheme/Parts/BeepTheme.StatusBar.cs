using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(243,242,241);
            this.StatusBarForeColor = Color.FromArgb(32,31,30);
            this.StatusBarBorderColor = Color.FromArgb(225,225,225);
            this.StatusBarHoverBackColor = Color.FromArgb(243,242,241);
            this.StatusBarHoverForeColor = Color.FromArgb(32,31,30);
            this.StatusBarHoverBorderColor = Color.FromArgb(225,225,225);
        }
    }
}