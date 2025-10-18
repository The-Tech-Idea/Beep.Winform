using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(26,27,38);
            this.TaskCardForeColor = Color.FromArgb(192,202,245);
            this.TaskCardBorderColor = Color.FromArgb(86,95,137);
            this.TaskCardTitleForeColor = Color.FromArgb(192,202,245);
            this.TaskCardTitleBackColor = Color.FromArgb(26,27,38);
            this.TaskCardSubTitleForeColor = Color.FromArgb(192,202,245);
            this.TaskCardSubTitleBackColor = Color.FromArgb(26,27,38);
            this.TaskCardMetricTextForeColor = Color.FromArgb(192,202,245);
            this.TaskCardMetricTextBackColor = Color.FromArgb(26,27,38);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(86,95,137);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(192,202,245);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(26,27,38);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(86,95,137);
            this.TaskCardProgressValueForeColor = Color.FromArgb(192,202,245);
            this.TaskCardProgressValueBackColor = Color.FromArgb(26,27,38);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(86,95,137);
        }
    }
}