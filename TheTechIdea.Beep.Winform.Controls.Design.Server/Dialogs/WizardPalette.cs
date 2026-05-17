// WizardPalette.cs
// Phase 07 polish — centralised colour palette for DocumentSetupWizardDialog
// and any future Beep design-time wizard.
//
// The source of truth is the Beep theme system: we map
// BeepThemesManager.CurrentTheme (an IBeepTheme) onto a small set of
// wizard-specific named colours so the wizard chrome, mode tiles, preview
// pane, and footer all paint with the same palette the runtime controls use.
// This keeps the design-time wizard visually consistent with the runtime
// surface a user is about to build.
//
// One snapshot per wizard invocation — no mid-dialog hot-swap, which matches
// commercial wizards (DevExpress Template Gallery, Visual Studio New Project
// dialog) and avoids re-layout flicker.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs
{
    /// <summary>
    /// Stable colour palette used by <see cref="DocumentSetupWizardDialog"/>.
    /// Built once per dialog invocation from
    /// <see cref="BeepThemesManager.CurrentTheme"/> (or
    /// <see cref="BeepThemesManager.GetDefaultTheme"/> if no current theme is
    /// set). When the Beep theme system fails to resolve a theme (rare; only
    /// happens when the design-time assembly load order skips theme
    /// discovery) we fall back to <see cref="SystemColors"/> so the wizard
    /// still renders correctly under the OS theme.
    /// </summary>
    internal sealed class WizardPalette
    {
        /// <summary>The Beep theme this palette was derived from, or null when SystemColors fallback was used.</summary>
        public IBeepTheme? Theme { get; private set; }

        /// <summary>True when running with the OS High Contrast accessibility mode.</summary>
        public bool IsHighContrast { get; private set; }

        // ── Chrome ────────────────────────────────────────────────────────────
        public Color FormBack      { get; private set; }
        public Color FormSurface   { get; private set; }
        public Color Separator     { get; private set; }
        public Color FooterBack    { get; private set; }

        // ── Text ──────────────────────────────────────────────────────────────
        public Color HeadingFore   { get; private set; }
        public Color BodyFore      { get; private set; }
        public Color MutedFore     { get; private set; }

        // ── Accents ───────────────────────────────────────────────────────────
        public Color Accent        { get; private set; }
        public Color AccentFore    { get; private set; }
        public Color WarningBack   { get; private set; }
        public Color WarningFore   { get; private set; }

        // ── Mode-tile palette ─────────────────────────────────────────────────
        public Color TileBack      { get; private set; }
        public Color TileHover     { get; private set; }
        public Color TileSelected  { get; private set; }
        public Color TileBorder    { get; private set; }
        public Color TileTitleFore { get; private set; }
        public Color TileDescFore  { get; private set; }

        // ── Preview palette ───────────────────────────────────────────────────
        public Color PreviewBack       { get; private set; }
        public Color PreviewBorder     { get; private set; }
        public Color PreviewActiveTab  { get; private set; }
        public Color PreviewIdleTab    { get; private set; }
        public Color PreviewStrip      { get; private set; }
        public Color PreviewText       { get; private set; }
        public Color PreviewDimText    { get; private set; }
        public Color PreviewMdiSurface { get; private set; }

        public WizardPalette()
        {
            Refresh();
        }

        /// <summary>
        /// Re-samples the active Beep theme (or system colours under High
        /// Contrast) and rebuilds every colour slot in place. Lets a long-
        /// lived host (the wizard form) react to
        /// <see cref="BeepThemesManager.ThemeChanged"/> mid-flow without
        /// re-threading the palette reference through every control.
        /// </summary>
        /// <returns><c>true</c> when any colour actually changed since the
        /// previous snapshot, so the caller can short-circuit a re-paint.</returns>
        public bool Refresh()
        {
            var beforeKey = SnapshotKey();

            IsHighContrast = SystemInformation.HighContrast;

            // Honour the user's accessibility setting first. High Contrast
            // means "ignore custom themes and use my chosen system palette".
            if (IsHighContrast)
            {
                Theme = null;
                FillFromSystemColors();
            }
            else
            {
                // Beep theme is the primary source of truth.
                Theme = ResolveBeepTheme();
                if (Theme != null)
                {
                    FillFromBeepTheme(Theme);
                }
                else
                {
                    // Defensive fallback for the rare design-time path where
                    // theme discovery hasn't run.
                    FillFromSystemColors();
                }
            }

            return beforeKey != SnapshotKey();
        }

        /// <summary>
        /// Compact identity for the current colour set. Used by
        /// <see cref="Refresh"/> so callers can skip a re-paint when the
        /// theme event fires but produces an identical palette (e.g. style
        /// change with no colour delta).
        /// </summary>
        private string SnapshotKey() =>
            $"{FormBack.ToArgb():X8}|{Accent.ToArgb():X8}|{HeadingFore.ToArgb():X8}|" +
            $"{TileBack.ToArgb():X8}|{TileSelected.ToArgb():X8}|{PreviewActiveTab.ToArgb():X8}|" +
            $"{WarningFore.ToArgb():X8}|{Separator.ToArgb():X8}|{IsHighContrast}";

        private static IBeepTheme? ResolveBeepTheme()
        {
            try
            {
                return BeepThemesManager.CurrentTheme ?? BeepThemesManager.GetDefaultTheme();
            }
            catch
            {
                // Theme discovery can throw in restricted design-time domains.
                return null;
            }
        }

        private void FillFromBeepTheme(IBeepTheme t)
        {
            // Chrome — PanelBackColor is the dominant surface in Beep themes.
            FormBack    = t.BackColor;
            FormSurface = t.PanelBackColor;
            Separator   = t.BorderColor;
            FooterBack  = t.PanelBackColor;

            // Text
            HeadingFore = t.ForeColor;
            BodyFore    = t.ForeColor;
            MutedFore   = t.DisabledForeColor;

            // Accents
            Accent      = t.AccentColor;
            AccentFore  = t.OnPrimaryColor;
            WarningBack = MixColor(t.WarningColor, t.BackColor, 0.85f);
            WarningFore = t.WarningColor;

            // Mode tiles
            TileBack      = t.SurfaceColor;
            TileHover     = MixColor(t.AccentColor, t.SurfaceColor, 0.85f);
            TileSelected  = MixColor(t.AccentColor, t.SurfaceColor, 0.75f);
            TileBorder    = t.BorderColor;
            TileTitleFore = t.ForeColor;
            TileDescFore  = t.DisabledForeColor;

            // Preview pane — mirror the runtime tabbed/browser/MDI look.
            PreviewBack       = t.BackgroundColor;
            PreviewBorder     = t.BorderColor;
            PreviewActiveTab  = t.PanelBackColor;
            PreviewIdleTab    = MixColor(t.PanelBackColor, t.BackgroundColor, 0.5f);
            PreviewStrip      = MixColor(t.PanelBackColor, t.BackgroundColor, 0.6f);
            PreviewText       = t.ForeColor;
            PreviewDimText    = t.DisabledForeColor;
            PreviewMdiSurface = MixColor(t.SecondaryColor, t.BackgroundColor, 0.5f);
        }

        private void FillFromSystemColors()
        {
            FormBack    = SystemColors.Window;
            FormSurface = SystemColors.Window;
            Separator   = SystemColors.WindowFrame;
            FooterBack  = SystemColors.Control;

            HeadingFore = SystemColors.WindowText;
            BodyFore    = SystemColors.WindowText;
            MutedFore   = SystemColors.GrayText;

            Accent      = SystemColors.Highlight;
            AccentFore  = SystemColors.HighlightText;
            WarningBack = SystemColors.Info;
            WarningFore = SystemColors.InfoText;

            TileBack      = SystemColors.Window;
            TileHover     = SystemColors.ControlLight;
            TileSelected  = SystemColors.Highlight;
            TileBorder    = SystemColors.WindowFrame;
            TileTitleFore = SystemColors.WindowText;
            TileDescFore  = SystemColors.GrayText;

            PreviewBack       = SystemColors.Window;
            PreviewBorder     = SystemColors.WindowFrame;
            PreviewActiveTab  = SystemColors.Window;
            PreviewIdleTab    = SystemColors.Control;
            PreviewStrip      = SystemColors.Control;
            PreviewText       = SystemColors.WindowText;
            PreviewDimText    = SystemColors.GrayText;
            PreviewMdiSurface = SystemColors.ControlDark;
        }

        /// <summary>
        /// Linearly blends <paramref name="from"/> toward <paramref name="to"/>
        /// by <paramref name="weight"/> (0 = pure <paramref name="from"/>,
        /// 1 = pure <paramref name="to"/>). Used to derive hover / selected /
        /// idle-tab shades from the theme's primary palette without baking in
        /// a second set of hard-coded colours.
        /// </summary>
        private static Color MixColor(Color from, Color to, float weight)
        {
            weight = Math.Clamp(weight, 0f, 1f);
            int Lerp(int a, int b) => (int)Math.Round(a + (b - a) * weight);
            return Color.FromArgb(
                Lerp(from.A, to.A),
                Lerp(from.R, to.R),
                Lerp(from.G, to.G),
                Lerp(from.B, to.B));
        }
    }
}
