using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(46,52,64);
            this.StatsCardForeColor = Color.FromArgb(216,222,233);
            this.StatsCardBorderColor = Color.FromArgb(76,86,106);
            this.StatsCardTitleForeColor = Color.FromArgb(216,222,233);
            this.StatsCardTitleBackColor = Color.FromArgb(46,52,64);
            this.StatsCardSubTitleForeColor = Color.FromArgb(216,222,233);
            this.StatsCardSubTitleBackColor = Color.FromArgb(46,52,64);
            this.StatsCardValueForeColor = Color.FromArgb(216,222,233);
            this.StatsCardValueBackColor = Color.FromArgb(46,52,64);
            this.StatsCardValueBorderColor = Color.FromArgb(76,86,106);
            this.StatsCardValueHoverForeColor = Color.FromArgb(216,222,233);
            this.StatsCardValueHoverBackColor = Color.FromArgb(46,52,64);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(76,86,106);
            this.StatsCardInfoForeColor = Color.FromArgb(216,222,233);
            this.StatsCardInfoBackColor = Color.FromArgb(46,52,64);
            this.StatsCardInfoBorderColor = Color.FromArgb(76,86,106);
            this.StatsCardTrendForeColor = Color.FromArgb(216,222,233);
            this.StatsCardTrendBackColor = Color.FromArgb(46,52,64);
            this.StatsCardTrendBorderColor = Color.FromArgb(76,86,106);
        }
    }
}