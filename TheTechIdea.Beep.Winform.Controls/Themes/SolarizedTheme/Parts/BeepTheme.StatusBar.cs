using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(0,43,54);
            this.StatusBarForeColor = Color.FromArgb(147,161,161);
            this.StatusBarBorderColor = Color.FromArgb(88,110,117);
            this.StatusBarHoverBackColor = Color.FromArgb(0,43,54);
            this.StatusBarHoverForeColor = Color.FromArgb(147,161,161);
            this.StatusBarHoverBorderColor = Color.FromArgb(88,110,117);
        }
    }
}