using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(0,43,54);
            this.TaskCardForeColor = Color.FromArgb(147,161,161);
            this.TaskCardBorderColor = Color.FromArgb(88,110,117);
            this.TaskCardTitleForeColor = Color.FromArgb(147,161,161);
            this.TaskCardTitleBackColor = Color.FromArgb(0,43,54);
            this.TaskCardSubTitleForeColor = Color.FromArgb(147,161,161);
            this.TaskCardSubTitleBackColor = Color.FromArgb(0,43,54);
            this.TaskCardMetricTextForeColor = Color.FromArgb(147,161,161);
            this.TaskCardMetricTextBackColor = Color.FromArgb(0,43,54);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(88,110,117);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(147,161,161);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(0,43,54);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(88,110,117);
            this.TaskCardProgressValueForeColor = Color.FromArgb(147,161,161);
            this.TaskCardProgressValueBackColor = Color.FromArgb(0,43,54);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(88,110,117);
        }
    }
}