using System;
using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    public sealed class AGGridFilterPanelPainter : BaseFilterPanelPainter
    {
        public override navigationStyle Style => navigationStyle.AGGrid;
        public override string StyleName => "AGGrid";

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
                new ModernToolbarOptions
                {
                    LeftPadding = 12,
                    RightPadding = 12,
                    Spacing = 8,
                    SeparatorInset = 8,
                    ControlHeight = 24,
                    CornerRadius = 6,
                    SearchMinWidth = 150,
                    SearchMaxWidth = 280,
                    ClearWidth = 86,
                    CountWidth = 86,
                    FilterWidth = 74,
                    SearchPlaceholder = "Search all columns",
                    FilterText = "Filter",
                    ClearText = "Clear All",
                    CountFormat = "{0} active",
                    FlatControls = false
                });
        }

        protected override void ApplyStyleTokens(FilterPanelStyleTokens tokens, IBeepTheme? theme)
        {
            base.ApplyStyleTokens(tokens, theme);
            Color accent = ResolveAccent(theme, tokens.ActiveGlyphColor);
            tokens.ActiveGlyphColor = accent;
            tokens.PanelBackColor = Blend(tokens.PanelBackColor, Color.White, 0.10f);
            tokens.PanelBorderColor = Blend(tokens.PanelBorderColor, accent, 0.10f);
            tokens.SeparatorColor = Alpha(tokens.PanelBorderColor, 180);
            tokens.InactiveTextColor = Blend(tokens.InactiveTextColor, tokens.ActiveTextColor, 0.30f);
            tokens.ChipBackColor = Alpha(accent, 26);
            tokens.ChipBorderColor = Alpha(accent, 90);
            tokens.ChipTextColor = Blend(tokens.ActiveTextColor, accent, 0.35f);
            tokens.SearchBackColor = Blend(tokens.PanelBackColor, Color.White, 0.35f);
            tokens.BadgeBackColor = Alpha(accent, 22);
            tokens.ButtonBackColor = Alpha(accent, 26);
        }
    }
}
