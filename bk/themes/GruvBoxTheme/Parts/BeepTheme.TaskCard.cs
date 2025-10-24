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
            this.TaskCardBackColor = Color.FromArgb(40,40,40);
            this.TaskCardForeColor = Color.FromArgb(235,219,178);
            this.TaskCardBorderColor = Color.FromArgb(168,153,132);
            this.TaskCardTitleForeColor = Color.FromArgb(235,219,178);
            this.TaskCardTitleBackColor = Color.FromArgb(40,40,40);
            this.TaskCardSubTitleForeColor = Color.FromArgb(235,219,178);
            this.TaskCardSubTitleBackColor = Color.FromArgb(40,40,40);
            this.TaskCardMetricTextForeColor = Color.FromArgb(235,219,178);
            this.TaskCardMetricTextBackColor = Color.FromArgb(40,40,40);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(168,153,132);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(235,219,178);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(40,40,40);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(168,153,132);
            this.TaskCardProgressValueForeColor = Color.FromArgb(235,219,178);
            this.TaskCardProgressValueBackColor = Color.FromArgb(40,40,40);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(168,153,132);
        }
    }
}