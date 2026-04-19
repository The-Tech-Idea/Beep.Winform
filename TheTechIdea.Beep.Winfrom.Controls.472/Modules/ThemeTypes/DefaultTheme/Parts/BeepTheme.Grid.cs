using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle GridRowFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle GridCellFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle GridCellSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);
        public TypographyStyle GridCellHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Italic);
        public TypographyStyle GridCellErrorFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Regular);
        public TypographyStyle GridColumnFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 8f, FontStyle.Bold);

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.White;
        public Color GridForeColor { get; set; } = Color.FromArgb(40, 40, 40);
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(20, 20, 20);
        public Color GridHeaderBorderColor { get; set; } = Color.LightGray;
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(230, 230, 250);
        public Color GridHeaderHoverForeColor { get; set; } = Color.Black;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(180, 200, 255);
        public Color GridHeaderSelectedForeColor { get; set; } = Color.Black;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.SteelBlue;
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.DodgerBlue;
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(245, 245, 255);
        public Color GridRowHoverForeColor { get; set; } = Color.Black;
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(200, 220, 255);
        public Color GridRowSelectedForeColor { get; set; } = Color.Black;
        public Color GridRowHoverBorderColor { get; set; } = Color.LightSteelBlue;
        public Color GridRowSelectedBorderColor { get; set; } = Color.RoyalBlue;
        public Color GridLineColor { get; set; } = Color.LightGray;
        public Color RowBackColor { get; set; } = Color.White;
        public Color RowForeColor { get; set; } = Color.Black;
        public Color AltRowBackColor { get; set; } = Color.FromArgb(250, 250, 250);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(200, 220, 255);
        public Color SelectedRowForeColor { get; set; } = Color.Black;
    }
}
