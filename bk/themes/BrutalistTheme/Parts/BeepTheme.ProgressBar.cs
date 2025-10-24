using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(250,250,250);
            this.ProgressBarForeColor = Color.FromArgb(20,20,20);
            this.ProgressBarBorderColor = Color.FromArgb(0,0,0);
            this.ProgressBarChunkColor = Color.FromArgb(250,250,250);
            this.ProgressBarErrorColor = Color.FromArgb(220,0,0);
            this.ProgressBarSuccessColor = Color.FromArgb(0,200,70);
            this.ProgressBarInsideTextColor = Color.FromArgb(20,20,20);
            this.ProgressBarHoverBackColor = Color.FromArgb(250,250,250);
            this.ProgressBarHoverForeColor = Color.FromArgb(20,20,20);
            this.ProgressBarHoverBorderColor = Color.FromArgb(0,0,0);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(20,20,20);
        }
    }
}