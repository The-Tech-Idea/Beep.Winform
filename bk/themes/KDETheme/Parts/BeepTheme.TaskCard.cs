using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(248,249,250);
            this.TaskCardForeColor = Color.FromArgb(33,37,41);
            this.TaskCardBorderColor = Color.FromArgb(222,226,230);
            this.TaskCardTitleForeColor = Color.FromArgb(33,37,41);
            this.TaskCardTitleBackColor = Color.FromArgb(248,249,250);
            this.TaskCardSubTitleForeColor = Color.FromArgb(33,37,41);
            this.TaskCardSubTitleBackColor = Color.FromArgb(248,249,250);
            this.TaskCardMetricTextForeColor = Color.FromArgb(33,37,41);
            this.TaskCardMetricTextBackColor = Color.FromArgb(248,249,250);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(222,226,230);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(33,37,41);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(248,249,250);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(222,226,230);
            this.TaskCardProgressValueForeColor = Color.FromArgb(33,37,41);
            this.TaskCardProgressValueBackColor = Color.FromArgb(248,249,250);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(222,226,230);
        }
    }
}