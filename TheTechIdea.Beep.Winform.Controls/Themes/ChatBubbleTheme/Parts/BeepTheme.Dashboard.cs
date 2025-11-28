using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = PanelGradiantStartColor;
            this.DashboardCardBackColor = PanelGradiantStartColor;
            this.DashboardCardHoverBackColor = PanelGradiantMiddleColor;
            this.DashboardTitleForeColor = ForeColor;
            this.DashboardTitleBackColor = PanelGradiantStartColor;
            this.DashboardSubTitleForeColor = ForeColor;
            this.DashboardSubTitleBackColor = PanelGradiantStartColor;
            this.DashboardGradiantStartColor = PanelGradiantStartColor;
            this.DashboardGradiantEndColor = PanelGradiantEndColor;
            this.DashboardGradiantMiddleColor = PanelGradiantMiddleColor;
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}