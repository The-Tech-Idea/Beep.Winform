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
            this.ToolTipBackColor = BackgroundColor;
            this.ToolTipForeColor = ForeColor;
            this.ToolTipBorderColor = BorderColor;
            this.ToolTipShadowColor = PanelBackColor;
            this.ToolTipShadowOpacity = PanelBackColor;
            this.ToolTipTextColor = ForeColor;
            this.ToolTipLinkColor = PrimaryColor;
            this.ToolTipLinkHoverColor = ThemeUtil.Lighten(PrimaryColor, 0.15);
            this.ToolTipLinkVisitedColor = SecondaryColor;
        }
    }
}