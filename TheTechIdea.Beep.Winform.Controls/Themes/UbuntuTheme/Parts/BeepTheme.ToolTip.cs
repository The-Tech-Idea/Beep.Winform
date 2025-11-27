using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = SurfaceColor;
            this.ToolTipForeColor = ForeColor;
            this.ToolTipBorderColor = BorderColor;
            this.ToolTipShadowColor = ShadowColor;
            this.ToolTipShadowOpacity = ShadowOpacityColor;
            this.ToolTipTextColor = ForeColor;
            this.ToolTipLinkColor = AccentColor;
            this.ToolTipLinkHoverColor = PrimaryColor;
            this.ToolTipLinkVisitedColor = SecondaryColor;
        }
    }
}