using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Dialog Button Colors and Fonts

        public Color DialogBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color DialogForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy

        // Fonts: playful for Yes/Ok, clear for others
        public Font DialogYesButtonFont { get; set; } = new Font("Comic Sans MS", 10.5f, FontStyle.Bold);
        public Font DialogNoButtonFont { get; set; } = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        public Font DialogOkButtonFont { get; set; } = new Font("Comic Sans MS", 10.5f, FontStyle.Bold);
        public Font DialogCancelButtonFont { get; set; } = new Font("Segoe UI", 10.5f, FontStyle.Regular);
        public Font DialogWarningButtonFont { get; set; } = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        public Font DialogErrorButtonFont { get; set; } = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        public Font DialogInformationButtonFont { get; set; } = new Font("Segoe UI", 10.5f, FontStyle.Regular);
        public Font DialogQuestionButtonFont { get; set; } = new Font("Comic Sans MS", 10.5f, FontStyle.Italic);
        public Font DialogHelpButtonFont { get; set; } = new Font("Segoe UI", 10.5f, FontStyle.Italic);
        public Font DialogCloseButtonFont { get; set; } = new Font("Comic Sans MS", 10.5f, FontStyle.Bold);

        // Hover fonts
        public Font DialogYesButtonHoverFont { get; set; } = new Font("Comic Sans MS", 10.5f, FontStyle.Bold | FontStyle.Underline);
        public Font DialogNoButtonHoverFont { get; set; } = new Font("Segoe UI", 10.5f, FontStyle.Bold | FontStyle.Underline);
        public Font DialogOkButtonHoverFont { get; set; } = new Font("Comic Sans MS", 10.5f, FontStyle.Bold | FontStyle.Underline);

        // Yes button: Mint and Candy Pink
        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint
        public Color DialogYesButtonForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Pastel Blue
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 182, 193); // Light Pink

        // Cancel button: Lemon Yellow and Navy
        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public Color DialogCancelButtonForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 182, 193); // Light Pink

        // Close button: Candy Red and White
        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(255, 99, 132); // Candy Red
        public Color DialogCloseButtonForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(255, 205, 86); // Candy Yellow
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.FromArgb(44, 62, 80);   // Navy
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint

        // Help button: Mint background
        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint

        // No button: Pastel Blue and Candy Pink
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Pastel Blue
        public Color DialogNoButtonForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint

        // OK button: Pastel Pink and Navy
        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color DialogOkButtonForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 223, 93); // Lemon

        // Warning button: Lemon Yellow and Navy
        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(255, 223, 93); // Lemon
        public Color DialogWarningButtonForeColor { get; set; } = Color.FromArgb(44, 62, 80);   // Navy
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lighter Lemon
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(54, 162, 235); // Pastel Blue

        // Error button: Candy Red and White
        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(255, 99, 132); // Candy Red
        public Color DialogErrorButtonForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.FromArgb(44, 62, 80);   // Navy
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 205, 86); // Candy Yellow

        // Information button: Pastel Blue and Navy
        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Pastel Blue
        public Color DialogInformationButtonForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(228, 222, 255); // Lavender

        // Question button: Mint and Lemon Yellow
        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint
        public Color DialogQuestionButtonForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint
    }
}
