using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Tab Fonts & Colors
<<<<<<< HEAD
        public Font TabFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Font TabHoverFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font TabSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Color TabBackColor { get; set; } = Color.LightGray;
        public Color TabForeColor { get; set; } = Color.Black;
        public Color ActiveTabBackColor { get; set; } = Color.DodgerBlue;
        public Color ActiveTabForeColor { get; set; } = Color.White;
        public Color InactiveTabBackColor { get; set; } = Color.Gray;
        public Color InactiveTabForeColor { get; set; } = Color.DarkGray;
        public Color TabBorderColor { get; set; } = Color.Silver;
        public Color TabHoverBackColor { get; set; } = Color.SkyBlue;
        public Color TabHoverForeColor { get; set; } = Color.Black;
        public Color TabSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color TabSelectedForeColor { get; set; } = Color.White;
        public Color TabSelectedBorderColor { get; set; } = Color.DodgerBlue;
        public Color TabHoverBorderColor { get; set; } = Color.SkyBlue;
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
