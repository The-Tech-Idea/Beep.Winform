using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = PanelBackColor;
            this.StatsCardForeColor = ForeColor;
            this.StatsCardBorderColor = InactiveBorderColor;
            this.StatsCardTitleForeColor = ForeColor;
            this.StatsCardTitleBackColor = PanelBackColor;
            this.StatsCardSubTitleForeColor = ForeColor;
            this.StatsCardSubTitleBackColor = PanelBackColor;
            this.StatsCardValueForeColor = ForeColor;
            this.StatsCardValueBackColor = PanelBackColor;
            this.StatsCardValueBorderColor = InactiveBorderColor;
            this.StatsCardValueHoverForeColor = ForeColor;
            this.StatsCardValueHoverBackColor = PanelBackColor;
            this.StatsCardValueHoverBorderColor = InactiveBorderColor;
            this.StatsCardInfoForeColor = ForeColor;
            this.StatsCardInfoBackColor = PanelBackColor;
            this.StatsCardInfoBorderColor = InactiveBorderColor;
            this.StatsCardTrendForeColor = ForeColor;
            this.StatsCardTrendBackColor = PanelBackColor;
            this.StatsCardTrendBorderColor = InactiveBorderColor;
        }
    }
}