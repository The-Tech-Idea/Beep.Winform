using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Dialog Button Colors and Fonts
<<<<<<< HEAD
        public Color DialogBackColor { get; set; } = Color.Black;
        public Color DialogForeColor { get; set; } = Color.White;
=======
        public Color DialogBackColor { get; set; }
        public Color DialogForeColor { get; set; }
        public TypographyStyle DialogYesButtonFont { get; set; }
        public TypographyStyle DialogNoButtonFont { get; set; }
        public TypographyStyle DialogOkButtonFont { get; set; }
        public TypographyStyle DialogCancelButtonFont { get; set; }
        public TypographyStyle DialogWarningButtonFont { get; set; }
        public TypographyStyle DialogErrorButtonFont { get; set; }
        public TypographyStyle DialogInformationButtonFont { get; set; }
        public TypographyStyle DialogQuestionButtonFont { get; set; }
        public TypographyStyle DialogHelpButtonFont { get; set; }
        public TypographyStyle DialogCloseButtonFont { get; set; }
        public TypographyStyle DialogYesButtonHoverFont { get; set; }
        public TypographyStyle DialogNoButtonHoverFont { get; set; }
        public TypographyStyle DialogOkButtonHoverFont { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634

        public Font DialogYesButtonFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font DialogNoButtonFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font DialogOkButtonFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font DialogCancelButtonFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font DialogWarningButtonFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font DialogErrorButtonFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font DialogInformationButtonFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font DialogQuestionButtonFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font DialogHelpButtonFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font DialogCloseButtonFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font DialogYesButtonHoverFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold | FontStyle.Italic);
        public Font DialogNoButtonHoverFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold | FontStyle.Italic);
        public Font DialogOkButtonHoverFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold | FontStyle.Italic);

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
