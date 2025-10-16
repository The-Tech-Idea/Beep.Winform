using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(255,251,235);
            this.TaskCardForeColor = Color.FromArgb(33,37,41);
            this.TaskCardBorderColor = Color.FromArgb(247,208,136);
            this.TaskCardTitleForeColor = Color.FromArgb(33,37,41);
            this.TaskCardTitleBackColor = Color.FromArgb(255,251,235);
            this.TaskCardSubTitleForeColor = Color.FromArgb(33,37,41);
            this.TaskCardSubTitleBackColor = Color.FromArgb(255,251,235);
            this.TaskCardMetricTextForeColor = Color.FromArgb(33,37,41);
            this.TaskCardMetricTextBackColor = Color.FromArgb(255,251,235);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(247,208,136);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(33,37,41);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(255,251,235);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(247,208,136);
            this.TaskCardProgressValueForeColor = Color.FromArgb(33,37,41);
            this.TaskCardProgressValueBackColor = Color.FromArgb(255,251,235);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(247,208,136);
        }
    }
}