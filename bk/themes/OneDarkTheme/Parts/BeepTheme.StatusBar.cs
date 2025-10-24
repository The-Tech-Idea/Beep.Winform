using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(40,44,52);
            this.StatusBarForeColor = Color.FromArgb(171,178,191);
            this.StatusBarBorderColor = Color.FromArgb(92,99,112);
            this.StatusBarHoverBackColor = Color.FromArgb(40,44,52);
            this.StatusBarHoverForeColor = Color.FromArgb(171,178,191);
            this.StatusBarHoverBorderColor = Color.FromArgb(92,99,112);
        }
    }
}