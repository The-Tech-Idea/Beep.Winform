using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(236,240,243);
            this.TabForeColor = Color.FromArgb(58,66,86);
            this.TabBorderColor = Color.FromArgb(221,228,235);
            this.TabHoverBackColor = Color.FromArgb(236,240,243);
            this.TabHoverForeColor = Color.FromArgb(58,66,86);
            this.TabSelectedBackColor = Color.FromArgb(236,240,243);
            this.TabSelectedForeColor = Color.FromArgb(58,66,86);
            this.TabSelectedBorderColor = Color.FromArgb(221,228,235);
            this.TabHoverBorderColor = Color.FromArgb(221,228,235);
        }
    }
}