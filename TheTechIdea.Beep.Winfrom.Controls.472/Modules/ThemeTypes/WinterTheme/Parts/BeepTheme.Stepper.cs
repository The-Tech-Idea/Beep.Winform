using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class WinterTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 16,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(27, 62, 92),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StepperSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
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
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color StepperForeColor { get; set; } = Color.FromArgb(27, 62, 92);
        public Color StepperBorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color StepperItemForeColor { get; set; } = Color.FromArgb(200, 220, 240);
        public TypographyStyle StepperItemFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StepperSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(200, 220, 240),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperItemHoverForeColor { get; set; } = Color.White;
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(60, 100, 140);
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(120, 169, 255);
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(80, 120, 160);
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(100, 149, 237);
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(120, 169, 255);
    }
}