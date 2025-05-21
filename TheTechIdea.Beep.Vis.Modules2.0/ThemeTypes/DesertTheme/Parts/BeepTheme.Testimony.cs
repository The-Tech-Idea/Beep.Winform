using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Testimony/Testimonial Colors & Fonts
        public TypographyStyle TestimoniaTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle TestimoniaSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle TestimoniaUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);

        public Color TestimonialBackColor { get; set; } = Color.FromArgb(255, 244, 229); // Light warm sand
        public Color TestimonialTextColor { get; set; } = Color.FromArgb(102, 51, 0);    // Dark brown
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(153, 102, 51);  // Soft brown
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(160, 82, 45); // Sienna
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(139, 69, 19);   // Saddle Brown
        public Color TestimonialRatingColor { get; set; } = Color.FromArgb(218, 165, 32); // Goldenrod (warm gold)
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(85, 107, 47);  // Dark olive green
    }
}
