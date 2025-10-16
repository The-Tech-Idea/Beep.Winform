using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(236,244,255);
            this.BadgeForeColor = Color.FromArgb(17,24,39);
        }
    }
}