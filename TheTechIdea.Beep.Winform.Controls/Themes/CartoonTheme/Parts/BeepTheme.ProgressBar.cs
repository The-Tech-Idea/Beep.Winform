using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(255,251,235);
            this.ProgressBarForeColor = Color.FromArgb(33,37,41);
            this.ProgressBarBorderColor = Color.FromArgb(247,208,136);
            this.ProgressBarChunkColor = Color.FromArgb(255,251,235);
            this.ProgressBarErrorColor = Color.FromArgb(255,82,82);
            this.ProgressBarSuccessColor = Color.FromArgb(72,199,142);
            this.ProgressBarInsideTextColor = Color.FromArgb(33,37,41);
            this.ProgressBarHoverBackColor = Color.FromArgb(255,251,235);
            this.ProgressBarHoverForeColor = Color.FromArgb(33,37,41);
            this.ProgressBarHoverBorderColor = Color.FromArgb(247,208,136);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(33,37,41);
        }
    }
}