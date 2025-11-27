using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = PanelBackColor;
            this.ToolTipForeColor = ForeColor;
            this.ToolTipBorderColor = BorderColor;
            this.ToolTipShadowColor = PanelBackColor;
            this.ToolTipShadowOpacity = this.ToolTipShadowOpacity; // leave as-is (set by theme palette or elsewhere)
            this.ToolTipTextColor = ForeColor;
            this.ToolTipLinkColor = PrimaryColor;
            this.ToolTipLinkHoverColor = PrimaryColor;
            this.ToolTipLinkVisitedColor = AccentColor;
        }
    }
}