using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(236,244,255);
            this.TaskCardForeColor = Color.FromArgb(17,24,39);
            this.TaskCardBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.TaskCardTitleForeColor = Color.FromArgb(17,24,39);
            this.TaskCardTitleBackColor = Color.FromArgb(236,244,255);
            this.TaskCardSubTitleForeColor = Color.FromArgb(17,24,39);
            this.TaskCardSubTitleBackColor = Color.FromArgb(236,244,255);
            this.TaskCardMetricTextForeColor = Color.FromArgb(17,24,39);
            this.TaskCardMetricTextBackColor = Color.FromArgb(236,244,255);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(17,24,39);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(236,244,255);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.TaskCardProgressValueForeColor = Color.FromArgb(17,24,39);
            this.TaskCardProgressValueBackColor = Color.FromArgb(236,244,255);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(140, 255, 255, 255);
        }
    }
}