using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(250,250,250);
            this.StatusBarForeColor = Color.FromArgb(33,33,33);
            this.StatusBarBorderColor = Color.FromArgb(224,224,224);
            this.StatusBarHoverBackColor = Color.FromArgb(250,250,250);
            this.StatusBarHoverForeColor = Color.FromArgb(33,33,33);
            this.StatusBarHoverBorderColor = Color.FromArgb(224,224,224);
        }
    }
}