using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(15,16,32);
            this.TaskCardForeColor = Color.FromArgb(245,247,255);
            this.TaskCardBorderColor = Color.FromArgb(74,79,123);
            this.TaskCardTitleForeColor = Color.FromArgb(245,247,255);
            this.TaskCardTitleBackColor = Color.FromArgb(15,16,32);
            this.TaskCardSubTitleForeColor = Color.FromArgb(245,247,255);
            this.TaskCardSubTitleBackColor = Color.FromArgb(15,16,32);
            this.TaskCardMetricTextForeColor = Color.FromArgb(245,247,255);
            this.TaskCardMetricTextBackColor = Color.FromArgb(15,16,32);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(74,79,123);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(245,247,255);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(15,16,32);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(74,79,123);
            this.TaskCardProgressValueForeColor = Color.FromArgb(245,247,255);
            this.TaskCardProgressValueBackColor = Color.FromArgb(15,16,32);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(74,79,123);
        }
    }
}