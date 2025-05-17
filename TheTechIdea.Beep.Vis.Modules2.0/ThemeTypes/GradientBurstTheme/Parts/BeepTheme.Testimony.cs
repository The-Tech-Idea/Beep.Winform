using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Testimony/Testimonial Colors & Fonts
        public Font TestimoniaTitleFont { get; set; }
        public Font TestimoniaSelectedFont { get; set; }
        public Font TestimoniaUnSelectedFont { get; set; }
        public Color TestimonialBackColor { get; set; } = Color.White;
        public Color TestimonialTextColor { get; set; } = Color.Black;
        public Color TestimonialNameColor { get; set; } = Color.DarkBlue;
        public Color TestimonialDetailsColor { get; set; } = Color.Gray;
        public Color TestimonialDateColor { get; set; } = Color.DarkGray;
        public Color TestimonialRatingColor { get; set; } = Color.Gold;
        public Color TestimonialStatusColor { get; set; } = Color.Green;
    }
}
