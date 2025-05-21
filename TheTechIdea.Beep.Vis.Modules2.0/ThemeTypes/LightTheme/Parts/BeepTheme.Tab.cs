using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Tab Fonts & Colors
//<<<<<<< HEAD
        public Font TabFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font TabHoverFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font TabSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);

        public Color TabBackColor { get; set; } = Color.White;
        public Color TabForeColor { get; set; } = Color.Black;

        public Color ActiveTabBackColor { get; set; } = Color.DodgerBlue;
        public Color ActiveTabForeColor { get; set; } = Color.White;

        public Color InactiveTabBackColor { get; set; } = Color.LightGray;
        public Color InactiveTabForeColor { get; set; } = Color.DarkGray;

        public Color TabBorderColor { get; set; } = Color.LightGray;
        public Color TabHoverBackColor { get; set; } = Color.LightBlue;
        public Color TabHoverForeColor { get; set; } = Color.Black;

        public Color TabSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color TabSelectedForeColor { get; set; } = Color.White;

        public Color TabSelectedBorderColor { get; set; } = Color.Blue;
        public Color TabHoverBorderColor { get; set; } = Color.SteelBlue;
    }
}
