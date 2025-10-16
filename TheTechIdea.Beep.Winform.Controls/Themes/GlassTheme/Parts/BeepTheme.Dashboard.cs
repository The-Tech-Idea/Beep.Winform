using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GlassTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(236,244,255);
            this.DashboardCardBackColor = Color.FromArgb(236,244,255);
            this.DashboardCardHoverBackColor = Color.FromArgb(236,244,255);
            this.DashboardTitleForeColor = Color.FromArgb(17,24,39);
            this.DashboardTitleBackColor = Color.FromArgb(236,244,255);
            this.DashboardSubTitleForeColor = Color.FromArgb(17,24,39);
            this.DashboardSubTitleBackColor = Color.FromArgb(236,244,255);
            this.DashboardGradiantStartColor = Color.FromArgb(236,244,255);
            this.DashboardGradiantEndColor = Color.FromArgb(236,244,255);
            this.DashboardGradiantMiddleColor = Color.FromArgb(236,244,255);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}