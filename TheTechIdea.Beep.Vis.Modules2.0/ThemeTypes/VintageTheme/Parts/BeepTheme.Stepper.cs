using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Stepper Fonts & Colors
        public TypographyStyle StepperTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 18,
            LineHeight = 1.2f,
            LetterSpacing = 0.3f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StepperSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StepperUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color StepperForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color StepperBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color StepperItemForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public TypographyStyle StepperItemFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StepperSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 14,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Italic,
            TextColor = Color.FromArgb(90, 45, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperItemHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color StepperItemSelectedForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(200, 180, 160);
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(101, 51, 0);
    }
}