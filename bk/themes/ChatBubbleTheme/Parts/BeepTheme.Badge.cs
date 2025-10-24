using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyBadge()
        {
            this.BadgeBackColor = Color.FromArgb(245,248,255);
            this.BadgeForeColor = Color.FromArgb(24,28,35);
        }
    }
}