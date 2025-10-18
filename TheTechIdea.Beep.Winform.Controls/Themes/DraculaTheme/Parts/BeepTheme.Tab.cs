using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(40,42,54);
            this.TabForeColor = Color.FromArgb(248,248,242);
            this.TabBorderColor = Color.FromArgb(98,114,164);
            this.TabHoverBackColor = Color.FromArgb(40,42,54);
            this.TabHoverForeColor = Color.FromArgb(248,248,242);
            this.TabSelectedBackColor = Color.FromArgb(40,42,54);
            this.TabSelectedForeColor = Color.FromArgb(248,248,242);
            this.TabSelectedBorderColor = Color.FromArgb(98,114,164);
            this.TabHoverBorderColor = Color.FromArgb(98,114,164);
        }
    }
}