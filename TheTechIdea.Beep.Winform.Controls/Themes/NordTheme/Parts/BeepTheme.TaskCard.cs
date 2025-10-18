using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(46,52,64);
            this.TaskCardForeColor = Color.FromArgb(216,222,233);
            this.TaskCardBorderColor = Color.FromArgb(76,86,106);
            this.TaskCardTitleForeColor = Color.FromArgb(216,222,233);
            this.TaskCardTitleBackColor = Color.FromArgb(46,52,64);
            this.TaskCardSubTitleForeColor = Color.FromArgb(216,222,233);
            this.TaskCardSubTitleBackColor = Color.FromArgb(46,52,64);
            this.TaskCardMetricTextForeColor = Color.FromArgb(216,222,233);
            this.TaskCardMetricTextBackColor = Color.FromArgb(46,52,64);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(76,86,106);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(216,222,233);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(46,52,64);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(76,86,106);
            this.TaskCardProgressValueForeColor = Color.FromArgb(216,222,233);
            this.TaskCardProgressValueBackColor = Color.FromArgb(46,52,64);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(76,86,106);
        }
    }
}