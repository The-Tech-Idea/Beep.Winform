using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Testimony/Testimonial Colors & Fonts
        public TypographyStyle TestimoniaTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TestimoniaSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TestimoniaUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(192, 192, 192),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TestimonialBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color TestimonialTextColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color TestimonialNameColor { get; set; } = Color.White;
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(160, 160, 160);
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color TestimonialRatingColor { get; set; } = Color.FromArgb(255, 165, 0);
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(64, 255, 64);
    }
}