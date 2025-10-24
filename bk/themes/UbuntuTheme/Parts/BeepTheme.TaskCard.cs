using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(242,242,245);
            this.TaskCardForeColor = Color.FromArgb(44,44,44);
            this.TaskCardBorderColor = Color.FromArgb(218,218,222);
            this.TaskCardTitleForeColor = Color.FromArgb(44,44,44);
            this.TaskCardTitleBackColor = Color.FromArgb(242,242,245);
            this.TaskCardSubTitleForeColor = Color.FromArgb(44,44,44);
            this.TaskCardSubTitleBackColor = Color.FromArgb(242,242,245);
            this.TaskCardMetricTextForeColor = Color.FromArgb(44,44,44);
            this.TaskCardMetricTextBackColor = Color.FromArgb(242,242,245);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(218,218,222);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(44,44,44);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(242,242,245);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(218,218,222);
            this.TaskCardProgressValueForeColor = Color.FromArgb(44,44,44);
            this.TaskCardProgressValueBackColor = Color.FromArgb(242,242,245);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(218,218,222);
        }
    }
}