using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(245,246,248);
            this.TaskCardForeColor = Color.FromArgb(32,32,32);
            this.TaskCardBorderColor = Color.FromArgb(218,223,230);
            this.TaskCardTitleForeColor = Color.FromArgb(32,32,32);
            this.TaskCardTitleBackColor = Color.FromArgb(245,246,248);
            this.TaskCardSubTitleForeColor = Color.FromArgb(32,32,32);
            this.TaskCardSubTitleBackColor = Color.FromArgb(245,246,248);
            this.TaskCardMetricTextForeColor = Color.FromArgb(32,32,32);
            this.TaskCardMetricTextBackColor = Color.FromArgb(245,246,248);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(218,223,230);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(32,32,32);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(245,246,248);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(218,223,230);
            this.TaskCardProgressValueForeColor = Color.FromArgb(32,32,32);
            this.TaskCardProgressValueBackColor = Color.FromArgb(245,246,248);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(218,223,230);
        }
    }
}