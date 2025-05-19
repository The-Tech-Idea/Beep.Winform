using System.Drawing;


namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // ComboBox Colors and Fonts
<<<<<<< HEAD
        public Color ComboBoxBackColor { get; set; } = Color.FromArgb(0x1F, 0x19, 0x39); // SurfaceColor (#1F1939)
        public Color ComboBoxForeColor { get; set; } = Color.FromArgb(0xC8, 0xC8, 0xC8); // OnBackgroundColor
        public Color ComboBoxBorderColor { get; set; } = Color.FromArgb(0x1A, 0x1A, 0x2E); // PrimaryColor (#1A1A2E)
        public Color ComboBoxHoverBackColor { get; set; } = Color.FromArgb(0x16, 0x21, 0x3E); // SecondaryColor (#16213E)
        public Color ComboBoxHoverForeColor { get; set; } = Color.FromArgb(0xC8, 0xC8, 0xC8); // OnBackgroundColor
        public Color ComboBoxHoverBorderColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor (#0F3460)
        public Color ComboBoxSelectedBackColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color ComboBoxSelectedForeColor { get; set; } = Color.White;                     // OnPrimaryColor
        public Color ComboBoxSelectedBorderColor { get; set; } = Color.White;                     // OnPrimaryColor
        public Color ComboBoxErrorBackColor { get; set; } = Color.FromArgb(0xFF, 0x45, 0x60); // ErrorColor (#FF4560)
        public Color ComboBoxErrorForeColor { get; set; } = Color.White;

        public Font ComboBoxItemFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Regular);
        public Font ComboBoxListFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Regular);

        public Color CheckBoxSelectedForeColor { get; set; } = Color.FromArgb(0x0F, 0x34, 0x60); // AccentColor
        public Color CheckBoxSelectedBackColor { get; set; } = Color.FromArgb(0x05, 0x05, 0x14); // BackgroundColor (#050514)
=======
        public Color ComboBoxBackColor { get; set; }
        public Color ComboBoxForeColor { get; set; }
        public Color ComboBoxBorderColor { get; set; }
        public Color ComboBoxHoverBackColor { get; set; }
        public Color ComboBoxHoverForeColor { get; set; }
        public Color ComboBoxHoverBorderColor { get; set; }
        public Color ComboBoxSelectedBackColor { get; set; }
        public Color ComboBoxSelectedForeColor { get; set; }
        public Color ComboBoxSelectedBorderColor { get; set; }
        public Color ComboBoxErrorBackColor { get; set; }
        public Color ComboBoxErrorForeColor { get; set; }
        public TypographyStyle ComboBoxItemFont { get; set; }
        public TypographyStyle ComboBoxListFont { get; set; }
        public Color CheckBoxSelectedForeColor { get; set; }
        public Color CheckBoxSelectedBackColor { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
