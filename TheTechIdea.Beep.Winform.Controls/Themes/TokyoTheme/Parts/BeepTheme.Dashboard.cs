using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class TokyoTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(26,27,38);
            this.DashboardCardBackColor = Color.FromArgb(26,27,38);
            this.DashboardCardHoverBackColor = Color.FromArgb(26,27,38);
            this.DashboardTitleForeColor = Color.FromArgb(192,202,245);
            this.DashboardTitleBackColor = Color.FromArgb(26,27,38);
            this.DashboardSubTitleForeColor = Color.FromArgb(192,202,245);
            this.DashboardSubTitleBackColor = Color.FromArgb(26,27,38);
            this.DashboardGradiantStartColor = Color.FromArgb(26,27,38);
            this.DashboardGradiantEndColor = Color.FromArgb(26,27,38);
            this.DashboardGradiantMiddleColor = Color.FromArgb(26,27,38);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}