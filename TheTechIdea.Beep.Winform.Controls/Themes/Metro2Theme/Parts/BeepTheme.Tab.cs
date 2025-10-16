using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(243,242,241);
            this.TabForeColor = Color.FromArgb(32,31,30);
            this.TabBorderColor = Color.FromArgb(220,220,220);
            this.TabHoverBackColor = Color.FromArgb(243,242,241);
            this.TabHoverForeColor = Color.FromArgb(32,31,30);
            this.TabSelectedBackColor = Color.FromArgb(243,242,241);
            this.TabSelectedForeColor = Color.FromArgb(32,31,30);
            this.TabSelectedBorderColor = Color.FromArgb(220,220,220);
            this.TabHoverBorderColor = Color.FromArgb(220,220,220);
        }
    }
}