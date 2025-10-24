using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(40,40,40);
            this.StatsCardForeColor = Color.FromArgb(235,219,178);
            this.StatsCardBorderColor = Color.FromArgb(168,153,132);
            this.StatsCardTitleForeColor = Color.FromArgb(235,219,178);
            this.StatsCardTitleBackColor = Color.FromArgb(40,40,40);
            this.StatsCardSubTitleForeColor = Color.FromArgb(235,219,178);
            this.StatsCardSubTitleBackColor = Color.FromArgb(40,40,40);
            this.StatsCardValueForeColor = Color.FromArgb(235,219,178);
            this.StatsCardValueBackColor = Color.FromArgb(40,40,40);
            this.StatsCardValueBorderColor = Color.FromArgb(168,153,132);
            this.StatsCardValueHoverForeColor = Color.FromArgb(235,219,178);
            this.StatsCardValueHoverBackColor = Color.FromArgb(40,40,40);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(168,153,132);
            this.StatsCardInfoForeColor = Color.FromArgb(235,219,178);
            this.StatsCardInfoBackColor = Color.FromArgb(40,40,40);
            this.StatsCardInfoBorderColor = Color.FromArgb(168,153,132);
            this.StatsCardTrendForeColor = Color.FromArgb(235,219,178);
            this.StatsCardTrendBackColor = Color.FromArgb(40,40,40);
            this.StatsCardTrendBorderColor = Color.FromArgb(168,153,132);
        }
    }
}