using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(46,52,64);
            this.TabForeColor = Color.FromArgb(216,222,233);
            this.TabBorderColor = Color.FromArgb(76,86,106);
            this.TabHoverBackColor = Color.FromArgb(46,52,64);
            this.TabHoverForeColor = Color.FromArgb(216,222,233);
            this.TabSelectedBackColor = Color.FromArgb(46,52,64);
            this.TabSelectedForeColor = Color.FromArgb(216,222,233);
            this.TabSelectedBorderColor = Color.FromArgb(76,86,106);
            this.TabHoverBorderColor = Color.FromArgb(76,86,106);
        }
    }
}