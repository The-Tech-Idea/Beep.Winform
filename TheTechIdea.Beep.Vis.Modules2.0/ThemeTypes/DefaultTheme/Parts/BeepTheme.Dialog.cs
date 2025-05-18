using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Dialog Button Colors and Fonts

        public Color DialogBackColor { get; set; } = Color.White;
        public Color DialogForeColor { get; set; } = Color.Black;

        public Font DialogYesButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogNoButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogOkButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogCancelButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogWarningButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogErrorButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogInformationButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogQuestionButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogHelpButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogCloseButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);

        public Font DialogYesButtonHoverFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogNoButtonHoverFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font DialogOkButtonHoverFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);

        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color DialogYesButtonForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(0, 100, 180);
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 80, 150);

        public Color DialogCancelButtonBackColor { get; set; } = Color.LightGray;
        public Color DialogCancelButtonForeColor { get; set; } = Color.Black;
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.Gray;
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.DarkGray;

        public Color DialogCloseButtonBackColor { get; set; } = Color.LightGray;
        public Color DialogCloseButtonForeColor { get; set; } = Color.Black;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.Gray;
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.DarkGray;

        public Color DialogHelpButtonBackColor { get; set; } = Color.LightBlue;
        public Color DialogNoButtonBackColor { get; set; } = Color.LightGray;
        public Color DialogNoButtonForeColor { get; set; } = Color.Black;
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.Gray;
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.DarkGray;

        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color DialogOkButtonForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(0, 100, 180);
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 80, 150);

        public Color DialogWarningButtonBackColor { get; set; } = Color.Orange;
        public Color DialogWarningButtonForeColor { get; set; } = Color.Black;
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.DarkOrange;
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.OrangeRed;

        public Color DialogErrorButtonBackColor { get; set; } = Color.Red;
        public Color DialogErrorButtonForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.DarkRed;
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.Maroon;

        public Color DialogInformationButtonBackColor { get; set; } = Color.LightBlue;
        public Color DialogInformationButtonForeColor { get; set; } = Color.Black;
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.DodgerBlue;
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.RoyalBlue;

        public Color DialogQuestionButtonBackColor { get; set; } = Color.LightGreen;
        public Color DialogQuestionButtonForeColor { get; set; } = Color.Black;
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.Green;
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.DarkGreen;
    }
}
