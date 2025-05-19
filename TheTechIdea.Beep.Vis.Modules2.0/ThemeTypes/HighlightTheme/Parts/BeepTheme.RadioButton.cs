using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // RadioButton properties
<<<<<<< HEAD
        public Color RadioButtonBackColor { get; set; } = Color.White;
        public Color RadioButtonForeColor { get; set; } = Color.Black;
        public Color RadioButtonBorderColor { get; set; } = Color.Gray;
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(0, 120, 215);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(230, 240, 255);
        public Color RadioButtonHoverForeColor { get; set; } = Color.Black;
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 100, 180);
        public Font RadioButtonFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);
        public Font RadioButtonCheckedFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Bold);
        public Color RadioButtonSelectedForeColor { get; set; } = Color.White;
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(0, 120, 215);
=======
        public Color RadioButtonBackColor { get; set; }
        public Color RadioButtonForeColor { get; set; }
        public Color RadioButtonBorderColor { get; set; }
        public Color RadioButtonCheckedBackColor { get; set; }
        public Color RadioButtonCheckedForeColor { get; set; }
        public Color RadioButtonCheckedBorderColor { get; set; }
        public Color RadioButtonHoverBackColor { get; set; }
        public Color RadioButtonHoverForeColor { get; set; }
        public Color RadioButtonHoverBorderColor { get; set; }
        public TypographyStyle RadioButtonFont { get; set; }
        public TypographyStyle RadioButtonCheckedFont { get; set; }
        public Color RadioButtonSelectedForeColor { get; set; }
        public Color RadioButtonSelectedBackColor { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
