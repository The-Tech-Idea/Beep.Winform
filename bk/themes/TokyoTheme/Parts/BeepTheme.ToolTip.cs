using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(26,27,38);
            this.ToolTipForeColor = Color.FromArgb(192,202,245);
            this.ToolTipBorderColor = Color.FromArgb(86,95,137);
            this.ToolTipShadowColor = Color.FromArgb(26,27,38);
            this.ToolTipShadowOpacity = Color.FromArgb(26,27,38);
            this.ToolTipTextColor = Color.FromArgb(192,202,245);
            this.ToolTipLinkColor = Color.FromArgb(26,27,38);
            this.ToolTipLinkHoverColor = Color.FromArgb(26,27,38);
            this.ToolTipLinkVisitedColor = Color.FromArgb(26,27,38);
        }
    }
}