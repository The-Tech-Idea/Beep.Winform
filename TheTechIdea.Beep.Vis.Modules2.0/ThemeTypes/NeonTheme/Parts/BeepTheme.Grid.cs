using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class NeonTheme
    {
        // Grid Fonts
        public Font GridHeaderFont { get; set; }
        public Font GridRowFont { get; set; }
        public Font GridCellFont { get; set; }
        public Font GridCellSelectedFont { get; set; }
        public Font GridCellHoverFont { get; set; }
        public Font GridCellErrorFont { get; set; }
        public Font GridColumnFont { get; set; }

        // Grid Colors
        public Color GridBackColor { get; set; }
        public Color GridForeColor { get; set; }
        public Color GridHeaderBackColor { get; set; }
        public Color GridHeaderForeColor { get; set; }
        public Color GridHeaderBorderColor { get; set; }
        public Color GridHeaderHoverBackColor { get; set; }
        public Color GridHeaderHoverForeColor { get; set; }
        public Color GridHeaderSelectedBackColor { get; set; }
        public Color GridHeaderSelectedForeColor { get; set; }
        public Color GridHeaderHoverBorderColor { get; set; }
        public Color GridHeaderSelectedBorderColor { get; set; }
        public Color GridRowHoverBackColor { get; set; }
        public Color GridRowHoverForeColor { get; set; }
        public Color GridRowSelectedBackColor { get; set; }
        public Color GridRowSelectedForeColor { get; set; }
        public Color GridRowHoverBorderColor { get; set; }
        public Color GridRowSelectedBorderColor { get; set; }
        public Color GridLineColor { get; set; }
        public Color RowBackColor { get; set; }
        public Color RowForeColor { get; set; }
        public Color AltRowBackColor { get; set; }
        public Color SelectedRowBackColor { get; set; }
        public Color SelectedRowForeColor { get; set; }
    }
}
