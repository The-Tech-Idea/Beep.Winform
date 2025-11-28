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
            this.ToolTipBackColor = BackgroundColor;
            this.ToolTipForeColor = ForeColor;
            this.ToolTipBorderColor = BorderColor;
            this.ToolTipShadowColor = ShadowColor;
            this.ToolTipShadowOpacity = ShadowColor;
            this.ToolTipTextColor = ForeColor;
            this.ToolTipLinkColor = AccentColor;
            this.ToolTipLinkHoverColor = AccentColor;
            this.ToolTipLinkVisitedColor = AccentColor;
        }
    }
}