using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Dialog Button Colors and Fonts
        public Color DialogBackColor { get; set; } = Color.White;
        public Color DialogForeColor { get; set; } = Color.Black;
        public TypographyStyle  DialogYesButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle  DialogNoButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle  DialogOkButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle  DialogCancelButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle  DialogWarningButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle  DialogErrorButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle  DialogInformationButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle  DialogQuestionButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle  DialogHelpButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle  DialogCloseButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle  DialogYesButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Bold);
        public TypographyStyle  DialogNoButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Bold);
        public TypographyStyle  DialogOkButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Bold);

        public Color DialogYesButtonBackColor { get; set; } = Color.LightGreen;
        public Color DialogYesButtonForeColor { get; set; } = Color.Black;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.Green;
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.DarkGreen;
        public Color DialogCancelButtonBackColor { get; set; } = Color.LightGray;
        public Color DialogCancelButtonForeColor { get; set; } = Color.Black;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.Gray;
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.DarkGray;
        public Color DialogCloseButtonBackColor { get; set; } = Color.LightCoral;
        public Color DialogCloseButtonForeColor { get; set; } = Color.Black;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.Red;
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.DarkRed;
        public Color DialogHelpButtonBackColor { get; set; } = Color.LightBlue;
        public Color DialogNoButtonBackColor { get; set; } = Color.LightCoral;
        public Color DialogNoButtonForeColor { get; set; } = Color.Black;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.Red;
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.DarkRed;
        public Color DialogOkButtonBackColor { get; set; } = Color.LightGreen;
        public Color DialogOkButtonForeColor { get; set; } = Color.Black;
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.Green;
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.DarkGreen;
        public Color DialogWarningButtonBackColor { get; set; } = Color.LightGoldenrodYellow;
        public Color DialogWarningButtonForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.Goldenrod;
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.DarkGoldenrod;
        public Color DialogErrorButtonBackColor { get; set; } = Color.LightCoral;
        public Color DialogErrorButtonForeColor { get; set; } = Color.Black;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.Red;
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.DarkRed;
        public Color DialogInformationButtonBackColor { get; set; } = Color.LightBlue;
        public Color DialogInformationButtonForeColor { get; set; } = Color.Black;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.DodgerBlue;
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.DarkBlue;
        public Color DialogQuestionButtonBackColor { get; set; } = Color.LightGray;
        public Color DialogQuestionButtonForeColor { get; set; } = Color.Black;
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.Gray;
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.DarkGray;
    }
}
