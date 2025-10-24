using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(26,27,38);
            this.ProgressBarForeColor = Color.FromArgb(192,202,245);
            this.ProgressBarBorderColor = Color.FromArgb(86,95,137);
            this.ProgressBarChunkColor = Color.FromArgb(26,27,38);
            this.ProgressBarErrorColor = Color.FromArgb(26,27,38);
            this.ProgressBarSuccessColor = Color.FromArgb(26,27,38);
            this.ProgressBarInsideTextColor = Color.FromArgb(192,202,245);
            this.ProgressBarHoverBackColor = Color.FromArgb(26,27,38);
            this.ProgressBarHoverForeColor = Color.FromArgb(192,202,245);
            this.ProgressBarHoverBorderColor = Color.FromArgb(86,95,137);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(192,202,245);
        }
    }
}