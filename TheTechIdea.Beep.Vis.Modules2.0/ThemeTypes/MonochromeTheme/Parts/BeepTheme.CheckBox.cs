using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MonochromeTheme
    {
        // CheckBox properties
        public Color CheckBoxBackColor { get; set; } = Color.Black;
        public Color CheckBoxForeColor { get; set; } = Color.WhiteSmoke;
        public Color CheckBoxBorderColor { get; set; } = Color.Gray;
        public Color CheckBoxCheckedBackColor { get; set; } = Color.DimGray;
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.Silver;
        public Color CheckBoxHoverBackColor { get; set; } = Color.DimGray;
        public Color CheckBoxHoverForeColor { get; set; } = Color.WhiteSmoke;
        public Color CheckBoxHoverBorderColor { get; set; } = Color.LightGray;
        public TypographyStyle CheckBoxFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Regular);
        public TypographyStyle CheckBoxCheckedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 9F, FontStyle.Bold);
    }
}
