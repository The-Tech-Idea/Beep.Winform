using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(242,242,247);
            this.TaskCardForeColor = Color.FromArgb(28,28,30);
            this.TaskCardBorderColor = Color.FromArgb(198,198,207);
            this.TaskCardTitleForeColor = Color.FromArgb(28,28,30);
            this.TaskCardTitleBackColor = Color.FromArgb(242,242,247);
            this.TaskCardSubTitleForeColor = Color.FromArgb(28,28,30);
            this.TaskCardSubTitleBackColor = Color.FromArgb(242,242,247);
            this.TaskCardMetricTextForeColor = Color.FromArgb(28,28,30);
            this.TaskCardMetricTextBackColor = Color.FromArgb(242,242,247);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(198,198,207);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(28,28,30);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(242,242,247);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(198,198,207);
            this.TaskCardProgressValueForeColor = Color.FromArgb(28,28,30);
            this.TaskCardProgressValueBackColor = Color.FromArgb(242,242,247);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(198,198,207);
        }
    }
}