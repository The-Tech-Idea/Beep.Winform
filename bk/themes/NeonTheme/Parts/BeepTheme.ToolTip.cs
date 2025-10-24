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
            this.ToolTipBackColor = Color.FromArgb(10,12,20);
            this.ToolTipForeColor = Color.FromArgb(235,245,255);
            this.ToolTipBorderColor = Color.FromArgb(60,70,100);
            this.ToolTipShadowColor = Color.FromArgb(10,12,20);
            this.ToolTipShadowOpacity = Color.FromArgb(10,12,20);
            this.ToolTipTextColor = Color.FromArgb(235,245,255);
            this.ToolTipLinkColor = Color.FromArgb(10,12,20);
            this.ToolTipLinkHoverColor = Color.FromArgb(10,12,20);
            this.ToolTipLinkVisitedColor = Color.FromArgb(10,12,20);
        }
    }
}