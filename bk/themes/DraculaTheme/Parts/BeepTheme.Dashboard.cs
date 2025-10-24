using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class DraculaTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(40,42,54);
            this.DashboardCardBackColor = Color.FromArgb(40,42,54);
            this.DashboardCardHoverBackColor = Color.FromArgb(40,42,54);
            this.DashboardTitleForeColor = Color.FromArgb(248,248,242);
            this.DashboardTitleBackColor = Color.FromArgb(40,42,54);
            this.DashboardSubTitleForeColor = Color.FromArgb(248,248,242);
            this.DashboardSubTitleBackColor = Color.FromArgb(40,42,54);
            this.DashboardGradiantStartColor = Color.FromArgb(40,42,54);
            this.DashboardGradiantEndColor = Color.FromArgb(40,42,54);
            this.DashboardGradiantMiddleColor = Color.FromArgb(40,42,54);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}