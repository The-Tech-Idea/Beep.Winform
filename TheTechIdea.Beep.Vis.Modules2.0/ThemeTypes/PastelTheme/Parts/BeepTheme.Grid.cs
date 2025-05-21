using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class PastelTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Bold, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle GridRowFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public TypographyStyle GridCellFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(120, 120, 120) };
        public TypographyStyle GridCellSelectedFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Medium, TextColor = Color.White };
        public TypographyStyle GridCellHoverFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(80, 80, 80) };
        public TypographyStyle GridCellErrorFont { get; set; } = new TypographyStyle() { FontSize = 11, FontWeight = FontWeight.Regular, TextColor = Color.FromArgb(255, 182, 182) };
        public TypographyStyle GridColumnFont { get; set; } = new TypographyStyle() { FontSize = 12, FontWeight = FontWeight.Medium, TextColor = Color.FromArgb(80, 80, 80) };

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color GridForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(242, 201, 215);
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color GridHeaderHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(247, 221, 229);
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(255, 224, 239);
        public Color GridRowHoverForeColor { get; set; } = Color.FromArgb(80, 80, 80);
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color GridRowSelectedForeColor { get; set; } = Color.White;
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(247, 221, 229);
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(230, 170, 190);
        public Color GridLineColor { get; set; } = Color.FromArgb(237, 181, 201);
        public Color RowBackColor { get; set; } = Color.FromArgb(255, 245, 247);
        public Color RowForeColor { get; set; } = Color.FromArgb(120, 120, 120);
        public Color AltRowBackColor { get; set; } = Color.FromArgb(245, 235, 237);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(245, 183, 203);
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}