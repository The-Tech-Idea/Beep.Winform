using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = BackgroundColor;
            this.StatsCardForeColor = ForeColor;
            this.StatsCardBorderColor = BorderColor;
            this.StatsCardTitleForeColor = ForeColor;
            this.StatsCardTitleBackColor = BackgroundColor;
            this.StatsCardSubTitleForeColor = ForeColor;
            this.StatsCardSubTitleBackColor = BackgroundColor;
            this.StatsCardValueForeColor = ForeColor;
            this.StatsCardValueBackColor = BackgroundColor;
            this.StatsCardValueBorderColor = BorderColor;
            this.StatsCardValueHoverForeColor = ForeColor;
            this.StatsCardValueHoverBackColor = PanelGradiantMiddleColor;
            this.StatsCardValueHoverBorderColor = BorderColor;
            this.StatsCardInfoForeColor = OnPrimaryColor;
            this.StatsCardInfoBackColor = PrimaryColor;
            this.StatsCardInfoBorderColor = BorderColor;
            this.StatsCardTrendForeColor = ForeColor;
            this.StatsCardTrendBackColor = BackgroundColor;
            this.StatsCardTrendBorderColor = BorderColor;
        }
    }
}