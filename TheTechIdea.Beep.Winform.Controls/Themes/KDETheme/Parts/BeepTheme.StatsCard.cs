using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = PanelBackColor;
            this.StatsCardForeColor = ForeColor;
            this.StatsCardBorderColor = BorderColor;
            this.StatsCardTitleForeColor = ForeColor;
            this.StatsCardTitleBackColor = PanelBackColor;
            this.StatsCardSubTitleForeColor = ForeColor;
            this.StatsCardSubTitleBackColor = PanelBackColor;
            this.StatsCardValueForeColor = ForeColor;
            this.StatsCardValueBackColor = PanelBackColor;
            this.StatsCardValueBorderColor = BorderColor;
            this.StatsCardValueHoverForeColor = ForeColor;
            this.StatsCardValueHoverBackColor = PanelBackColor;
            this.StatsCardValueHoverBorderColor = BorderColor;
            this.StatsCardInfoForeColor = ForeColor;
            this.StatsCardInfoBackColor = AccentColor;
            this.StatsCardInfoBorderColor = BorderColor;
            this.StatsCardTrendForeColor = ForeColor;
            this.StatsCardTrendBackColor = PanelBackColor;
            this.StatsCardTrendBorderColor = BorderColor;
        }
    }
}