using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(236,244,255);
            this.ToolTipForeColor = Color.FromArgb(17,24,39);
            this.ToolTipBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.ToolTipShadowColor = Color.FromArgb(236,244,255);
            this.ToolTipShadowOpacity = Color.FromArgb(236,244,255);
            this.ToolTipTextColor = Color.FromArgb(17,24,39);
            this.ToolTipLinkColor = Color.FromArgb(236,244,255);
            this.ToolTipLinkHoverColor = Color.FromArgb(236,244,255);
            this.ToolTipLinkVisitedColor = Color.FromArgb(236,244,255);
        }
    }
}