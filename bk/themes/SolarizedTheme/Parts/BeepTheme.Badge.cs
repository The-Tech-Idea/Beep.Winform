using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(0,43,54);
            this.BadgeForeColor = Color.FromArgb(147,161,161);
        }
    }
}