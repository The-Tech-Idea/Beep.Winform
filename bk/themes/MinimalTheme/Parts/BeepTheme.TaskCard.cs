using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(255,255,255);
            this.TaskCardForeColor = Color.FromArgb(31,41,55);
            this.TaskCardBorderColor = Color.FromArgb(209,213,219);
            this.TaskCardTitleForeColor = Color.FromArgb(31,41,55);
            this.TaskCardTitleBackColor = Color.FromArgb(255,255,255);
            this.TaskCardSubTitleForeColor = Color.FromArgb(31,41,55);
            this.TaskCardSubTitleBackColor = Color.FromArgb(255,255,255);
            this.TaskCardMetricTextForeColor = Color.FromArgb(31,41,55);
            this.TaskCardMetricTextBackColor = Color.FromArgb(255,255,255);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(209,213,219);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(31,41,55);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(255,255,255);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(209,213,219);
            this.TaskCardProgressValueForeColor = Color.FromArgb(31,41,55);
            this.TaskCardProgressValueBackColor = Color.FromArgb(255,255,255);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(209,213,219);
        }
    }
}