using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(246,245,244);
            this.StatsCardForeColor = Color.FromArgb(46,52,54);
            this.StatsCardBorderColor = Color.FromArgb(205,207,212);
            this.StatsCardTitleForeColor = Color.FromArgb(46,52,54);
            this.StatsCardTitleBackColor = Color.FromArgb(246,245,244);
            this.StatsCardSubTitleForeColor = Color.FromArgb(46,52,54);
            this.StatsCardSubTitleBackColor = Color.FromArgb(246,245,244);
            this.StatsCardValueForeColor = Color.FromArgb(46,52,54);
            this.StatsCardValueBackColor = Color.FromArgb(246,245,244);
            this.StatsCardValueBorderColor = Color.FromArgb(205,207,212);
            this.StatsCardValueHoverForeColor = Color.FromArgb(46,52,54);
            this.StatsCardValueHoverBackColor = Color.FromArgb(246,245,244);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(205,207,212);
            this.StatsCardInfoForeColor = Color.FromArgb(46,52,54);
            this.StatsCardInfoBackColor = Color.FromArgb(28,113,216);
            this.StatsCardInfoBorderColor = Color.FromArgb(205,207,212);
            this.StatsCardTrendForeColor = Color.FromArgb(46,52,54);
            this.StatsCardTrendBackColor = Color.FromArgb(246,245,244);
            this.StatsCardTrendBorderColor = Color.FromArgb(205,207,212);
        }
    }
}