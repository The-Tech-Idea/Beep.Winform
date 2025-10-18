using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(242,242,245);
            this.TabForeColor = Color.FromArgb(44,44,44);
            this.TabBorderColor = Color.FromArgb(218,218,222);
            this.TabHoverBackColor = Color.FromArgb(242,242,245);
            this.TabHoverForeColor = Color.FromArgb(44,44,44);
            this.TabSelectedBackColor = Color.FromArgb(242,242,245);
            this.TabSelectedForeColor = Color.FromArgb(44,44,44);
            this.TabSelectedBorderColor = Color.FromArgb(218,218,222);
            this.TabHoverBorderColor = Color.FromArgb(218,218,222);
        }
    }
}