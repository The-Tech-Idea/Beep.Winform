using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(250,250,251);
            this.TaskCardForeColor = Color.FromArgb(31,41,55);
            this.TaskCardBorderColor = Color.FromArgb(229,231,235);
            this.TaskCardTitleForeColor = Color.FromArgb(31,41,55);
            this.TaskCardTitleBackColor = Color.FromArgb(250,250,251);
            this.TaskCardSubTitleForeColor = Color.FromArgb(31,41,55);
            this.TaskCardSubTitleBackColor = Color.FromArgb(250,250,251);
            this.TaskCardMetricTextForeColor = Color.FromArgb(31,41,55);
            this.TaskCardMetricTextBackColor = Color.FromArgb(250,250,251);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(229,231,235);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(31,41,55);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(250,250,251);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(229,231,235);
            this.TaskCardProgressValueForeColor = Color.FromArgb(31,41,55);
            this.TaskCardProgressValueBackColor = Color.FromArgb(250,250,251);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(229,231,235);
        }
    }
}