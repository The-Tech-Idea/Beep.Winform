using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Testimony/Testimonial Colors & Fonts
//<<<<<<< HEAD
        public TypographyStyle  TestimoniaTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  TestimoniaSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Bold);
        public TypographyStyle  TestimoniaUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12f, FontStyle.Regular);

        public Color TestimonialBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color TestimonialTextColor { get; set; } = Color.White;
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(0xA0, 0xA0, 0xFF); // Soft violet
        public Color TestimonialDetailsColor { get; set; } = Color.LightGray;
        public Color TestimonialDateColor { get; set; } = Color.Gray;
        public Color TestimonialRatingColor { get; set; } = Color.Gold;
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(0x23, 0xB9, 0x5C); // Success green
    }
}
