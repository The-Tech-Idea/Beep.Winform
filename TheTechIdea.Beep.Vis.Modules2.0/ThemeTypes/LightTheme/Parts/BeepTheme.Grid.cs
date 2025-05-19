using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class LightTheme
    {
        // Grid Fonts
<<<<<<< HEAD
        public Font GridHeaderFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Font GridRowFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);
        public Font GridCellFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);
        public Font GridCellSelectedFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Bold);
        public Font GridCellHoverFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Italic);
        public Font GridCellErrorFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);
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
        public Color GridBackColor { get; set; } = Color.White;
        public Color GridForeColor { get; set; } = Color.Black;
        public Color GridHeaderBackColor { get; set; } = Color.LightGray;
        public Color GridHeaderForeColor { get; set; } = Color.Black;
        public Color GridHeaderBorderColor { get; set; } = Color.DarkGray;
        public Color GridHeaderHoverBackColor { get; set; } = Color.Gainsboro;
        public Color GridHeaderHoverForeColor { get; set; } = Color.Black;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.Gray;
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.RoyalBlue;
        public Color GridRowHoverBackColor { get; set; } = Color.LightCyan;
        public Color GridRowHoverForeColor { get; set; } = Color.Black;
        public Color GridRowSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color GridRowSelectedForeColor { get; set; } = Color.White;
        public Color GridRowHoverBorderColor { get; set; } = Color.LightBlue;
        public Color GridRowSelectedBorderColor { get; set; } = Color.RoyalBlue;
        public Color GridLineColor { get; set; } = Color.LightGray;
        public Color RowBackColor { get; set; } = Color.White;
        public Color RowForeColor { get; set; } = Color.Black;
        public Color AltRowBackColor { get; set; } = Color.WhiteSmoke;
        public Color SelectedRowBackColor { get; set; } = Color.DodgerBlue;
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}
