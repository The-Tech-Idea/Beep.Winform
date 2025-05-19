using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeumorphismTheme
    {
        // Dialog Button Colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color DialogBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray for dialog background
        public Color DialogForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray for dialog text
        public TypographyStyle DialogYesButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(50, 50, 60), // Dark gray
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
            TextColor = Color.FromArgb(90, 180, 90), // Soft green
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
            TextColor = Color.FromArgb(255, 90, 90), // Soft red
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
            TextColor = Color.FromArgb(90, 180, 90), // Soft green
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green for yes button
        public Color DialogYesButtonForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(80, 160, 80); // Darker green for hover
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green
        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray
        public Color DialogCancelButtonForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Darker gray
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.FromArgb(200, 200, 205); // Soft gray
        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray
        public Color DialogCloseButtonForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Darker gray
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red
        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(230, 230, 235); // Light gray
        public Color DialogNoButtonForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(210, 210, 215); // Darker gray
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red
        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green
        public Color DialogOkButtonForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(80, 160, 80); // Darker green
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(90, 180, 90); // Soft green
        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(255, 180, 90); // Soft orange
        public Color DialogWarningButtonForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(235, 160, 80); // Darker orange
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 180, 90); // Soft orange
        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red
        public Color DialogErrorButtonForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(235, 80, 80); // Darker red
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 90, 90); // Soft red
        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(80, 150, 200); // Soft blue
        public Color DialogInformationButtonForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(70, 130, 180); // Darker blue
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(80, 150, 200); // Soft blue
        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(150, 90, 180); // Soft purple
        public Color DialogQuestionButtonForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(130, 80, 160); // Darker purple
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.FromArgb(50, 50, 60); // Dark gray
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(150, 90, 180); // Soft purple
    }
}
