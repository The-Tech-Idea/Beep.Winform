using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(10,12,20);
            this.TabForeColor = Color.FromArgb(235,245,255);
            this.TabBorderColor = Color.FromArgb(60,70,100);
            this.TabHoverBackColor = Color.FromArgb(10,12,20);
            this.TabHoverForeColor = Color.FromArgb(235,245,255);
            this.TabSelectedBackColor = Color.FromArgb(10,12,20);
            this.TabSelectedForeColor = Color.FromArgb(235,245,255);
            this.TabSelectedBorderColor = Color.FromArgb(60,70,100);
            this.TabHoverBorderColor = Color.FromArgb(60,70,100);
        }
    }
}