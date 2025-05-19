using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class OceanTheme
    {
        // Dialog Button Colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color DialogBackColor { get; set; } = Color.FromArgb(10, 25, 47); // Deep navy blue for dialog background
        public Color DialogForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white for dialog text
        public TypographyStyle DialogYesButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.Medium,
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
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
            TextColor = Color.FromArgb(240, 245, 255), // Light off-white
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal for yes button
        public Color DialogYesButtonForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(80, 180, 160); // Slightly darker teal for hover
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua
        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue
        public Color DialogCancelButtonForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal
        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue
        public Color DialogCloseButtonForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red
        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(20, 40, 70); // Mid-tone ocean blue
        public Color DialogNoButtonForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(30, 60, 90); // Muted blue
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red
        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(100, 200, 180); // Bright teal
        public Color DialogOkButtonForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(80, 180, 160); // Slightly darker teal
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(150, 180, 200); // Soft aqua
        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(255, 180, 90); // Soft orange
        public Color DialogWarningButtonForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(235, 160, 80); // Darker orange
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 180, 90); // Soft orange
        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red
        public Color DialogErrorButtonForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(235, 80, 80); // Darker red
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 90, 90); // Coral red
        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(50, 120, 160); // Deep sky blue
        public Color DialogInformationButtonForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(40, 100, 140); // Darker sky blue
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(50, 120, 160); // Deep sky blue
        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(120, 150, 180); // Muted blue
        public Color DialogQuestionButtonForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(100, 130, 160); // Darker muted blue
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.FromArgb(240, 245, 255); // Light off-white
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(120, 150, 180); // Muted blue
    }
}