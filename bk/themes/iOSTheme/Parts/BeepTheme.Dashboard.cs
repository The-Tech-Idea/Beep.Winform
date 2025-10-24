using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class iOSTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(242,242,247);
            this.DashboardCardBackColor = Color.FromArgb(242,242,247);
            this.DashboardCardHoverBackColor = Color.FromArgb(242,242,247);
            this.DashboardTitleForeColor = Color.FromArgb(28,28,30);
            this.DashboardTitleBackColor = Color.FromArgb(242,242,247);
            this.DashboardSubTitleForeColor = Color.FromArgb(28,28,30);
            this.DashboardSubTitleBackColor = Color.FromArgb(242,242,247);
            this.DashboardGradiantStartColor = Color.FromArgb(242,242,247);
            this.DashboardGradiantEndColor = Color.FromArgb(242,242,247);
            this.DashboardGradiantMiddleColor = Color.FromArgb(242,242,247);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}