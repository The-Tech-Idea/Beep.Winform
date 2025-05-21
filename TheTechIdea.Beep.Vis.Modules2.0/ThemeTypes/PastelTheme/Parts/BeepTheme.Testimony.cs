using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Testimony/Testimonial Colors & Fonts
        public TypographyStyle TestimoniaTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle TestimoniaSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle TestimoniaUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public Color TestimonialBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color TestimonialTextColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(150, 150, 150);
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(150, 150, 150);
        public Color TestimonialRatingColor { get; set; } = Color.FromArgb(255, 215, 0);
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(180, 255, 180);
    }
}