using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(255,255,255);
            this.StatusBarForeColor = Color.FromArgb(31,41,55);
            this.StatusBarBorderColor = Color.FromArgb(209,213,219);
            this.StatusBarHoverBackColor = Color.FromArgb(255,255,255);
            this.StatusBarHoverForeColor = Color.FromArgb(31,41,55);
            this.StatusBarHoverBorderColor = Color.FromArgb(209,213,219);
        }
    }
}