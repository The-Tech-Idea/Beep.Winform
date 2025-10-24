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
            this.ToolTipBackColor = Color.FromArgb(40,40,40);
            this.ToolTipForeColor = Color.FromArgb(235,219,178);
            this.ToolTipBorderColor = Color.FromArgb(168,153,132);
            this.ToolTipShadowColor = Color.FromArgb(40,40,40);
            this.ToolTipShadowOpacity = Color.FromArgb(40,40,40);
            this.ToolTipTextColor = Color.FromArgb(235,219,178);
            this.ToolTipLinkColor = Color.FromArgb(40,40,40);
            this.ToolTipLinkHoverColor = Color.FromArgb(40,40,40);
            this.ToolTipLinkVisitedColor = Color.FromArgb(40,40,40);
        }
    }
}