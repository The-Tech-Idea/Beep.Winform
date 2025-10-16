using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(246,245,244);
            this.StatusBarForeColor = Color.FromArgb(46,52,54);
            this.StatusBarBorderColor = Color.FromArgb(205,207,212);
            this.StatusBarHoverBackColor = Color.FromArgb(246,245,244);
            this.StatusBarHoverForeColor = Color.FromArgb(46,52,54);
            this.StatusBarHoverBorderColor = Color.FromArgb(205,207,212);
        }
    }
}