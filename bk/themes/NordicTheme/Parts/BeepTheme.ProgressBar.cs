using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(250,250,251);
            this.ProgressBarForeColor = Color.FromArgb(31,41,55);
            this.ProgressBarBorderColor = Color.FromArgb(229,231,235);
            this.ProgressBarChunkColor = Color.FromArgb(250,250,251);
            this.ProgressBarErrorColor = Color.FromArgb(220,38,38);
            this.ProgressBarSuccessColor = Color.FromArgb(16,157,108);
            this.ProgressBarInsideTextColor = Color.FromArgb(31,41,55);
            this.ProgressBarHoverBackColor = Color.FromArgb(250,250,251);
            this.ProgressBarHoverForeColor = Color.FromArgb(31,41,55);
            this.ProgressBarHoverBorderColor = Color.FromArgb(229,231,235);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(31,41,55);
        }
    }
}