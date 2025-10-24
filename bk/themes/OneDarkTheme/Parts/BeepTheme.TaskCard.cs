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
            this.TaskCardBackColor = Color.FromArgb(40,44,52);
            this.TaskCardForeColor = Color.FromArgb(171,178,191);
            this.TaskCardBorderColor = Color.FromArgb(92,99,112);
            this.TaskCardTitleForeColor = Color.FromArgb(171,178,191);
            this.TaskCardTitleBackColor = Color.FromArgb(40,44,52);
            this.TaskCardSubTitleForeColor = Color.FromArgb(171,178,191);
            this.TaskCardSubTitleBackColor = Color.FromArgb(40,44,52);
            this.TaskCardMetricTextForeColor = Color.FromArgb(171,178,191);
            this.TaskCardMetricTextBackColor = Color.FromArgb(40,44,52);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(92,99,112);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(171,178,191);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(40,44,52);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(92,99,112);
            this.TaskCardProgressValueForeColor = Color.FromArgb(171,178,191);
            this.TaskCardProgressValueBackColor = Color.FromArgb(40,44,52);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(92,99,112);
        }
    }
}