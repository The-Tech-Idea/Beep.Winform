using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(236,240,243);
            this.ProgressBarForeColor = Color.FromArgb(58,66,86);
            this.ProgressBarBorderColor = Color.FromArgb(221,228,235);
            this.ProgressBarChunkColor = Color.FromArgb(236,240,243);
            this.ProgressBarErrorColor = Color.FromArgb(231,76,60);
            this.ProgressBarSuccessColor = Color.FromArgb(46,204,113);
            this.ProgressBarInsideTextColor = Color.FromArgb(58,66,86);
            this.ProgressBarHoverBackColor = Color.FromArgb(236,240,243);
            this.ProgressBarHoverForeColor = Color.FromArgb(58,66,86);
            this.ProgressBarHoverBorderColor = Color.FromArgb(221,228,235);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(58,66,86);
        }
    }
}