using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(40,40,40);
            this.BadgeForeColor = Color.FromArgb(235,219,178);
        }
    }
}