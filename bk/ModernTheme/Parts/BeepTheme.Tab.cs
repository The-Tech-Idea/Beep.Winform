using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ModernTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(255,255,255);
            this.TabForeColor = Color.FromArgb(17,24,39);
            this.TabBorderColor = Color.FromArgb(203,213,225);
            this.TabHoverBackColor = Color.FromArgb(255,255,255);
            this.TabHoverForeColor = Color.FromArgb(17,24,39);
            this.TabSelectedBackColor = Color.FromArgb(255,255,255);
            this.TabSelectedForeColor = Color.FromArgb(17,24,39);
            this.TabSelectedBorderColor = Color.FromArgb(203,213,225);
            this.TabHoverBorderColor = Color.FromArgb(203,213,225);
        }
    }
}