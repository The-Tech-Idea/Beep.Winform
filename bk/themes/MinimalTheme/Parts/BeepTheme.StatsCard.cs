using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(255,255,255);
            this.StatsCardForeColor = Color.FromArgb(31,41,55);
            this.StatsCardBorderColor = Color.FromArgb(209,213,219);
            this.StatsCardTitleForeColor = Color.FromArgb(31,41,55);
            this.StatsCardTitleBackColor = Color.FromArgb(255,255,255);
            this.StatsCardSubTitleForeColor = Color.FromArgb(31,41,55);
            this.StatsCardSubTitleBackColor = Color.FromArgb(255,255,255);
            this.StatsCardValueForeColor = Color.FromArgb(31,41,55);
            this.StatsCardValueBackColor = Color.FromArgb(255,255,255);
            this.StatsCardValueBorderColor = Color.FromArgb(209,213,219);
            this.StatsCardValueHoverForeColor = Color.FromArgb(31,41,55);
            this.StatsCardValueHoverBackColor = Color.FromArgb(255,255,255);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(209,213,219);
            this.StatsCardInfoForeColor = Color.FromArgb(31,41,55);
            this.StatsCardInfoBackColor = Color.FromArgb(59,130,246);
            this.StatsCardInfoBorderColor = Color.FromArgb(209,213,219);
            this.StatsCardTrendForeColor = Color.FromArgb(31,41,55);
            this.StatsCardTrendBackColor = Color.FromArgb(255,255,255);
            this.StatsCardTrendBorderColor = Color.FromArgb(209,213,219);
        }
    }
}