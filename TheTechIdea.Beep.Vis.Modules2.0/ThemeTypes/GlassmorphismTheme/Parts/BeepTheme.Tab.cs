using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Tab Fonts & Colors
//<<<<<<< HEAD
        public Font TabFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font TabHoverFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Italic);
        public Font TabSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);

        public Color TabBackColor { get; set; } = Color.WhiteSmoke;
        public Color TabForeColor { get; set; } = Color.Black;

        public Color ActiveTabBackColor { get; set; } = Color.DeepSkyBlue;
        public Color ActiveTabForeColor { get; set; } = Color.White;

        public Color InactiveTabBackColor { get; set; } = Color.Gainsboro;
        public Color InactiveTabForeColor { get; set; } = Color.Black;

        public Color TabBorderColor { get; set; } = Color.LightGray;

        public Color TabHoverBackColor { get; set; } = Color.LightBlue;
        public Color TabHoverForeColor { get; set; } = Color.Black;

        public Color TabSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.SteelBlue;

        public Color TabHoverBorderColor { get; set; } = Color.CornflowerBlue;
    }
}
