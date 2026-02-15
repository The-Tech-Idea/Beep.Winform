using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    public sealed class DataTablesFilterPanelPainter : BaseFilterPanelPainter
    {
        public override navigationStyle Style => navigationStyle.DataTables;
        public override string StyleName => "DataTables";

        public override void PaintFilterPanel(
            Graphics g,
            Rectangle panelRect,
            BeepGridPro grid,
            IBeepTheme? theme,
            Dictionary<int, Rectangle> filterCellRects,
            Dictionary<int, Rectangle> clearIconRects)
        {
            PaintModernToolbar(
                g,
                panelRect,
                grid,
                theme,
                filterCellRects,
                clearIconRects,
                ScaleModernToolbarOptions(new ModernToolbarOptions
                {
                    LeftPadding = 10,
                    RightPadding = 10,
                    Spacing = 8,
                    SeparatorInset = 7,
                    ControlHeight = 22,
                    CornerRadius = 2,
                    SearchMinWidth = 130,
                    SearchMaxWidth = 240,
                    ClearWidth = 78,
                    CountWidth = 100,
                    FilterWidth = 72,
                    SearchPlaceholder = "Search…",
                    FilterText = "Filters",
                    ClearText = "Clear",
                    CountFormat = "Filtered: {0}",
                    FlatControls = true
                }, grid));
        }

        protected override void ApplyStyleTokens(FilterPanelStyleTokens tokens, IBeepTheme? theme)
        {
            base.ApplyStyleTokens(tokens, theme);
            Color accent = ResolveAccent(theme, tokens.ActiveGlyphColor);
            tokens.ActiveGlyphColor = accent;
            tokens.PanelBackColor = Blend(tokens.PanelBackColor, Color.White, 0.08f);
            tokens.PanelBorderColor = Blend(tokens.PanelBorderColor, accent, 0.05f);
            tokens.SeparatorColor = Alpha(tokens.PanelBorderColor, 190);
            tokens.InactiveTextColor = Blend(tokens.InactiveTextColor, tokens.ActiveTextColor, 0.35f);
            tokens.SearchBackColor = Blend(tokens.PanelBackColor, Color.White, 0.22f);
            tokens.BadgeBackColor = Blend(tokens.PanelBackColor, tokens.PanelBorderColor, 0.18f);
            tokens.ButtonBackColor = Alpha(accent, 20);
        }
    }
}
