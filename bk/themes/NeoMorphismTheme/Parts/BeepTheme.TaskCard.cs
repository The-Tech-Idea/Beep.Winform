using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(236,240,243);
            this.TaskCardForeColor = Color.FromArgb(58,66,86);
            this.TaskCardBorderColor = Color.FromArgb(221,228,235);
            this.TaskCardTitleForeColor = Color.FromArgb(58,66,86);
            this.TaskCardTitleBackColor = Color.FromArgb(236,240,243);
            this.TaskCardSubTitleForeColor = Color.FromArgb(58,66,86);
            this.TaskCardSubTitleBackColor = Color.FromArgb(236,240,243);
            this.TaskCardMetricTextForeColor = Color.FromArgb(58,66,86);
            this.TaskCardMetricTextBackColor = Color.FromArgb(236,240,243);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(221,228,235);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(58,66,86);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(236,240,243);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(221,228,235);
            this.TaskCardProgressValueForeColor = Color.FromArgb(58,66,86);
            this.TaskCardProgressValueBackColor = Color.FromArgb(236,240,243);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(221,228,235);
        }
    }
}