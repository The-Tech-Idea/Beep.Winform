using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = SurfaceColor;
            this.TaskCardForeColor = ForeColor;
            this.TaskCardBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.TaskCardTitleForeColor = ForeColor;
            this.TaskCardTitleBackColor = SurfaceColor;
            this.TaskCardSubTitleForeColor = ForeColor;
            this.TaskCardSubTitleBackColor = SurfaceColor;
            this.TaskCardMetricTextForeColor = ForeColor;
            this.TaskCardMetricTextBackColor = SurfaceColor;
            this.TaskCardMetricTextBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.TaskCardMetricTextHoverForeColor = ForeColor;
            this.TaskCardMetricTextHoverBackColor = SurfaceColor;
            this.TaskCardMetricTextHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.TaskCardProgressValueForeColor = ForeColor;
            this.TaskCardProgressValueBackColor = SurfaceColor;
            this.TaskCardProgressValueBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
        }
    }
}
