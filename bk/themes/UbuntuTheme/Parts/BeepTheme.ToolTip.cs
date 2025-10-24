using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(242,242,245);
            this.ToolTipForeColor = Color.FromArgb(44,44,44);
            this.ToolTipBorderColor = Color.FromArgb(218,218,222);
            this.ToolTipShadowColor = Color.FromArgb(242,242,245);
            this.ToolTipShadowOpacity = Color.FromArgb(242,242,245);
            this.ToolTipTextColor = Color.FromArgb(44,44,44);
            this.ToolTipLinkColor = Color.FromArgb(242,242,245);
            this.ToolTipLinkHoverColor = Color.FromArgb(242,242,245);
            this.ToolTipLinkVisitedColor = Color.FromArgb(242,242,245);
        }
    }
}