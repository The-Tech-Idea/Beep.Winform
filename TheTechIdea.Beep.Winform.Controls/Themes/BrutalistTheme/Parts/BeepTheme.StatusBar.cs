using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(250,250,250);
            this.StatusBarForeColor = Color.FromArgb(20,20,20);
            this.StatusBarBorderColor = Color.FromArgb(0,0,0);
            this.StatusBarHoverBackColor = Color.FromArgb(250,250,250);
            this.StatusBarHoverForeColor = Color.FromArgb(20,20,20);
            this.StatusBarHoverBorderColor = Color.FromArgb(0,0,0);
        }
    }
}