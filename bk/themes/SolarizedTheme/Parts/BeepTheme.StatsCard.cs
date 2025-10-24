using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(0,43,54);
            this.StatsCardForeColor = Color.FromArgb(147,161,161);
            this.StatsCardBorderColor = Color.FromArgb(88,110,117);
            this.StatsCardTitleForeColor = Color.FromArgb(147,161,161);
            this.StatsCardTitleBackColor = Color.FromArgb(0,43,54);
            this.StatsCardSubTitleForeColor = Color.FromArgb(147,161,161);
            this.StatsCardSubTitleBackColor = Color.FromArgb(0,43,54);
            this.StatsCardValueForeColor = Color.FromArgb(147,161,161);
            this.StatsCardValueBackColor = Color.FromArgb(0,43,54);
            this.StatsCardValueBorderColor = Color.FromArgb(88,110,117);
            this.StatsCardValueHoverForeColor = Color.FromArgb(147,161,161);
            this.StatsCardValueHoverBackColor = Color.FromArgb(0,43,54);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(88,110,117);
            this.StatsCardInfoForeColor = Color.FromArgb(147,161,161);
            this.StatsCardInfoBackColor = Color.FromArgb(0,43,54);
            this.StatsCardInfoBorderColor = Color.FromArgb(88,110,117);
            this.StatsCardTrendForeColor = Color.FromArgb(147,161,161);
            this.StatsCardTrendBackColor = Color.FromArgb(0,43,54);
            this.StatsCardTrendBorderColor = Color.FromArgb(88,110,117);
        }
    }
}