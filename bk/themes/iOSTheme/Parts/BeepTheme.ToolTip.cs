using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(242,242,247);
            this.ToolTipForeColor = Color.FromArgb(28,28,30);
            this.ToolTipBorderColor = Color.FromArgb(198,198,207);
            this.ToolTipShadowColor = Color.FromArgb(242,242,247);
            this.ToolTipShadowOpacity = Color.FromArgb(242,242,247);
            this.ToolTipTextColor = Color.FromArgb(28,28,30);
            this.ToolTipLinkColor = Color.FromArgb(242,242,247);
            this.ToolTipLinkHoverColor = Color.FromArgb(242,242,247);
            this.ToolTipLinkVisitedColor = Color.FromArgb(242,242,247);
        }
    }
}