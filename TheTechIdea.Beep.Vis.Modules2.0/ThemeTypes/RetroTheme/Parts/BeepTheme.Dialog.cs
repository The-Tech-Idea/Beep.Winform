using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Dialog Button Colors and Fonts
        // Note: Ensure 'Courier New' font family is available for retro aesthetic. If unavailable, 'Consolas' is a fallback.
        public Color DialogBackColor { get; set; } = Color.FromArgb(0, 43, 43); // Dark retro teal for dialog background
        public Color DialogForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White for dialog text
        public TypographyStyle DialogYesButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogNoButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogOkButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogCancelButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogWarningButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogErrorButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogInformationButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogQuestionButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogHelpButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogCloseButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Regular,
            TextColor = Color.FromArgb(255, 255, 255), // White
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogYesButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(0, 255, 255), // Bright cyan
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogNoButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(255, 85, 85), // Retro red
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogOkButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New", // Fallback: Consolas
            FontSize = 12f,
            FontWeight = FontWeight.Bold,
            TextColor = Color.FromArgb(0, 255, 255), // Bright cyan
            LineHeight = 1.2f,
            LetterSpacing = 0.2f,
            FontStyle = FontStyle.Regular,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(0, 128, 128); // Darker teal for yes button
        public Color DialogYesButtonForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(0, 170, 170); // Lighter teal for hover
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow
        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal
        public Color DialogCancelButtonForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(0, 170, 170); // Lighter teal
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow
        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal
        public Color DialogCloseButtonForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(139, 0, 0); // Dark red
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.FromArgb(255, 85, 85); // Retro red
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 85, 85); // Retro red
        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(0, 85, 85); // Retro teal
        public Color DialogNoButtonForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(139, 0, 0); // Dark red
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.FromArgb(255, 85, 85); // Retro red
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 85, 85); // Retro red
        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(0, 128, 128); // Darker teal
        public Color DialogOkButtonForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(0, 170, 170); // Lighter teal
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 215, 0); // Retro yellow
        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(255, 165, 0); // Retro orange
        public Color DialogWarningButtonForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(200, 130, 0); // Darker orange
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 165, 0); // Retro orange
        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(139, 0, 0); // Dark red
        public Color DialogErrorButtonForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(255, 85, 85); // Retro red
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 85, 85); // Retro red
        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(0, 128, 128); // Darker teal
        public Color DialogInformationButtonForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(0, 170, 170); // Lighter teal
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan
        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(0, 128, 128); // Darker teal
        public Color DialogQuestionButtonForeColor { get; set; } = Color.FromArgb(255, 255, 255); // White
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(0, 170, 170); // Lighter teal
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 255, 255); // Bright cyan
    }
}