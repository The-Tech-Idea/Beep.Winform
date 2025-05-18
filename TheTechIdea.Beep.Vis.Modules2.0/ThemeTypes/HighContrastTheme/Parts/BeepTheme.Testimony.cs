using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Testimony/Testimonial Colors & Fonts
        public Font TestimoniaTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font TestimoniaSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font TestimoniaUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        public Color TestimonialBackColor { get; set; } = Color.Black;
        public Color TestimonialTextColor { get; set; } = Color.White;
        public Color TestimonialNameColor { get; set; } = Color.Yellow;
        public Color TestimonialDetailsColor { get; set; } = Color.LightGray;
        public Color TestimonialDateColor { get; set; } = Color.Gray;
        public Color TestimonialRatingColor { get; set; } = Color.Yellow;
        public Color TestimonialStatusColor { get; set; } = Color.Lime;
    }
}
