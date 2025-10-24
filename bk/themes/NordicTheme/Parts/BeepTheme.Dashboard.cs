using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NordicTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(250,250,251);
            this.DashboardCardBackColor = Color.FromArgb(250,250,251);
            this.DashboardCardHoverBackColor = Color.FromArgb(250,250,251);
            this.DashboardTitleForeColor = Color.FromArgb(31,41,55);
            this.DashboardTitleBackColor = Color.FromArgb(250,250,251);
            this.DashboardSubTitleForeColor = Color.FromArgb(31,41,55);
            this.DashboardSubTitleBackColor = Color.FromArgb(250,250,251);
            this.DashboardGradiantStartColor = Color.FromArgb(250,250,251);
            this.DashboardGradiantEndColor = Color.FromArgb(250,250,251);
            this.DashboardGradiantMiddleColor = Color.FromArgb(250,250,251);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}