using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = SurfaceColor;
            this.ToolTipForeColor = ForeColor;
            this.ToolTipBorderColor = InactiveBorderColor;
            this.ToolTipShadowColor = PanelGradiantMiddleColor;
            this.ToolTipShadowOpacity = PanelGradiantMiddleColor;
            this.ToolTipTextColor = ForeColor;
            this.ToolTipLinkColor = AccentColor;
            this.ToolTipLinkHoverColor = PanelGradiantStartColor;
            this.ToolTipLinkVisitedColor = AccentColor;
        }
    }
}