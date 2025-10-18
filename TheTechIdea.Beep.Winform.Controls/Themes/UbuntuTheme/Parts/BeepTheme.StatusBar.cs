using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(242,242,245);
            this.StatusBarForeColor = Color.FromArgb(44,44,44);
            this.StatusBarBorderColor = Color.FromArgb(218,218,222);
            this.StatusBarHoverBackColor = Color.FromArgb(242,242,245);
            this.StatusBarHoverForeColor = Color.FromArgb(44,44,44);
            this.StatusBarHoverBorderColor = Color.FromArgb(218,218,222);
        }
    }
}