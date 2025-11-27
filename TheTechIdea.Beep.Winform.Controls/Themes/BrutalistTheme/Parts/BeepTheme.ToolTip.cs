using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = SurfaceColor;
            this.ToolTipForeColor = ForeColor;
            this.ToolTipBorderColor = BorderColor;
            this.ToolTipShadowColor = SurfaceColor;
            this.ToolTipShadowOpacity = SurfaceColor;
            this.ToolTipTextColor = ForeColor;
            this.ToolTipLinkColor = AccentColor;
            this.ToolTipLinkHoverColor = AccentColor;
            this.ToolTipLinkVisitedColor = AccentColor;
        }
    }
}