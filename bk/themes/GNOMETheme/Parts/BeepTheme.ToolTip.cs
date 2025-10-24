using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(246,245,244);
            this.ToolTipForeColor = Color.FromArgb(46,52,54);
            this.ToolTipBorderColor = Color.FromArgb(205,207,212);
            this.ToolTipShadowColor = Color.FromArgb(246,245,244);
            this.ToolTipShadowOpacity = Color.FromArgb(246,245,244);
            this.ToolTipTextColor = Color.FromArgb(46,52,54);
            this.ToolTipLinkColor = Color.FromArgb(246,245,244);
            this.ToolTipLinkHoverColor = Color.FromArgb(246,245,244);
            this.ToolTipLinkVisitedColor = Color.FromArgb(246,245,244);
        }
    }
}