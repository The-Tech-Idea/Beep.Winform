using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(250,250,250);
            this.ToolTipForeColor = Color.FromArgb(20,20,20);
            this.ToolTipBorderColor = Color.FromArgb(0,0,0);
            this.ToolTipShadowColor = Color.FromArgb(250,250,250);
            this.ToolTipShadowOpacity = Color.FromArgb(250,250,250);
            this.ToolTipTextColor = Color.FromArgb(20,20,20);
            this.ToolTipLinkColor = Color.FromArgb(250,250,250);
            this.ToolTipLinkHoverColor = Color.FromArgb(250,250,250);
            this.ToolTipLinkVisitedColor = Color.FromArgb(250,250,250);
        }
    }
}