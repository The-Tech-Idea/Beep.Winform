using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Theme application across the docked panels, strips, float windows and splitters.
    /// </summary>
    /// <remarks>
    /// Extracted from <c>BeepDockingManager.cs</c>, which had grown to 3,317 lines with no region
    /// markers at all. The split is by concern, established by reading the file in order rather
    /// than assumed: the Filtering decomposition began with a hypothesis that reading disproved.
    /// </remarks>
    public partial class BeepDockingManager
    {
        /// <summary>
        /// Applies explicit theme colours to every docking surface.
        /// Call this from your application's ThemeChanged handler.
        /// Mirrors Krypton's <c>KryptonManager.GlobalPalette</c> reassignment pattern:
        /// push new colours into the painter adapter, then invalidate all panels.
        /// </summary>
        /// <param name="background">Panel / strip background colour.</param>
        /// <param name="foreground">Title / tab text colour.</param>
        /// <param name="border">Panel border / splitter colour.</param>
        /// <param name="hover">Mouse-hover highlight colour.</param>
        /// <param name="accent">Active-tab / active-caption accent colour.</param>
        public void ApplyTheme(Color background, Color foreground, Color border,
                               Color hover, Color accent)
        {
            ApplyDockingThemeColors(
                DockingThemeColors.FromExplicit(background, foreground, border, hover, accent),
                updatePainter: true);
        }

        /// <summary>
        /// Applies the currently selected Beep theme to every docking surface.
        /// </summary>
        public void ApplyTheme()
        {
            try
            {
                _currentTheme ??= BeepThemesManager.GetTheme(_themeName)
                                  ?? BeepThemesManager.CurrentTheme
                                  ?? BeepThemesManager.GetDefaultTheme();

                if (string.IsNullOrWhiteSpace(_themeName))
                    _themeName = BeepThemesManager.GetThemeName(_currentTheme);

                ApplyDockingThemeColors(DockingThemeColors.FromTheme(_currentTheme, _useThemeColors), updatePainter: true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockingManager] ApplyTheme error: {ex.Message}");
            }
        }

        /// <summary>
        /// Applies a Beep theme object directly to every docking surface.
        /// </summary>
        public void ApplyTheme(IBeepTheme theme)
        {
            _currentTheme = theme ?? BeepThemesManager.GetDefaultTheme();
            _themeName = BeepThemesManager.GetThemeName(_currentTheme);
            ApplyTheme();
        }

        /// <summary>
        /// Re-applies the active theme + style to every docking surface, including float windows
        /// and any other transient chrome that <see cref="ApplyTheme()"/> may not reach when
        /// the host application's <c>IBeepTheme</c> changes at runtime. Use this from a theme
        /// change handler when you want a single call to refresh all managed surfaces.
        /// </summary>
        public void RefreshTheme()
        {
            try
            {
                // 1. Rebuild palette from the current theme and walk every managed surface.
                ApplyTheme();

                // 2. Push style + repaint to surfaces not covered by ApplyTheme().
                PropagateControlStyle();

                // 3. Float windows live outside the host form's control tree; push to each.
                foreach (var fw in _floatWindowsByKey.Values)
                {
                    if (fw == null || fw.IsDisposed) continue;
                    fw.ControlStyle = _style;
                    fw.ApplyDockingTheme(_themeColors);
                }

                // 4. The navigator popup, when open, reads the cached theme colours directly.
                if (_navigator != null && !_navigator.IsDisposed)
                    _navigator.RefreshTheme(_themeColors);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockingManager] RefreshTheme error: {ex.Message}");
            }
        }

        /// <summary>
        /// Registers a delegate that will be invoked whenever the manager's theme colours change.
        /// Allows the host application to sync colours without sub-classing the manager.
        /// </summary>
        public void RegisterThemeHook(Action<Color, Color, Color, Color, Color> hook)
        {
            if (_painter is DockingPainterAdapter adapter)
            {
                adapter.ThemeChanged += (s, e) =>
                {
                    hook?.Invoke(adapter.BackgroundColor, adapter.ForegroundColor,
                                 adapter.BorderColor,    adapter.HoverColor,
                                 adapter.SelectedColor);
                };
            }
        }
    }
}
