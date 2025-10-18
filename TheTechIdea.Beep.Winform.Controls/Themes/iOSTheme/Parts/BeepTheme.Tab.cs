using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(242,242,247);
            this.TabForeColor = Color.FromArgb(28,28,30);
            this.TabBorderColor = Color.FromArgb(198,198,207);
            this.TabHoverBackColor = Color.FromArgb(242,242,247);
            this.TabHoverForeColor = Color.FromArgb(28,28,30);
            this.TabSelectedBackColor = Color.FromArgb(242,242,247);
            this.TabSelectedForeColor = Color.FromArgb(28,28,30);
            this.TabSelectedBorderColor = Color.FromArgb(198,198,207);
            this.TabHoverBorderColor = Color.FromArgb(198,198,207);
        }
    }
}