using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(250,250,250);
            this.BadgeForeColor = Color.FromArgb(33,33,33);
        }
    }
}