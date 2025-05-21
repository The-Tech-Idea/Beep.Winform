using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // Dialog Button Colors and Fonts
//<<<<<<< HEAD
        public Color DialogBackColor { get; set; } = Color.White;
        public Color DialogForeColor { get; set; } = Color.Black;

        public Font DialogYesButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font DialogNoButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DialogOkButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font DialogCancelButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DialogWarningButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font DialogErrorButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font DialogInformationButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DialogQuestionButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DialogHelpButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Italic);
        public Font DialogCloseButtonFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font DialogYesButtonHoverFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold | FontStyle.Underline);
        public Font DialogNoButtonHoverFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Underline);
        public Font DialogOkButtonHoverFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold | FontStyle.Underline);

        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(76, 175, 80);  // Green
        public Color DialogYesButtonForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(56, 142, 60);
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.DarkGreen;

        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(244, 67, 54);  // Red
        public Color DialogCancelButtonForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(211, 47, 47);
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.DarkRed;

        public Color DialogCloseButtonBackColor { get; set; } = Color.Gray;
        public Color DialogCloseButtonForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.DimGray;
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.DarkGray;

        public Color DialogHelpButtonBackColor { get; set; } = Color.SkyBlue;

        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(255, 152, 0);  // Amber
        public Color DialogNoButtonForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(255, 143, 0);
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.Orange;

        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(33, 150, 243);  // Blue
        public Color DialogOkButtonForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.Navy;

        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(255, 193, 7); // Yellow
        public Color DialogWarningButtonForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(255, 160, 0);
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.Goldenrod;

        public Color DialogErrorButtonBackColor { get; set; } = Color.DarkRed;
        public Color DialogErrorButtonForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.Maroon;
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.Firebrick;

        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(3, 169, 244);
        public Color DialogInformationButtonForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(2, 136, 209);
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.SteelBlue;

        public Color DialogQuestionButtonBackColor { get; set; } = Color.MediumPurple;
        public Color DialogQuestionButtonForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.Indigo;
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.DarkSlateBlue;
    }
}
