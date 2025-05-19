using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Grid Fonts
<<<<<<< HEAD
        public Font GridHeaderFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font GridRowFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);
        public Font GridCellFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);
        public Font GridCellSelectedFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Bold);
        public Font GridCellHoverFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Italic);
        public Font GridCellErrorFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Underline);
        public Font GridColumnFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Bold);
=======
        public TypographyStyle GridHeaderFont { get; set; }
        public TypographyStyle GridRowFont { get; set; }
        public TypographyStyle GridCellFont { get; set; }
        public TypographyStyle GridCellSelectedFont { get; set; }
        public TypographyStyle GridCellHoverFont { get; set; }
        public TypographyStyle GridCellErrorFont { get; set; }
        public TypographyStyle GridColumnFont { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.Black;
        public Color GridForeColor { get; set; } = Color.White;
        public Color GridHeaderBackColor { get; set; } = Color.DarkSlateGray;
        public Color GridHeaderForeColor { get; set; } = Color.White;
        public Color GridHeaderBorderColor { get; set; } = Color.White;
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color GridHeaderHoverForeColor { get; set; } = Color.Yellow;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.Yellow;
        public Color GridHeaderSelectedForeColor { get; set; } = Color.Black;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.Yellow;
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.Black;
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color GridRowHoverForeColor { get; set; } = Color.White;
        public Color GridRowSelectedBackColor { get; set; } = Color.White;
        public Color GridRowSelectedForeColor { get; set; } = Color.Black;
        public Color GridRowHoverBorderColor { get; set; } = Color.Gray;
        public Color GridRowSelectedBorderColor { get; set; } = Color.Yellow;
        public Color GridLineColor { get; set; } = Color.White;
        public Color RowBackColor { get; set; } = Color.Black;
        public Color RowForeColor { get; set; } = Color.White;
        public Color AltRowBackColor { get; set; } = Color.FromArgb(20, 20, 20);
        public Color SelectedRowBackColor { get; set; } = Color.Yellow;
        public Color SelectedRowForeColor { get; set; } = Color.Black;
    }
}
