using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(250,250,252);
            this.TabForeColor = Color.FromArgb(28,28,30);
            this.TabBorderColor = Color.FromArgb(229,229,234);
            this.TabHoverBackColor = Color.FromArgb(250,250,252);
            this.TabHoverForeColor = Color.FromArgb(28,28,30);
            this.TabSelectedBackColor = Color.FromArgb(250,250,252);
            this.TabSelectedForeColor = Color.FromArgb(28,28,30);
            this.TabSelectedBorderColor = Color.FromArgb(229,229,234);
            this.TabHoverBorderColor = Color.FromArgb(229,229,234);
        }
    }
}