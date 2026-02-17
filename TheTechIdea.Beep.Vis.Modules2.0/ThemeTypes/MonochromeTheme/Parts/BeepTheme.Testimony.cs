using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // Testimony/Testimonial Colors & Fonts
        public TypographyStyle TestimoniaTitleFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 14f, FontStyle.Bold);
        public TypographyStyle TestimoniaSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle TestimoniaUnSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public Color TestimonialBackColor { get; set; } = Color.WhiteSmoke;
        public Color TestimonialTextColor { get; set; } = Color.DimGray;
        public Color TestimonialNameColor { get; set; } = Color.Black;
        public Color TestimonialDetailsColor { get; set; } = Color.Gray;
        public Color TestimonialDateColor { get; set; } = Color.DarkGray;
        public Color TestimonialRatingColor { get; set; } = Color.Silver;
        public Color TestimonialStatusColor { get; set; } = Color.DimGray;
    }
}
