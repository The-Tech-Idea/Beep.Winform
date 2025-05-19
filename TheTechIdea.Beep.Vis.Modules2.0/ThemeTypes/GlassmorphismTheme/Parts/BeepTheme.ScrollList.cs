using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // ScrollList Fonts & Colors
<<<<<<< HEAD
        public Font ScrollListTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font ScrollListSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font ScrollListUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font ScrollListIItemFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font ScrollListItemSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);

        public Color ScrollListBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color ScrollListForeColor { get; set; } = Color.Black;
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(200, 210, 220);

        public Color ScrollListItemForeColor { get; set; } = Color.Black;
        public Color ScrollListItemHoverForeColor { get; set; } = Color.Black;
        public Color ScrollListItemHoverBackColor { get; set; } = Color.LightBlue;

        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.DeepSkyBlue;
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.DodgerBlue;

        public Color ScrollListItemBorderColor { get; set; } = Color.LightGray;
=======
        public TypographyStyle ScrollListTitleFont { get; set; }
        public TypographyStyle ScrollListSelectedFont { get; set; }
        public TypographyStyle ScrollListUnSelectedFont { get; set; }
        public Color ScrollListBackColor { get; set; }
        public Color ScrollListForeColor { get; set; }
        public Color ScrollListBorderColor { get; set; }
        public Color ScrollListItemForeColor { get; set; }
        public Color ScrollListItemHoverForeColor { get; set; }
        public Color ScrollListItemHoverBackColor { get; set; }
        public Color ScrollListItemSelectedForeColor { get; set; }
        public Color ScrollListItemSelectedBackColor { get; set; }
        public Color ScrollListItemSelectedBorderColor { get; set; }
        public Color ScrollListItemBorderColor { get; set; }
        public TypographyStyle ScrollListIItemFont { get; set; }
        public TypographyStyle ScrollListItemSelectedFont { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}
