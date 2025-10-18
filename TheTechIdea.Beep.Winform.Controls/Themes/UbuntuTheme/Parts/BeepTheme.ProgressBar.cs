using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = Color.FromArgb(242,242,245);
            this.ProgressBarForeColor = Color.FromArgb(44,44,44);
            this.ProgressBarBorderColor = Color.FromArgb(218,218,222);
            this.ProgressBarChunkColor = Color.FromArgb(242,242,245);
            this.ProgressBarErrorColor = Color.FromArgb(192,28,40);
            this.ProgressBarSuccessColor = Color.FromArgb(42,168,118);
            this.ProgressBarInsideTextColor = Color.FromArgb(44,44,44);
            this.ProgressBarHoverBackColor = Color.FromArgb(242,242,245);
            this.ProgressBarHoverForeColor = Color.FromArgb(44,44,44);
            this.ProgressBarHoverBorderColor = Color.FromArgb(218,218,222);
            this.ProgressBarHoverInsideTextColor = Color.FromArgb(44,44,44);
        }
    }
}