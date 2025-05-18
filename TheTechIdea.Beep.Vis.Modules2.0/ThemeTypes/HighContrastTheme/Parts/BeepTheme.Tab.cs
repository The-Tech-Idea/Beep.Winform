using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Tab Fonts & Colors
        public Font TabFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font TabHoverFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font TabSelectedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color TabBackColor { get; set; } = Color.Black;
        public Color TabForeColor { get; set; } = Color.White;
        public Color ActiveTabBackColor { get; set; } = Color.Yellow;
        public Color ActiveTabForeColor { get; set; } = Color.Black;
        public Color InactiveTabBackColor { get; set; } = Color.DarkGray;
        public Color InactiveTabForeColor { get; set; } = Color.White;
        public Color TabBorderColor { get; set; } = Color.White;
        public Color TabHoverBackColor { get; set; } = Color.Gray;
        public Color TabHoverForeColor { get; set; } = Color.Yellow;
        public Color TabSelectedBackColor { get; set; } = Color.Lime;
        public Color TabSelectedForeColor { get; set; } = Color.Black;
        public Color TabSelectedBorderColor { get; set; } = Color.White;
        public Color TabHoverBorderColor { get; set; } = Color.Yellow;
    }
}
