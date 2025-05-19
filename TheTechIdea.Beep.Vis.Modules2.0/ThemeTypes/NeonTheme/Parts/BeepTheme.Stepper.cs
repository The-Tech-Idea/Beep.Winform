using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Stepper Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle StepperTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(26, 188, 156), // Neon turquoise
            LineHeight = 1.4f,
            LetterSpacing = 0.4f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle StepperSelectedFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
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
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for stepper background
        public Color StepperForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for stepper text
        public Color StepperBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for stepper border
        public Color StepperItemForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for item text
        public TypographyStyle StepperItemFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
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
            TextColor = Color.FromArgb(46, 204, 113), // Neon green
            LineHeight = 1.3f,
            LetterSpacing = 0.3f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperItemHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover text
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray for hover background
        public Color StepperItemSelectedForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for selected item text
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected item background
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for selected item border
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise for item border
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow for hover border
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark for checked box text
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for checked box background
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green for checked box border
    }
}