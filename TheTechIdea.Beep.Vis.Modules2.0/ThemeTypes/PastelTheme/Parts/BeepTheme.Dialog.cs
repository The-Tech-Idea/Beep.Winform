using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Dialog Button Colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color DialogBackColor { get; set; } = Color.FromArgb(245, 245, 245); // Light gray for dialog background
        public Color DialogForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray for dialog text
        public TypographyStyle DialogYesButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogNoButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogOkButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogCancelButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogWarningButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogErrorButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogInformationButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogQuestionButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogHelpButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogCloseButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(60, 60, 60), // Dark gray
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogYesButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogNoButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(200, 100, 100), // Soft red
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogOkButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(120, 160, 190), // Pastel blue
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green for yes button
        public Color DialogYesButtonForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(150, 190, 150); // Slightly darker pastel green for hover
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue
        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink
        public Color DialogCancelButtonForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.FromArgb(180, 200, 220); // Pastel lavender
        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink
        public Color DialogCloseButtonForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.FromArgb(200, 100, 100); // Soft red
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.FromArgb(200, 100, 100); // Soft red
        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(235, 203, 217); // Soft pastel pink
        public Color DialogNoButtonForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(200, 220, 240); // Light pastel blue
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.FromArgb(200, 100, 100); // Soft red
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(200, 100, 100); // Soft red
        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(170, 210, 170); // Pastel green
        public Color DialogOkButtonForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(150, 190, 150); // Slightly darker pastel green
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(120, 160, 190); // Pastel blue
        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(255, 220, 200); // Soft peach
        public Color DialogWarningButtonForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(235, 200, 180); // Darker peach
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 220, 200); // Soft peach
        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(240, 150, 150); // Soft coral
        public Color DialogErrorButtonForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(220, 130, 130); // Darker coral
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(240, 150, 150); // Soft coral
        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(200, 200, 240); // Pastel lavender
        public Color DialogInformationButtonForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(180, 180, 220); // Darker lavender
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(200, 200, 240); // Pastel lavender
        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint
        public Color DialogQuestionButtonForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(190, 210, 200); // Darker mint
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.FromArgb(60, 60, 60); // Dark gray
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(210, 230, 220); // Pastel mint
    }
}