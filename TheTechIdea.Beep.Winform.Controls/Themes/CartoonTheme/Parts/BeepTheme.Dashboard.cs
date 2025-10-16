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
            this.DashboardBackColor = Color.FromArgb(255,251,235);
            this.DashboardCardBackColor = Color.FromArgb(255,251,235);
            this.DashboardCardHoverBackColor = Color.FromArgb(255,251,235);
            this.DashboardTitleForeColor = Color.FromArgb(33,37,41);
            this.DashboardTitleBackColor = Color.FromArgb(255,251,235);
            this.DashboardSubTitleForeColor = Color.FromArgb(33,37,41);
            this.DashboardSubTitleBackColor = Color.FromArgb(255,251,235);
            this.DashboardGradiantStartColor = Color.FromArgb(255,251,235);
            this.DashboardGradiantEndColor = Color.FromArgb(255,251,235);
            this.DashboardGradiantMiddleColor = Color.FromArgb(255,251,235);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}