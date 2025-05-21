using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Dialog Button Colors and Fonts
        public Color DialogBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color DialogForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public TypographyStyle DialogYesButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle DialogNoButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle DialogOkButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle DialogCancelButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle DialogWarningButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle DialogErrorButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle DialogInformationButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle DialogQuestionButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle DialogHelpButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle DialogCloseButtonFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle DialogYesButtonHoverFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle DialogNoButtonHoverFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle DialogOkButtonHoverFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.White };

        public Color DialogYesButtonBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color DialogYesButtonForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color DialogYesButtonHoverBackColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color DialogYesButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogYesButtonHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color DialogCancelButtonBackColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color DialogCancelButtonForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color DialogCancelButtonHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color DialogCancelButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCancelButtonHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color DialogCloseButtonBackColor { get; set; } = Color.FromArgb(255, 182, 182);
        public Color DialogCloseButtonForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color DialogCloseButtonHoverBackColor { get; set; } = Color.FromArgb(255, 200, 200);
        public Color DialogCloseButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogCloseButtonHoverBorderColor { get; set; } = Color.FromArgb(240, 150, 150);
        public Color DialogHelpButtonBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color DialogNoButtonBackColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color DialogNoButtonForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color DialogNoButtonHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color DialogNoButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogNoButtonHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color DialogOkButtonBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color DialogOkButtonForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color DialogOkButtonHoverBackColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color DialogOkButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogOkButtonHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color DialogWarningButtonBackColor { get; set; } = Color.FromArgb(255, 230, 180);
        public Color DialogWarningButtonForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color DialogWarningButtonHoverBackColor { get; set; } = Color.FromArgb(255, 240, 200);
        public Color DialogWarningButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogWarningButtonHoverBorderColor { get; set; } = Color.FromArgb(240, 200, 150);
        public Color DialogErrorButtonBackColor { get; set; } = Color.FromArgb(255, 182, 182);
        public Color DialogErrorButtonForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color DialogErrorButtonHoverBackColor { get; set; } = Color.FromArgb(255, 200, 200);
        public Color DialogErrorButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogErrorButtonHoverBorderColor { get; set; } = Color.FromArgb(240, 150, 150);
        public Color DialogInformationButtonBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color DialogInformationButtonForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color DialogInformationButtonHoverBackColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color DialogInformationButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogInformationButtonHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color DialogQuestionButtonBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color DialogQuestionButtonForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color DialogQuestionButtonHoverBackColor { get; set; } = Color.FromArgb(255, 204, 221);
        public Color DialogQuestionButtonHoverForeColor { get; set; } = Color.White;
        public Color DialogQuestionButtonHoverBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
    }
}