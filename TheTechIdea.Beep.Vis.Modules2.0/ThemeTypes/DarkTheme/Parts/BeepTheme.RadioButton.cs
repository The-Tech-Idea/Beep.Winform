using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // RadioButton properties
        public Color RadioButtonBackColor { get; set; } = Color.FromArgb(40, 40, 40); // dark gray background
        public Color RadioButtonForeColor { get; set; } = Color.LightGray; // light gray text
        public Color RadioButtonBorderColor { get; set; } = Color.Gray;
        public Color RadioButtonCheckedBackColor { get; set; } = Color.CornflowerBlue; // accent blue when checked
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.DodgerBlue;
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(60, 60, 65); // slightly lighter on hover
        public Color RadioButtonHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.LightBlue;
        public TypographyStyle RadioButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Regular);
        public TypographyStyle RadioButtonCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9f, FontStyle.Bold);
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.CornflowerBlue;
    }
}
