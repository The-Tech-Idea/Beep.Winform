using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = SurfaceColor;
            this.StatsCardForeColor = ForeColor;
            this.StatsCardBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.StatsCardTitleForeColor = ForeColor;
            this.StatsCardTitleBackColor = SurfaceColor;
            this.StatsCardSubTitleForeColor = ForeColor;
            this.StatsCardSubTitleBackColor = SurfaceColor;
            this.StatsCardValueForeColor = ForeColor;
            this.StatsCardValueBackColor = SurfaceColor;
            this.StatsCardValueBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.StatsCardValueHoverForeColor = ForeColor;
            this.StatsCardValueHoverBackColor = SurfaceColor;
            this.StatsCardValueHoverBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.StatsCardInfoForeColor = ForeColor;
            this.StatsCardInfoBackColor = PrimaryColor;
            this.StatsCardInfoBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
            this.StatsCardTrendForeColor = ForeColor;
            this.StatsCardTrendBackColor = SurfaceColor;
            this.StatsCardTrendBorderColor = ThemeUtil.Lighten(BackgroundColor, 0.25);
        }
    }
}
