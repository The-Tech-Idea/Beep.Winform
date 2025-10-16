using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(255,255,255);
            this.ToolTipForeColor = Color.FromArgb(31,41,55);
            this.ToolTipBorderColor = Color.FromArgb(209,213,219);
            this.ToolTipShadowColor = Color.FromArgb(255,255,255);
            this.ToolTipShadowOpacity = Color.FromArgb(255,255,255);
            this.ToolTipTextColor = Color.FromArgb(31,41,55);
            this.ToolTipLinkColor = Color.FromArgb(255,255,255);
            this.ToolTipLinkHoverColor = Color.FromArgb(255,255,255);
            this.ToolTipLinkVisitedColor = Color.FromArgb(255,255,255);
        }
    }
}