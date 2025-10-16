using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(245,246,248);
            this.ToolTipForeColor = Color.FromArgb(32,32,32);
            this.ToolTipBorderColor = Color.FromArgb(218,223,230);
            this.ToolTipShadowColor = Color.FromArgb(245,246,248);
            this.ToolTipShadowOpacity = Color.FromArgb(245,246,248);
            this.ToolTipTextColor = Color.FromArgb(32,32,32);
            this.ToolTipLinkColor = Color.FromArgb(245,246,248);
            this.ToolTipLinkHoverColor = Color.FromArgb(245,246,248);
            this.ToolTipLinkVisitedColor = Color.FromArgb(245,246,248);
        }
    }
}