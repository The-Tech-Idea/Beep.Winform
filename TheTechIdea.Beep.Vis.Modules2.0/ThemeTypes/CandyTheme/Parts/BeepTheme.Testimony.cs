using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Testimony/Testimonial Colors & Fonts

        public TypographyStyle TestimoniaTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 12f, FontStyle.Bold);
        public TypographyStyle TestimoniaSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);
        public TypographyStyle TestimoniaUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);

        // Background: soft pastel pink for a sweet look
        public Color TestimonialBackColor { get; set; } = Color.FromArgb(255, 224, 235);     // Pastel Pink

        // Main testimonial text: navy for easy reading
        public Color TestimonialTextColor { get; set; } = Color.FromArgb(44, 62, 80);        // Navy

        // Name: candy pink for attention and vibrance
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(240, 100, 180);     // Candy Pink

        // Details: mint for accent and harmony
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(127, 255, 212);  // Mint

        // Date: lavender, for subtle differentiation
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(206, 183, 255);     // Pastel Lavender

        // Rating: lemon yellow (gold-like), playful but readable
        public Color TestimonialRatingColor { get; set; } = Color.FromArgb(255, 223, 93);    // Lemon Yellow

        // Status: mint (approved), candy pink (pending), or pastel blue (rejected) as needed
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(127, 255, 212);   // Mint (success/active)
    }
}
