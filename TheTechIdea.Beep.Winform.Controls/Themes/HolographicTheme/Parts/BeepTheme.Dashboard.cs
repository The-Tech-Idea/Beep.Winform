using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class HolographicTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = BackgroundColor;
            this.DashboardCardBackColor = SurfaceColor;
            this.DashboardCardHoverBackColor = PanelGradiantStartColor;
            this.DashboardTitleForeColor = ForeColor;
            this.DashboardTitleBackColor = SurfaceColor;
            this.DashboardSubTitleForeColor = AccentColor;
            this.DashboardSubTitleBackColor = SurfaceColor;
            this.DashboardGradiantStartColor = PanelGradiantStartColor;
            this.DashboardGradiantEndColor = PanelGradiantEndColor;
            this.DashboardGradiantMiddleColor = PanelGradiantMiddleColor;
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}