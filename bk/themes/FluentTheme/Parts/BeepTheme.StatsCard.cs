using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(245,246,248);
            this.StatsCardForeColor = Color.FromArgb(32,32,32);
            this.StatsCardBorderColor = Color.FromArgb(218,223,230);
            this.StatsCardTitleForeColor = Color.FromArgb(32,32,32);
            this.StatsCardTitleBackColor = Color.FromArgb(245,246,248);
            this.StatsCardSubTitleForeColor = Color.FromArgb(32,32,32);
            this.StatsCardSubTitleBackColor = Color.FromArgb(245,246,248);
            this.StatsCardValueForeColor = Color.FromArgb(32,32,32);
            this.StatsCardValueBackColor = Color.FromArgb(245,246,248);
            this.StatsCardValueBorderColor = Color.FromArgb(218,223,230);
            this.StatsCardValueHoverForeColor = Color.FromArgb(32,32,32);
            this.StatsCardValueHoverBackColor = Color.FromArgb(245,246,248);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(218,223,230);
            this.StatsCardInfoForeColor = Color.FromArgb(32,32,32);
            this.StatsCardInfoBackColor = Color.FromArgb(0,120,215);
            this.StatsCardInfoBorderColor = Color.FromArgb(218,223,230);
            this.StatsCardTrendForeColor = Color.FromArgb(32,32,32);
            this.StatsCardTrendBackColor = Color.FromArgb(245,246,248);
            this.StatsCardTrendBorderColor = Color.FromArgb(218,223,230);
        }
    }
}