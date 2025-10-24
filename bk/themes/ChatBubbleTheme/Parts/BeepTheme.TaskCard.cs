using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = Color.FromArgb(245,248,255);
            this.TaskCardForeColor = Color.FromArgb(24,28,35);
            this.TaskCardBorderColor = Color.FromArgb(210,220,235);
            this.TaskCardTitleForeColor = Color.FromArgb(24,28,35);
            this.TaskCardTitleBackColor = Color.FromArgb(245,248,255);
            this.TaskCardSubTitleForeColor = Color.FromArgb(24,28,35);
            this.TaskCardSubTitleBackColor = Color.FromArgb(245,248,255);
            this.TaskCardMetricTextForeColor = Color.FromArgb(24,28,35);
            this.TaskCardMetricTextBackColor = Color.FromArgb(245,248,255);
            this.TaskCardMetricTextBorderColor = Color.FromArgb(210,220,235);
            this.TaskCardMetricTextHoverForeColor = Color.FromArgb(24,28,35);
            this.TaskCardMetricTextHoverBackColor = Color.FromArgb(245,248,255);
            this.TaskCardMetricTextHoverBorderColor = Color.FromArgb(210,220,235);
            this.TaskCardProgressValueForeColor = Color.FromArgb(24,28,35);
            this.TaskCardProgressValueBackColor = Color.FromArgb(245,248,255);
            this.TaskCardProgressValueBorderColor = Color.FromArgb(210,220,235);
        }
    }
}