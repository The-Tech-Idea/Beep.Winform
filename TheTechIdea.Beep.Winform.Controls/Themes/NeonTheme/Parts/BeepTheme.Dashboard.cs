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
            this.DashboardBackColor = PanelGradiantMiddleColor;
            this.DashboardCardBackColor = PanelGradiantMiddleColor;
            this.DashboardCardHoverBackColor = PanelGradiantMiddleColor;
            this.DashboardTitleForeColor = ForeColor;
            this.DashboardTitleBackColor = PanelGradiantMiddleColor;
            this.DashboardSubTitleForeColor = ForeColor;
            this.DashboardSubTitleBackColor = PanelGradiantMiddleColor;
            this.DashboardGradiantStartColor = PanelGradiantMiddleColor;
            this.DashboardGradiantEndColor = PanelGradiantMiddleColor;
            this.DashboardGradiantMiddleColor = PanelGradiantMiddleColor;
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}