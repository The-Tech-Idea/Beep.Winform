using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Login Popover Colors with inline defaults
        public Color LoginPopoverBackgroundColor { get; set; } = Color.White;
        public Color LoginTitleColor { get; set; } = Color.Black;
        public Font LoginTitleFont { get; set; } = new Font("Roboto", 18f, FontStyle.Bold);
        public Color LoginSubtitleColor { get; set; } = Color.DarkBlue;
        public Font LoginSubtitleFont { get; set; } = new Font("Roboto", 14f, FontStyle.Regular);
        public Color LoginDescriptionColor { get; set; } = Color.Gray;
        public Font LoginDescriptionFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);
        public Color LoginLinkColor { get; set; } = Color.FromArgb(33, 150, 243); // Material Blue 500
        public Font LoginLinkFont { get; set; } = new Font("Roboto", 12f, FontStyle.Underline);
        public Color LoginButtonBackgroundColor { get; set; } = Color.FromArgb(33, 150, 243); // Material Blue 500
        public Color LoginButtonTextColor { get; set; } = Color.White;
        public Font LoginButtonFont { get; set; } = new Font("Roboto", 14f, FontStyle.Bold);
        public Color LoginDropdownBackgroundColor { get; set; } = Color.White;
        public Color LoginDropdownTextColor { get; set; } = Color.Black;
        public Color LoginLogoBackgroundColor { get; set; } = Color.LightGray;
    }
}
