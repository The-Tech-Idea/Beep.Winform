using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = PanelGradiantMiddleColor;
            this.StatsCardForeColor = ForeColor;
            this.StatsCardBorderColor = InactiveBorderColor;
            this.StatsCardTitleForeColor = ForeColor;
            this.StatsCardTitleBackColor = PanelGradiantMiddleColor;
            this.StatsCardSubTitleForeColor = ForeColor;
            this.StatsCardSubTitleBackColor = PanelGradiantMiddleColor;
            this.StatsCardValueForeColor = ForeColor;
            this.StatsCardValueBackColor = PanelGradiantMiddleColor;
            this.StatsCardValueBorderColor = InactiveBorderColor;
            this.StatsCardValueHoverForeColor = ForeColor;
            this.StatsCardValueHoverBackColor = PanelGradiantMiddleColor;
            this.StatsCardValueHoverBorderColor = InactiveBorderColor;
            this.StatsCardInfoForeColor = ForeColor;
            this.StatsCardInfoBackColor = PanelGradiantMiddleColor;
            this.StatsCardInfoBorderColor = InactiveBorderColor;
            this.StatsCardTrendForeColor = ForeColor;
            this.StatsCardTrendBackColor = PanelGradiantMiddleColor;
            this.StatsCardTrendBorderColor = InactiveBorderColor;
        }
    }
}