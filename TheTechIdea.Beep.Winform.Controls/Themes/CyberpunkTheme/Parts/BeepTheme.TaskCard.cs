using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(10,8,20);
            this.TaskCardForeColor = Color.FromArgb(228,244,255);
            this.TaskCardBorderColor = Color.FromArgb(90,20,110);
            this.TaskCardTitleForeColor = Color.FromArgb(228,244,255);
            this.TaskCardTitleBackColor = Color.FromArgb(10,8,20);
            this.TaskCardSubTitleForeColor = Color.FromArgb(228,244,255);
            this.TaskCardSubTitleBackColor = Color.FromArgb(10,8,20);
            this.TaskCardMetricTextForeColor = Color.FromArgb(228,244,255);
            this.TaskCardMetricTextBackColor = Color.FromArgb(10,8,20);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(90,20,110);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(228,244,255);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(10,8,20);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(90,20,110);
            this.TaskCardProgressValueForeColor = Color.FromArgb(228,244,255);
            this.TaskCardProgressValueBackColor = Color.FromArgb(10,8,20);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(90,20,110);
        }
    }
}