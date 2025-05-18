using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Tab Fonts & Colors
        public Font TabFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font TabHoverFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font TabSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);

        public Color TabBackColor { get; set; } = Color.FromArgb(30, 30, 40);
        public Color TabForeColor { get; set; } = Color.LightGray;
        public Color ActiveTabBackColor { get; set; } = Color.FromArgb(50, 50, 70);
        public Color ActiveTabForeColor { get; set; } = Color.White;
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(30, 30, 40);
        public Color InactiveTabForeColor { get; set; } = Color.Gray;
        public Color TabBorderColor { get; set; } = Color.DimGray;
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(60, 60, 80);
        public Color TabHoverForeColor { get; set; } = Color.White;
        public Color TabSelectedBackColor { get; set; } = Color.FromArgb(70, 70, 90);
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.SteelBlue;
        public Color TabHoverBorderColor { get; set; } = Color.LightSteelBlue;
    }
}
