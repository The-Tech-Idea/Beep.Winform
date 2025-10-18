using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(46,52,64);
            this.StatusBarForeColor = Color.FromArgb(216,222,233);
            this.StatusBarBorderColor = Color.FromArgb(76,86,106);
            this.StatusBarHoverBackColor = Color.FromArgb(46,52,64);
            this.StatusBarHoverForeColor = Color.FromArgb(216,222,233);
            this.StatusBarHoverBorderColor = Color.FromArgb(76,86,106);
        }
    }
}