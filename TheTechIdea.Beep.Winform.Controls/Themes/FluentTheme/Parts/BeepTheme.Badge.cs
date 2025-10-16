using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(245,246,248);
            this.BadgeForeColor = Color.FromArgb(32,32,32);
        }
    }
}