using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Dialog Button Colors and Fonts

        public Color DialogBackColor { get; set; } = Color.FromArgb(24, 24, 48);                   // Cyberpunk Black
        public Color DialogForeColor { get; set; } = Color.FromArgb(0, 255, 255);                  // Neon Cyan

        // Fonts: Use bold and italic for focus/hover effects
        public TypographyStyle DialogYesButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold);
        public TypographyStyle DialogNoButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold);
        public TypographyStyle DialogOkButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold);
        public TypographyStyle DialogCancelButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold);
        public TypographyStyle DialogWarningButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle DialogErrorButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle DialogInformationButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle DialogQuestionButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold);
        public TypographyStyle DialogHelpButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Underline);
        public TypographyStyle DialogCloseButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Regular);

        // Hovered
        public TypographyStyle DialogYesButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle DialogNoButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle DialogOkButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Consolas", 11f, FontStyle.Bold | FontStyle.Italic);

        // YES Button: Neon Green, Magenta border, Cyan hover
        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(0, 255, 128);        // Neon Green
        public Color DialogYesButtonForeColor { get; set; } = Color.Black;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(0, 255, 255);   // Neon Cyan
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 0, 255); // Neon Magenta

        // NO Button: Neon Red, Yellow border, Magenta hover
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(255, 40, 80);         // Neon Red
        public Color DialogNoButtonForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(255, 0, 255);    // Neon Magenta
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 255, 0);  // Neon Yellow

        // OK Button: Neon Blue, Magenta border, Cyan hover
        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(0, 102, 255);         // Neon Blue
        public Color DialogOkButtonForeColor { get; set; } = Color.FromArgb(0, 255, 255);         // Neon Cyan
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(0, 255, 255);    // Neon Cyan
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 0, 255);  // Neon Magenta

        // Cancel Button: Neon Magenta, Cyan border, Yellow hover
        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(255, 0, 255);     // Neon Magenta
        public Color DialogCancelButtonForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(255, 255, 0);// Neon Yellow
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 255, 255);// Neon Cyan

        // Close Button: Neon Yellow, Blue border, Magenta hover
        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(255, 255, 0);      // Neon Yellow
        public Color DialogCloseButtonForeColor { get; set; } = Color.Black;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(255, 0, 255); // Neon Magenta
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 102, 255);// Neon Blue

        // Help Button: Neon Cyan
        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(0, 255, 255);       // Neon Cyan

        // Warning Button: Neon Orange
        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(255, 128, 0);    // Neon Orange
        public Color DialogWarningButtonForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(255, 255, 0);// Neon Yellow
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 0, 255);// Neon Magenta

        // Error Button: Neon Red
        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(255, 40, 80);      // Neon Red
        public Color DialogErrorButtonForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(255, 128, 0); // Neon Orange
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 255, 0);// Neon Yellow

        // Information Button: Neon Cyan
        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(0, 255, 255);// Neon Cyan
        public Color DialogInformationButtonForeColor { get; set; } = Color.Black;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(0, 255, 128);// Neon Green
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 0, 255);// Neon Magenta

        // Question Button: Neon Blue
        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(0, 102, 255);   // Neon Blue
        public Color DialogQuestionButtonForeColor { get; set; } = Color.FromArgb(0, 255, 255);   // Neon Cyan
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(255, 0, 255);// Neon Magenta
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 255, 0);// Neon Yellow
    }
}
