using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(0,43,54);
            this.TabForeColor = Color.FromArgb(147,161,161);
            this.TabBorderColor = Color.FromArgb(88,110,117);
            this.TabHoverBackColor = Color.FromArgb(0,43,54);
            this.TabHoverForeColor = Color.FromArgb(147,161,161);
            this.TabSelectedBackColor = Color.FromArgb(0,43,54);
            this.TabSelectedForeColor = Color.FromArgb(147,161,161);
            this.TabSelectedBorderColor = Color.FromArgb(88,110,117);
            this.TabHoverBorderColor = Color.FromArgb(88,110,117);
        }
    }
}