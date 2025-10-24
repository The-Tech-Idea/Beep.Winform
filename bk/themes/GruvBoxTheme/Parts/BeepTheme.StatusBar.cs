using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(40,40,40);
            this.StatusBarForeColor = Color.FromArgb(235,219,178);
            this.StatusBarBorderColor = Color.FromArgb(168,153,132);
            this.StatusBarHoverBackColor = Color.FromArgb(40,40,40);
            this.StatusBarHoverForeColor = Color.FromArgb(235,219,178);
            this.StatusBarHoverBorderColor = Color.FromArgb(168,153,132);
        }
    }
}