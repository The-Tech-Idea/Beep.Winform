using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = PanelBackColor;
            this.TaskCardForeColor = ForeColor;
            this.TaskCardBorderColor = SecondaryColor;
            this.TaskCardTitleForeColor = ForeColor;
            this.TaskCardTitleBackColor = BackgroundColor;
            this.TaskCardSubTitleForeColor = ForeColor;
            this.TaskCardSubTitleBackColor = BackgroundColor;
            this.TaskCardMetricTextForeColor = ForeColor;
            this.TaskCardMetricTextBackColor = BackgroundColor;
            this.TaskCardMetricTextBorderColor = SecondaryColor;
            this.TaskCardMetricTextHoverForeColor = ForeColor;
            this.TaskCardMetricTextHoverBackColor = PanelBackColor;
            this.TaskCardMetricTextHoverBorderColor = SecondaryColor;
            this.TaskCardProgressValueForeColor = ForeColor;
            this.TaskCardProgressValueBackColor = BackgroundColor;
            this.TaskCardProgressValueBorderColor = SecondaryColor;
        }
    }
}