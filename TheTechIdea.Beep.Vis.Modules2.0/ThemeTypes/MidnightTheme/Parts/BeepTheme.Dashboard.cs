﻿using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Dashboard Colors & Fonts
        public Font DashboardTitleFont { get; set; } = new Font("Segoe UI", 20f, FontStyle.Bold);
        public Font DashboardSubTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Regular);
        public Color DashboardBackColor { get; set; } = Color.FromArgb(18, 18, 18); // Very dark background
        public Color DashboardCardBackColor { get; set; } = Color.FromArgb(30, 30, 30); // Darker card background
        public Color DashboardCardHoverBackColor { get; set; } = Color.FromArgb(45, 45, 45); // Hover effect slightly lighter
        public Color DashboardTitleForeColor { get; set; } = Color.WhiteSmoke;
        public Color DashboardTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 20f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.WhiteSmoke
        };
        public Color DashboardSubTitleForeColor { get; set; } = Color.LightGray;
        public Color DashboardSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle DashboardSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.LightGray
        };
        public Color DashboardGradiantStartColor { get; set; } = Color.FromArgb(28, 28, 28);
        public Color DashboardGradiantEndColor { get; set; } = Color.FromArgb(18, 18, 18);
        public Color DashboardGradiantMiddleColor { get; set; } = Color.FromArgb(23, 23, 23);
        public LinearGradientMode DashboardGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
