using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class OneDarkTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(40,44,52);
            this.DashboardCardBackColor = Color.FromArgb(40,44,52);
            this.DashboardCardHoverBackColor = Color.FromArgb(40,44,52);
            this.DashboardTitleForeColor = Color.FromArgb(171,178,191);
            this.DashboardTitleBackColor = Color.FromArgb(40,44,52);
            this.DashboardSubTitleForeColor = Color.FromArgb(171,178,191);
            this.DashboardSubTitleBackColor = Color.FromArgb(40,44,52);
            this.DashboardGradiantStartColor = Color.FromArgb(40,44,52);
            this.DashboardGradiantEndColor = Color.FromArgb(40,44,52);
            this.DashboardGradiantMiddleColor = Color.FromArgb(40,44,52);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}