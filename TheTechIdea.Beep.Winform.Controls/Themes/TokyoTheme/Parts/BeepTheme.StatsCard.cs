using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(26,27,38);
            this.StatsCardForeColor = Color.FromArgb(192,202,245);
            this.StatsCardBorderColor = Color.FromArgb(86,95,137);
            this.StatsCardTitleForeColor = Color.FromArgb(192,202,245);
            this.StatsCardTitleBackColor = Color.FromArgb(26,27,38);
            this.StatsCardSubTitleForeColor = Color.FromArgb(192,202,245);
            this.StatsCardSubTitleBackColor = Color.FromArgb(26,27,38);
            this.StatsCardValueForeColor = Color.FromArgb(192,202,245);
            this.StatsCardValueBackColor = Color.FromArgb(26,27,38);
            this.StatsCardValueBorderColor = Color.FromArgb(86,95,137);
            this.StatsCardValueHoverForeColor = Color.FromArgb(192,202,245);
            this.StatsCardValueHoverBackColor = Color.FromArgb(26,27,38);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(86,95,137);
            this.StatsCardInfoForeColor = Color.FromArgb(192,202,245);
            this.StatsCardInfoBackColor = Color.FromArgb(26,27,38);
            this.StatsCardInfoBorderColor = Color.FromArgb(86,95,137);
            this.StatsCardTrendForeColor = Color.FromArgb(192,202,245);
            this.StatsCardTrendBackColor = Color.FromArgb(26,27,38);
            this.StatsCardTrendBorderColor = Color.FromArgb(86,95,137);
        }
    }
}