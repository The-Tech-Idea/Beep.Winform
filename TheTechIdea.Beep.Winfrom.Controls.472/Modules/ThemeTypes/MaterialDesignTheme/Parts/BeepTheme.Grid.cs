using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Grid Fonts
        public TypographyStyle  GridHeaderFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold);
        public TypographyStyle  GridRowFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);
        public TypographyStyle  GridCellFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);
        public TypographyStyle  GridCellSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold);
        public TypographyStyle  GridCellHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Regular);
        public TypographyStyle  GridCellErrorFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Italic);
        public TypographyStyle  GridColumnFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Roboto", 8f, FontStyle.Bold);

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.White;
        public Color GridForeColor { get; set; } = Color.Black;
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(238, 238, 238); // Light Grey
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(33, 33, 33);    // Dark Grey
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(224, 224, 224);
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(210, 210, 210);
        public Color GridHeaderHoverForeColor { get; set; } = Color.Black;
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Material Blue
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(33, 150, 243);
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(30, 136, 229);
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(245, 245, 245);
        public Color GridRowHoverForeColor { get; set; } = Color.Black;
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(227, 242, 253);
        public Color GridRowSelectedForeColor { get; set; } = Color.Black;
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(200, 200, 200);
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(33, 150, 243);
        public Color GridLineColor { get; set; } = Color.FromArgb(224, 224, 224);
        public Color RowBackColor { get; set; } = Color.White;
        public Color RowForeColor { get; set; } = Color.Black;
        public Color AltRowBackColor { get; set; } = Color.FromArgb(250, 250, 250);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(227, 242, 253);
        public Color SelectedRowForeColor { get; set; } = Color.Black;
    }
}
