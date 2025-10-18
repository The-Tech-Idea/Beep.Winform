using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(40,44,52);
            this.ToolTipForeColor = Color.FromArgb(171,178,191);
            this.ToolTipBorderColor = Color.FromArgb(92,99,112);
            this.ToolTipShadowColor = Color.FromArgb(40,44,52);
            this.ToolTipShadowOpacity = Color.FromArgb(40,44,52);
            this.ToolTipTextColor = Color.FromArgb(171,178,191);
            this.ToolTipLinkColor = Color.FromArgb(40,44,52);
            this.ToolTipLinkHoverColor = Color.FromArgb(40,44,52);
            this.ToolTipLinkVisitedColor = Color.FromArgb(40,44,52);
        }
    }
}