using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = SurfaceColor;
            this.ToolTipForeColor = ForeColor;
            this.ToolTipBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.ToolTipShadowColor = SurfaceColor;
            this.ToolTipShadowOpacity = SurfaceColor;
            this.ToolTipTextColor = ForeColor;
            this.ToolTipLinkColor = SurfaceColor;
            this.ToolTipLinkHoverColor = SurfaceColor;
            this.ToolTipLinkVisitedColor = SurfaceColor;
        }
    }
}
