using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Dialog Button Colors and Fonts
        public Color DialogBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color DialogForeColor { get; set; } = Color.Black;

        public TypographyStyle  DialogYesButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogNoButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  DialogOkButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogCancelButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  DialogWarningButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogErrorButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogInformationButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  DialogQuestionButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  DialogHelpButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  DialogCloseButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle  DialogYesButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle  DialogNoButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Italic);
        public TypographyStyle  DialogOkButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        public Color DialogYesButtonBackColor { get; set; } = Color.LightGreen;
        public Color DialogYesButtonForeColor { get; set; } = Color.Black;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.MediumSeaGreen;
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.DarkGreen;

        public Color DialogCancelButtonBackColor { get; set; } = Color.LightGray;
        public Color DialogCancelButtonForeColor { get; set; } = Color.Black;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.Gray;
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.DarkGray;

        public Color DialogCloseButtonBackColor { get; set; } = Color.LightSlateGray;
        public Color DialogCloseButtonForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.SlateGray;
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.Black;

        public Color DialogHelpButtonBackColor { get; set; } = Color.LightBlue;

        public Color DialogNoButtonBackColor { get; set; } = Color.LightCoral;
        public Color DialogNoButtonForeColor { get; set; } = Color.Black;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.IndianRed;
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.DarkRed;

        public Color DialogOkButtonBackColor { get; set; } = Color.SkyBlue;
        public Color DialogOkButtonForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.DeepSkyBlue;
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.Blue;

        public Color DialogWarningButtonBackColor { get; set; } = Color.Gold;
        public Color DialogWarningButtonForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.DarkOrange;
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.Chocolate;

        public Color DialogErrorButtonBackColor { get; set; } = Color.LightPink;
        public Color DialogErrorButtonForeColor { get; set; } = Color.DarkRed;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.Red;
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.DarkRed;

        public Color DialogInformationButtonBackColor { get; set; } = Color.LightSteelBlue;
        public Color DialogInformationButtonForeColor { get; set; } = Color.Black;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.SteelBlue;
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.MidnightBlue;

        public Color DialogQuestionButtonBackColor { get; set; } = Color.Lavender;
        public Color DialogQuestionButtonForeColor { get; set; } = Color.Black;
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.MediumPurple;
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.Indigo;
    }
}
