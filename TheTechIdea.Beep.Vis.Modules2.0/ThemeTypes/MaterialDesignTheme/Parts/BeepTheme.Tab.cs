using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
<<<<<<< HEAD
        // Tab Fonts & Colors with Material Design defaults
        public Font TabFont { get; set; } = new Font("Roboto", 14f, FontStyle.Regular);
        public Font TabHoverFont { get; set; } = new Font("Roboto", 14f, FontStyle.Bold);
        public Font TabSelectedFont { get; set; } = new Font("Roboto", 14f, FontStyle.Bold);

        public Color TabBackColor { get; set; } = Color.FromArgb(250, 250, 250); // Grey 50
        public Color TabForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600
        public Color ActiveTabBackColor { get; set; } = Color.White;
        public Color ActiveTabForeColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color InactiveTabBackColor { get; set; } = Color.FromArgb(238, 238, 238); // Grey 200
        public Color InactiveTabForeColor { get; set; } = Color.FromArgb(117, 117, 117); // Grey 600

        public Color TabBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300
        public Color TabHoverBackColor { get; set; } = Color.FromArgb(232, 234, 246); // Light Blue 50
        public Color TabHoverForeColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color TabSelectedBackColor { get; set; } = Color.White;
        public Color TabSelectedForeColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color TabSelectedBorderColor { get; set; } = Color.FromArgb(25, 118, 210); // Blue 700
        public Color TabHoverBorderColor { get; set; } = Color.FromArgb(197, 202, 233); // Indigo 100
=======
        // Tab Fonts & Colors
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
