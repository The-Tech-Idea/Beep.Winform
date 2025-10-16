using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(250,250,252);
            this.ToolTipForeColor = Color.FromArgb(28,28,30);
            this.ToolTipBorderColor = Color.FromArgb(229,229,234);
            this.ToolTipShadowColor = Color.FromArgb(250,250,252);
            this.ToolTipShadowOpacity = Color.FromArgb(250,250,252);
            this.ToolTipTextColor = Color.FromArgb(28,28,30);
            this.ToolTipLinkColor = Color.FromArgb(250,250,252);
            this.ToolTipLinkHoverColor = Color.FromArgb(250,250,252);
            this.ToolTipLinkVisitedColor = Color.FromArgb(250,250,252);
        }
    }
}