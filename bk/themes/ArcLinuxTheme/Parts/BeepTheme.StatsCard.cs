using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(245,246,247);
            this.StatsCardForeColor = Color.FromArgb(43,45,48);
            this.StatsCardBorderColor = Color.FromArgb(220,223,230);
            this.StatsCardTitleForeColor = Color.FromArgb(43,45,48);
            this.StatsCardTitleBackColor = Color.FromArgb(245,246,247);
            this.StatsCardSubTitleForeColor = Color.FromArgb(43,45,48);
            this.StatsCardSubTitleBackColor = Color.FromArgb(245,246,247);
            this.StatsCardValueForeColor = Color.FromArgb(43,45,48);
            this.StatsCardValueBackColor = Color.FromArgb(245,246,247);
            this.StatsCardValueBorderColor = Color.FromArgb(220,223,230);
            this.StatsCardValueHoverForeColor = Color.FromArgb(43,45,48);
            this.StatsCardValueHoverBackColor = Color.FromArgb(245,246,247);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(220,223,230);
            this.StatsCardInfoForeColor = Color.FromArgb(43,45,48);
            this.StatsCardInfoBackColor = Color.FromArgb(33,150,243);
            this.StatsCardInfoBorderColor = Color.FromArgb(220,223,230);
            this.StatsCardTrendForeColor = Color.FromArgb(43,45,48);
            this.StatsCardTrendBackColor = Color.FromArgb(245,246,247);
            this.StatsCardTrendBorderColor = Color.FromArgb(220,223,230);
        }
    }
}