using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(236,240,243);
            this.StatsCardForeColor = Color.FromArgb(58,66,86);
            this.StatsCardBorderColor = Color.FromArgb(221,228,235);
            this.StatsCardTitleForeColor = Color.FromArgb(58,66,86);
            this.StatsCardTitleBackColor = Color.FromArgb(236,240,243);
            this.StatsCardSubTitleForeColor = Color.FromArgb(58,66,86);
            this.StatsCardSubTitleBackColor = Color.FromArgb(236,240,243);
            this.StatsCardValueForeColor = Color.FromArgb(58,66,86);
            this.StatsCardValueBackColor = Color.FromArgb(236,240,243);
            this.StatsCardValueBorderColor = Color.FromArgb(221,228,235);
            this.StatsCardValueHoverForeColor = Color.FromArgb(58,66,86);
            this.StatsCardValueHoverBackColor = Color.FromArgb(236,240,243);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(221,228,235);
            this.StatsCardInfoForeColor = Color.FromArgb(58,66,86);
            this.StatsCardInfoBackColor = Color.FromArgb(52,152,219);
            this.StatsCardInfoBorderColor = Color.FromArgb(221,228,235);
            this.StatsCardTrendForeColor = Color.FromArgb(58,66,86);
            this.StatsCardTrendBackColor = Color.FromArgb(236,240,243);
            this.StatsCardTrendBorderColor = Color.FromArgb(221,228,235);
        }
    }
}