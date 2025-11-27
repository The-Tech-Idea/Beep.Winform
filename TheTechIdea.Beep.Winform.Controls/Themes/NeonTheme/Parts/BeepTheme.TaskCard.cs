using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = PanelGradiantMiddleColor;
            this.TaskCardForeColor = ForeColor;
            this.TaskCardBorderColor = InactiveBorderColor;
            this.TaskCardTitleForeColor = ForeColor;
            this.TaskCardTitleBackColor = PanelGradiantMiddleColor;
            this.TaskCardSubTitleForeColor = ForeColor;
            this.TaskCardSubTitleBackColor = PanelGradiantMiddleColor;
            this.TaskCardMetricTextForeColor = ForeColor;
            this.TaskCardMetricTextBackColor = PanelGradiantMiddleColor;
            this.TaskCardMetricTextBorderColor = InactiveBorderColor;
            this.TaskCardMetricTextHoverForeColor = ForeColor;
            this.TaskCardMetricTextHoverBackColor = PanelGradiantMiddleColor;
            this.TaskCardMetricTextHoverBorderColor = InactiveBorderColor;
            this.TaskCardProgressValueForeColor = ForeColor;
            this.TaskCardProgressValueBackColor = PanelGradiantMiddleColor;
            this.TaskCardProgressValueBorderColor = InactiveBorderColor;
        }
    }
}