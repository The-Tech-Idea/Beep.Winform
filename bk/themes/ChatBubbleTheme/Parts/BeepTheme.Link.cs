using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyLink()
        {
            this.LinkColor = Color.FromArgb(245,248,255);
            this.LinkHoverColor = Color.FromArgb(245,248,255);
            this.LinkIsUnderline = false;
        }
    }
}