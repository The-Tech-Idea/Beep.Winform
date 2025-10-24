using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(246,245,244);
            this.TabForeColor = Color.FromArgb(46,52,54);
            this.TabBorderColor = Color.FromArgb(205,207,212);
            this.TabHoverBackColor = Color.FromArgb(246,245,244);
            this.TabHoverForeColor = Color.FromArgb(46,52,54);
            this.TabSelectedBackColor = Color.FromArgb(246,245,244);
            this.TabSelectedForeColor = Color.FromArgb(46,52,54);
            this.TabSelectedBorderColor = Color.FromArgb(205,207,212);
            this.TabHoverBorderColor = Color.FromArgb(205,207,212);
        }
    }
}