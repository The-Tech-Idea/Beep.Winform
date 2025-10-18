using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(40,42,54);
            this.ToolTipForeColor = Color.FromArgb(248,248,242);
            this.ToolTipBorderColor = Color.FromArgb(98,114,164);
            this.ToolTipShadowColor = Color.FromArgb(40,42,54);
            this.ToolTipShadowOpacity = Color.FromArgb(40,42,54);
            this.ToolTipTextColor = Color.FromArgb(248,248,242);
            this.ToolTipLinkColor = Color.FromArgb(40,42,54);
            this.ToolTipLinkHoverColor = Color.FromArgb(40,42,54);
            this.ToolTipLinkVisitedColor = Color.FromArgb(40,42,54);
        }
    }
}