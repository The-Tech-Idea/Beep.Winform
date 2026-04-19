using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Side Menu Fonts & Colors

        public TypographyStyle SideMenuTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 14f, FontStyle.Bold);
        public TypographyStyle SideMenuSubTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Italic);
        public TypographyStyle SideMenuTextFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public Color SideMenuBackColor { get; set; } = Color.FromArgb(255, 224, 235);       // Pastel Pink
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255);  // Baby Blue
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint

        public Color SideMenuForeColor { get; set; } = Color.FromArgb(44, 62, 80);           // Navy
        public Color SideMenuSelectedForeColor { get; set; } = Color.FromArgb(255, 223, 93); // Lemon Yellow
        public Color SideMenuHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180);   // Candy Pink

        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(127, 255, 212);      // Mint

        public Color SideMenuTitleTextColor { get; set; } = Color.FromArgb(240, 100, 180);   // Candy Pink
        public Color SideMenuTitleBackColor { get; set; } = Color.FromArgb(228, 222, 255);   // Pastel Lavender

        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Comic Sans MS",
            FontSize = 13f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(240, 100, 180), // Candy Pink
            LetterSpacing = 0.12f,
            LineHeight = 1.15f
        };

        public Color SideMenuSubTitleTextColor { get; set; } = Color.FromArgb(127, 255, 212);   // Mint
        public Color SideMenuSubTitleBackColor { get; set; } = Color.FromArgb(255, 224, 235);   // Pastel Pink

        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 11f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(127, 255, 212), // Mint
            LetterSpacing = 0.08f,
            LineHeight = 1.1f
        };

        // Gradient: pastel pink -> mint -> lemon
        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(204, 255, 240);   // Mint
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
