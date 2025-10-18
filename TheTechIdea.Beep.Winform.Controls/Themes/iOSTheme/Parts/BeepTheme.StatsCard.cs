using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(242,242,247);
            this.StatsCardForeColor = Color.FromArgb(28,28,30);
            this.StatsCardBorderColor = Color.FromArgb(198,198,207);
            this.StatsCardTitleForeColor = Color.FromArgb(28,28,30);
            this.StatsCardTitleBackColor = Color.FromArgb(242,242,247);
            this.StatsCardSubTitleForeColor = Color.FromArgb(28,28,30);
            this.StatsCardSubTitleBackColor = Color.FromArgb(242,242,247);
            this.StatsCardValueForeColor = Color.FromArgb(28,28,30);
            this.StatsCardValueBackColor = Color.FromArgb(242,242,247);
            this.StatsCardValueBorderColor = Color.FromArgb(198,198,207);
            this.StatsCardValueHoverForeColor = Color.FromArgb(28,28,30);
            this.StatsCardValueHoverBackColor = Color.FromArgb(242,242,247);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(198,198,207);
            this.StatsCardInfoForeColor = Color.FromArgb(28,28,30);
            this.StatsCardInfoBackColor = Color.FromArgb(242,242,247);
            this.StatsCardInfoBorderColor = Color.FromArgb(198,198,207);
            this.StatsCardTrendForeColor = Color.FromArgb(28,28,30);
            this.StatsCardTrendBackColor = Color.FromArgb(242,242,247);
            this.StatsCardTrendBorderColor = Color.FromArgb(198,198,207);
        }
    }
}