using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Testimony/Testimonial Colors & Fonts
        public TypographyStyle TestimoniaTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14, FontStyle.Bold);
        public TypographyStyle TestimoniaSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Bold);
        public TypographyStyle TestimoniaUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 12, FontStyle.Regular);
        public Color TestimonialBackColor { get; set; } = Color.FromArgb(245, 255, 245); // Light greenish white
        public Color TestimonialTextColor { get; set; } = Color.DarkGreen;
        public Color TestimonialNameColor { get; set; } = Color.ForestGreen;
        public Color TestimonialDetailsColor { get; set; } = Color.DarkOliveGreen;
        public Color TestimonialDateColor { get; set; } = Color.OliveDrab;
        public Color TestimonialRatingColor { get; set; } = Color.Gold;
        public Color TestimonialStatusColor { get; set; } = Color.DarkGreen;
    }
}
