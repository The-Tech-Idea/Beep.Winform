using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Stepper Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle StepperTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(100, 200, 180), // Bright teal
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for stepper background
        public Color StepperForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for stepper text
        public Color StepperBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for stepper border
        public Color StepperItemForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for item text
        public TypographyStyle StepperItemFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(150, 180, 200), // Soft aqua
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperItemHoverForeColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover text
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue for hover background
        public Color StepperItemSelectedForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for selected item text
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected item background
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for selected item border
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for item border
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua for hover border
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for checked box text
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for checked box background
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for checked box border
    }
}