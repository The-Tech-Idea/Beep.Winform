using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Stepper Fonts & Colors
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public TypographyStyle StepperTitleFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 16f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
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
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
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
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for stepper background
        public Color StepperForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for stepper text
        public Color StepperBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for stepper border
        public Color StepperItemForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for item text
        public TypographyStyle StepperItemFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
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
            TextColor = Color.FromArgb(100, 100, 100), // Medium gray
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color StepperItemHoverForeColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover text
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue for hover background
        public Color StepperItemSelectedForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for selected item text
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint for selected item background
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for selected item border
        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender for item border
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue for hover border
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for checked box text
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for checked box background
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for checked box border
    }
}