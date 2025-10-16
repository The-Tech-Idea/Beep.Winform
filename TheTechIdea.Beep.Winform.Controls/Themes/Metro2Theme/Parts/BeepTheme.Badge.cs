using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(243,242,241);
            this.BadgeForeColor = Color.FromArgb(32,31,30);
        }
    }
}