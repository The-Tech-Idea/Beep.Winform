using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(242,242,245);
            this.StatsCardForeColor = Color.FromArgb(44,44,44);
            this.StatsCardBorderColor = Color.FromArgb(218,218,222);
            this.StatsCardTitleForeColor = Color.FromArgb(44,44,44);
            this.StatsCardTitleBackColor = Color.FromArgb(242,242,245);
            this.StatsCardSubTitleForeColor = Color.FromArgb(44,44,44);
            this.StatsCardSubTitleBackColor = Color.FromArgb(242,242,245);
            this.StatsCardValueForeColor = Color.FromArgb(44,44,44);
            this.StatsCardValueBackColor = Color.FromArgb(242,242,245);
            this.StatsCardValueBorderColor = Color.FromArgb(218,218,222);
            this.StatsCardValueHoverForeColor = Color.FromArgb(44,44,44);
            this.StatsCardValueHoverBackColor = Color.FromArgb(242,242,245);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(218,218,222);
            this.StatsCardInfoForeColor = Color.FromArgb(44,44,44);
            this.StatsCardInfoBackColor = Color.FromArgb(44,124,186);
            this.StatsCardInfoBorderColor = Color.FromArgb(218,218,222);
            this.StatsCardTrendForeColor = Color.FromArgb(44,44,44);
            this.StatsCardTrendBackColor = Color.FromArgb(242,242,245);
            this.StatsCardTrendBorderColor = Color.FromArgb(218,218,222);
        }
    }
}