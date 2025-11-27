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
            this.DashboardBackColor = SurfaceColor;
            this.DashboardCardBackColor = SurfaceColor;
            this.DashboardCardHoverBackColor = SecondaryColor;
            this.DashboardTitleForeColor = ForeColor;
            this.DashboardTitleBackColor = SurfaceColor;
            this.DashboardSubTitleForeColor = SecondaryColor;
            this.DashboardSubTitleBackColor = SurfaceColor;
            this.DashboardGradiantStartColor = SecondaryColor;
            this.DashboardGradiantEndColor = SurfaceColor;
            this.DashboardGradiantMiddleColor = SurfaceColor;
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}