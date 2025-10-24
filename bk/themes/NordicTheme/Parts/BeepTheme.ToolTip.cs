using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(250,250,251);
            this.ToolTipForeColor = Color.FromArgb(31,41,55);
            this.ToolTipBorderColor = Color.FromArgb(229,231,235);
            this.ToolTipShadowColor = Color.FromArgb(250,250,251);
            this.ToolTipShadowOpacity = Color.FromArgb(250,250,251);
            this.ToolTipTextColor = Color.FromArgb(31,41,55);
            this.ToolTipLinkColor = Color.FromArgb(250,250,251);
            this.ToolTipLinkHoverColor = Color.FromArgb(250,250,251);
            this.ToolTipLinkVisitedColor = Color.FromArgb(250,250,251);
        }
    }
}