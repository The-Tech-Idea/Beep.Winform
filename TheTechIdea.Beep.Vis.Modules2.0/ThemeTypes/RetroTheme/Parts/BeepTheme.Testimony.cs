using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Testimony/Testimonial Colors & Fonts
        public TypographyStyle TestimoniaTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 16, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle TestimoniaSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle TestimoniaUnSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color TestimonialBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color TestimonialTextColor { get; set; } = Color.White;
        public Color TestimonialNameColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color TestimonialDetailsColor { get; set; } = Color.FromArgb(147, 161, 161);
        public Color TestimonialDateColor { get; set; } = Color.FromArgb(108, 123, 127);
        public Color TestimonialRatingColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color TestimonialStatusColor { get; set; } = Color.FromArgb(0, 128, 0);
    }
}