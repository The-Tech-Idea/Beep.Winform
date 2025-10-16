using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(255,251,235);
            this.StatsCardForeColor = Color.FromArgb(33,37,41);
            this.StatsCardBorderColor = Color.FromArgb(247,208,136);
            this.StatsCardTitleForeColor = Color.FromArgb(33,37,41);
            this.StatsCardTitleBackColor = Color.FromArgb(255,251,235);
            this.StatsCardSubTitleForeColor = Color.FromArgb(33,37,41);
            this.StatsCardSubTitleBackColor = Color.FromArgb(255,251,235);
            this.StatsCardValueForeColor = Color.FromArgb(33,37,41);
            this.StatsCardValueBackColor = Color.FromArgb(255,251,235);
            this.StatsCardValueBorderColor = Color.FromArgb(247,208,136);
            this.StatsCardValueHoverForeColor = Color.FromArgb(33,37,41);
            this.StatsCardValueHoverBackColor = Color.FromArgb(255,251,235);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(247,208,136);
            this.StatsCardInfoForeColor = Color.FromArgb(33,37,41);
            this.StatsCardInfoBackColor = Color.FromArgb(30,144,255);
            this.StatsCardInfoBorderColor = Color.FromArgb(247,208,136);
            this.StatsCardTrendForeColor = Color.FromArgb(33,37,41);
            this.StatsCardTrendBackColor = Color.FromArgb(255,251,235);
            this.StatsCardTrendBorderColor = Color.FromArgb(247,208,136);
        }
    }
}