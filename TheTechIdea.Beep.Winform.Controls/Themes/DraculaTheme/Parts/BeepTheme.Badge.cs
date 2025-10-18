using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(40,42,54);
            this.BadgeForeColor = Color.FromArgb(248,248,242);
        }
    }
}