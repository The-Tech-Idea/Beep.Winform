using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MinimalTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = BackgroundColor;
            this.DashboardCardBackColor = SurfaceColor;
            this.DashboardCardHoverBackColor = ThemeUtil.Lighten(SurfaceColor, 0.02);
            this.DashboardTitleForeColor = ForeColor;
            this.DashboardTitleBackColor = BackgroundColor;
            this.DashboardSubTitleForeColor = SecondaryColor;
            this.DashboardSubTitleBackColor = BackgroundColor;
            this.DashboardGradiantStartColor = BackgroundColor;
            this.DashboardGradiantEndColor = BackgroundColor;
            this.DashboardGradiantMiddleColor = BackgroundColor;
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}
