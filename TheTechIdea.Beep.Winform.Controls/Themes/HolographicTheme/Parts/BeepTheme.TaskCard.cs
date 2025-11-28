using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyTaskCard()
        {
            this.TaskCardBackColor = SurfaceColor;
            this.TaskCardForeColor = ForeColor;
            this.TaskCardBorderColor = InactiveBorderColor;
            this.TaskCardTitleForeColor = ForeColor;
            this.TaskCardTitleBackColor = SurfaceColor;
            this.TaskCardSubTitleForeColor = AccentColor;
            this.TaskCardSubTitleBackColor = SurfaceColor;
            this.TaskCardMetricTextForeColor = ForeColor;
            this.TaskCardMetricTextBackColor = SurfaceColor;
            this.TaskCardMetricTextBorderColor = InactiveBorderColor;
            this.TaskCardMetricTextHoverForeColor = ForeColor;
            this.TaskCardMetricTextHoverBackColor = PanelGradiantStartColor;
            this.TaskCardMetricTextHoverBorderColor = ActiveBorderColor;
            this.TaskCardProgressValueForeColor = ForeColor;
            this.TaskCardProgressValueBackColor = SurfaceColor;
            this.TaskCardProgressValueBorderColor = InactiveBorderColor;
        }
    }
}