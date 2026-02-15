using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    public sealed class CardFilterPanelPainter : BaseFilterPanelPainter
    {
        public override navigationStyle Style => navigationStyle.Card;
        public override string StyleName => "Card";

        public override void PaintFilterPanel(Graphics g, Rectangle panelRect, BeepGridPro grid, IBeepTheme? theme, Dictionary<int, Rectangle> filterCellRects, Dictionary<int, Rectangle> clearIconRects)
        {
            PaintModernToolbar(g, panelRect, grid, theme, filterCellRects, clearIconRects,
                new ModernToolbarOptions
                {
                    LeftPadding = 14,
                    RightPadding = 14,
                    ControlHeight = 26,
                    CornerRadius = 12,
                    SearchMinWidth = 150,
                    SearchMaxWidth = 290,
                    ClearWidth = 90,
                    CountWidth = 96,
                    FilterWidth = 78,
                    FilterText = "Filters",
                    ClearText = "Clear All",
                    CountFormat = "{0} active",
                    SearchPlaceholder = "Search cards",
                    FlatControls = false
                });
        }

        protected override void ApplyStyleTokens(FilterPanelStyleTokens tokens, IBeepTheme? theme)
        {
            base.ApplyStyleTokens(tokens, theme);
            var accent = ResolveAccent(theme, tokens.ActiveGlyphColor);
            tokens.ActiveGlyphColor = accent;
            tokens.PanelBackColor = Blend(tokens.PanelBackColor, Color.White, 0.14f);
            tokens.PanelBorderColor = Blend(tokens.PanelBorderColor, accent, 0.08f);
            tokens.SeparatorColor = Alpha(tokens.PanelBorderColor, 185);
            tokens.ChipCornerRadius = 12;
            tokens.ChipBackColor = Alpha(accent, 24);
            tokens.ChipBorderColor = Alpha(accent, 88);
            tokens.ChipTextColor = Blend(tokens.ActiveTextColor, accent, 0.32f);
            tokens.SearchBackColor = Blend(tokens.PanelBackColor, Color.White, 0.24f);
            tokens.BadgeBackColor = Alpha(accent, 22);
            tokens.ButtonBackColor = Alpha(accent, 26);
        }
    }
}
