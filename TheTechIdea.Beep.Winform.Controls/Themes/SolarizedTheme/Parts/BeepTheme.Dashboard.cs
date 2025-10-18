using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class SolarizedTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(0,43,54);
            this.DashboardCardBackColor = Color.FromArgb(0,43,54);
            this.DashboardCardHoverBackColor = Color.FromArgb(0,43,54);
            this.DashboardTitleForeColor = Color.FromArgb(147,161,161);
            this.DashboardTitleBackColor = Color.FromArgb(0,43,54);
            this.DashboardSubTitleForeColor = Color.FromArgb(147,161,161);
            this.DashboardSubTitleBackColor = Color.FromArgb(0,43,54);
            this.DashboardGradiantStartColor = Color.FromArgb(0,43,54);
            this.DashboardGradiantEndColor = Color.FromArgb(0,43,54);
            this.DashboardGradiantMiddleColor = Color.FromArgb(0,43,54);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}