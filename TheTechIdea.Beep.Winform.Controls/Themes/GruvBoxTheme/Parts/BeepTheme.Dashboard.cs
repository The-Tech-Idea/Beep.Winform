using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GruvBoxTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(40,40,40);
            this.DashboardCardBackColor = Color.FromArgb(40,40,40);
            this.DashboardCardHoverBackColor = Color.FromArgb(40,40,40);
            this.DashboardTitleForeColor = Color.FromArgb(235,219,178);
            this.DashboardTitleBackColor = Color.FromArgb(40,40,40);
            this.DashboardSubTitleForeColor = Color.FromArgb(235,219,178);
            this.DashboardSubTitleBackColor = Color.FromArgb(40,40,40);
            this.DashboardGradiantStartColor = Color.FromArgb(40,40,40);
            this.DashboardGradiantEndColor = Color.FromArgb(40,40,40);
            this.DashboardGradiantMiddleColor = Color.FromArgb(40,40,40);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}