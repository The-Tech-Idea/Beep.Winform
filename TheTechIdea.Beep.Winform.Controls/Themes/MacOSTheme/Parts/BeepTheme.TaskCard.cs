using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(250,250,252);
            this.TaskCardForeColor = Color.FromArgb(28,28,30);
            this.TaskCardBorderColor = Color.FromArgb(229,229,234);
            this.TaskCardTitleForeColor = Color.FromArgb(28,28,30);
            this.TaskCardTitleBackColor = Color.FromArgb(250,250,252);
            this.TaskCardSubTitleForeColor = Color.FromArgb(28,28,30);
            this.TaskCardSubTitleBackColor = Color.FromArgb(250,250,252);
            this.TaskCardMetricTextForeColor = Color.FromArgb(28,28,30);
            this.TaskCardMetricTextBackColor = Color.FromArgb(250,250,252);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(229,229,234);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(28,28,30);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(250,250,252);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(229,229,234);
            this.TaskCardProgressValueForeColor = Color.FromArgb(28,28,30);
            this.TaskCardProgressValueBackColor = Color.FromArgb(250,250,252);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(229,229,234);
        }
    }
}