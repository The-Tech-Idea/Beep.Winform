using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ZenTheme
    {
        // Dialog Button Colors and Fonts
        public Color DialogBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color DialogForeColor { get; set; } = Color.FromArgb(34, 34, 34);
        public TypographyStyle DialogYesButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogNoButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogOkButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogCancelButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogWarningButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogErrorButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogInformationButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogQuestionButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogHelpButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogCloseButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogYesButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogNoButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogOkButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Roboto",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.White,
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color DialogYesButtonForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(116, 215, 120);
        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color DialogCancelButtonForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(244, 67, 54);
        public Color DialogCloseButtonForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(255, 87, 74);
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.FromArgb(200, 40, 30);
        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(64, 64, 64);
        public Color DialogNoButtonForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color DialogOkButtonForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(116, 215, 120);
        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(255, 193, 7);
        public Color DialogWarningButtonForeColor { get; set; } = Color.White;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(255, 213, 27);
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(200, 150, 0);
        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(244, 67, 54);
        public Color DialogErrorButtonForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(255, 87, 74);
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(200, 40, 30);
        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color DialogInformationButtonForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(116, 215, 120);
        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(76, 175, 80);
        public Color DialogQuestionButtonForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(96, 195, 100);
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(116, 215, 120);
    }
}