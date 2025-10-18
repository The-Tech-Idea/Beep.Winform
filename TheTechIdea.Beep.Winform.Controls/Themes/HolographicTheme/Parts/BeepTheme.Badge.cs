using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(15,16,32);
            this.BadgeForeColor = Color.FromArgb(245,247,255);
        }
    }
}