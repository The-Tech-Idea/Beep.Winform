using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Label Colors and Fonts
<<<<<<< HEAD
        public Color LabelBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color LabelForeColor { get; set; } = Color.Black;
        public Color LabelBorderColor { get; set; } = Color.FromArgb(200, 210, 220);
        public Color LabelHoverBorderColor { get; set; } = Color.SteelBlue;
        public Color LabelHoverBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color LabelHoverForeColor { get; set; } = Color.Black;

        public Color LabelSelectedBorderColor { get; set; } = Color.DodgerBlue;
        public Color LabelSelectedBackColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color LabelSelectedForeColor { get; set; } = Color.Black;

        public Color LabelDisabledBackColor { get; set; } = Color.LightGray;
        public Color LabelDisabledForeColor { get; set; } = Color.DarkGray;
        public Color LabelDisabledBorderColor { get; set; } = Color.Gray;

        public Font LabelFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font SubLabelFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Italic);

        public Color SubLabelForColor { get; set; } = Color.DimGray;
        public Color SubLabelBackColor { get; set; } = Color.WhiteSmoke;
        public Color SubLabelHoverBackColor { get; set; } = Color.LightBlue;
        public Color SubLabelHoverForeColor { get; set; } = Color.Black;
=======
        public Color LabelBackColor { get; set; }
        public Color LabelForeColor { get; set; }
        public Color LabelBorderColor { get; set; }
        public Color LabelHoverBorderColor { get; set; }
        public Color LabelHoverBackColor { get; set; }
        public Color LabelHoverForeColor { get; set; }
        public Color LabelSelectedBorderColor { get; set; }
        public Color LabelSelectedBackColor { get; set; }
        public Color LabelSelectedForeColor { get; set; }
        public Color LabelDisabledBackColor { get; set; }
        public Color LabelDisabledForeColor { get; set; }
        public Color LabelDisabledBorderColor { get; set; }
        public TypographyStyle LabelFont { get; set; }
        public TypographyStyle SubLabelFont { get; set; }
        public Color SubLabelForColor { get; set; }
        public Color SubLabelBackColor { get; set; }
        public Color SubLabelHoverBackColor { get; set; }
        public Color SubLabelHoverForeColor { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
