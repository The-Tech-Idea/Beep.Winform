using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(250,250,250);
            this.ProgressBarForeColor = Color.FromArgb(33,33,33);
            this.ProgressBarBorderColor = Color.FromArgb(224,224,224);
            this.ProgressBarChunkColor = Color.FromArgb(250,250,250);
            this.ProgressBarErrorColor = Color.FromArgb(250,250,250);
            this.ProgressBarSuccessColor = Color.FromArgb(250,250,250);
            this.ProgressBarInsideTextColor = Color.FromArgb(33,33,33);
            this.ProgressBarHoverBackColor = Color.FromArgb(250,250,250);
            this.ProgressBarHoverForeColor = Color.FromArgb(33,33,33);
            this.ProgressBarHoverBorderColor = Color.FromArgb(224,224,224);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(33,33,33);
        }
    }
}