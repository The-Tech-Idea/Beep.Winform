using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Testimony/Testimonial Colors & Fonts
        public TypographyStyle TestimoniaTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 18,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 215, 0), // Gold
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TestimoniaSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112), // Deep midnight blue
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TestimoniaUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 200, 220), // Soft silver
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TestimonialBackColor { get; set; } = Color.FromArgb(245, 245, 220); // Beige
        public Color TestimonialTextColor { get; set; } = Color.FromArgb(25, 25, 112); // Deep midnight blue
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(65, 65, 145); // Royal blue
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(200, 200, 220); // Soft silver
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(150, 150, 170); // Muted silver
        public Color TestimonialRatingColor { get; set; } = Color.FromArgb(255, 215, 0); // Gold
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(0, 128, 0); // Emerald
    }
}