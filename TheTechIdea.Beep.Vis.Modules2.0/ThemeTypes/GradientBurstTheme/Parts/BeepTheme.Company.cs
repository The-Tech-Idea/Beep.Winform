using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.White;
<<<<<<< HEAD
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(33, 33, 33);    // Almost Black
        public Font CompanyTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);

        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(25, 118, 210);   // Blue
        public Font CompanySubTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);

        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(97, 97, 97);     // Gray
        public Font CompanyDescriptionFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Italic);

        public Color CompanyLinkColor { get; set; } = Color.FromArgb(0, 123, 255);    // Bootstrap blue
        public Font CompanyLinkFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Underline);

        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(63, 81, 181);    // Indigo
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        public Font CompanyButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);

=======
        public Color CompanyTitleColor { get; set; } = Color.Black;
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
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
        public Color CompanyDropdownBackgroundColor { get; set; } = Color.White;
        public Color CompanyDropdownTextColor { get; set; } = Color.Black;

        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(158, 158, 158);  // Medium Gray
    }
}
