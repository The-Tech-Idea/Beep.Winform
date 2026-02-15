using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    public sealed class TelerikFilterPanelPainter : BaseFilterPanelPainter
    {
        public override navigationStyle Style => navigationStyle.Telerik;
        public override string StyleName => "Telerik";

        public override void PaintFilterPanel(Graphics g, Rectangle panelRect, BeepGridPro grid, IBeepTheme? theme, Dictionary<int, Rectangle> filterCellRects, Dictionary<int, Rectangle> clearIconRects)
        {
            PaintModernToolbar(g, panelRect, grid, theme, filterCellRects, clearIconRects,
                new ModernToolbarOptions
                {
                    ControlHeight = 24,
                    CornerRadius = 2,
                    SearchMinWidth = 145,
                    SearchMaxWidth = 250,
                    ClearWidth = 80,
                    CountWidth = 92,
                    FilterWidth = 74,
                    FilterText = "Filter",
                    ClearText = "Clear",
                    CountFormat = "{0} rules",
                    SearchPlaceholder = "Search records",
                    FlatControls = true
                });
        }

        protected override void ApplyStyleTokens(FilterPanelStyleTokens tokens, IBeepTheme? theme)
        {
            base.ApplyStyleTokens(tokens, theme);
            var accent = ResolveAccent(theme, tokens.ActiveGlyphColor);
            tokens.PanelBackColor = Blend(tokens.PanelBackColor, Color.White, 0.06f);
            tokens.PanelBorderColor = Blend(tokens.PanelBorderColor, accent, 0.08f);
            tokens.SeparatorColor = Alpha(tokens.PanelBorderColor, 185);
            tokens.ChipCornerRadius = 2;
            tokens.ActiveGlyphColor = accent;
            tokens.ChipBackColor = Alpha(accent, 20);
            tokens.ChipBorderColor = Alpha(accent, 68);
            tokens.ChipTextColor = Blend(tokens.ActiveTextColor, accent, 0.28f);
            tokens.SearchBackColor = Blend(tokens.PanelBackColor, Color.White, 0.18f);
            tokens.BadgeBackColor = Blend(tokens.PanelBackColor, tokens.PanelBorderColor, 0.16f);
            tokens.ButtonBackColor = Alpha(accent, 18);
        }
    }
}
