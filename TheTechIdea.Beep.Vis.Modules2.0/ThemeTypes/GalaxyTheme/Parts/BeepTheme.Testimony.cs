using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Testimony/Testimonial Colors & Fonts
<<<<<<< HEAD
        public Font TestimoniaTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Font TestimoniaSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font TestimoniaUnSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);

        public Color TestimonialBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor
        public Color TestimonialTextColor { get; set; } = Color.White;
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(0xA0, 0xA0, 0xFF); // Soft violet
        public Color TestimonialDetailsColor { get; set; } = Color.LightGray;
        public Color TestimonialDateColor { get; set; } = Color.Gray;
=======
        public TypographyStyle TestimoniaTitleFont { get; set; }
        public TypographyStyle TestimoniaSelectedFont { get; set; }
        public TypographyStyle TestimoniaUnSelectedFont { get; set; }
        public Color TestimonialBackColor { get; set; } = Color.White;
        public Color TestimonialTextColor { get; set; } = Color.Black;
        public Color TestimonialNameColor { get; set; } = Color.DarkBlue;
        public Color TestimonialDetailsColor { get; set; } = Color.Gray;
        public Color TestimonialDateColor { get; set; } = Color.DarkGray;
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
        public Color TestimonialRatingColor { get; set; } = Color.Gold;
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(0x23, 0xB9, 0x5C); // Success green
    }
}
