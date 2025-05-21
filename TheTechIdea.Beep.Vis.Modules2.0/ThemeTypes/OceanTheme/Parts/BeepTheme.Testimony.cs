using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Testimony/Testimonial Colors & Fonts
        public TypographyStyle TestimoniaTitleFont { get; set; } = new TypographyStyle() { FontSize = 16, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(0, 80, 120) };
        public TypographyStyle TestimoniaSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle TestimoniaUnSelectedFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(200, 255, 255) };
        public Color TestimonialBackColor { get; set; } = Color.FromArgb(240, 245, 250);
        public Color TestimonialTextColor { get; set; } = Color.FromArgb(0, 80, 120);
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(0, 105, 148);
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(0, 130, 180);
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(0, 120, 170);
        public Color TestimonialRatingColor { get; set; } = Color.FromArgb(255, 215, 0);
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(0, 200, 100);
    }
}