using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(26,27,38);
            this.StatusBarForeColor = Color.FromArgb(192,202,245);
            this.StatusBarBorderColor = Color.FromArgb(86,95,137);
            this.StatusBarHoverBackColor = Color.FromArgb(26,27,38);
            this.StatusBarHoverForeColor = Color.FromArgb(192,202,245);
            this.StatusBarHoverBorderColor = Color.FromArgb(86,95,137);
        }
    }
}