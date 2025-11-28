using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = SurfaceColor;
            this.ToolTipForeColor = ForeColor;
            this.ToolTipBorderColor = InactiveBorderColor;
            this.ToolTipShadowColor = BackgroundColor;
            this.ToolTipShadowOpacity = BackgroundColor;
            this.ToolTipTextColor = ForeColor;
            this.ToolTipLinkColor = SecondaryColor;
            this.ToolTipLinkHoverColor = PrimaryColor;
            this.ToolTipLinkVisitedColor = AccentColor;
        }
    }
}