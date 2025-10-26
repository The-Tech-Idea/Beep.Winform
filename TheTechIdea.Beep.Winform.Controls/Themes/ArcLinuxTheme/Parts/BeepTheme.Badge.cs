using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = PrimaryColor;
            this.BadgeForeColor = OnPrimaryColor;
        }
    }
}
