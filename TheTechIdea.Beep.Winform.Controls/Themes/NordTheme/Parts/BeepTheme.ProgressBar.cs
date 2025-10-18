using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(46,52,64);
            this.ProgressBarForeColor = Color.FromArgb(216,222,233);
            this.ProgressBarBorderColor = Color.FromArgb(76,86,106);
            this.ProgressBarChunkColor = Color.FromArgb(46,52,64);
            this.ProgressBarErrorColor = Color.FromArgb(46,52,64);
            this.ProgressBarSuccessColor = Color.FromArgb(46,52,64);
            this.ProgressBarInsideTextColor = Color.FromArgb(216,222,233);
            this.ProgressBarHoverBackColor = Color.FromArgb(46,52,64);
            this.ProgressBarHoverForeColor = Color.FromArgb(216,222,233);
            this.ProgressBarHoverBorderColor = Color.FromArgb(76,86,106);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(216,222,233);
        }
    }
}