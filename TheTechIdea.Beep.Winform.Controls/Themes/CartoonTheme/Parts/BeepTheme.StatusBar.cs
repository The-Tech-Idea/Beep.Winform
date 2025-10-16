using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(255,251,235);
            this.StatusBarForeColor = Color.FromArgb(33,37,41);
            this.StatusBarBorderColor = Color.FromArgb(247,208,136);
            this.StatusBarHoverBackColor = Color.FromArgb(255,251,235);
            this.StatusBarHoverForeColor = Color.FromArgb(33,37,41);
            this.StatusBarHoverBorderColor = Color.FromArgb(247,208,136);
        }
    }
}