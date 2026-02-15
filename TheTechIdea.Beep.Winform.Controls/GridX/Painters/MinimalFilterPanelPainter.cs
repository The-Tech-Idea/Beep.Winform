using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    public sealed class MinimalFilterPanelPainter : BaseFilterPanelPainter
    {
        public override navigationStyle Style => navigationStyle.Minimal;
        public override string StyleName => "Minimal";

        public override void PaintFilterPanel(Graphics g, Rectangle panelRect, BeepGridPro grid, IBeepTheme? theme, Dictionary<int, Rectangle> filterCellRects, Dictionary<int, Rectangle> clearIconRects)
        {
            PaintModernToolbar(g, panelRect, grid, theme, filterCellRects, clearIconRects,
                ScaleModernToolbarOptions(new ModernToolbarOptions
                {
                    ControlHeight = 24,
                    CornerRadius = 6,
                    SearchMinWidth = 140,
                    SearchMaxWidth = 260,
                    ClearWidth = 84,
                    CountWidth = 88,
                    FilterWidth = 74,
                    FilterText = "Filter",
                    ClearText = "Clear",
                    CountFormat = "{0} active",
                    SearchPlaceholder = "Search all columns",
                    FlatControls = false
                }, grid));
        }

        protected override void ApplyStyleTokens(FilterPanelStyleTokens tokens, IBeepTheme? theme)
        {
            base.ApplyStyleTokens(tokens, theme);
            var accent = ResolveAccent(theme, tokens.ActiveGlyphColor);
            tokens.PanelBackColor = Blend(tokens.PanelBackColor, Color.White, 0.15f);
            tokens.PanelBorderColor = Blend(tokens.PanelBorderColor, accent, 0.03f);
            tokens.SeparatorColor = Alpha(tokens.PanelBorderColor, 150);
            tokens.InactiveTextColor = Blend(tokens.InactiveTextColor, tokens.ActiveTextColor, 0.45f);
            tokens.ChipCornerRadius = 10;
            tokens.ActiveGlyphColor = accent;
            tokens.ChipBackColor = Alpha(accent, 12);
            tokens.ChipBorderColor = Alpha(accent, 42);
            tokens.ChipTextColor = Blend(tokens.ActiveTextColor, accent, 0.20f);
            tokens.SearchBackColor = Blend(tokens.PanelBackColor, Color.White, 0.10f);
            tokens.BadgeBackColor = Blend(tokens.PanelBackColor, tokens.PanelBorderColor, 0.08f);
            tokens.ButtonBackColor = Alpha(accent, 10);
        }
    }
}
