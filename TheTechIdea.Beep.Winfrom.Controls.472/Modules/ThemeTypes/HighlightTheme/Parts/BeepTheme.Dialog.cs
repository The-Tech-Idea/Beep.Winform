using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Dialog Button Colors and Fonts
        public Color DialogBackColor { get; set; } = Color.White;
        public Color DialogForeColor { get; set; } = Color.Black;
        public TypographyStyle  DialogYesButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogNoButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  DialogOkButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogCancelButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  DialogWarningButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogErrorButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogInformationButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogQuestionButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogHelpButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogCloseButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogYesButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold | FontStyle.Underline);
        public TypographyStyle  DialogNoButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Underline);
        public TypographyStyle  DialogOkButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold | FontStyle.Underline);

        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color DialogYesButtonForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color DialogCancelButtonBackColor { get; set; } = Color.LightGray;
        public Color DialogCancelButtonForeColor { get; set; } = Color.Black;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.Gray;
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.Gray;
        public Color DialogCloseButtonBackColor { get; set; } = Color.LightGray;
        public Color DialogCloseButtonForeColor { get; set; } = Color.Black;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.Gray;
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.Gray;
        public Color DialogHelpButtonBackColor { get; set; } = Color.LightBlue;
        public Color DialogNoButtonBackColor { get; set; } = Color.LightGray;
        public Color DialogNoButtonForeColor { get; set; } = Color.Black;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.Gray;
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.Gray;
        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color DialogOkButtonForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color DialogWarningButtonBackColor { get; set; } = Color.Orange;
        public Color DialogWarningButtonForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.DarkOrange;
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.DarkOrange;
        public Color DialogErrorButtonBackColor { get; set; } = Color.Red;
        public Color DialogErrorButtonForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.DarkRed;
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.DarkRed;
        public Color DialogInformationButtonBackColor { get; set; } = Color.LightBlue;
        public Color DialogInformationButtonForeColor { get; set; } = Color.Black;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.SkyBlue;
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.SkyBlue;
        public Color DialogQuestionButtonBackColor { get; set; } = Color.LightGray;
        public Color DialogQuestionButtonForeColor { get; set; } = Color.Black;
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.Gray;
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.Gray;
    }
}
