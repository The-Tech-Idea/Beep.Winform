using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(40,42,54);
            this.TaskCardForeColor = Color.FromArgb(248,248,242);
            this.TaskCardBorderColor = Color.FromArgb(98,114,164);
            this.TaskCardTitleForeColor = Color.FromArgb(248,248,242);
            this.TaskCardTitleBackColor = Color.FromArgb(40,42,54);
            this.TaskCardSubTitleForeColor = Color.FromArgb(248,248,242);
            this.TaskCardSubTitleBackColor = Color.FromArgb(40,42,54);
            this.TaskCardMetricTextForeColor = Color.FromArgb(248,248,242);
            this.TaskCardMetricTextBackColor = Color.FromArgb(40,42,54);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(98,114,164);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(248,248,242);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(40,42,54);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(98,114,164);
            this.TaskCardProgressValueForeColor = Color.FromArgb(248,248,242);
            this.TaskCardProgressValueBackColor = Color.FromArgb(40,42,54);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(98,114,164);
        }
    }
}