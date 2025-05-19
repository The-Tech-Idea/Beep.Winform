using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Tab Fonts & Colors
<<<<<<< HEAD
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
=======
        public TypographyStyle TabFont { get; set; }
        public TypographyStyle TabHoverFont { get; set; }
        public TypographyStyle TabSelectedFont { get; set; }
        public Color TabBackColor { get; set; }
        public Color TabForeColor { get; set; }
        public Color ActiveTabBackColor { get; set; }
        public Color ActiveTabForeColor { get; set; }
        public Color InactiveTabBackColor { get; set; }
        public Color InactiveTabForeColor { get; set; }
        public Color TabBorderColor { get; set; }
        public Color TabHoverBackColor { get; set; }
        public Color TabHoverForeColor { get; set; }
        public Color TabSelectedBackColor { get; set; }
        public Color TabSelectedForeColor { get; set; }
        public Color TabSelectedBorderColor { get; set; }
        public Color TabHoverBorderColor { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
