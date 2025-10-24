using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(40,44,52);
            this.StatsCardForeColor = Color.FromArgb(171,178,191);
            this.StatsCardBorderColor = Color.FromArgb(92,99,112);
            this.StatsCardTitleForeColor = Color.FromArgb(171,178,191);
            this.StatsCardTitleBackColor = Color.FromArgb(40,44,52);
            this.StatsCardSubTitleForeColor = Color.FromArgb(171,178,191);
            this.StatsCardSubTitleBackColor = Color.FromArgb(40,44,52);
            this.StatsCardValueForeColor = Color.FromArgb(171,178,191);
            this.StatsCardValueBackColor = Color.FromArgb(40,44,52);
            this.StatsCardValueBorderColor = Color.FromArgb(92,99,112);
            this.StatsCardValueHoverForeColor = Color.FromArgb(171,178,191);
            this.StatsCardValueHoverBackColor = Color.FromArgb(40,44,52);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(92,99,112);
            this.StatsCardInfoForeColor = Color.FromArgb(171,178,191);
            this.StatsCardInfoBackColor = Color.FromArgb(40,44,52);
            this.StatsCardInfoBorderColor = Color.FromArgb(92,99,112);
            this.StatsCardTrendForeColor = Color.FromArgb(171,178,191);
            this.StatsCardTrendBackColor = Color.FromArgb(40,44,52);
            this.StatsCardTrendBorderColor = Color.FromArgb(92,99,112);
        }
    }
}