using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(250,250,250);
            this.StatsCardForeColor = Color.FromArgb(20,20,20);
            this.StatsCardBorderColor = Color.FromArgb(0,0,0);
            this.StatsCardTitleForeColor = Color.FromArgb(20,20,20);
            this.StatsCardTitleBackColor = Color.FromArgb(250,250,250);
            this.StatsCardSubTitleForeColor = Color.FromArgb(20,20,20);
            this.StatsCardSubTitleBackColor = Color.FromArgb(250,250,250);
            this.StatsCardValueForeColor = Color.FromArgb(20,20,20);
            this.StatsCardValueBackColor = Color.FromArgb(250,250,250);
            this.StatsCardValueBorderColor = Color.FromArgb(0,0,0);
            this.StatsCardValueHoverForeColor = Color.FromArgb(20,20,20);
            this.StatsCardValueHoverBackColor = Color.FromArgb(250,250,250);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(0,0,0);
            this.StatsCardInfoForeColor = Color.FromArgb(20,20,20);
            this.StatsCardInfoBackColor = Color.FromArgb(0,90,255);
            this.StatsCardInfoBorderColor = Color.FromArgb(0,0,0);
            this.StatsCardTrendForeColor = Color.FromArgb(20,20,20);
            this.StatsCardTrendBackColor = Color.FromArgb(250,250,250);
            this.StatsCardTrendBorderColor = Color.FromArgb(0,0,0);
        }
    }
}