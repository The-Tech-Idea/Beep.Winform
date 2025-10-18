using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyToolTip()
        {
            this.ToolTipBackColor = Color.FromArgb(248,249,250);
            this.ToolTipForeColor = Color.FromArgb(33,37,41);
            this.ToolTipBorderColor = Color.FromArgb(222,226,230);
            this.ToolTipShadowColor = Color.FromArgb(248,249,250);
            this.ToolTipShadowOpacity = Color.FromArgb(248,249,250);
            this.ToolTipTextColor = Color.FromArgb(33,37,41);
            this.ToolTipLinkColor = Color.FromArgb(248,249,250);
            this.ToolTipLinkHoverColor = Color.FromArgb(248,249,250);
            this.ToolTipLinkVisitedColor = Color.FromArgb(248,249,250);
        }
    }
}