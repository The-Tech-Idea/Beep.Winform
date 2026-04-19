using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class SpringTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(25, 25, 112),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StepperSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
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
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperBackColor { get; set; } = Color.FromArgb(240, 248, 255);
        public Color StepperForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color StepperBorderColor { get; set; } = Color.FromArgb(173, 216, 230);
        public Color StepperItemForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public TypographyStyle StepperItemFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(50, 50, 50),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StepperSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Segoe UI",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(70, 70, 70),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperItemHoverForeColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(144, 238, 144);
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(50, 205, 50);
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(60, 179, 113);
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(34, 139, 34);
    }
}