using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(40,42,54);
            this.StatusBarForeColor = Color.FromArgb(248,248,242);
            this.StatusBarBorderColor = Color.FromArgb(98,114,164);
            this.StatusBarHoverBackColor = Color.FromArgb(40,42,54);
            this.StatusBarHoverForeColor = Color.FromArgb(248,248,242);
            this.StatusBarHoverBorderColor = Color.FromArgb(98,114,164);
        }
    }
}