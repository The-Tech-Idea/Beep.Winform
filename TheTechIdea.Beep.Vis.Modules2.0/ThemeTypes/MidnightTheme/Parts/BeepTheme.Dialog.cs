using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Dialog Button Colors and Fonts
        public Color DialogBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color DialogForeColor { get; set; } = Color.WhiteSmoke;
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
        public TypographyStyle  DialogYesButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogNoButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogOkButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(0, 150, 136); // Teal
        public Color DialogYesButtonForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(0, 188, 212);
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 150, 136);

        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(55, 71, 79); // Dark Slate
        public Color DialogCancelButtonForeColor { get; set; } = Color.WhiteSmoke;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(69, 90, 100);
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.FromArgb(55, 71, 79);

        public Color DialogCloseButtonBackColor { get; set; } =Color.FromArgb(20, 24, 30);
        public Color DialogCloseButtonForeColor { get; set; } = Color.LightGray;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(55, 71, 79);
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } =Color.FromArgb(20, 24, 30);

        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(63, 81, 181); // Indigo
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(244, 67, 54); // Red
        public Color DialogNoButtonForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(229, 57, 53);
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(244, 67, 54);

        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue
        public Color DialogOkButtonForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(30, 136, 229);
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(33, 150, 243);

        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(255, 152, 0); // Amber
        public Color DialogWarningButtonForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(255, 167, 38);
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 152, 0);

        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(211, 47, 47); // Darker Red
        public Color DialogErrorButtonForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(198, 40, 40);
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(211, 47, 47);

        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(2, 136, 209); // Info Blue
        public Color DialogInformationButtonForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(3, 155, 229);
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(2, 136, 209);

        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(21, 101, 192); // Dark Blue
        public Color DialogQuestionButtonForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(21, 101, 192);
    }
}
