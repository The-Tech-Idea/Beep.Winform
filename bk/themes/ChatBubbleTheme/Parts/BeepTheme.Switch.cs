using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplySwitch()
        {
            this.SwitchBackColor = Color.FromArgb(245,248,255);
            this.SwitchBorderColor = Color.FromArgb(210,220,235);
            this.SwitchForeColor = Color.FromArgb(24,28,35);
            this.SwitchSelectedBackColor = Color.FromArgb(245,248,255);
            this.SwitchSelectedBorderColor = Color.FromArgb(210,220,235);
            this.SwitchSelectedForeColor = Color.FromArgb(24,28,35);
            this.SwitchHoverBackColor = Color.FromArgb(245,248,255);
            this.SwitchHoverBorderColor = Color.FromArgb(210,220,235);
            this.SwitchHoverForeColor = Color.FromArgb(24,28,35);
        }
    }
}