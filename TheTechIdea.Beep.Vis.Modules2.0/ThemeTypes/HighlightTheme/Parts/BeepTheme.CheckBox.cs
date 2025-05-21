using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // CheckBox properties
//<<<<<<< HEAD
        public Color CheckBoxBackColor { get; set; } = Color.White;
        public Color CheckBoxForeColor { get; set; } = Color.Black;
        public Color CheckBoxBorderColor { get; set; } = Color.DodgerBlue;
        public Color CheckBoxCheckedBackColor { get; set; } = Color.DodgerBlue;
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.DodgerBlue;
        public Color CheckBoxHoverBackColor { get; set; } = Color.LightSkyBlue;
        public Color CheckBoxHoverForeColor { get; set; } = Color.Black;
        public Color CheckBoxHoverBorderColor { get; set; } = Color.SkyBlue;
        public Font CheckBoxFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Font CheckBoxCheckedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
    }
}
