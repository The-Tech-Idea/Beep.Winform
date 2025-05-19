using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Dialog Button Colors and Fonts
        // Note: Ensure 'Roboto' font family is available. If unavailable, 'Arial' is a fallback.
        public Color DialogBackColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark blue-purple for dialog background
        public Color DialogForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray for dialog text
        public TypographyStyle DialogYesButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto", // Fallback: Arial
            FontSize = 12f,
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
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
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
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
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
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
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
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
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
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
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
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
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
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
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(30, 30, 50), // Dark for contrast
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
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
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
            FontWeight = FontWeight.SemiBold,
            TextColor = Color.FromArgb(236, 240, 241), // Light gray
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
            TextColor = Color.FromArgb(241, 196, 15), // Neon yellow
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
            TextColor = Color.FromArgb(241, 196, 15), // Neon yellow
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
            TextColor = Color.FromArgb(241, 196, 15), // Neon yellow
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Neon green
        public Color DialogYesButtonForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray
        public Color DialogCancelButtonForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray
        public Color DialogCloseButtonForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(40, 40, 60); // Dark blue-gray
        public Color DialogNoButtonForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise
        public Color DialogOkButtonForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(243, 156, 18); // Neon orange
        public Color DialogWarningButtonForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(231, 76, 60); // Neon red
        public Color DialogErrorButtonForeColor { get; set; } = Color.FromArgb(236, 240, 241); // Light gray
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(26, 188, 156); // Neon turquoise
        public Color DialogInformationButtonForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(155, 89, 182); // Neon purple
        public Color DialogQuestionButtonForeColor { get; set; } = Color.FromArgb(30, 30, 50); // Dark
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Lighter blue-gray
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(241, 196, 15); // Neon yellow
    }
}