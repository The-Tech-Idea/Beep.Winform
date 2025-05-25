using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class VintageTheme
    {
        // Dialog Button Colors and Fonts
        public Color DialogBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color DialogForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public TypographyStyle DialogYesButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogNoButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogOkButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogCancelButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogWarningButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogErrorButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogInformationButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogQuestionButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogHelpButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogCloseButtonFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogYesButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogNoButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(51, 25, 0),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public TypographyStyle DialogOkButtonHoverFont { get; set; } = new TypographyStyle
        {
            FontFamily = "Times New Roman",
            FontSize = 12,
            LineHeight = 1.3f,
            LetterSpacing = 0.2f,
            FontWeight = FontWeight.Medium,
            FontStyle = FontStyle.Regular,
            TextColor = Color.FromArgb(255, 245, 238),
            IsUnderlined = false,
            IsStrikeout = false
        };
        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color DialogYesButtonForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(240, 235, 215);
        public Color DialogCancelButtonForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(200, 180, 160);
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(240, 235, 215);
        public Color DialogCloseButtonForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.FromArgb(128, 0, 0);
        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(188, 143, 143);
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(240, 235, 215);
        public Color DialogNoButtonForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(200, 180, 160);
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.FromArgb(51, 25, 0);
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color DialogOkButtonForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(204, 85, 0);
        public Color DialogWarningButtonForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(184, 75, 0);
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(139, 69, 0);
        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(178, 34, 34);
        public Color DialogErrorButtonForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(128, 0, 0);
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(100, 0, 0);
        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(188, 143, 143);
        public Color DialogInformationButtonForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color DialogQuestionButtonForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.FromArgb(255, 245, 238);
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(139, 69, 19);
    }
}