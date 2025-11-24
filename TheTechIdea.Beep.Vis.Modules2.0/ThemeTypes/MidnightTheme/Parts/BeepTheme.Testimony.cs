using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Testimony/Testimonial Colors & Fonts
        public TypographyStyle  TestimoniaTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  TestimoniaSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle  TestimoniaUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Regular);

        public Color TestimonialBackColor { get; set; } = Color.FromArgb(30, 30, 40);
        public Color TestimonialTextColor { get; set; } = Color.LightGray;
        public Color TestimonialNameColor { get; set; } = Color.LightSteelBlue;
        public Color TestimonialDetailsColor { get; set; } = Color.Gray;
        public Color TestimonialDateColor { get; set; } = Color.DarkGray;
        public Color TestimonialRatingColor { get; set; } = Color.Gold;
        public Color TestimonialStatusColor { get; set; } = Color.LightGreen;
    }
}
