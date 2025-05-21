using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Testimony/Testimonial Colors & Fonts
//<<<<<<< HEAD
        public Font TestimoniaTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font TestimoniaSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font TestimoniaUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color TestimonialBackColor { get; set; } = Color.White;
        public Color TestimonialTextColor { get; set; } = Color.Black;
        public Color TestimonialNameColor { get; set; } = Color.DarkBlue;
        public Color TestimonialDetailsColor { get; set; } = Color.Gray;
        public Color TestimonialDateColor { get; set; } = Color.DarkGray;
        public Color TestimonialRatingColor { get; set; } = Color.Gold;
        public Color TestimonialStatusColor { get; set; } = Color.Green;
    }
}
