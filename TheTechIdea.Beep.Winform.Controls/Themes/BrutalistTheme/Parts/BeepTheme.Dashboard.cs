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
            this.DashboardBackColor = SurfaceColor;
            this.DashboardCardBackColor = SurfaceColor;
            this.DashboardCardHoverBackColor = SurfaceColor;
            this.DashboardTitleForeColor = ForeColor;
            this.DashboardTitleBackColor = SurfaceColor;
            this.DashboardSubTitleForeColor = ForeColor;
            this.DashboardSubTitleBackColor = SurfaceColor;
            this.DashboardGradiantStartColor = SurfaceColor;
            this.DashboardGradiantEndColor = SurfaceColor;
            this.DashboardGradiantMiddleColor = SurfaceColor;
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
        }
    }
}