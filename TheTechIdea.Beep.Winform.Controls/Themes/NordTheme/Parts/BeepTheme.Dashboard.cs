using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(46,52,64);
            this.DashboardCardBackColor = Color.FromArgb(46,52,64);
            this.DashboardCardHoverBackColor = Color.FromArgb(46,52,64);
            this.DashboardTitleForeColor = Color.FromArgb(216,222,233);
            this.DashboardTitleBackColor = Color.FromArgb(46,52,64);
            this.DashboardSubTitleForeColor = Color.FromArgb(216,222,233);
            this.DashboardSubTitleBackColor = Color.FromArgb(46,52,64);
            this.DashboardGradiantStartColor = Color.FromArgb(46,52,64);
            this.DashboardGradiantEndColor = Color.FromArgb(46,52,64);
            this.DashboardGradiantMiddleColor = Color.FromArgb(46,52,64);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}