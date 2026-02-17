using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Dialog Button Colors and Fonts
        public Color DialogBackColor { get; set; } = Color.Black;
        public Color DialogForeColor { get; set; } = Color.White;

        public TypographyStyle  DialogYesButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogNoButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogOkButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogCancelButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogWarningButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogErrorButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogInformationButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogQuestionButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogHelpButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogCloseButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogYesButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle  DialogNoButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle  DialogOkButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold | FontStyle.Italic);

        public Color DialogYesButtonBackColor { get; set; } = Color.Black;
        public Color DialogYesButtonForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogCancelButtonBackColor { get; set; } = Color.Black;
        public Color DialogCancelButtonForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogCloseButtonBackColor { get; set; } = Color.Black;
        public Color DialogCloseButtonForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogHelpButtonBackColor { get; set; } = Color.Black;

        public Color DialogNoButtonBackColor { get; set; } = Color.Black;
        public Color DialogNoButtonForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogOkButtonBackColor { get; set; } = Color.Black;
        public Color DialogOkButtonForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogWarningButtonBackColor { get; set; } = Color.Black;
        public Color DialogWarningButtonForeColor { get; set; } = Color.Yellow;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.Yellow;
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.Yellow;

        public Color DialogErrorButtonBackColor { get; set; } = Color.Black;
        public Color DialogErrorButtonForeColor { get; set; } = Color.Red;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.Red;
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.Red;

        public Color DialogInformationButtonBackColor { get; set; } = Color.Black;
        public Color DialogInformationButtonForeColor { get; set; } = Color.Cyan;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.Cyan;
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.Cyan;

        public Color DialogQuestionButtonBackColor { get; set; } = Color.Black;
        public Color DialogQuestionButtonForeColor { get; set; } = Color.Lime;
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.Lime;
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.Lime;
    }
}
