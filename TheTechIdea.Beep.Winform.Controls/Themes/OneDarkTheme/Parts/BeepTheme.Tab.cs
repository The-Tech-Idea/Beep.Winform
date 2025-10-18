using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(40,44,52);
            this.TabForeColor = Color.FromArgb(171,178,191);
            this.TabBorderColor = Color.FromArgb(92,99,112);
            this.TabHoverBackColor = Color.FromArgb(40,44,52);
            this.TabHoverForeColor = Color.FromArgb(171,178,191);
            this.TabSelectedBackColor = Color.FromArgb(40,44,52);
            this.TabSelectedForeColor = Color.FromArgb(171,178,191);
            this.TabSelectedBorderColor = Color.FromArgb(92,99,112);
            this.TabHoverBorderColor = Color.FromArgb(92,99,112);
        }
    }
}