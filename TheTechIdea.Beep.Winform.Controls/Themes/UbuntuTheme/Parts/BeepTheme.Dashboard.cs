using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class UbuntuTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(242,242,245);
            this.DashboardCardBackColor = Color.FromArgb(242,242,245);
            this.DashboardCardHoverBackColor = Color.FromArgb(242,242,245);
            this.DashboardTitleForeColor = Color.FromArgb(44,44,44);
            this.DashboardTitleBackColor = Color.FromArgb(242,242,245);
            this.DashboardSubTitleForeColor = Color.FromArgb(44,44,44);
            this.DashboardSubTitleBackColor = Color.FromArgb(242,242,245);
            this.DashboardGradiantStartColor = Color.FromArgb(242,242,245);
            this.DashboardGradiantEndColor = Color.FromArgb(242,242,245);
            this.DashboardGradiantMiddleColor = Color.FromArgb(242,242,245);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}