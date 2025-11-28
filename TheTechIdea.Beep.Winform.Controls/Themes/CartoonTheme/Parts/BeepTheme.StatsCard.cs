using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = SurfaceColor;
            this.StatsCardForeColor = ForeColor;
            this.StatsCardBorderColor = InactiveBorderColor;
            this.StatsCardTitleForeColor = ForeColor;
            this.StatsCardTitleBackColor = SurfaceColor;
            this.StatsCardSubTitleForeColor = ForeColor;
            this.StatsCardSubTitleBackColor = SurfaceColor;
            this.StatsCardValueForeColor = ForeColor;
            this.StatsCardValueBackColor = SurfaceColor;
            this.StatsCardValueBorderColor = InactiveBorderColor;
            this.StatsCardValueHoverForeColor = ForeColor;
            this.StatsCardValueHoverBackColor = PanelGradiantStartColor;
            this.StatsCardValueHoverBorderColor = ActiveBorderColor;
            this.StatsCardInfoForeColor = OnPrimaryColor;
            this.StatsCardInfoBackColor = SecondaryColor;
            this.StatsCardInfoBorderColor = SecondaryColor;
            this.StatsCardTrendForeColor = ForeColor;
            this.StatsCardTrendBackColor = SurfaceColor;
            this.StatsCardTrendBorderColor = InactiveBorderColor;
        }
    }
}