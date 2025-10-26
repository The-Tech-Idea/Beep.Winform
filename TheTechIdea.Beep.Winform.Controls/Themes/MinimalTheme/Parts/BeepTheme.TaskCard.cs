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
            this.TaskCardBackColor = SurfaceColor;
            this.TaskCardForeColor = ForeColor;
            this.TaskCardBorderColor = BorderColor;
            this.TaskCardTitleForeColor = ForeColor;
            this.TaskCardTitleBackColor = SurfaceColor;
            this.TaskCardSubTitleForeColor = SecondaryColor;
            this.TaskCardSubTitleBackColor = SurfaceColor;
            this.TaskCardMetricTextForeColor = ForeColor;
            this.TaskCardMetricTextBackColor = ThemeUtil.Lighten(SurfaceColor, 0.03);
            this.TaskCardMetricTextBorderColor = ActiveBorderColor;
            this.TaskCardMetricTextHoverForeColor = ForeColor;
            this.TaskCardMetricTextHoverBackColor = ThemeUtil.Lighten(SurfaceColor, 0.05);
            this.TaskCardMetricTextHoverBorderColor = ActiveBorderColor;
            this.TaskCardProgressValueForeColor = OnPrimaryColor;
            this.TaskCardProgressValueBackColor = PrimaryColor;
            this.TaskCardProgressValueBorderColor = ActiveBorderColor;
        }
    }
}
