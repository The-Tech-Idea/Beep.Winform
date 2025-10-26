using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = BackgroundColor;
            this.ProgressBarForeColor = ForeColor;
            this.ProgressBarBorderColor = BorderColor;
            this.ProgressBarChunkColor = PrimaryColor;
            this.ProgressBarErrorColor = ErrorColor;
            this.ProgressBarSuccessColor = SuccessColor;
            this.ProgressBarInsideTextColor = OnPrimaryColor;
            this.ProgressBarHoverBackColor = SurfaceColor;
            this.ProgressBarHoverForeColor = ForeColor;
            this.ProgressBarHoverBorderColor = ActiveBorderColor;
            this.ProgressBarHoverInsideTextColor = OnPrimaryColor;
        }
    }
}
