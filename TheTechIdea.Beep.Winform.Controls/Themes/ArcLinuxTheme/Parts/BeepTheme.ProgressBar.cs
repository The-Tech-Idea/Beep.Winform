using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = SurfaceColor;
            this.ProgressBarForeColor = ForeColor;
            this.ProgressBarBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.ProgressBarChunkColor = SurfaceColor;
            this.ProgressBarErrorColor = ErrorColor;
            this.ProgressBarSuccessColor = SuccessColor;
            this.ProgressBarInsideTextColor = ForeColor;
            this.ProgressBarHoverBackColor = SurfaceColor;
            this.ProgressBarHoverForeColor = ForeColor;
            this.ProgressBarHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.ProgressBarHoverInsideTextColor = ForeColor;
        }
    }
}
