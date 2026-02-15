using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    public sealed class BootstrapFilterPanelPainter : BaseFilterPanelPainter
    {
        public override navigationStyle Style => navigationStyle.Bootstrap;
        public override string StyleName => "Bootstrap";

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
            tokens.PanelBackColor = Blend(tokens.PanelBackColor, Color.White, 0.08f);
            tokens.PanelBorderColor = Blend(tokens.PanelBorderColor, accent, 0.06f);
            tokens.SeparatorColor = Alpha(tokens.PanelBorderColor, 190);
            tokens.ChipCornerRadius = 4;
            tokens.ActiveGlyphColor = accent;
            tokens.ChipBackColor = Alpha(accent, 20);
            tokens.ChipBorderColor = Alpha(accent, 85);
            tokens.ChipTextColor = Blend(tokens.ActiveTextColor, accent, 0.35f);
            tokens.SearchBackColor = Blend(tokens.PanelBackColor, Color.White, 0.20f);
            tokens.BadgeBackColor = Blend(tokens.PanelBackColor, tokens.PanelBorderColor, 0.18f);
            tokens.ButtonBackColor = Alpha(accent, 18);
        }
    }
}
