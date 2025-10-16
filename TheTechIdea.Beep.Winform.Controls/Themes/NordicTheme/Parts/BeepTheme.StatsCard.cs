using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(250,250,251);
            this.StatsCardForeColor = Color.FromArgb(31,41,55);
            this.StatsCardBorderColor = Color.FromArgb(229,231,235);
            this.StatsCardTitleForeColor = Color.FromArgb(31,41,55);
            this.StatsCardTitleBackColor = Color.FromArgb(250,250,251);
            this.StatsCardSubTitleForeColor = Color.FromArgb(31,41,55);
            this.StatsCardSubTitleBackColor = Color.FromArgb(250,250,251);
            this.StatsCardValueForeColor = Color.FromArgb(31,41,55);
            this.StatsCardValueBackColor = Color.FromArgb(250,250,251);
            this.StatsCardValueBorderColor = Color.FromArgb(229,231,235);
            this.StatsCardValueHoverForeColor = Color.FromArgb(31,41,55);
            this.StatsCardValueHoverBackColor = Color.FromArgb(250,250,251);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(229,231,235);
            this.StatsCardInfoForeColor = Color.FromArgb(31,41,55);
            this.StatsCardInfoBackColor = Color.FromArgb(59,130,246);
            this.StatsCardInfoBorderColor = Color.FromArgb(229,231,235);
            this.StatsCardTrendForeColor = Color.FromArgb(31,41,55);
            this.StatsCardTrendBackColor = Color.FromArgb(250,250,251);
            this.StatsCardTrendBorderColor = Color.FromArgb(229,231,235);
        }
    }
}