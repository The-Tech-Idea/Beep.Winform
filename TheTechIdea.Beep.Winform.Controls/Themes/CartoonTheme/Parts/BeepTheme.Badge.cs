using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(255,251,235);
            this.BadgeForeColor = Color.FromArgb(33,37,41);
        }
    }
}