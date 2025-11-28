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
            this.BadgeBackColor = BackgroundColor;
            this.BadgeForeColor = ForeColor;
        }
    }
}