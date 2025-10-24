using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class BrutalistTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(250,250,250);
            this.DashboardCardBackColor = Color.FromArgb(250,250,250);
            this.DashboardCardHoverBackColor = Color.FromArgb(250,250,250);
            this.DashboardTitleForeColor = Color.FromArgb(20,20,20);
            this.DashboardTitleBackColor = Color.FromArgb(250,250,250);
            this.DashboardSubTitleForeColor = Color.FromArgb(20,20,20);
            this.DashboardSubTitleBackColor = Color.FromArgb(250,250,250);
            this.DashboardGradiantStartColor = Color.FromArgb(250,250,250);
            this.DashboardGradiantEndColor = Color.FromArgb(250,250,250);
            this.DashboardGradiantMiddleColor = Color.FromArgb(250,250,250);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
        }
    }
}