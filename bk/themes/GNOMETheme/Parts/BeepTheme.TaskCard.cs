using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(246,245,244);
            this.TaskCardForeColor = Color.FromArgb(46,52,54);
            this.TaskCardBorderColor = Color.FromArgb(205,207,212);
            this.TaskCardTitleForeColor = Color.FromArgb(46,52,54);
            this.TaskCardTitleBackColor = Color.FromArgb(246,245,244);
            this.TaskCardSubTitleForeColor = Color.FromArgb(46,52,54);
            this.TaskCardSubTitleBackColor = Color.FromArgb(246,245,244);
            this.TaskCardMetricTextForeColor = Color.FromArgb(46,52,54);
            this.TaskCardMetricTextBackColor = Color.FromArgb(246,245,244);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(205,207,212);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(46,52,54);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(246,245,244);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(205,207,212);
            this.TaskCardProgressValueForeColor = Color.FromArgb(46,52,54);
            this.TaskCardProgressValueBackColor = Color.FromArgb(246,245,244);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(205,207,212);
        }
    }
}