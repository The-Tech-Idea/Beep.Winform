using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = SurfaceColor;
            this.StatsCardForeColor = ForeColor;
            this.StatsCardBorderColor = BorderColor;
            this.StatsCardTitleForeColor = ForeColor;
            this.StatsCardTitleBackColor = SurfaceColor;
            this.StatsCardSubTitleForeColor = ForeColor;
            this.StatsCardSubTitleBackColor = SurfaceColor;
            this.StatsCardValueForeColor = ForeColor;
            this.StatsCardValueBackColor = SurfaceColor;
            this.StatsCardValueBorderColor = BorderColor;
            this.StatsCardValueHoverForeColor = ForeColor;
            this.StatsCardValueHoverBackColor = PanelGradiantMiddleColor;
            this.StatsCardValueHoverBorderColor = ActiveBorderColor;
            this.StatsCardInfoForeColor = PrimaryColor;
            this.StatsCardInfoBackColor = PrimaryColor;
            this.StatsCardInfoBorderColor = BorderColor;
            this.StatsCardTrendForeColor = ForeColor;
            this.StatsCardTrendBackColor = PanelBackColor;
            this.StatsCardTrendBorderColor = BorderColor;
        }
    }
}