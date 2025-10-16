using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class GNOMETheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(246,245,244);
            this.DashboardCardBackColor = Color.FromArgb(246,245,244);
            this.DashboardCardHoverBackColor = Color.FromArgb(246,245,244);
            this.DashboardTitleForeColor = Color.FromArgb(46,52,54);
            this.DashboardTitleBackColor = Color.FromArgb(246,245,244);
            this.DashboardSubTitleForeColor = Color.FromArgb(46,52,54);
            this.DashboardSubTitleBackColor = Color.FromArgb(246,245,244);
            this.DashboardGradiantStartColor = Color.FromArgb(246,245,244);
            this.DashboardGradiantEndColor = Color.FromArgb(246,245,244);
            this.DashboardGradiantMiddleColor = Color.FromArgb(246,245,244);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}