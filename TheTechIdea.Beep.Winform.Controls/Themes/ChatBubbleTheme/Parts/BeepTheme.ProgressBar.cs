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
            this.ProgressBarBackColor = BackgroundColor;
            this.ProgressBarForeColor = ForeColor;
            this.ProgressBarBorderColor = BorderColor;
            this.ProgressBarChunkColor = BackgroundColor;
            this.ProgressBarErrorColor = ErrorColor;
            this.ProgressBarSuccessColor = SuccessColor;
            this.ProgressBarInsideTextColor = ForeColor;
            this.ProgressBarHoverBackColor = BackgroundColor;
            this.ProgressBarHoverForeColor = ForeColor;
            this.ProgressBarHoverBorderColor = BorderColor;
            this.ProgressBarHoverInsideTextColor = ForeColor;
        }
    }
}