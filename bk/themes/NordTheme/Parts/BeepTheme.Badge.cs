using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(46,52,64);
            this.BadgeForeColor = Color.FromArgb(216,222,233);
        }
    }
}