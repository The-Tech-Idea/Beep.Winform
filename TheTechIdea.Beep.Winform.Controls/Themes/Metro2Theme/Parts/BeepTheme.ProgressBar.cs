using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyProgressBar()
        {
            this.ProgressBarBackColor = PanelBackColor;
            this.ProgressBarForeColor = ForeColor;
            this.ProgressBarBorderColor = InactiveBorderColor;
            this.ProgressBarChunkColor = PrimaryColor;
            this.ProgressBarErrorColor = ErrorColor;
            this.ProgressBarSuccessColor = SuccessColor;
            this.ProgressBarInsideTextColor = ForeColor;
            this.ProgressBarHoverBackColor = PanelGradiantMiddleColor;
            this.ProgressBarHoverForeColor = ForeColor;
            this.ProgressBarHoverBorderColor = InactiveBorderColor;
            this.ProgressBarHoverInsideTextColor = ForeColor;
        }
    }
}