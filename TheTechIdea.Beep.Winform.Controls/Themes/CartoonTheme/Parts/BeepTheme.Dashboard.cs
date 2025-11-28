using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class CartoonTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = SurfaceColor;
            this.DashboardCardBackColor = SurfaceColor;
            this.DashboardCardHoverBackColor = PanelGradiantStartColor;
            this.DashboardTitleForeColor = ForeColor;
            this.DashboardTitleBackColor = SurfaceColor;
            this.DashboardSubTitleForeColor = ForeColor;
            this.DashboardSubTitleBackColor = SurfaceColor;
            this.DashboardGradiantStartColor = PanelGradiantStartColor;
            this.DashboardGradiantEndColor = PanelGradiantEndColor;
            this.DashboardGradiantMiddleColor = PanelGradiantMiddleColor;
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}