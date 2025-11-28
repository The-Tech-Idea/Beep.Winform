using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = BackgroundColor;
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
            this.TaskCardMetricTextHoverBackColor = PanelGradiantMiddleColor;
            this.TaskCardMetricTextHoverBorderColor = BorderColor;
            this.TaskCardProgressValueForeColor = ForeColor;
            this.TaskCardProgressValueBackColor = BackgroundColor;
            this.TaskCardProgressValueBorderColor = BorderColor;
        }
    }
}