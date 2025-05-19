using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Task Card Fonts & Colors
        public TypographyStyle TaskCardTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 16, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle TaskCardSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle TaskCardUnSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color TaskCardBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color TaskCardForeColor { get; set; } = Color.White;
        public Color TaskCardBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color TaskCardTitleForeColor { get; set; } = Color.White;
        public Color TaskCardTitleBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public TypographyStyle TaskCardTitleStyle { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 16, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color TaskCardSubTitleForeColor { get; set; } = Color.FromArgb(147, 161, 161);
        public Color TaskCardSubTitleBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public TypographyStyle TaskCardSubStyleStyle { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(147, 161, 161), IsUnderlined = false, IsStrikeout = false };
        public Color TaskCardMetricTextForeColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color TaskCardMetricTextBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color TaskCardMetricTextBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color TaskCardMetricTextHoverForeColor { get; set; } = Color.White;
        public Color TaskCardMetricTextHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color TaskCardMetricTextHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
        public TypographyStyle TaskCardMetricTextStyle { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 14, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(181, 137, 0), IsUnderlined = false, IsStrikeout = false };
        public Color TaskCardProgressValueForeColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color TaskCardProgressValueBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color TaskCardProgressValueBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public TypographyStyle TaskCardProgressValueStyle { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(38, 139, 210), IsUnderlined = false, IsStrikeout = false };
    }
}