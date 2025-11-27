using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = SurfaceColor;
            this.TaskCardForeColor = ForeColor;
            this.TaskCardBorderColor = BorderColor;
            this.TaskCardTitleForeColor = ForeColor;
            this.TaskCardTitleBackColor = BackgroundColor;
            this.TaskCardSubTitleForeColor = ForeColor;
            this.TaskCardSubTitleBackColor = BackgroundColor;
            this.TaskCardMetricTextForeColor = ForeColor;
            this.TaskCardMetricTextBackColor = BackgroundColor;
            this.TaskCardMetricTextBorderColor = BorderColor;
            this.TaskCardMetricTextHoverForeColor = ForeColor;
            this.TaskCardMetricTextHoverBackColor = BackgroundColor;
            this.TaskCardMetricTextHoverBorderColor = BorderColor;
            this.TaskCardProgressValueForeColor = ForeColor;
            this.TaskCardProgressValueBackColor = BackgroundColor;
            this.TaskCardProgressValueBorderColor = BorderColor;
        }
    }
}