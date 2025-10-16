using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(245,248,255);
            this.ProgressBarForeColor = Color.FromArgb(24,28,35);
            this.ProgressBarBorderColor = Color.FromArgb(210,220,235);
            this.ProgressBarChunkColor = Color.FromArgb(245,248,255);
            this.ProgressBarErrorColor = Color.FromArgb(245,80,100);
            this.ProgressBarSuccessColor = Color.FromArgb(72,199,142);
            this.ProgressBarInsideTextColor = Color.FromArgb(24,28,35);
            this.ProgressBarHoverBackColor = Color.FromArgb(245,248,255);
            this.ProgressBarHoverForeColor = Color.FromArgb(24,28,35);
            this.ProgressBarHoverBorderColor = Color.FromArgb(210,220,235);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(24,28,35);
        }
    }
}