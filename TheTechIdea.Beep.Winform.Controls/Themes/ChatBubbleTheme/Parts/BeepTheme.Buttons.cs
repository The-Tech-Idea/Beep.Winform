using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyButtons()
        {
            this.ButtonHoverBackColor = Color.FromArgb(245,248,255);
            this.ButtonHoverForeColor = Color.FromArgb(24,28,35);
            this.ButtonHoverBorderColor = Color.FromArgb(210,220,235);
            this.ButtonSelectedBorderColor = Color.FromArgb(210,220,235);
            this.ButtonSelectedBackColor = Color.FromArgb(245,248,255);
            this.ButtonSelectedForeColor = Color.FromArgb(24,28,35);
            this.ButtonSelectedHoverBackColor = Color.FromArgb(245,248,255);
            this.ButtonSelectedHoverForeColor = Color.FromArgb(24,28,35);
            this.ButtonSelectedHoverBorderColor = Color.FromArgb(210,220,235);
            this.ButtonBackColor = Color.FromArgb(245,248,255);
            this.ButtonForeColor = Color.FromArgb(24,28,35);
            this.ButtonBorderColor = Color.FromArgb(210,220,235);
            this.ButtonErrorBackColor = Color.FromArgb(245,80,100);
            this.ButtonErrorForeColor = Color.FromArgb(24,28,35);
            this.ButtonErrorBorderColor = Color.FromArgb(210,220,235);
            this.ButtonPressedBackColor = Color.FromArgb(245,248,255);
            this.ButtonPressedForeColor = Color.FromArgb(24,28,35);
            this.ButtonPressedBorderColor = Color.FromArgb(210,220,235);
        }
    }
}