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
            this.DashboardBackColor = BackgroundColor;
            this.DashboardCardBackColor = PanelBackColor;
            this.DashboardCardHoverBackColor = PanelGradiantMiddleColor;
            this.DashboardTitleForeColor = ForeColor;
            this.DashboardTitleBackColor = PanelBackColor;
            this.DashboardSubTitleForeColor = ForeColor;
            this.DashboardSubTitleBackColor = PanelBackColor;
            this.DashboardGradiantStartColor = PanelBackColor;
            this.DashboardGradiantEndColor = PanelBackColor;
            this.DashboardGradiantMiddleColor = PanelGradiantMiddleColor;
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}