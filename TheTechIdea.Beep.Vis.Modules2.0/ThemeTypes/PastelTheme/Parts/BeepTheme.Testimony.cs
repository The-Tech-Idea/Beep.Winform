using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Testimony/Testimonial Colors & Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle TestimoniaTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 18f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
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
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
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
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color TestimonialBackColor { get; set; } = Color.FromArgb(255, 255, 255); // White for background
        public Color TestimonialTextColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for text
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for name
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(100, 100, 100); // Medium gray for details
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(140, 140, 140); // Slightly lighter gray for date
        public Color TestimonialRatingColor { get; set; } = Color.FromArgb(255, 220, 200); // Soft peach for rating
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for status
    }
}