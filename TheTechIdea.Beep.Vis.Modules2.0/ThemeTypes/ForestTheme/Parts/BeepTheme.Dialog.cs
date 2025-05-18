using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Dialog Button Colors and Fonts
        public Color DialogBackColor { get; set; } = Color.FromArgb(34, 49, 34); // Dark forest green background
        public Color DialogForeColor { get; set; } = Color.White;

        public Font DialogYesButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogNoButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font DialogOkButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogCancelButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font DialogWarningButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogErrorButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogInformationButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font DialogQuestionButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font DialogHelpButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font DialogCloseButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);

        public Font DialogYesButtonHoverFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogNoButtonHoverFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font DialogOkButtonHoverFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);

        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(34, 70, 34);
        public Color DialogYesButtonForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(50, 120, 50);
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.LimeGreen;

        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(60, 80, 60);
        public Color DialogCancelButtonForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(80, 100, 80);
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.DarkOliveGreen;

        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(40, 50, 40);
        public Color DialogCloseButtonForeColor { get; set; } = Color.LightGray;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(70, 90, 70);
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.DarkGreen;

        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(50, 65, 50);

        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(60, 80, 60);
        public Color DialogNoButtonForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(90, 110, 90);
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.ForestGreen;

        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(34, 60, 34);
        public Color DialogOkButtonForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(60, 100, 60);
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.LawnGreen;

        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(180, 130, 30);
        public Color DialogWarningButtonForeColor { get; set; } = Color.White;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(210, 160, 40);
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.Goldenrod;

        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(120, 0, 0);
        public Color DialogErrorButtonForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(160, 0, 0);
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.DarkRed;

        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(30, 90, 120);
        public Color DialogInformationButtonForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(50, 120, 160);
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.CadetBlue;

        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(45, 80, 90);
        public Color DialogQuestionButtonForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(60, 110, 120);
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.LightSteelBlue;
    }
}
