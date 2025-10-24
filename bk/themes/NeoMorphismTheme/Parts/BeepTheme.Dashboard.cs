using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class NeoMorphismTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(236,240,243);
            this.DashboardCardBackColor = Color.FromArgb(236,240,243);
            this.DashboardCardHoverBackColor = Color.FromArgb(236,240,243);
            this.DashboardTitleForeColor = Color.FromArgb(58,66,86);
            this.DashboardTitleBackColor = Color.FromArgb(236,240,243);
            this.DashboardSubTitleForeColor = Color.FromArgb(58,66,86);
            this.DashboardSubTitleBackColor = Color.FromArgb(236,240,243);
            this.DashboardGradiantStartColor = Color.FromArgb(236,240,243);
            this.DashboardGradiantEndColor = Color.FromArgb(236,240,243);
            this.DashboardGradiantMiddleColor = Color.FromArgb(236,240,243);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
        }
    }
}