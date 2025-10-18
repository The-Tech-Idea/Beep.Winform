using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(0,43,54);
            this.ProgressBarForeColor = Color.FromArgb(147,161,161);
            this.ProgressBarBorderColor = Color.FromArgb(88,110,117);
            this.ProgressBarChunkColor = Color.FromArgb(0,43,54);
            this.ProgressBarErrorColor = Color.FromArgb(0,43,54);
            this.ProgressBarSuccessColor = Color.FromArgb(0,43,54);
            this.ProgressBarInsideTextColor = Color.FromArgb(147,161,161);
            this.ProgressBarHoverBackColor = Color.FromArgb(0,43,54);
            this.ProgressBarHoverForeColor = Color.FromArgb(147,161,161);
            this.ProgressBarHoverBorderColor = Color.FromArgb(88,110,117);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(147,161,161);
        }
    }
}