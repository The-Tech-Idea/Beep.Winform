using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = BackgroundColor;
            this.ToolTipForeColor = ForeColor;
            this.ToolTipBorderColor = BorderColor;
            this.ToolTipShadowColor = ShadowColor;
            this.ToolTipShadowOpacity = BackgroundColor;
            this.ToolTipTextColor = ForeColor;
            this.ToolTipLinkColor = BackgroundColor;
            this.ToolTipLinkHoverColor = BackgroundColor;
            this.ToolTipLinkVisitedColor = BackgroundColor;
        }
    }
}