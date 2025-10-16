using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(250,250,250);
            this.BadgeForeColor = Color.FromArgb(20,20,20);
        }
    }
}