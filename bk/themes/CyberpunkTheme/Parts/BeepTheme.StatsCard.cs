using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CyberpunkTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(10,8,20);
            this.StatsCardForeColor = Color.FromArgb(228,244,255);
            this.StatsCardBorderColor = Color.FromArgb(90,20,110);
            this.StatsCardTitleForeColor = Color.FromArgb(228,244,255);
            this.StatsCardTitleBackColor = Color.FromArgb(10,8,20);
            this.StatsCardSubTitleForeColor = Color.FromArgb(228,244,255);
            this.StatsCardSubTitleBackColor = Color.FromArgb(10,8,20);
            this.StatsCardValueForeColor = Color.FromArgb(228,244,255);
            this.StatsCardValueBackColor = Color.FromArgb(10,8,20);
            this.StatsCardValueBorderColor = Color.FromArgb(90,20,110);
            this.StatsCardValueHoverForeColor = Color.FromArgb(228,244,255);
            this.StatsCardValueHoverBackColor = Color.FromArgb(10,8,20);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(90,20,110);
            this.StatsCardInfoForeColor = Color.FromArgb(228,244,255);
            this.StatsCardInfoBackColor = Color.FromArgb(10,8,20);
            this.StatsCardInfoBorderColor = Color.FromArgb(90,20,110);
            this.StatsCardTrendForeColor = Color.FromArgb(228,244,255);
            this.StatsCardTrendBackColor = Color.FromArgb(10,8,20);
            this.StatsCardTrendBorderColor = Color.FromArgb(90,20,110);
        }
    }
}