using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(242,242,247);
            this.StatusBarForeColor = Color.FromArgb(28,28,30);
            this.StatusBarBorderColor = Color.FromArgb(198,198,207);
            this.StatusBarHoverBackColor = Color.FromArgb(242,242,247);
            this.StatusBarHoverForeColor = Color.FromArgb(28,28,30);
            this.StatusBarHoverBorderColor = Color.FromArgb(198,198,207);
        }
    }
}