using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(243,242,241);
            this.StatsCardForeColor = Color.FromArgb(32,31,30);
            this.StatsCardBorderColor = Color.FromArgb(220,220,220);
            this.StatsCardTitleForeColor = Color.FromArgb(32,31,30);
            this.StatsCardTitleBackColor = Color.FromArgb(243,242,241);
            this.StatsCardSubTitleForeColor = Color.FromArgb(32,31,30);
            this.StatsCardSubTitleBackColor = Color.FromArgb(243,242,241);
            this.StatsCardValueForeColor = Color.FromArgb(32,31,30);
            this.StatsCardValueBackColor = Color.FromArgb(243,242,241);
            this.StatsCardValueBorderColor = Color.FromArgb(220,220,220);
            this.StatsCardValueHoverForeColor = Color.FromArgb(32,31,30);
            this.StatsCardValueHoverBackColor = Color.FromArgb(243,242,241);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(220,220,220);
            this.StatsCardInfoForeColor = Color.FromArgb(32,31,30);
            this.StatsCardInfoBackColor = Color.FromArgb(0,120,215);
            this.StatsCardInfoBorderColor = Color.FromArgb(220,220,220);
            this.StatsCardTrendForeColor = Color.FromArgb(32,31,30);
            this.StatsCardTrendBackColor = Color.FromArgb(243,242,241);
            this.StatsCardTrendBorderColor = Color.FromArgb(220,220,220);
        }
    }
}