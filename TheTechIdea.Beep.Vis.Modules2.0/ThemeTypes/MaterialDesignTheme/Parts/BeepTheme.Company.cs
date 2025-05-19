using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Company Colors
        public Color CompanyPopoverBackgroundColor { get; set; } = Color.White;
<<<<<<< HEAD
        public Color CompanyTitleColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Font CompanyTitleFont { get; set; } = new Font(new FontFamily("Roboto"), 14f, FontStyle.Bold);
        public Color CompanySubtitleColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public Font CompanySubTitleFont { get; set; } = new Font(new FontFamily("Roboto"), 12f, FontStyle.Regular);
        public Color CompanyDescriptionColor { get; set; } = Color.FromArgb(158, 158, 158); // Grey 500
        public Font CompanyDescriptionFont { get; set; } = new Font(new FontFamily("Roboto"), 10f, FontStyle.Regular);
        public Color CompanyLinkColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Font CompanyLinkFont { get; set; } = new Font(new FontFamily("Roboto"), 10f, FontStyle.Regular);
        public Color CompanyButtonBackgroundColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color CompanyButtonTextColor { get; set; } = Color.White;
        // Use Bold instead of Medium
        public Font CompanyButtonFont { get; set; } = new Font(new FontFamily("Roboto"), 10f, FontStyle.Bold);
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
        public Color CompanyDropdownTextColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color CompanyLogoBackgroundColor { get; set; } = Color.FromArgb(238, 238, 238); // Grey 200
    }
}
