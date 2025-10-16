using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(245,248,255);
            this.StatsCardForeColor = Color.FromArgb(24,28,35);
            this.StatsCardBorderColor = Color.FromArgb(210,220,235);
            this.StatsCardTitleForeColor = Color.FromArgb(24,28,35);
            this.StatsCardTitleBackColor = Color.FromArgb(245,248,255);
            this.StatsCardSubTitleForeColor = Color.FromArgb(24,28,35);
            this.StatsCardSubTitleBackColor = Color.FromArgb(245,248,255);
            this.StatsCardValueForeColor = Color.FromArgb(24,28,35);
            this.StatsCardValueBackColor = Color.FromArgb(245,248,255);
            this.StatsCardValueBorderColor = Color.FromArgb(210,220,235);
            this.StatsCardValueHoverForeColor = Color.FromArgb(24,28,35);
            this.StatsCardValueHoverBackColor = Color.FromArgb(245,248,255);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(210,220,235);
            this.StatsCardInfoForeColor = Color.FromArgb(24,28,35);
            this.StatsCardInfoBackColor = Color.FromArgb(74,158,255);
            this.StatsCardInfoBorderColor = Color.FromArgb(210,220,235);
            this.StatsCardTrendForeColor = Color.FromArgb(24,28,35);
            this.StatsCardTrendBackColor = Color.FromArgb(245,248,255);
            this.StatsCardTrendBorderColor = Color.FromArgb(210,220,235);
        }
    }
}