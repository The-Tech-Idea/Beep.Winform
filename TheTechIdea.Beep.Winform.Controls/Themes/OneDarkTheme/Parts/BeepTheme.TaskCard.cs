using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = PanelBackColor;
            this.TaskCardForeColor = ForeColor;
            this.TaskCardBorderColor = BorderColor;
            this.TaskCardTitleForeColor = ForeColor;
            this.TaskCardTitleBackColor = PanelBackColor;
            this.TaskCardSubTitleForeColor = ForeColor;
            this.TaskCardSubTitleBackColor = PanelBackColor;
            this.TaskCardMetricTextForeColor = ForeColor;
            this.TaskCardMetricTextBackColor = PanelBackColor;
            this.TaskCardMetricTextBorderColor = BorderColor;
            this.TaskCardMetricTextHoverForeColor = ForeColor;
            this.TaskCardMetricTextHoverBackColor = PanelGradiantMiddleColor;
            this.TaskCardMetricTextHoverBorderColor = ActiveBorderColor;
            this.TaskCardProgressValueForeColor = ForeColor;
            this.TaskCardProgressValueBackColor = PanelBackColor;
            this.TaskCardProgressValueBorderColor = BorderColor;
        }
    }
}