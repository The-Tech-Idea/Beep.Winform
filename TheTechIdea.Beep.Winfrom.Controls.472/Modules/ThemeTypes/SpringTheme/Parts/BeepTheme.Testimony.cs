using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Testimony/Testimonial Colors & Fonts
        public TypographyStyle TestimoniaTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TestimoniaSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TestimoniaUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(100, 100, 100),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TestimonialBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color TestimonialTextColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(25, 25, 112);
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(100, 100, 100);
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(70, 70, 70);
        public Color TestimonialRatingColor { get; set; } = Color.FromArgb(255, 215, 0);
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(60, 179, 113);
    }
}