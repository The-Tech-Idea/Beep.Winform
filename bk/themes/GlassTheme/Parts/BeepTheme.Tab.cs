using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(236,244,255);
            this.TabForeColor = Color.FromArgb(17,24,39);
            this.TabBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.TabHoverBackColor = Color.FromArgb(236,244,255);
            this.TabHoverForeColor = Color.FromArgb(17,24,39);
            this.TabSelectedBackColor = Color.FromArgb(236,244,255);
            this.TabSelectedForeColor = Color.FromArgb(17,24,39);
            this.TabSelectedBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.TabHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
        }
    }
}