using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(15,16,32);
            this.StatusBarForeColor = Color.FromArgb(245,247,255);
            this.StatusBarBorderColor = Color.FromArgb(74,79,123);
            this.StatusBarHoverBackColor = Color.FromArgb(15,16,32);
            this.StatusBarHoverForeColor = Color.FromArgb(245,247,255);
            this.StatusBarHoverBorderColor = Color.FromArgb(74,79,123);
        }
    }
}