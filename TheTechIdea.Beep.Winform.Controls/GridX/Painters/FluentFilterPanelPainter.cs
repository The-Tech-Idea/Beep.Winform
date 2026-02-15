using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    public sealed class FluentFilterPanelPainter : BaseFilterPanelPainter
    {
        public override navigationStyle Style => navigationStyle.Fluent;
        public override string StyleName => "Fluent";

        public override void PaintFilterPanel(Graphics g, Rectangle panelRect, BeepGridPro grid, IBeepTheme? theme, Dictionary<int, Rectangle> filterCellRects, Dictionary<int, Rectangle> clearIconRects)
        {
            PaintModernToolbar(g, panelRect, grid, theme, filterCellRects, clearIconRects,
                new ModernToolbarOptions
                {
                    ControlHeight = 24,
                    CornerRadius = 3,
                    SearchMinWidth = 150,
                    SearchMaxWidth = 270,
                    ClearWidth = 84,
                    CountWidth = 90,
                    FilterWidth = 76,
                    FilterText = "Filter",
                    ClearText = "Reset",
                    CountFormat = "{0} active",
                    SearchPlaceholder = "Search all",
                    FlatControls = false
                });
        }

        protected override void ApplyStyleTokens(FilterPanelStyleTokens tokens, IBeepTheme? theme)
        {
            base.ApplyStyleTokens(tokens, theme);
            var accent = ResolveAccent(theme, tokens.ActiveGlyphColor);
            tokens.PanelBackColor = Blend(tokens.PanelBackColor, Color.White, 0.10f);
            tokens.PanelBorderColor = Blend(tokens.PanelBorderColor, accent, 0.07f);
            tokens.SeparatorColor = Alpha(tokens.PanelBorderColor, 185);
            tokens.ChipCornerRadius = 3;
            tokens.ActiveGlyphColor = accent;
            tokens.ChipBackColor = Alpha(accent, 22);
            tokens.ChipBorderColor = Alpha(accent, 80);
            tokens.ChipTextColor = Blend(tokens.ActiveTextColor, accent, 0.30f);
            tokens.SearchBackColor = Blend(tokens.PanelBackColor, Color.White, 0.24f);
            tokens.BadgeBackColor = Alpha(accent, 20);
            tokens.ButtonBackColor = Alpha(accent, 22);
        }
    }
}
