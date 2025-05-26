using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Dialog Button Colors and Fonts
        public Color DialogBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color DialogForeColor { get; set; } = Color.White;

        public TypographyStyle DialogYesButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public TypographyStyle DialogNoButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public TypographyStyle DialogOkButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public TypographyStyle DialogCancelButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public TypographyStyle DialogWarningButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public TypographyStyle DialogErrorButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public TypographyStyle DialogInformationButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public TypographyStyle DialogQuestionButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public TypographyStyle DialogHelpButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);
        public TypographyStyle DialogCloseButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);

        public TypographyStyle DialogYesButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold | FontStyle.Underline);
        public TypographyStyle DialogNoButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold | FontStyle.Underline);
        public TypographyStyle DialogOkButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold | FontStyle.Underline);

        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(46, 204, 113); // Green
        public Color DialogYesButtonForeColor { get; set; } = Color.Black;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(39, 174, 96);
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(149, 165, 166); // Gray
        public Color DialogCancelButtonForeColor { get; set; } = Color.Black;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(127, 140, 141);
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color DialogCloseButtonForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(231, 76, 60); // Red
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(52, 152, 219); // Blue
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(231, 76, 60); // Red
        public Color DialogNoButtonForeColor { get; set; } = Color.Black;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(192, 57, 43);
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(46, 204, 113);
        public Color DialogOkButtonForeColor { get; set; } = Color.Black;
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(39, 174, 96);
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(241, 196, 15); // Yellow
        public Color DialogWarningButtonForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(243, 156, 18);
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(231, 76, 60);
        public Color DialogErrorButtonForeColor { get; set; } = Color.Black;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(192, 57, 43);
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(52, 152, 219);
        public Color DialogInformationButtonForeColor { get; set; } = Color.Black;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(41, 128, 185);
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.White;

        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(41, 128, 185);
        public Color DialogQuestionButtonForeColor { get; set; } = Color.Black;
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(31, 97, 141);
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.White;
    }
}
