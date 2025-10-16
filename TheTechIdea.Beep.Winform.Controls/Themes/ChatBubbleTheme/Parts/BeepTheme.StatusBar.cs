using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyStatusBar()
        {
            this.StatusBarBackColor = Color.FromArgb(245,248,255);
            this.StatusBarForeColor = Color.FromArgb(24,28,35);
            this.StatusBarBorderColor = Color.FromArgb(210,220,235);
            this.StatusBarHoverBackColor = Color.FromArgb(245,248,255);
            this.StatusBarHoverForeColor = Color.FromArgb(24,28,35);
            this.StatusBarHoverBorderColor = Color.FromArgb(210,220,235);
        }
    }
}