using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Grid Fonts
        public TypographyStyle  GridHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle  GridRowFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle  GridCellFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Regular);
        public TypographyStyle  GridCellSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);
        public TypographyStyle  GridCellHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Italic);
        public TypographyStyle  GridCellErrorFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Strikeout);
        public TypographyStyle  GridColumnFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10f, FontStyle.Bold);

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(245, 250, 255);
        public Color GridForeColor { get; set; } = Color.Black;

        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color GridHeaderForeColor { get; set; } = Color.Black;
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(180, 200, 220);
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(210, 220, 240);
        public Color GridHeaderHoverForeColor { get; set; } = Color.Black;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color GridHeaderSelectedForeColor { get; set; } = Color.Black;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.SteelBlue;
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.DodgerBlue;

        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(230, 240, 250);
        public Color GridRowHoverForeColor { get; set; } = Color.Black;
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(200, 220, 240);
        public Color GridRowSelectedForeColor { get; set; } = Color.Black;
        public Color GridRowHoverBorderColor { get; set; } = Color.LightBlue;
        public Color GridRowSelectedBorderColor { get; set; } = Color.CornflowerBlue;

        public Color GridLineColor { get; set; } = Color.LightGray;
        public Color RowBackColor { get; set; } = Color.White;
        public Color RowForeColor { get; set; } = Color.Black;
        public Color AltRowBackColor { get; set; } = Color.FromArgb(240, 245, 250);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(210, 225, 240);
        public Color SelectedRowForeColor { get; set; } = Color.Black;
    }
}
