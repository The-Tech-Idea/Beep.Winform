using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Themes
{
    public sealed partial class ChatBubbleTheme
    {
        private void ApplyDashboard()
        {
            this.DashboardBackColor = Color.FromArgb(245,248,255);
            this.DashboardCardBackColor = Color.FromArgb(245,248,255);
            this.DashboardCardHoverBackColor = Color.FromArgb(245,248,255);
            this.DashboardTitleForeColor = Color.FromArgb(24,28,35);
            this.DashboardTitleBackColor = Color.FromArgb(245,248,255);
            this.DashboardSubTitleForeColor = Color.FromArgb(24,28,35);
            this.DashboardSubTitleBackColor = Color.FromArgb(245,248,255);
            this.DashboardGradiantStartColor = Color.FromArgb(245,248,255);
            this.DashboardGradiantEndColor = Color.FromArgb(245,248,255);
            this.DashboardGradiantMiddleColor = Color.FromArgb(245,248,255);
            this.DashboardGradiantDirection = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        }
    }
}