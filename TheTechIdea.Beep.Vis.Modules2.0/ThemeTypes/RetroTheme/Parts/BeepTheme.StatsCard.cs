using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Stats Card Fonts & Colors
        public TypographyStyle StatsTitleFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 16, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle StatsSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.Black, IsUnderlined = false, IsStrikeout = false };
        public TypographyStyle StatsUnSelectedFont { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color StatsCardBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color StatsCardForeColor { get; set; } = Color.White;
        public Color StatsCardBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color StatsCardTitleForeColor { get; set; } = Color.White;
        public Color StatsCardTitleBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public TypographyStyle StatsCardTitleStyle { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 16, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Bold, FontStyle = FontStyle.Regular, TextColor = Color.White, IsUnderlined = false, IsStrikeout = false };
        public Color StatsCardSubTitleForeColor { get; set; } = Color.FromArgb(147, 161, 161);
        public Color StatsCardSubTitleBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public TypographyStyle StatsCardSubStyleStyle { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(147, 161, 161), IsUnderlined = false, IsStrikeout = false };
        public Color StatsCardValueForeColor { get; set; } = Color.FromArgb(181, 137, 0);
        public Color StatsCardValueBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color StatsCardValueBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public Color StatsCardValueHoverForeColor { get; set; } = Color.White;
        public Color StatsCardValueHoverBackColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color StatsCardValueHoverBorderColor { get; set; } = Color.FromArgb(131, 148, 150);
        public TypographyStyle StatsCardValueStyle { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 14, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Medium, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(181, 137, 0), IsUnderlined = false, IsStrikeout = false };
        public Color StatsCardInfoForeColor { get; set; } = Color.FromArgb(108, 123, 127);
        public Color StatsCardInfoBackColor { get; set; } = Color.FromArgb(0, 43, 54);
        public Color StatsCardInfoBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public TypographyStyle StatsCardInfoStyle { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(108, 123, 127), IsUnderlined = false, IsStrikeout = false };
        public Color StatsCardTrendForeColor { get; set; } = Color.FromArgb(38, 139, 210);
        public Color StatsCardTrendBackColor { get; set; } = Color.FromArgb(7, 54, 66);
        public Color StatsCardTrendBorderColor { get; set; } = Color.FromArgb(88, 110, 117);
        public TypographyStyle StatsCardTrendStyle { get; set; } = new TypographyStyle { FontFamily = "Courier New", FontSize = 12, LineHeight = 1.2f, LetterSpacing = 0.5f, FontWeight = FontWeight.Normal, FontStyle = FontStyle.Regular, TextColor = Color.FromArgb(38, 139, 210), IsUnderlined = false, IsStrikeout = false };
    }
}