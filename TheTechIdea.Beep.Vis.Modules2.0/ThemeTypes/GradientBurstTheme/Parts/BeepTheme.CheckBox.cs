using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // CheckBox properties
<<<<<<< HEAD
        public Color CheckBoxBackColor { get; set; } = Color.White;
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(33, 33, 33);
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(120, 144, 156); // Blue Gray

        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(76, 175, 80);   // Green
        public Color CheckBoxCheckedForeColor { get; set; } = Color.White;
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(56, 142, 60);   // Dark Green

        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(232, 240, 253); // Light Blue Hover
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(25, 118, 210);  // Blue
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(21, 101, 192);  // Darker Blue

        public Font CheckBoxFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Regular);
        public Font CheckBoxCheckedFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Bold);
=======
        public Color CheckBoxBackColor { get; set; }
        public Color CheckBoxForeColor { get; set; }
        public Color CheckBoxBorderColor { get; set; }
        public Color CheckBoxCheckedBackColor { get; set; }
        public Color CheckBoxCheckedForeColor { get; set; }
        public Color CheckBoxCheckedBorderColor { get; set; }
        public Color CheckBoxHoverBackColor { get; set; }
        public Color CheckBoxHoverForeColor { get; set; }
        public Color CheckBoxHoverBorderColor { get; set; }
        public TypographyStyle CheckBoxFont { get; set; }
        public TypographyStyle CheckBoxCheckedFont { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
