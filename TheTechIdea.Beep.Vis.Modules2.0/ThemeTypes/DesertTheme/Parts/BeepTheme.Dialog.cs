using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DesertTheme
    {
        // Dialog Colors & Fonts
        public Color DialogBackColor { get; set; } = Color.FromArgb(255, 250, 240); // light sand

        public Color DialogForeColor { get; set; } = Color.FromArgb(101, 67, 33); // rich brown

        // Fonts
        public TypographyStyle DialogYesButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle DialogNoButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle DialogOkButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle DialogCancelButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle DialogWarningButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle DialogErrorButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle DialogInformationButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle DialogQuestionButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle DialogHelpButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);
        public TypographyStyle DialogCloseButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10, FontStyle.Bold);

        // Hover Fonts (same style for simplicity)
        public TypographyStyle DialogYesButtonHoverFont { get; set; } = DialogYesButtonFont;
        public TypographyStyle DialogNoButtonHoverFont { get; set; } = DialogNoButtonFont;
        public TypographyStyle DialogOkButtonHoverFont { get; set; } = DialogOkButtonFont;

        // Yes Button Colors
        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(210, 180, 140); // tan
        public Color DialogYesButtonForeColor { get; set; } = Color.FromArgb(101, 67, 33);
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(244, 208, 132); // light gold
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.FromArgb(101, 67, 33);
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // goldenrod

        // Cancel Button Colors
        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(205, 133, 63); // peru
        public Color DialogCancelButtonForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135); // burlywood
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.FromArgb(101, 67, 33);
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.FromArgb(139, 69, 19); // saddle brown

        // Close Button Colors
        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(188, 143, 143); // rosy brown
        public Color DialogCloseButtonForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(205, 92, 92); // indian red
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.FromArgb(139, 0, 0); // dark red

        // Help Button Colors
        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(244, 208, 132); // light gold
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(210, 180, 140); // tan
        public Color DialogNoButtonForeColor { get; set; } = Color.FromArgb(101, 67, 33);
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(238, 214, 175); // pale gold
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.FromArgb(101, 67, 33);
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // goldenrod

        // OK Button Colors
        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(222, 184, 135); // burlywood
        public Color DialogOkButtonForeColor { get; set; } = Color.FromArgb(101, 67, 33);
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(255, 228, 181); // moccasin
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.FromArgb(101, 67, 33);
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(184, 134, 11);

        // Warning Button Colors
        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(255, 222, 173); // navajo white
        public Color DialogWarningButtonForeColor { get; set; } = Color.FromArgb(101, 67, 33);
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(255, 239, 213); // papaya whip
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.FromArgb(101, 67, 33);
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(184, 134, 11);

        // Error Button Colors
        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(178, 34, 34); // firebrick
        public Color DialogErrorButtonForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(220, 20, 60); // crimson
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(139, 0, 0); // dark red

        // Information Button Colors
        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(70, 130, 180); // steel blue
        public Color DialogInformationButtonForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(100, 149, 237); // cornflower blue
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(65, 105, 225); // royal blue

        // Question Button Colors
        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(244, 208, 132); // light gold
        public Color DialogQuestionButtonForeColor { get; set; } = Color.FromArgb(101, 67, 33);
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(238, 214, 175); // pale gold
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.FromArgb(101, 67, 33);
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(184, 134, 11); // goldenrod
    }
}
