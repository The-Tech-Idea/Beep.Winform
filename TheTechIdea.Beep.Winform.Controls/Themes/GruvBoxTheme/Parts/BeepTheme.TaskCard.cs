using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = PanelBackColor;
            this.TaskCardForeColor = ForeColor;
            this.TaskCardBorderColor = InactiveBorderColor;
            this.TaskCardTitleForeColor = ForeColor;
            this.TaskCardTitleBackColor = PanelBackColor;
            this.TaskCardSubTitleForeColor = ForeColor;
            this.TaskCardSubTitleBackColor = PanelBackColor;
            this.TaskCardMetricTextForeColor = ForeColor;
            this.TaskCardMetricTextBackColor = PanelBackColor;
            this.TaskCardMetricTextBorderColor = InactiveBorderColor;
            this.TaskCardMetricTextHoverForeColor = ForeColor;
            this.TaskCardMetricTextHoverBackColor = PanelBackColor;
            this.TaskCardMetricTextHoverBorderColor = InactiveBorderColor;
            this.TaskCardProgressValueForeColor = ForeColor;
            this.TaskCardProgressValueBackColor = PanelBackColor;
            this.TaskCardProgressValueBorderColor = InactiveBorderColor;
        }
    }
}