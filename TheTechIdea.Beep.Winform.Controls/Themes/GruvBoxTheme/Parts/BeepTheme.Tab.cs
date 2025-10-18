using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(40,40,40);
            this.TabForeColor = Color.FromArgb(235,219,178);
            this.TabBorderColor = Color.FromArgb(168,153,132);
            this.TabHoverBackColor = Color.FromArgb(40,40,40);
            this.TabHoverForeColor = Color.FromArgb(235,219,178);
            this.TabSelectedBackColor = Color.FromArgb(40,40,40);
            this.TabSelectedForeColor = Color.FromArgb(235,219,178);
            this.TabSelectedBorderColor = Color.FromArgb(168,153,132);
            this.TabHoverBorderColor = Color.FromArgb(168,153,132);
        }
    }
}