using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Testimony/Testimonial Colors & Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle TestimoniaTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.4f,
            LetterSpacing = 0.5f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle TestimoniaSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
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
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TestimonialBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray for background
        public Color TestimonialTextColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for text
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for name
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple for details
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(100, 100, 120); // Muted gray-blue for date
        public Color TestimonialRatingColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for rating
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for status
    }
}