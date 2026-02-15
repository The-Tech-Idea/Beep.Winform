using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    public sealed class AntDesignFilterPanelPainter : BaseFilterPanelPainter
    {
        public override navigationStyle Style => navigationStyle.AntDesign;
        public override string StyleName => "AntDesign";

        public override void PaintFilterPanel(Graphics g, Rectangle panelRect, BeepGridPro grid, IBeepTheme? theme, Dictionary<int, Rectangle> filterCellRects, Dictionary<int, Rectangle> clearIconRects)
        {
            PaintModernToolbar(g, panelRect, grid, theme, filterCellRects, clearIconRects,
                new ModernToolbarOptions
                {
                    ControlHeight = 24,
                    CornerRadius = 6,
                    SearchMinWidth = 150,
                    SearchMaxWidth = 270,
                    ClearWidth = 86,
                    CountWidth = 90,
                    FilterWidth = 74,
                    FilterText = "Filters",
                    ClearText = "Clear",
                    CountFormat = "{0} rules",
                    SearchPlaceholder = "Search records",
                    FlatControls = false
                });
        }

        protected override void ApplyStyleTokens(FilterPanelStyleTokens tokens, IBeepTheme? theme)
        {
            base.ApplyStyleTokens(tokens, theme);
            var accent = ResolveAccent(theme, tokens.ActiveGlyphColor);
            tokens.PanelBackColor = Blend(tokens.PanelBackColor, Color.White, 0.12f);
            tokens.PanelBorderColor = Blend(tokens.PanelBorderColor, accent, 0.08f);
            tokens.SeparatorColor = Alpha(tokens.PanelBorderColor, 185);
            tokens.ChipCornerRadius = 6;
            tokens.ActiveGlyphColor = accent;
            tokens.ChipBackColor = Alpha(accent, 24);
            tokens.ChipBorderColor = Alpha(accent, 88);
            tokens.ChipTextColor = Blend(tokens.ActiveTextColor, accent, 0.35f);
            tokens.SearchBackColor = Blend(tokens.PanelBackColor, Color.White, 0.22f);
            tokens.BadgeBackColor = Alpha(accent, 22);
            tokens.ButtonBackColor = Alpha(accent, 24);
        }
    }
}
