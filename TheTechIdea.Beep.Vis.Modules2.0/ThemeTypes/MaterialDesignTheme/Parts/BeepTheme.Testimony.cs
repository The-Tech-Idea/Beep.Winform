using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Testimony/Testimonial Colors & Fonts with Material Design defaults
        public TypographyStyle  TestimoniaTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 14f, FontStyle.Bold);
        public TypographyStyle  TestimoniaSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold);
        public TypographyStyle  TestimoniaUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);

        public Color TestimonialBackColor { get; set; } = Color.White;
        public Color TestimonialTextColor { get; set; } = Color.FromArgb(33, 33, 33); // Grey 900
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(21, 101, 192); // Blue 700
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(158, 158, 158); // Grey 500
        public Color TestimonialRatingColor { get; set; } = Color.FromArgb(255, 193, 7); // Amber 500 (gold-like)
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(76, 175, 80); // Green 500
    }
}
