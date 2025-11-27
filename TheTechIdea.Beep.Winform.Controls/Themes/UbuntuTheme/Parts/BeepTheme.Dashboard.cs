using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
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
            this.DashboardGradiantStartColor = PanelGradiantStartColor;
            this.DashboardGradiantEndColor = PanelGradiantEndColor;
            this.DashboardGradiantMiddleColor = PanelGradiantMiddleColor;
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}