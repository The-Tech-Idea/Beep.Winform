using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(15,16,32);
            this.TabForeColor = Color.FromArgb(245,247,255);
            this.TabBorderColor = Color.FromArgb(74,79,123);
            this.TabHoverBackColor = Color.FromArgb(15,16,32);
            this.TabHoverForeColor = Color.FromArgb(245,247,255);
            this.TabSelectedBackColor = Color.FromArgb(15,16,32);
            this.TabSelectedForeColor = Color.FromArgb(245,247,255);
            this.TabSelectedBorderColor = Color.FromArgb(74,79,123);
            this.TabHoverBorderColor = Color.FromArgb(74,79,123);
        }
    }
}