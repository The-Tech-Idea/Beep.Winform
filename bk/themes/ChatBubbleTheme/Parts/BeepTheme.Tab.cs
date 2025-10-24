using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyTab()
        {
            this.TabBackColor = Color.FromArgb(245,248,255);
            this.TabForeColor = Color.FromArgb(24,28,35);
            this.TabBorderColor = Color.FromArgb(210,220,235);
            this.TabHoverBackColor = Color.FromArgb(245,248,255);
            this.TabHoverForeColor = Color.FromArgb(24,28,35);
            this.TabSelectedBackColor = Color.FromArgb(245,248,255);
            this.TabSelectedForeColor = Color.FromArgb(24,28,35);
            this.TabSelectedBorderColor = Color.FromArgb(210,220,235);
            this.TabHoverBorderColor = Color.FromArgb(210,220,235);
        }
    }
}