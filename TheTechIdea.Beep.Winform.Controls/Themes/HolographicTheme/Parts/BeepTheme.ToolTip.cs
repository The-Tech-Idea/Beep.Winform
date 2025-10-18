using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(15,16,32);
            this.ToolTipForeColor = Color.FromArgb(245,247,255);
            this.ToolTipBorderColor = Color.FromArgb(74,79,123);
            this.ToolTipShadowColor = Color.FromArgb(15,16,32);
            this.ToolTipShadowOpacity = Color.FromArgb(15,16,32);
            this.ToolTipTextColor = Color.FromArgb(245,247,255);
            this.ToolTipLinkColor = Color.FromArgb(15,16,32);
            this.ToolTipLinkHoverColor = Color.FromArgb(15,16,32);
            this.ToolTipLinkVisitedColor = Color.FromArgb(15,16,32);
        }
    }
}