using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    public sealed class CompactFilterPanelPainter : BaseFilterPanelPainter
    {
        public override navigationStyle Style => navigationStyle.Compact;
        public override string StyleName => "Compact";

        /// <summary>
        /// Calculate filter panel height with DPI awareness for compact style.
        /// Uses reduced padding and minimum spacing for a more condensed appearance.
        /// </summary>
        public override int CalculateFilterPanelHeight(BeepGridPro grid)
        {
            if (grid == null) return 30;

            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(grid);
            
            // Get theme for font calculations
            var theme = grid.Theme != null ? BeepThemesManager.GetTheme(grid.Theme) : BeepThemesManager.GetDefaultTheme();
            
            // Calculate heights of all components that need to fit
            int titleFontHeight = 0;
            int regularFontHeight = 0;
            
            try
            {
                var titleFont = GetTitleFont(theme, grid);
                if (titleFont != null)
                {
                    titleFontHeight = FontManagement.FontListHelper.GetFontHeightSafe(titleFont, grid);
                }
                
                var regularFont = GetFont(theme);
                if (regularFont != null)
                {
                    regularFontHeight = FontManagement.FontListHelper.GetFontHeightSafe(regularFont, grid);
                }
            }
            catch
            {
                titleFontHeight = 14;
                regularFontHeight = 12;
            }
            
            // Compact uses smaller chip height
            int compactChipHeight = 20; // Reduced from 24px
            
            // Find tallest component
            int maxComponentHeight = System.Math.Max(System.Math.Max(titleFontHeight, compactChipHeight), regularFontHeight);
            
            // Reduced padding for compact style (8px top + 8px bottom = 16px total)
            int compactPadding = 8 * 2; // 16px total vertical padding
            
            // Minimal extra spacing
            int extraSpacing = 2;
            
            // Calculate total minimum height
            int baseHeight = maxComponentHeight + compactPadding + extraSpacing;
            
            // Ensure absolute minimum (don't go below 30px base)
            baseHeight = System.Math.Max(baseHeight, 30);
            
            // Apply DPI scaling
            return DpiScalingHelper.GetDpiScaleFactor(grid) != 0 
                ? DpiScalingHelper.ScaleValue(baseHeight, dpiScale) 
                : baseHeight;
        }

        public override void PaintFilterPanel(Graphics g, Rectangle panelRect, BeepGridPro grid, IBeepTheme? theme, Dictionary<int, Rectangle> filterCellRects, Dictionary<int, Rectangle> clearIconRects)
        {
            PaintModernToolbar(g, panelRect, grid, theme, filterCellRects, clearIconRects,
                ScaleModernToolbarOptions(new ModernToolbarOptions
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
                }, grid));
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
