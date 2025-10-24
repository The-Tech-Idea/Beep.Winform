using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(46,52,64);
            this.ToolTipForeColor = Color.FromArgb(216,222,233);
            this.ToolTipBorderColor = Color.FromArgb(76,86,106);
            this.ToolTipShadowColor = Color.FromArgb(46,52,64);
            this.ToolTipShadowOpacity = Color.FromArgb(46,52,64);
            this.ToolTipTextColor = Color.FromArgb(216,222,233);
            this.ToolTipLinkColor = Color.FromArgb(46,52,64);
            this.ToolTipLinkHoverColor = Color.FromArgb(46,52,64);
            this.ToolTipLinkVisitedColor = Color.FromArgb(46,52,64);
        }
    }
}