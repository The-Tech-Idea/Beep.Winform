using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(10,12,20);
            this.BadgeForeColor = Color.FromArgb(235,245,255);
        }
    }
}