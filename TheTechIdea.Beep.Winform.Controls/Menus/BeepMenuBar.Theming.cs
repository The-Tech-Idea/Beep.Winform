// BeepMenuBar.Theming.cs
// Phase 02 — Partial-Class Split.
//
// Owns ApplyTheme. Reads the active IBeepTheme via the inherited
// _currentTheme, routes through MenuFontHelpers / MenuThemeHelpers, and
// only updates the text font when UseThemeFont is true.
//
// See .plans/Menus-Phase-02-PartialClassSplit.md.
// ─────────────────────────────────────────────────────────────────────────────
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Menus.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMenuBar
    {
        public override void ApplyTheme()
        {
            // Snapshot the current height so an unexpected re-layout cannot
            // collapse a developer-set height.
            int savedHeight = Height;

            // CRITICAL: base.ApplyTheme() handles font safety + DPI scaling.
            base.ApplyTheme();

            if (_currentTheme == null)
                return;

            MenuFontHelpers.ApplyFontTheme(ControlStyle, _currentTheme);

            ForeColor   = MenuThemeHelpers.GetMenuBarForegroundColor(_currentTheme, UseThemeColors);
            BorderColor = MenuThemeHelpers.GetMenuBarBorderColor(_currentTheme, UseThemeColors);
            BackColor   = MenuThemeHelpers.GetMenuBarBackgroundColor(_currentTheme, UseThemeColors);

            var gradientStart = MenuThemeHelpers.GetMenuBarGradientStartColor(_currentTheme, UseThemeColors);
            var gradientEnd   = MenuThemeHelpers.GetMenuBarGradientEndColor(_currentTheme, UseThemeColors);
            if (gradientStart != Color.Empty && gradientEnd != Color.Empty && UseGradientBackground)
            {
                GradientStartColor = gradientStart;
                GradientEndColor   = gradientEnd;
                GradientDirection  = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            }

            // Only override the text font when UseThemeFont is true *and* the
            // developer has not explicitly assigned TextFont (tracked by the
            // _explicitTextFont flag set in the TextFont setter).
            if (UseThemeFont)
            {
                try
                {
                    Font newFont = MenuFontHelpers.GetMenuItemFont(ControlStyle, _currentTheme);
                    if (newFont != null && newFont.FontFamily != null)
                    {
                        _textFont = newFont;
                    }
                }
                catch (Exception ex)
                {
                    Invalidate();
                    System.Diagnostics.Debug.WriteLine(
                        $"BeepMenuBar: Failed to create font from theme: {ex.Message}");
                }
            }

            Invalidate();
        }
    }
}
