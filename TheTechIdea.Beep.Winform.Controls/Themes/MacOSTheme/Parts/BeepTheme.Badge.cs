using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(250,250,252);
            this.BadgeForeColor = Color.FromArgb(28,28,30);
        }
    }
}