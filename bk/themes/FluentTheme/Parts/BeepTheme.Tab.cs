using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(245,246,248);
            this.TabForeColor = Color.FromArgb(32,32,32);
            this.TabBorderColor = Color.FromArgb(218,223,230);
            this.TabHoverBackColor = Color.FromArgb(245,246,248);
            this.TabHoverForeColor = Color.FromArgb(32,32,32);
            this.TabSelectedBackColor = Color.FromArgb(245,246,248);
            this.TabSelectedForeColor = Color.FromArgb(32,32,32);
            this.TabSelectedBorderColor = Color.FromArgb(218,223,230);
            this.TabHoverBorderColor = Color.FromArgb(218,223,230);
        }
    }
}