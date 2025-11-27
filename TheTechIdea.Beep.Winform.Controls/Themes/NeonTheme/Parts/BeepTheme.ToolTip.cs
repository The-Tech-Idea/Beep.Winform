using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = PanelGradiantMiddleColor;
            this.ToolTipForeColor = ForeColor;
            this.ToolTipBorderColor = InactiveBorderColor;
            this.ToolTipShadowColor = PanelGradiantMiddleColor;
            this.ToolTipShadowOpacity = PanelGradiantMiddleColor;
            this.ToolTipTextColor = ForeColor;
            this.ToolTipLinkColor = PanelGradiantMiddleColor;
            this.ToolTipLinkHoverColor = PanelGradiantMiddleColor;
            this.ToolTipLinkVisitedColor = PanelGradiantMiddleColor;
        }
    }
}