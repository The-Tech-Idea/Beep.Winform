using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class FluentTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(245,246,248);
            this.DashboardCardBackColor = Color.FromArgb(245,246,248);
            this.DashboardCardHoverBackColor = Color.FromArgb(245,246,248);
            this.DashboardTitleForeColor = Color.FromArgb(32,32,32);
            this.DashboardTitleBackColor = Color.FromArgb(245,246,248);
            this.DashboardSubTitleForeColor = Color.FromArgb(32,32,32);
            this.DashboardSubTitleBackColor = Color.FromArgb(245,246,248);
            this.DashboardGradiantStartColor = Color.FromArgb(245,246,248);
            this.DashboardGradiantEndColor = Color.FromArgb(245,246,248);
            this.DashboardGradiantMiddleColor = Color.FromArgb(245,246,248);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}