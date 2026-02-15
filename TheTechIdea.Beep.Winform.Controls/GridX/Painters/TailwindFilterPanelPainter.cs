using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    public sealed class TailwindFilterPanelPainter : BaseFilterPanelPainter
    {
        public override navigationStyle Style => navigationStyle.Tailwind;
        public override string StyleName => "Tailwind";

        public override void PaintFilterPanel(Graphics g, Rectangle panelRect, BeepGridPro grid, IBeepTheme? theme, Dictionary<int, Rectangle> filterCellRects, Dictionary<int, Rectangle> clearIconRects)
        {
            PaintModernToolbar(g, panelRect, grid, theme, filterCellRects, clearIconRects,
                new ModernToolbarOptions
                {
                    ControlHeight = 24,
                    CornerRadius = 999,
                    SearchMinWidth = 155,
                    SearchMaxWidth = 295,
                    ClearWidth = 88,
                    CountWidth = 92,
                    FilterWidth = 74,
                    FilterText = "Filter",
                    ClearText = "Clear",
                    CountFormat = "{0} active",
                    SearchPlaceholder = "Search...",
                    FlatControls = false
                });
        }

        protected override void ApplyStyleTokens(FilterPanelStyleTokens tokens, IBeepTheme? theme)
        {
            base.ApplyStyleTokens(tokens, theme);
            var accent = ResolveAccent(theme, tokens.ActiveGlyphColor);
            tokens.PanelBackColor = Blend(tokens.PanelBackColor, Color.White, 0.11f);
            tokens.PanelBorderColor = Blend(tokens.PanelBorderColor, accent, 0.06f);
            tokens.SeparatorColor = Alpha(tokens.PanelBorderColor, 180);
            tokens.ChipCornerRadius = 999;
            tokens.ActiveGlyphColor = accent;
            tokens.ChipBackColor = Alpha(accent, 22);
            tokens.ChipBorderColor = Alpha(accent, 82);
            tokens.ChipTextColor = Blend(tokens.ActiveTextColor, accent, 0.36f);
            tokens.SearchBackColor = Blend(tokens.PanelBackColor, Color.White, 0.22f);
            tokens.BadgeBackColor = Alpha(accent, 20);
            tokens.ButtonBackColor = Alpha(accent, 24);
        }
    }
}
