using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Side Menu Fonts & Colors
        public Font SideMenuTitleFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);
        public Font SideMenuSubTitleFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Italic);
        public Font SideMenuTextFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Regular);

        public Color SideMenuBackColor { get; set; } = Color.FromArgb(245, 222, 179); // wheat, light desert sand
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135); // burlywood, warm hover
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(210, 180, 140); // tan, selected background

        public Color SideMenuForeColor { get; set; } = Color.FromArgb(101, 67, 33); // dark brown text
        public Color SideMenuSelectedForeColor { get; set; } = Color.FromArgb(255, 255, 240); // ivory for selected text
        public Color SideMenuHoverForeColor { get; set; } = Color.FromArgb(139, 69, 19); // saddle brown hover text

        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(160, 82, 45); // sienna border

        public Color SideMenuTitleTextColor { get; set; } = Color.FromArgb(92, 64, 51); // dark coffee brown
        public Color SideMenuTitleBackColor { get; set; } = Color.FromArgb(250, 235, 215); // antique white background

        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(92, 64, 51),
            LineHeight = 1.2f
        };

        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(139, 115, 85); // lighter brown
        public Color SideMenuSubTitleBackColor { get; set; } = Color.FromArgb(245, 222, 179); // wheat

        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(139, 115, 85),
            LineHeight = 1.15f
        };

        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(245, 222, 179);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(210, 180, 140);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
