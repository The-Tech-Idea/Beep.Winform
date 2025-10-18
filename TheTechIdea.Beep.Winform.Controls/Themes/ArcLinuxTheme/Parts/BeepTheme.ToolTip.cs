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
            this.ToolTipBackColor = Color.FromArgb(245,246,247);
            this.ToolTipForeColor = Color.FromArgb(43,45,48);
            this.ToolTipBorderColor = Color.FromArgb(220,223,230);
            this.ToolTipShadowColor = Color.FromArgb(245,246,247);
            this.ToolTipShadowOpacity = Color.FromArgb(245,246,247);
            this.ToolTipTextColor = Color.FromArgb(43,45,48);
            this.ToolTipLinkColor = Color.FromArgb(245,246,247);
            this.ToolTipLinkHoverColor = Color.FromArgb(245,246,247);
            this.ToolTipLinkVisitedColor = Color.FromArgb(245,246,247);
        }
    }
}