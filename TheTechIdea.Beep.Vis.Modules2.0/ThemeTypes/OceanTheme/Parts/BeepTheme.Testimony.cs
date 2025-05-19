using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Testimony/Testimonial Colors & Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle TestimoniaTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TestimoniaSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TestimoniaUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TestimonialBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue for background
        public Color TestimonialTextColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for text
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for name
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(120, 150, 180); // Muted blue for details
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(120, 150, 180); // Muted blue for date
        public Color TestimonialRatingColor { get; set; } = Color.FromArgb(255, 180, 90); // Soft orange for rating
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for status
    }
}