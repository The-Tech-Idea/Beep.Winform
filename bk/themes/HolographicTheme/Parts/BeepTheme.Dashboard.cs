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
            this.DashboardBackColor = Color.FromArgb(15,16,32);
            this.DashboardCardBackColor = Color.FromArgb(15,16,32);
            this.DashboardCardHoverBackColor = Color.FromArgb(15,16,32);
            this.DashboardTitleForeColor = Color.FromArgb(245,247,255);
            this.DashboardTitleBackColor = Color.FromArgb(15,16,32);
            this.DashboardSubTitleForeColor = Color.FromArgb(245,247,255);
            this.DashboardSubTitleBackColor = Color.FromArgb(15,16,32);
            this.DashboardGradiantStartColor = Color.FromArgb(15,16,32);
            this.DashboardGradiantEndColor = Color.FromArgb(15,16,32);
            this.DashboardGradiantMiddleColor = Color.FromArgb(15,16,32);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}