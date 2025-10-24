using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyNavigation()
        {
            this.NavigationBackColor = Color.FromArgb(245,248,255);
            this.NavigationForeColor = Color.FromArgb(24,28,35);
            this.NavigationHoverBackColor = Color.FromArgb(245,248,255);
            this.NavigationHoverForeColor = Color.FromArgb(24,28,35);
            this.NavigationSelectedBackColor = Color.FromArgb(245,248,255);
            this.NavigationSelectedForeColor = Color.FromArgb(24,28,35);
        }
    }
}