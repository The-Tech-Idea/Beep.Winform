using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class FlatDesignTheme
    {
        // Dialog Button Colors and Fonts
        public Color DialogBackColor { get; set; } = Color.White;
        public Color DialogForeColor { get; set; } = Color.Black;

        public TypographyStyle DialogYesButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle DialogNoButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle DialogOkButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle DialogCancelButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle DialogWarningButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle DialogErrorButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle DialogInformationButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle DialogQuestionButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle DialogHelpButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle DialogCloseButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);

        public TypographyStyle DialogYesButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle DialogNoButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle DialogOkButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(76, 175, 80); // Green
        public Color DialogYesButtonForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(56, 142, 60);
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(46, 125, 50);

        public Color DialogCancelButtonBackColor { get; set; } = Color.LightGray;
        public Color DialogCancelButtonForeColor { get; set; } = Color.Black;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.Gray;
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.DarkGray;

        public Color DialogCloseButtonBackColor { get; set; } =Color.FromArgb(33, 150, 243);
        public Color DialogCloseButtonForeColor { get; set; } = Color.Black;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.LightGray;
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.Gray;

        public Color DialogHelpButtonBackColor { get; set; } = Color.LightBlue;
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(244, 67, 54); // Red
        public Color DialogNoButtonForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(211, 47, 47);
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(198, 40, 40);

        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue
        public Color DialogOkButtonForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(21, 101, 192);

        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(255, 193, 7); // Amber
        public Color DialogWarningButtonForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(255, 160, 0);
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 143, 0);

        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(244, 67, 54); // Red
        public Color DialogErrorButtonForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(211, 47, 47);
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(198, 40, 40);

        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue
        public Color DialogInformationButtonForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(21, 101, 192);

        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(0, 150, 136); // Teal
        public Color DialogQuestionButtonForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(0, 121, 107);
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 105, 92);
    }
}
