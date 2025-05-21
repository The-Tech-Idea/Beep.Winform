using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 11f, FontStyle.Bold);
        public TypographyStyle GridRowFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);
        public TypographyStyle GridCellFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Regular);
        public TypographyStyle GridCellSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 10.5f, FontStyle.Bold);
        public TypographyStyle GridCellHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Italic);
        public TypographyStyle GridCellErrorFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10.5f, FontStyle.Bold | FontStyle.Italic);
        public TypographyStyle GridColumnFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Comic Sans MS", 10.5f, FontStyle.Bold);

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public Color GridForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy

        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint

        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Baby Blue
        public Color GridHeaderHoverForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy

        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public Color GridHeaderSelectedForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(127, 255, 212); // Mint

        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint
        public Color GridRowHoverForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink
        public Color GridRowSelectedForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(206, 183, 255); // Lavender
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(255, 223, 93); // Lemon

        public Color GridLineColor { get; set; } = Color.FromArgb(228, 222, 255); // Pastel Lavender (gentle but visible)

        public Color RowBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon Yellow
        public Color RowForeColor { get; set; } = Color.FromArgb(44, 62, 80);    // Navy

        public Color AltRowBackColor { get; set; } = Color.FromArgb(255, 224, 235); // Pastel Pink (for alternate rows)
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint (for selected row)
        public Color SelectedRowForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
    }
}
