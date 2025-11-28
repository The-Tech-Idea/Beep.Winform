using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = PanelBackColor;
            this.ToolTipForeColor = ForeColor;
            this.ToolTipBorderColor = InactiveBorderColor;
            this.ToolTipShadowColor = ShadowColor;
            this.ToolTipShadowOpacity = ShadowColor;
            this.ToolTipTextColor = ForeColor;
            this.ToolTipLinkColor = PanelBackColor;
            this.ToolTipLinkHoverColor = PanelBackColor;
            this.ToolTipLinkVisitedColor = PanelBackColor;
        }
    }
}