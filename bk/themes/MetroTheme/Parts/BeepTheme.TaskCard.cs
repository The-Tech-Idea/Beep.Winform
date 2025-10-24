using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MetroTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(243,242,241);
            this.TaskCardForeColor = Color.FromArgb(32,31,30);
            this.TaskCardBorderColor = Color.FromArgb(225,225,225);
            this.TaskCardTitleForeColor = Color.FromArgb(32,31,30);
            this.TaskCardTitleBackColor = Color.FromArgb(243,242,241);
            this.TaskCardSubTitleForeColor = Color.FromArgb(32,31,30);
            this.TaskCardSubTitleBackColor = Color.FromArgb(243,242,241);
            this.TaskCardMetricTextForeColor = Color.FromArgb(32,31,30);
            this.TaskCardMetricTextBackColor = Color.FromArgb(243,242,241);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(225,225,225);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(32,31,30);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(243,242,241);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(225,225,225);
            this.TaskCardProgressValueForeColor = Color.FromArgb(32,31,30);
            this.TaskCardProgressValueBackColor = Color.FromArgb(243,242,241);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(225,225,225);
        }
    }
}