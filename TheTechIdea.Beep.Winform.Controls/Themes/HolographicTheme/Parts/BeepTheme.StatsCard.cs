using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(15,16,32);
            this.StatsCardForeColor = Color.FromArgb(245,247,255);
            this.StatsCardBorderColor = Color.FromArgb(74,79,123);
            this.StatsCardTitleForeColor = Color.FromArgb(245,247,255);
            this.StatsCardTitleBackColor = Color.FromArgb(15,16,32);
            this.StatsCardSubTitleForeColor = Color.FromArgb(245,247,255);
            this.StatsCardSubTitleBackColor = Color.FromArgb(15,16,32);
            this.StatsCardValueForeColor = Color.FromArgb(245,247,255);
            this.StatsCardValueBackColor = Color.FromArgb(15,16,32);
            this.StatsCardValueBorderColor = Color.FromArgb(74,79,123);
            this.StatsCardValueHoverForeColor = Color.FromArgb(245,247,255);
            this.StatsCardValueHoverBackColor = Color.FromArgb(15,16,32);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(74,79,123);
            this.StatsCardInfoForeColor = Color.FromArgb(245,247,255);
            this.StatsCardInfoBackColor = Color.FromArgb(15,16,32);
            this.StatsCardInfoBorderColor = Color.FromArgb(74,79,123);
            this.StatsCardTrendForeColor = Color.FromArgb(245,247,255);
            this.StatsCardTrendBackColor = Color.FromArgb(15,16,32);
            this.StatsCardTrendBorderColor = Color.FromArgb(74,79,123);
        }
    }
}