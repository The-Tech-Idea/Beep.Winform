using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(0,43,54);
            this.ToolTipForeColor = Color.FromArgb(147,161,161);
            this.ToolTipBorderColor = Color.FromArgb(88,110,117);
            this.ToolTipShadowColor = Color.FromArgb(0,43,54);
            this.ToolTipShadowOpacity = Color.FromArgb(0,43,54);
            this.ToolTipTextColor = Color.FromArgb(147,161,161);
            this.ToolTipLinkColor = Color.FromArgb(0,43,54);
            this.ToolTipLinkHoverColor = Color.FromArgb(0,43,54);
            this.ToolTipLinkVisitedColor = Color.FromArgb(0,43,54);
        }
    }
}