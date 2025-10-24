using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(255,251,235);
            this.ToolTipForeColor = Color.FromArgb(33,37,41);
            this.ToolTipBorderColor = Color.FromArgb(247,208,136);
            this.ToolTipShadowColor = Color.FromArgb(255,251,235);
            this.ToolTipShadowOpacity = Color.FromArgb(255,251,235);
            this.ToolTipTextColor = Color.FromArgb(33,37,41);
            this.ToolTipLinkColor = Color.FromArgb(255,251,235);
            this.ToolTipLinkHoverColor = Color.FromArgb(255,251,235);
            this.ToolTipLinkVisitedColor = Color.FromArgb(255,251,235);
        }
    }
}