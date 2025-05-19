using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color CompanyTitleColor { get; set; } = Color.Black;
<<<<<<< HEAD
        public Font CompanyTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);

        public Color CompanySubtitleColor { get; set; } = Color.DarkBlue;
        public Font CompanySubTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Italic);

        public Color CompanyDescriptionColor { get; set; } = Color.Gray;
        public Font CompanyDescriptionFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color CompanyLinkColor { get; set; } = Color.SteelBlue;
        public Font CompanyLinkFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Underline);

        public Color CompanyButtonBackgroundColor { get; set; } = Color.SkyBlue;
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public Font CompanyButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);

        public Color CompanyDropdownBackgroundColor { get; set; } = Color.WhiteSmoke;
=======
        public TypographyStyle CompanyTitleFont { get; set; } 
        public Color CompanySubtitleColor { get; set; } = Color.DarkBlue;
        public TypographyStyle CompanySubTitleFont { get; set; }
        public Color CompanyDescriptionColor { get; set; } = Color.Gray;
        public TypographyStyle CompanyDescriptionFont { get; set; }
        public Color CompanyLinkColor { get; set; } = Color.Gray;
        public TypographyStyle CompanyLinkFont { get; set; }
        public Color CompanyButtonBackgroundColor { get; set; } = Color.Blue;
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public TypographyStyle CompanyButtonFont { get; set; }
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.White;
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
        public Color CompanyDropdownTextColor { get; set; } = Color.Black;

        public Color CompanyLogoBackgroundColor { get; set; } = Color.LightGray;
    }
}
