using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class RusticTheme
    {
        // Grid Fonts
        public TypographyStyle GridHeaderFont { get; set; }
        public TypographyStyle GridRowFont { get; set; }
        public TypographyStyle GridCellFont { get; set; }
        public TypographyStyle GridCellSelectedFont { get; set; }
        public TypographyStyle GridCellHoverFont { get; set; }
        public TypographyStyle GridCellErrorFont { get; set; }
        public TypographyStyle GridColumnFont { get; set; }

        // Grid Colors
        public Color GridBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color GridForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color GridHeaderBackColor { get; set; } = Color.FromArgb(210, 180, 140);
        public Color GridHeaderForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color GridHeaderBorderColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color GridHeaderHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color GridHeaderHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color GridHeaderSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color GridHeaderSelectedForeColor { get; set; } = Color.White;
        public Color GridHeaderHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color GridHeaderSelectedBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color GridRowHoverBackColor { get; set; } = Color.FromArgb(222, 184, 135);
        public Color GridRowHoverForeColor { get; set; } = Color.FromArgb(62, 39, 35);
        public Color GridRowSelectedBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color GridRowSelectedForeColor { get; set; } = Color.White;
        public Color GridRowHoverBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color GridRowSelectedBorderColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color GridLineColor { get; set; } = Color.FromArgb(205, 133, 63);
        public Color RowBackColor { get; set; } = Color.FromArgb(245, 245, 220);
        public Color RowForeColor { get; set; } = Color.FromArgb(51, 51, 51);
        public Color AltRowBackColor { get; set; } = Color.FromArgb(238, 232, 205);
        public Color SelectedRowBackColor { get; set; } = Color.FromArgb(160, 82, 45);
        public Color SelectedRowForeColor { get; set; } = Color.White;
    }
}
