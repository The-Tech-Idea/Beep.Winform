using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(250,250,252);
            this.StatsCardForeColor = Color.FromArgb(28,28,30);
            this.StatsCardBorderColor = Color.FromArgb(229,229,234);
            this.StatsCardTitleForeColor = Color.FromArgb(28,28,30);
            this.StatsCardTitleBackColor = Color.FromArgb(250,250,252);
            this.StatsCardSubTitleForeColor = Color.FromArgb(28,28,30);
            this.StatsCardSubTitleBackColor = Color.FromArgb(250,250,252);
            this.StatsCardValueForeColor = Color.FromArgb(28,28,30);
            this.StatsCardValueBackColor = Color.FromArgb(250,250,252);
            this.StatsCardValueBorderColor = Color.FromArgb(229,229,234);
            this.StatsCardValueHoverForeColor = Color.FromArgb(28,28,30);
            this.StatsCardValueHoverBackColor = Color.FromArgb(250,250,252);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(229,229,234);
            this.StatsCardInfoForeColor = Color.FromArgb(28,28,30);
            this.StatsCardInfoBackColor = Color.FromArgb(0,122,255);
            this.StatsCardInfoBorderColor = Color.FromArgb(229,229,234);
            this.StatsCardTrendForeColor = Color.FromArgb(28,28,30);
            this.StatsCardTrendBackColor = Color.FromArgb(250,250,252);
            this.StatsCardTrendBorderColor = Color.FromArgb(229,229,234);
        }
    }
}