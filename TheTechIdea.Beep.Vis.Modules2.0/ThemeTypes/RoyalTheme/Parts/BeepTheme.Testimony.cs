using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Testimony/Testimonial Colors & Fonts
        public TypographyStyle TestimoniaTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TestimoniaSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TestimoniaUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 200),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TestimonialBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color TestimonialTextColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color TestimonialNameColor { get; set; } = Color.White;
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color TestimonialRatingColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(75, 192, 192);
    }
}