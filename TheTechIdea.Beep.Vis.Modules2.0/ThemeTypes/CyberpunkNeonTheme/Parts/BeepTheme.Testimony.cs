using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Testimony/Testimonial Colors & Fonts

        public TypographyStyle TestimoniaTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 14f, FontStyle.Bold);
        public TypographyStyle TestimoniaSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle TestimoniaUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 12f, FontStyle.Regular);

        public Color TestimonialBackColor { get; set; } = Color.FromArgb(18, 18, 32);             // Dark background
        public Color TestimonialTextColor { get; set; } = Color.FromArgb(0, 255, 255);            // Neon cyan text
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(255, 0, 255);            // Neon magenta name
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(255, 255, 0);         // Neon yellow details
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(0, 255, 128);            // Neon green date
        public Color TestimonialRatingColor { get; set; } = Color.Gold;                           // Gold rating stars
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(0, 255, 128);          // Neon green status
    }
}
