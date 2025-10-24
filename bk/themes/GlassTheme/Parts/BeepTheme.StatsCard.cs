using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(236,244,255);
            this.StatsCardForeColor = Color.FromArgb(17,24,39);
            this.StatsCardBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.StatsCardTitleForeColor = Color.FromArgb(17,24,39);
            this.StatsCardTitleBackColor = Color.FromArgb(236,244,255);
            this.StatsCardSubTitleForeColor = Color.FromArgb(17,24,39);
            this.StatsCardSubTitleBackColor = Color.FromArgb(236,244,255);
            this.StatsCardValueForeColor = Color.FromArgb(17,24,39);
            this.StatsCardValueBackColor = Color.FromArgb(236,244,255);
            this.StatsCardValueBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.StatsCardValueHoverForeColor = Color.FromArgb(17,24,39);
            this.StatsCardValueHoverBackColor = Color.FromArgb(236,244,255);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.StatsCardInfoForeColor = Color.FromArgb(17,24,39);
            this.StatsCardInfoBackColor = Color.FromArgb(56,189,248);
            this.StatsCardInfoBorderColor = Color.FromArgb(140, 255, 255, 255);
            this.StatsCardTrendForeColor = Color.FromArgb(17,24,39);
            this.StatsCardTrendBackColor = Color.FromArgb(236,244,255);
            this.StatsCardTrendBorderColor = Color.FromArgb(140, 255, 255, 255);
        }
    }
}