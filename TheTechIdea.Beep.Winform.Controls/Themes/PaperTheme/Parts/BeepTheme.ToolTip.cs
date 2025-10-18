using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(250,250,250);
            this.ToolTipForeColor = Color.FromArgb(33,33,33);
            this.ToolTipBorderColor = Color.FromArgb(224,224,224);
            this.ToolTipShadowColor = Color.FromArgb(250,250,250);
            this.ToolTipShadowOpacity = Color.FromArgb(250,250,250);
            this.ToolTipTextColor = Color.FromArgb(33,33,33);
            this.ToolTipLinkColor = Color.FromArgb(250,250,250);
            this.ToolTipLinkHoverColor = Color.FromArgb(250,250,250);
            this.ToolTipLinkVisitedColor = Color.FromArgb(250,250,250);
        }
    }
}