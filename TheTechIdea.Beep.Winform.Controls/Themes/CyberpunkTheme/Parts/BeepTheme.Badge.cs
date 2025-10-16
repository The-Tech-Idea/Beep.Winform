using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(10,8,20);
            this.BadgeForeColor = Color.FromArgb(228,244,255);
        }
    }
}