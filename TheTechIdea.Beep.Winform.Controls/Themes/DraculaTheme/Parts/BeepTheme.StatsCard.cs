using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(40,42,54);
            this.StatsCardForeColor = Color.FromArgb(248,248,242);
            this.StatsCardBorderColor = Color.FromArgb(98,114,164);
            this.StatsCardTitleForeColor = Color.FromArgb(248,248,242);
            this.StatsCardTitleBackColor = Color.FromArgb(40,42,54);
            this.StatsCardSubTitleForeColor = Color.FromArgb(248,248,242);
            this.StatsCardSubTitleBackColor = Color.FromArgb(40,42,54);
            this.StatsCardValueForeColor = Color.FromArgb(248,248,242);
            this.StatsCardValueBackColor = Color.FromArgb(40,42,54);
            this.StatsCardValueBorderColor = Color.FromArgb(98,114,164);
            this.StatsCardValueHoverForeColor = Color.FromArgb(248,248,242);
            this.StatsCardValueHoverBackColor = Color.FromArgb(40,42,54);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(98,114,164);
            this.StatsCardInfoForeColor = Color.FromArgb(248,248,242);
            this.StatsCardInfoBackColor = Color.FromArgb(40,42,54);
            this.StatsCardInfoBorderColor = Color.FromArgb(98,114,164);
            this.StatsCardTrendForeColor = Color.FromArgb(248,248,242);
            this.StatsCardTrendBackColor = Color.FromArgb(40,42,54);
            this.StatsCardTrendBorderColor = Color.FromArgb(98,114,164);
        }
    }
}