using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(245,246,247);
            this.TaskCardForeColor = Color.FromArgb(43,45,48);
            this.TaskCardBorderColor = Color.FromArgb(220,223,230);
            this.TaskCardTitleForeColor = Color.FromArgb(43,45,48);
            this.TaskCardTitleBackColor = Color.FromArgb(245,246,247);
            this.TaskCardSubTitleForeColor = Color.FromArgb(43,45,48);
            this.TaskCardSubTitleBackColor = Color.FromArgb(245,246,247);
            this.TaskCardMetricTextForeColor = Color.FromArgb(43,45,48);
            this.TaskCardMetricTextBackColor = Color.FromArgb(245,246,247);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(220,223,230);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(43,45,48);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(245,246,247);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(220,223,230);
            this.TaskCardProgressValueForeColor = Color.FromArgb(43,45,48);
            this.TaskCardProgressValueBackColor = Color.FromArgb(245,246,247);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(220,223,230);
        }
    }
}