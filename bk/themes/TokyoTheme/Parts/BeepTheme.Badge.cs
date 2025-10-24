using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(26,27,38);
            this.BadgeForeColor = Color.FromArgb(192,202,245);
        }
    }
}