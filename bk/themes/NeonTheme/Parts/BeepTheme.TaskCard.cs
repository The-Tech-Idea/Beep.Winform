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
            this.TaskCardBackColor = Color.FromArgb(10,12,20);
            this.TaskCardForeColor = Color.FromArgb(235,245,255);
            this.TaskCardBorderColor = Color.FromArgb(60,70,100);
            this.TaskCardTitleForeColor = Color.FromArgb(235,245,255);
            this.TaskCardTitleBackColor = Color.FromArgb(10,12,20);
            this.TaskCardSubTitleForeColor = Color.FromArgb(235,245,255);
            this.TaskCardSubTitleBackColor = Color.FromArgb(10,12,20);
            this.TaskCardMetricTextForeColor = Color.FromArgb(235,245,255);
            this.TaskCardMetricTextBackColor = Color.FromArgb(10,12,20);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(60,70,100);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(235,245,255);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(10,12,20);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(60,70,100);
            this.TaskCardProgressValueForeColor = Color.FromArgb(235,245,255);
            this.TaskCardProgressValueBackColor = Color.FromArgb(10,12,20);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(60,70,100);
        }
    }
}