using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.White;
        public Color LoginTitleColor { get; set; } = Color.Black;
<<<<<<< HEAD
        public Font LoginTitleFont { get; set; } = new Font("Segoe UI", 16F, FontStyle.Bold);
        public Color LoginSubtitleColor { get; set; } = Color.DarkBlue;
        public Font LoginSubtitleFont { get; set; } = new Font("Segoe UI", 12F, FontStyle.Regular);
        public Color LoginDescriptionColor { get; set; } = Color.Gray;
        public Font LoginDescriptionFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Regular);
        public Color LoginLinkColor { get; set; } = Color.Blue;
        public Font LoginLinkFont { get; set; } = new Font("Segoe UI", 10F, FontStyle.Underline);
        public Color LoginButtonBackgroundColor { get; set; } = Color.Blue;
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public Font LoginButtonFont { get; set; } = new Font("Segoe UI", 12F, FontStyle.Bold);
=======
        public TypographyStyle LoginTitleFont { get; set; } 
        public Color LoginSubtitleColor { get; set; } = Color.DarkBlue;
        public TypographyStyle LoginSubtitleFont { get; set; } 
        public Color LoginDescriptionColor { get; set; } = Color.Gray;
        public TypographyStyle LoginDescriptionFont { get; set; }
        public Color LoginLinkColor { get; set; } = Color.Blue;
        public TypographyStyle LoginLinkFont { get; set; }
        public Color LoginButtonBackgroundColor { get; set; } = Color.Blue;
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public TypographyStyle LoginButtonFont { get; set; } 
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
        public Color LoginDropdownBackgroundColor { get; set; } = Color.White;
        public Color LoginDropdownTextColor { get; set; } = Color.Black;
        public Color LoginLogoBackgroundColor { get; set; } = Color.LightGray;
    }
}
