using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class Metro2Theme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(243,242,241);
            this.DashboardCardBackColor = Color.FromArgb(243,242,241);
            this.DashboardCardHoverBackColor = Color.FromArgb(243,242,241);
            this.DashboardTitleForeColor = Color.FromArgb(32,31,30);
            this.DashboardTitleBackColor = Color.FromArgb(243,242,241);
            this.DashboardSubTitleForeColor = Color.FromArgb(32,31,30);
            this.DashboardSubTitleBackColor = Color.FromArgb(243,242,241);
            this.DashboardGradiantStartColor = Color.FromArgb(243,242,241);
            this.DashboardGradiantEndColor = Color.FromArgb(243,242,241);
            this.DashboardGradiantMiddleColor = Color.FromArgb(243,242,241);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}