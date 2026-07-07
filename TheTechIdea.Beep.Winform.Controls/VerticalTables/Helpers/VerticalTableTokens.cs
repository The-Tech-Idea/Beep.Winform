namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers
{
    /// <summary>
    /// Shared vertical-table design tokens.
    /// All logical values expressed in base pixels and scaled at render/layout time via
    /// <c>DpiScalingHelper.ScaleValue()</c>.
    /// </summary>
    internal static class VerticalTableTokens
    {
        public const int HeaderHeight   = 80;
        public const int RowHeight      = 40;
        public const int ColumnWidth    = 150;
        public const int CellPadding    = 8;
        public const int ColumnSpacing  = 8;
        public const int HeaderFontSize = 12;
        public const int CellFontSize   = 10;
        public const int IconSize       = 24;     // feature check/cross icon size
        public const int BadgeHeight    = 22;     // "Best Value" badge chip height
        public const int BorderRadius   = 12;     // card corner radius
        public const int ShadowOffset   = 4;      // card shadow Y offset
        public const int MinColumnW     = 100;    // minimum column width
    }
}
