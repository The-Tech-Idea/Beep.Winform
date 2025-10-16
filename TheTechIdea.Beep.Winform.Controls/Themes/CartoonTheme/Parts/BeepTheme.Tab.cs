using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(255,251,235);
            this.TabForeColor = Color.FromArgb(33,37,41);
            this.TabBorderColor = Color.FromArgb(247,208,136);
            this.TabHoverBackColor = Color.FromArgb(255,251,235);
            this.TabHoverForeColor = Color.FromArgb(33,37,41);
            this.TabSelectedBackColor = Color.FromArgb(255,251,235);
            this.TabSelectedForeColor = Color.FromArgb(33,37,41);
            this.TabSelectedBorderColor = Color.FromArgb(247,208,136);
            this.TabHoverBorderColor = Color.FromArgb(247,208,136);
        }
    }
}