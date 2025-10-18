using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class KDETheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(248,249,250);
            this.StatsCardForeColor = Color.FromArgb(33,37,41);
            this.StatsCardBorderColor = Color.FromArgb(222,226,230);
            this.StatsCardTitleForeColor = Color.FromArgb(33,37,41);
            this.StatsCardTitleBackColor = Color.FromArgb(248,249,250);
            this.StatsCardSubTitleForeColor = Color.FromArgb(33,37,41);
            this.StatsCardSubTitleBackColor = Color.FromArgb(248,249,250);
            this.StatsCardValueForeColor = Color.FromArgb(33,37,41);
            this.StatsCardValueBackColor = Color.FromArgb(248,249,250);
            this.StatsCardValueBorderColor = Color.FromArgb(222,226,230);
            this.StatsCardValueHoverForeColor = Color.FromArgb(33,37,41);
            this.StatsCardValueHoverBackColor = Color.FromArgb(248,249,250);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(222,226,230);
            this.StatsCardInfoForeColor = Color.FromArgb(33,37,41);
            this.StatsCardInfoBackColor = Color.FromArgb(23,162,184);
            this.StatsCardInfoBorderColor = Color.FromArgb(222,226,230);
            this.StatsCardTrendForeColor = Color.FromArgb(33,37,41);
            this.StatsCardTrendBackColor = Color.FromArgb(248,249,250);
            this.StatsCardTrendBorderColor = Color.FromArgb(222,226,230);
        }
    }
}