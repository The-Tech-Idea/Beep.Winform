using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(26,27,38);
            this.TabForeColor = Color.FromArgb(192,202,245);
            this.TabBorderColor = Color.FromArgb(86,95,137);
            this.TabHoverBackColor = Color.FromArgb(26,27,38);
            this.TabHoverForeColor = Color.FromArgb(192,202,245);
            this.TabSelectedBackColor = Color.FromArgb(26,27,38);
            this.TabSelectedForeColor = Color.FromArgb(192,202,245);
            this.TabSelectedBorderColor = Color.FromArgb(86,95,137);
            this.TabHoverBorderColor = Color.FromArgb(86,95,137);
        }
    }
}