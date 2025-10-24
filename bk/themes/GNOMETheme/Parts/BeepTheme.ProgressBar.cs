using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(246,245,244);
            this.ProgressBarForeColor = Color.FromArgb(46,52,54);
            this.ProgressBarBorderColor = Color.FromArgb(205,207,212);
            this.ProgressBarChunkColor = Color.FromArgb(246,245,244);
            this.ProgressBarErrorColor = Color.FromArgb(224,27,36);
            this.ProgressBarSuccessColor = Color.FromArgb(51,209,122);
            this.ProgressBarInsideTextColor = Color.FromArgb(46,52,54);
            this.ProgressBarHoverBackColor = Color.FromArgb(246,245,244);
            this.ProgressBarHoverForeColor = Color.FromArgb(46,52,54);
            this.ProgressBarHoverBorderColor = Color.FromArgb(205,207,212);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(46,52,54);
        }
    }
}