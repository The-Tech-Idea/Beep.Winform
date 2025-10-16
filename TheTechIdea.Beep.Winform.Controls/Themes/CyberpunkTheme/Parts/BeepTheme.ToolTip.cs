using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(10,8,20);
            this.ToolTipForeColor = Color.FromArgb(228,244,255);
            this.ToolTipBorderColor = Color.FromArgb(90,20,110);
            this.ToolTipShadowColor = Color.FromArgb(10,8,20);
            this.ToolTipShadowOpacity = Color.FromArgb(10,8,20);
            this.ToolTipTextColor = Color.FromArgb(228,244,255);
            this.ToolTipLinkColor = Color.FromArgb(10,8,20);
            this.ToolTipLinkHoverColor = Color.FromArgb(10,8,20);
            this.ToolTipLinkVisitedColor = Color.FromArgb(10,8,20);
        }
    }
}