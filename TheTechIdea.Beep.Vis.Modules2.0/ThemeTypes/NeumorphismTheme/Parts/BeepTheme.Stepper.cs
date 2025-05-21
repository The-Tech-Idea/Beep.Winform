using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Stepper Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle StepperTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.4f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StepperSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StepperUnSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for stepper background
        public Color StepperForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for stepper text
        public Color StepperBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for stepper border
        public Color StepperItemForeColor { get; set; } = Color.FromArgb(80, 80, 90); // Medium gray for item text
        public TypographyStyle StepperItemFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StepperSubTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 14f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(80, 80, 90), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperItemHoverForeColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover text
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Slightly darker gray for hover background
        public Color StepperItemSelectedForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for selected item text
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected item background
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for selected item border
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray for item border
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for hover border
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for checked box text
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for checked box background
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for checked box border
    }
}