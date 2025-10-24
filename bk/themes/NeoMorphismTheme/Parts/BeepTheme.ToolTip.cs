using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(236,240,243);
            this.ToolTipForeColor = Color.FromArgb(58,66,86);
            this.ToolTipBorderColor = Color.FromArgb(221,228,235);
            this.ToolTipShadowColor = Color.FromArgb(236,240,243);
            this.ToolTipShadowOpacity = Color.FromArgb(236,240,243);
            this.ToolTipTextColor = Color.FromArgb(58,66,86);
            this.ToolTipLinkColor = Color.FromArgb(236,240,243);
            this.ToolTipLinkHoverColor = Color.FromArgb(236,240,243);
            this.ToolTipLinkVisitedColor = Color.FromArgb(236,240,243);
        }
    }
}