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
            this.ToolTipBackColor = SurfaceColor;
            this.ToolTipForeColor = ForeColor;
            this.ToolTipBorderColor = BorderColor;
            this.ToolTipShadowColor = BorderColor;
            this.ToolTipShadowOpacity = PanelBackColor;
            this.ToolTipTextColor = ForeColor;
            this.ToolTipLinkColor = PrimaryColor;
            this.ToolTipLinkHoverColor = SecondaryColor;
            this.ToolTipLinkVisitedColor = AccentColor;
        }
    }
}