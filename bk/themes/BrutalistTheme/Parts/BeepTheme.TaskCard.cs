using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(250,250,250);
            this.TaskCardForeColor = Color.FromArgb(20,20,20);
            this.TaskCardBorderColor = Color.FromArgb(0,0,0);
            this.TaskCardTitleForeColor = Color.FromArgb(20,20,20);
            this.TaskCardTitleBackColor = Color.FromArgb(250,250,250);
            this.TaskCardSubTitleForeColor = Color.FromArgb(20,20,20);
            this.TaskCardSubTitleBackColor = Color.FromArgb(250,250,250);
            this.TaskCardMetricTextForeColor = Color.FromArgb(20,20,20);
            this.TaskCardMetricTextBackColor = Color.FromArgb(250,250,250);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(0,0,0);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(20,20,20);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(250,250,250);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(0,0,0);
            this.TaskCardProgressValueForeColor = Color.FromArgb(20,20,20);
            this.TaskCardProgressValueBackColor = Color.FromArgb(250,250,250);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(0,0,0);
        }
    }
}