using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(10,12,20);
            this.StatsCardForeColor = Color.FromArgb(235,245,255);
            this.StatsCardBorderColor = Color.FromArgb(60,70,100);
            this.StatsCardTitleForeColor = Color.FromArgb(235,245,255);
            this.StatsCardTitleBackColor = Color.FromArgb(10,12,20);
            this.StatsCardSubTitleForeColor = Color.FromArgb(235,245,255);
            this.StatsCardSubTitleBackColor = Color.FromArgb(10,12,20);
            this.StatsCardValueForeColor = Color.FromArgb(235,245,255);
            this.StatsCardValueBackColor = Color.FromArgb(10,12,20);
            this.StatsCardValueBorderColor = Color.FromArgb(60,70,100);
            this.StatsCardValueHoverForeColor = Color.FromArgb(235,245,255);
            this.StatsCardValueHoverBackColor = Color.FromArgb(10,12,20);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(60,70,100);
            this.StatsCardInfoForeColor = Color.FromArgb(235,245,255);
            this.StatsCardInfoBackColor = Color.FromArgb(10,12,20);
            this.StatsCardInfoBorderColor = Color.FromArgb(60,70,100);
            this.StatsCardTrendForeColor = Color.FromArgb(235,245,255);
            this.StatsCardTrendBackColor = Color.FromArgb(10,12,20);
            this.StatsCardTrendBorderColor = Color.FromArgb(60,70,100);
        }
    }
}