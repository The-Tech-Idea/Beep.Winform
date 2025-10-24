using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(40,42,54);
            this.ProgressBarForeColor = Color.FromArgb(248,248,242);
            this.ProgressBarBorderColor = Color.FromArgb(98,114,164);
            this.ProgressBarChunkColor = Color.FromArgb(40,42,54);
            this.ProgressBarErrorColor = Color.FromArgb(40,42,54);
            this.ProgressBarSuccessColor = Color.FromArgb(40,42,54);
            this.ProgressBarInsideTextColor = Color.FromArgb(248,248,242);
            this.ProgressBarHoverBackColor = Color.FromArgb(40,42,54);
            this.ProgressBarHoverForeColor = Color.FromArgb(248,248,242);
            this.ProgressBarHoverBorderColor = Color.FromArgb(98,114,164);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(248,248,242);
        }
    }
}