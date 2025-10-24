using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class PaperTheme
    {
        private void ApplyStatsCard()
        {
            this.StatsCardBackColor = Color.FromArgb(250,250,250);
            this.StatsCardForeColor = Color.FromArgb(33,33,33);
            this.StatsCardBorderColor = Color.FromArgb(224,224,224);
            this.StatsCardTitleForeColor = Color.FromArgb(33,33,33);
            this.StatsCardTitleBackColor = Color.FromArgb(250,250,250);
            this.StatsCardSubTitleForeColor = Color.FromArgb(33,33,33);
            this.StatsCardSubTitleBackColor = Color.FromArgb(250,250,250);
            this.StatsCardValueForeColor = Color.FromArgb(33,33,33);
            this.StatsCardValueBackColor = Color.FromArgb(250,250,250);
            this.StatsCardValueBorderColor = Color.FromArgb(224,224,224);
            this.StatsCardValueHoverForeColor = Color.FromArgb(33,33,33);
            this.StatsCardValueHoverBackColor = Color.FromArgb(250,250,250);
            this.StatsCardValueHoverBorderColor = Color.FromArgb(224,224,224);
            this.StatsCardInfoForeColor = Color.FromArgb(33,33,33);
            this.StatsCardInfoBackColor = Color.FromArgb(250,250,250);
            this.StatsCardInfoBorderColor = Color.FromArgb(224,224,224);
            this.StatsCardTrendForeColor = Color.FromArgb(33,33,33);
            this.StatsCardTrendBackColor = Color.FromArgb(250,250,250);
            this.StatsCardTrendBorderColor = Color.FromArgb(224,224,224);
        }
    }
}