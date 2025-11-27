using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = PanelBackColor;
            this.StatsCardForeColor = ForeColor;
            this.StatsCardBorderColor = SecondaryColor;
            this.StatsCardTitleForeColor = ForeColor;
            this.StatsCardTitleBackColor = BackgroundColor;
            this.StatsCardSubTitleForeColor = ForeColor;
            this.StatsCardSubTitleBackColor = BackgroundColor;
            this.StatsCardValueForeColor = ForeColor;
            this.StatsCardValueBackColor = BackgroundColor;
            this.StatsCardValueBorderColor = SecondaryColor;
            this.StatsCardValueHoverForeColor = ForeColor;
            this.StatsCardValueHoverBackColor = PanelBackColor;
            this.StatsCardValueHoverBorderColor = SecondaryColor;
            this.StatsCardInfoForeColor = OnPrimaryColor;
            this.StatsCardInfoBackColor = PrimaryColor;
            this.StatsCardInfoBorderColor = SecondaryColor;
            this.StatsCardTrendForeColor = ForeColor;
            this.StatsCardTrendBackColor = BackgroundColor;
            this.StatsCardTrendBorderColor = SecondaryColor;
        }
    }
}