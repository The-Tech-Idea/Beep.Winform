using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
<<<<<<< HEAD
        // ScrollList Fonts & Colors with default Material Design values
        public Font ScrollListTitleFont { get; set; } = new Font("Roboto", 14f, FontStyle.Bold);
        public Font ScrollListSelectedFont { get; set; } = new Font("Roboto", 12f, FontStyle.Bold);
        public Font ScrollListUnSelectedFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);

        public Color ScrollListBackColor { get; set; } = Color.White;
        public Color ScrollListForeColor { get; set; } = Color.Black;
        public Color ScrollListBorderColor { get; set; } = Color.FromArgb(224, 224, 224);

        public Color ScrollListItemForeColor { get; set; } = Color.Black;
        public Color ScrollListItemHoverForeColor { get; set; } = Color.FromArgb(33, 150, 243);  // Material blue
        public Color ScrollListItemHoverBackColor { get; set; } = Color.FromArgb(232, 240, 254);  // Light blue hover

        public Color ScrollListItemSelectedForeColor { get; set; } = Color.White;
        public Color ScrollListItemSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243);
        public Color ScrollListItemSelectedBorderColor { get; set; } = Color.FromArgb(25, 118, 210);

        public Color ScrollListItemBorderColor { get; set; } = Color.FromArgb(224, 224, 224);

        public Font ScrollListIItemFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);
        public Font ScrollListItemSelectedFont { get; set; } = new Font("Roboto", 12f, FontStyle.Bold);
=======
        // ScrollList Fonts & Colors
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
