using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(250,250,251);
            this.BadgeForeColor = Color.FromArgb(31,41,55);
        }
    }
}