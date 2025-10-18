using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(242,242,247);
            this.ProgressBarForeColor = Color.FromArgb(28,28,30);
            this.ProgressBarBorderColor = Color.FromArgb(198,198,207);
            this.ProgressBarChunkColor = Color.FromArgb(242,242,247);
            this.ProgressBarErrorColor = Color.FromArgb(242,242,247);
            this.ProgressBarSuccessColor = Color.FromArgb(242,242,247);
            this.ProgressBarInsideTextColor = Color.FromArgb(28,28,30);
            this.ProgressBarHoverBackColor = Color.FromArgb(242,242,247);
            this.ProgressBarHoverForeColor = Color.FromArgb(28,28,30);
            this.ProgressBarHoverBorderColor = Color.FromArgb(198,198,207);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(28,28,30);
        }
    }
}