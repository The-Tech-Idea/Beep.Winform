using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(250,250,250);
            this.TaskCardForeColor = Color.FromArgb(33,33,33);
            this.TaskCardBorderColor = Color.FromArgb(224,224,224);
            this.TaskCardTitleForeColor = Color.FromArgb(33,33,33);
            this.TaskCardTitleBackColor = Color.FromArgb(250,250,250);
            this.TaskCardSubTitleForeColor = Color.FromArgb(33,33,33);
            this.TaskCardSubTitleBackColor = Color.FromArgb(250,250,250);
            this.TaskCardMetricTextForeColor = Color.FromArgb(33,33,33);
            this.TaskCardMetricTextBackColor = Color.FromArgb(250,250,250);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(224,224,224);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(33,33,33);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(250,250,250);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(224,224,224);
            this.TaskCardProgressValueForeColor = Color.FromArgb(33,33,33);
            this.TaskCardProgressValueBackColor = Color.FromArgb(250,250,250);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(224,224,224);
        }
    }
}