using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Side Menu Fonts & Colors with Material Design defaults
        public Font SideMenuTitleFont { get; set; } = new Font("Roboto", 18f, FontStyle.Bold);
        public Font SideMenuSubTitleFont { get; set; } = new Font("Roboto", 14f, FontStyle.Regular);
        public Font SideMenuTextFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);

        public Color SideMenuBackColor { get; set; } = Color.FromArgb(250, 250, 250);
        public Color SideMenuHoverBackColor { get; set; } = Color.FromArgb(224, 224, 224);
        public Color SideMenuSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243);  // Material Blue 500

        public Color SideMenuForeColor { get; set; } = Color.Black;
        public Color SideMenuSelectedForeColor { get; set; } = Color.White;
        public Color SideMenuHoverForeColor { get; set; } = Color.FromArgb(33, 150, 243);

        public Color SideMenuBorderColor { get; set; } = Color.FromArgb(224, 224, 224);

        public Color SideMenuTitleTextColor { get; set; } = Color.Black;
        public Color SideMenuTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.Black
        };

        public Color SideMenuSubTitleTextColor { get; set; } = Color.DimGray;
        public Color SideMenuSubTitleBackColor { get; set; } = Color.Transparent;
        public TypographyStyle SideMenuSubTitleStyle { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 14f,
            FontWeight = FontWeight.Normal,
            TextColor = Color.DimGray
        };

        public Color SideMenuGradiantStartColor { get; set; } = Color.FromArgb(250, 250, 250);
        public Color SideMenuGradiantEndColor { get; set; } = Color.FromArgb(230, 230, 230);
        public Color SideMenuGradiantMiddleColor { get; set; } = Color.FromArgb(240, 240, 240);
        public LinearGradientMode SideMenuGradiantDirection { get; set; } = LinearGradientMode.Vertical;
    }
}
