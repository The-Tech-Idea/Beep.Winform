using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(242,242,247);
            this.BadgeForeColor = Color.FromArgb(28,28,30);
        }
    }
}