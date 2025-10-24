using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(248,249,250);
            this.ProgressBarForeColor = Color.FromArgb(33,37,41);
            this.ProgressBarBorderColor = Color.FromArgb(222,226,230);
            this.ProgressBarChunkColor = Color.FromArgb(248,249,250);
            this.ProgressBarErrorColor = Color.FromArgb(220,53,69);
            this.ProgressBarSuccessColor = Color.FromArgb(46,204,113);
            this.ProgressBarInsideTextColor = Color.FromArgb(33,37,41);
            this.ProgressBarHoverBackColor = Color.FromArgb(248,249,250);
            this.ProgressBarHoverForeColor = Color.FromArgb(33,37,41);
            this.ProgressBarHoverBorderColor = Color.FromArgb(222,226,230);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(33,37,41);
        }
    }
}