using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(245,248,255);
            this.ToolTipForeColor = Color.FromArgb(24,28,35);
            this.ToolTipBorderColor = Color.FromArgb(210,220,235);
            this.ToolTipShadowColor = Color.FromArgb(245,248,255);
            this.ToolTipShadowOpacity = Color.FromArgb(245,248,255);
            this.ToolTipTextColor = Color.FromArgb(24,28,35);
            this.ToolTipLinkColor = Color.FromArgb(245,248,255);
            this.ToolTipLinkHoverColor = Color.FromArgb(245,248,255);
            this.ToolTipLinkVisitedColor = Color.FromArgb(245,248,255);
        }
    }
}