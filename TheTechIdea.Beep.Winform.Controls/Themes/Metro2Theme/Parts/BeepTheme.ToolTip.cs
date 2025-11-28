using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = PanelBackColor;
            this.ToolTipForeColor = ForeColor;
            this.ToolTipBorderColor = InactiveBorderColor;
            this.ToolTipShadowColor = ShadowColor;
            this.ToolTipShadowOpacity = PanelBackColor;
            this.ToolTipTextColor = ForeColor;
            this.ToolTipLinkColor = PrimaryColor;
            this.ToolTipLinkHoverColor = PrimaryColor;
            this.ToolTipLinkVisitedColor = PrimaryColor;
        }
    }
}