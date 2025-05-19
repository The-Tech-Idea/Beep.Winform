using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Button Colors and Styles
<<<<<<< HEAD
        public Font ButtonFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Regular);
        public Font ButtonHoverFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Regular);
        public Font ButtonSelectedFont { get; set; } = new Font("Segoe UI", 9F, FontStyle.Bold);
=======
        public TypographyStyle ButtonFont { get; set; }
        public TypographyStyle ButtonHoverFont { get; set; }
        public TypographyStyle ButtonSelectedFont { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634

        public Color ButtonHoverBackColor { get; set; } = Color.LightGray;
        public Color ButtonHoverForeColor { get; set; } = Color.Black;
        public Color ButtonHoverBorderColor { get; set; } = Color.Gray;

        public Color ButtonSelectedBorderColor { get; set; } = Color.DodgerBlue;
        public Color ButtonSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color ButtonSelectedForeColor { get; set; } = Color.White;

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.RoyalBlue;
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.RoyalBlue;

        public Color ButtonBackColor { get; set; } = Color.White;
        public Color ButtonForeColor { get; set; } = Color.Black;
        public Color ButtonBorderColor { get; set; } = Color.Gray;

        public Color ButtonErrorBackColor { get; set; } = Color.LightCoral;
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.Red;

        public Color ButtonPressedBackColor { get; set; } = Color.DodgerBlue;
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.RoyalBlue;
    }
}
