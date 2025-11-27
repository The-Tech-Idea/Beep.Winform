using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = BackgroundColor;
            this.StatsCardForeColor = ForeColor;
            this.StatsCardBorderColor = InactiveBorderColor;
            this.StatsCardTitleForeColor = ForeColor;
            this.StatsCardTitleBackColor = BackgroundColor;
            this.StatsCardSubTitleForeColor = ForeColor;
            this.StatsCardSubTitleBackColor = BackgroundColor;
            this.StatsCardValueForeColor = ForeColor;
            this.StatsCardValueBackColor = BackgroundColor;
            this.StatsCardValueBorderColor = InactiveBorderColor;
            this.StatsCardValueHoverForeColor = ForeColor;
            this.StatsCardValueHoverBackColor = BackgroundColor;
            this.StatsCardValueHoverBorderColor = InactiveBorderColor;
            this.StatsCardInfoForeColor = ForeColor;
            this.StatsCardInfoBackColor = BackgroundColor;
            this.StatsCardInfoBorderColor = InactiveBorderColor;
            this.StatsCardTrendForeColor = ForeColor;
            this.StatsCardTrendBackColor = BackgroundColor;
            this.StatsCardTrendBorderColor = InactiveBorderColor;
        }
    }
}