using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class BeepTheme
    {
        // Login Popover Colors
        public Color LoginPopoverBackgroundColor { get; set; } = Color.White;
        public Color LoginTitleColor { get; set; } = Color.Black;
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
        public Color LoginDropdownBackgroundColor { get; set; } = Color.White;
        public Color LoginDropdownTextColor { get; set; } = Color.Black;
        public Color LoginLogoBackgroundColor { get; set; } = Color.LightGray;
    }
}
