using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = SurfaceColor;
            this.StatsCardForeColor = ForeColor;
            this.StatsCardBorderColor = BorderColor;
            this.StatsCardTitleForeColor = ForeColor;
            this.StatsCardTitleBackColor = SurfaceColor;
            this.StatsCardSubTitleForeColor = SecondaryColor;
            this.StatsCardSubTitleBackColor = SurfaceColor;
            this.StatsCardValueForeColor = ForeColor;
            this.StatsCardValueBackColor = ThemeUtil.Lighten(SurfaceColor, 0.03);
            this.StatsCardValueBorderColor = ActiveBorderColor;
            this.StatsCardValueHoverForeColor = ForeColor;
            this.StatsCardValueHoverBackColor = ThemeUtil.Lighten(SurfaceColor, 0.05);
            this.StatsCardValueHoverBorderColor = ActiveBorderColor;
            this.StatsCardInfoForeColor = OnPrimaryColor;
            this.StatsCardInfoBackColor = PrimaryColor;
            this.StatsCardInfoBorderColor = ActiveBorderColor;
            this.StatsCardTrendForeColor = ForeColor;
            this.StatsCardTrendBackColor = SurfaceColor;
            this.StatsCardTrendBorderColor = BorderColor;
        }
    }
}
