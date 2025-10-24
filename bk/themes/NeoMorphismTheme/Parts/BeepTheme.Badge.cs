using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(236,240,243);
            this.BadgeForeColor = Color.FromArgb(58,66,86);
        }
    }
}