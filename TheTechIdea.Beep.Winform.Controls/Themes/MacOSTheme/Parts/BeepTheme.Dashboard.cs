using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class MacOSTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(250,250,252);
            this.DashboardCardBackColor = Color.FromArgb(250,250,252);
            this.DashboardCardHoverBackColor = Color.FromArgb(250,250,252);
            this.DashboardTitleForeColor = Color.FromArgb(28,28,30);
            this.DashboardTitleBackColor = Color.FromArgb(250,250,252);
            this.DashboardSubTitleForeColor = Color.FromArgb(28,28,30);
            this.DashboardSubTitleBackColor = Color.FromArgb(250,250,252);
            this.DashboardGradiantStartColor = Color.FromArgb(250,250,252);
            this.DashboardGradiantEndColor = Color.FromArgb(250,250,252);
            this.DashboardGradiantMiddleColor = Color.FromArgb(250,250,252);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}