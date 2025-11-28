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
            this.ProgressBarBackColor = PanelBackColor;
            this.ProgressBarForeColor = ForeColor;
            this.ProgressBarBorderColor = BorderColor;
            this.ProgressBarChunkColor = PanelBackColor;
            this.ProgressBarErrorColor = ErrorColor;
            this.ProgressBarSuccessColor = SuccessColor;
            this.ProgressBarInsideTextColor = ForeColor;
            this.ProgressBarHoverBackColor = PanelBackColor;
            this.ProgressBarHoverForeColor = ForeColor;
            this.ProgressBarHoverBorderColor = BorderColor;
            this.ProgressBarHoverInsideTextColor = ForeColor;
        }
    }
}