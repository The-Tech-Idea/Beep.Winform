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
            this.ToolTipBackColor = Color.FromArgb(243,242,241);
            this.ToolTipForeColor = Color.FromArgb(32,31,30);
            this.ToolTipBorderColor = Color.FromArgb(220,220,220);
            this.ToolTipShadowColor = Color.FromArgb(243,242,241);
            this.ToolTipShadowOpacity = Color.FromArgb(243,242,241);
            this.ToolTipTextColor = Color.FromArgb(32,31,30);
            this.ToolTipLinkColor = Color.FromArgb(243,242,241);
            this.ToolTipLinkHoverColor = Color.FromArgb(243,242,241);
            this.ToolTipLinkVisitedColor = Color.FromArgb(243,242,241);
        }
    }
}