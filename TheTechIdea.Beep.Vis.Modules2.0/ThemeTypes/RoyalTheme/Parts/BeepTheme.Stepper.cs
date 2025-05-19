using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RoyalTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StepperSelectedFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StepperUnSelectedFont { get; set; } = new TypographyStyle
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
        public Color StepperBackColor { get; set; } = Color.FromArgb(33, 37, 41);
        public Color StepperForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color StepperBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color StepperItemForeColor { get; set; } = Color.FromArgb(200, 200, 200);
        public TypographyStyle StepperItemFont { get; set; } = new TypographyStyle
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
        public TypographyStyle StepperSubTitleFont { get; set; } = new TypographyStyle
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
        public Color StepperItemHoverForeColor { get; set; } = Color.White;
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(52, 58, 64);
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(108, 117, 125);
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(75, 192, 192);
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(75, 192, 192);
    }
}