using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // ComboBox Colors and Fonts
        public Color ComboBoxBackColor { get; set; } = Color.White;
        public Color ComboBoxForeColor { get; set; } = Color.Black;
        public Color ComboBoxBorderColor { get; set; } = Color.DodgerBlue;
        public Color ComboBoxHoverBackColor { get; set; } = Color.LightSkyBlue;
        public Color ComboBoxHoverForeColor { get; set; } = Color.Black;
        public Color ComboBoxHoverBorderColor { get; set; } = Color.DodgerBlue;
        public Color ComboBoxSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.DodgerBlue;
        public Color ComboBoxErrorBackColor { get; set; } = Color.MistyRose;
        public Color ComboBoxErrorForeColor { get; set; } = Color.DarkRed;
        public Font ComboBoxItemFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font ComboBoxListFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Color CheckBoxSelectedForeColor { get; set; } = Color.White;
        public Color CheckBoxSelectedBackColor { get; set; } = Color.DodgerBlue;
    }
}
