using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(236,240,243);
            this.StatusBarForeColor = Color.FromArgb(58,66,86);
            this.StatusBarBorderColor = Color.FromArgb(221,228,235);
            this.StatusBarHoverBackColor = Color.FromArgb(236,240,243);
            this.StatusBarHoverForeColor = Color.FromArgb(58,66,86);
            this.StatusBarHoverBorderColor = Color.FromArgb(221,228,235);
        }
    }
}