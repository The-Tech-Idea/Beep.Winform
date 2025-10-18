using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(248,249,250);
            this.StatusBarForeColor = Color.FromArgb(33,37,41);
            this.StatusBarBorderColor = Color.FromArgb(222,226,230);
            this.StatusBarHoverBackColor = Color.FromArgb(248,249,250);
            this.StatusBarHoverForeColor = Color.FromArgb(33,37,41);
            this.StatusBarHoverBorderColor = Color.FromArgb(222,226,230);
        }
    }
}