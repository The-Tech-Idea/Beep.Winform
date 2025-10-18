using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ArcLinuxTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(245,246,247);
            this.DashboardCardBackColor = Color.FromArgb(245,246,247);
            this.DashboardCardHoverBackColor = Color.FromArgb(245,246,247);
            this.DashboardTitleForeColor = Color.FromArgb(43,45,48);
            this.DashboardTitleBackColor = Color.FromArgb(245,246,247);
            this.DashboardSubTitleForeColor = Color.FromArgb(43,45,48);
            this.DashboardSubTitleBackColor = Color.FromArgb(245,246,247);
            this.DashboardGradiantStartColor = Color.FromArgb(245,246,247);
            this.DashboardGradiantEndColor = Color.FromArgb(245,246,247);
            this.DashboardGradiantMiddleColor = Color.FromArgb(245,246,247);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}