using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeonTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(10,12,20);
            this.DashboardCardBackColor = Color.FromArgb(10,12,20);
            this.DashboardCardHoverBackColor = Color.FromArgb(10,12,20);
            this.DashboardTitleForeColor = Color.FromArgb(235,245,255);
            this.DashboardTitleBackColor = Color.FromArgb(10,12,20);
            this.DashboardSubTitleForeColor = Color.FromArgb(235,245,255);
            this.DashboardSubTitleBackColor = Color.FromArgb(10,12,20);
            this.DashboardGradiantStartColor = Color.FromArgb(10,12,20);
            this.DashboardGradiantEndColor = Color.FromArgb(10,12,20);
            this.DashboardGradiantMiddleColor = Color.FromArgb(10,12,20);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}