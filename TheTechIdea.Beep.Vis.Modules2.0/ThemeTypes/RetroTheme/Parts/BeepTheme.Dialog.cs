using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RetroTheme
    {
        // Dialog Button Colors and Fonts
        public Color DialogBackColor { get; set; } = Color.FromArgb(48, 48, 48);
        public Color DialogForeColor { get; set; } = Color.White;
        public TypographyStyle DialogYesButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogNoButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogOkButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogCancelButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogWarningButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogErrorButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogInformationButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogQuestionButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogHelpButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogCloseButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Normal,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogYesButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogNoButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogOkButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Courier New",
            FontSize = 12,
            LineHeight = 1.2f,
            LetterSpacing = 0.5f,
            FontWeight = FontWeight.Bold,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(64, 255, 64);
        public Color DialogYesButtonForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(96, 255, 96);
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(128, 192, 128);
        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color DialogCancelButtonForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(255, 64, 64);
        public Color DialogCloseButtonForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(255, 96, 96);
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.FromArgb(192, 64, 64);
        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(64, 64, 255);
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color DialogNoButtonForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(96, 96, 96);
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(128, 128, 128);
        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(64, 255, 64);
        public Color DialogOkButtonForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(96, 255, 96);
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(128, 192, 128);
        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(255, 192, 0);
        public Color DialogWarningButtonForeColor { get; set; } = Color.White;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(255, 224, 64);
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(192, 160, 0);
        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(255, 64, 64);
        public Color DialogErrorButtonForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(255, 96, 96);
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(192, 64, 64);
        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(64, 64, 255);
        public Color DialogInformationButtonForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(96, 96, 255);
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(128, 128, 192);
        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(64, 64, 255);
        public Color DialogQuestionButtonForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(96, 96, 255);
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(128, 128, 192);
    }
}