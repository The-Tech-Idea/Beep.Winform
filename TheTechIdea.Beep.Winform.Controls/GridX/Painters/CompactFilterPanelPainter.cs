using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    public sealed class CompactFilterPanelPainter : BaseFilterPanelPainter
    {
        public override navigationStyle Style => navigationStyle.Compact;
        public override string StyleName => "Compact";

        public override int CalculateFilterPanelHeight(BeepGridPro grid) => 30;

        public override void PaintFilterPanel(Graphics g, Rectangle panelRect, BeepGridPro grid, IBeepTheme? theme, Dictionary<int, Rectangle> filterCellRects, Dictionary<int, Rectangle> clearIconRects)
        {
            PaintModernToolbar(g, panelRect, grid, theme, filterCellRects, clearIconRects,
                new ModernToolbarOptions
                {
                    LeftPadding = 8,
                    RightPadding = 8,
                    Spacing = 6,
                    SeparatorInset = 6,
                    ControlHeight = 20,
                    CornerRadius = 3,
                    TitleMinWidth = 80,
                    SearchMinWidth = 120,
                    SearchMaxWidth = 210,
                    ClearWidth = 66,
                    CountWidth = 74,
                    FilterWidth = 64,
                    FilterText = "Filter",
                    ClearText = "Clear",
                    CountFormat = "{0}",
                    SearchPlaceholder = "Search",
                    FlatControls = true
                });
        }

        protected override void ApplyStyleTokens(FilterPanelStyleTokens tokens, IBeepTheme? theme)
        {
            base.ApplyStyleTokens(tokens, theme);
            var accent = ResolveAccent(theme, tokens.ActiveGlyphColor);
            tokens.PanelBackColor = Blend(tokens.PanelBackColor, Color.White, 0.05f);
            tokens.PanelBorderColor = Blend(tokens.PanelBorderColor, accent, 0.04f);
            tokens.SeparatorColor = Alpha(tokens.PanelBorderColor, 170);
            tokens.ChipCornerRadius = 3;
            tokens.ActiveGlyphColor = accent;
            tokens.ChipBackColor = Alpha(accent, 16);
            tokens.ChipBorderColor = Alpha(accent, 56);
            tokens.ChipTextColor = Blend(tokens.ActiveTextColor, accent, 0.22f);
            tokens.SearchBackColor = Blend(tokens.PanelBackColor, Color.White, 0.14f);
            tokens.BadgeBackColor = Blend(tokens.PanelBackColor, tokens.PanelBorderColor, 0.14f);
            tokens.ButtonBackColor = Alpha(accent, 14);
        }
    }
}
