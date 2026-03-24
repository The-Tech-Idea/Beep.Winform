using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Models
{
    /// <summary>
    /// Material Design 3 color roles resolved from IBeepTheme or StyleColors fallbacks.
    /// Aligns with Figma token names used in component handoff specs.
    /// Renderers should call <see cref="FromTheme"/> once per paint cycle and reuse the result.
    /// </summary>
    public sealed class RadioGroupColorTokens
    {
        // ── Surface roles ────────────────────────────────────────────────────
        /// <summary>MD3 surface — default item background</summary>
        public Color Surface          { get; init; }
        /// <summary>MD3 surface-variant — subtle grouped background</summary>
        public Color SurfaceVariant   { get; init; }
        /// <summary>MD3 surface-container — slightly elevated surface (cards, tiles)</summary>
        public Color SurfaceContainer { get; init; }

        // ── Content roles ────────────────────────────────────────────────────
        /// <summary>Text / icons on surface</summary>
        public Color OnSurface        { get; init; }
        /// <summary>Secondary text / description on surface</summary>
        public Color OnSurfaceVariant { get; init; }
        /// <summary>Border / outline ring on unselected item</summary>
        public Color Outline          { get; init; }
        /// <summary>Lighter border for subtler visual separation</summary>
        public Color OutlineVariant   { get; init; }

        // ── Interactive / brand roles ─────────────────────────────────────────
        /// <summary>Brand primary — selected indicator fill, focus outline</summary>
        public Color Primary          { get; init; }
        /// <summary>On-primary — text / icon on top of Primary fill</summary>
        public Color OnPrimary        { get; init; }
        /// <summary>Primary-container — background of selected card / chip</summary>
        public Color PrimaryContainer { get; init; }
        /// <summary>On-primary-container — text on selected card / chip</summary>
        public Color OnPrimaryContainer { get; init; }

        // ── State layers (pre-computed with opacity) ──────────────────────────
        /// <summary>Primary @ 8% alpha — hover overlay</summary>
        public Color HoverStateLayer  { get; init; }
        /// <summary>Primary @ 12% alpha — focus overlay</summary>
        public Color FocusStateLayer  { get; init; }
        /// <summary>Primary @ 12% alpha — pressed ripple overlay</summary>
        public Color PressStateLayer  { get; init; }

        // ── Semantic ──────────────────────────────────────────────────────────
        /// <summary>Error color — validation border / indicator</summary>
        public Color Error            { get; init; }
        /// <summary>Disabled foreground</summary>
        public Color Disabled         { get; init; }
        /// <summary>Disabled item background</summary>
        public Color DisabledContainer { get; init; }

        // ─────────────────────────────────────────────────────────────────────
        // Factory methods
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Creates a token set from an <see cref="IBeepTheme"/>.
        /// Falls back to <see cref="StyleColors"/> when the theme is null or
        /// <paramref name="useThemeColors"/> is false.
        /// </summary>
        public static RadioGroupColorTokens FromTheme(
            IBeepTheme theme,
            bool useThemeColors,
            BeepControlStyle style = BeepControlStyle.Material3)
        {
            if (SystemInformation.HighContrast)
                return ForHighContrast();

            if (!useThemeColors || theme == null)
                return FromStyleColors(style);

            var primary = Fallback(theme.PrimaryColor, StyleColors.GetPrimary(style));
            return new RadioGroupColorTokens
            {
                Surface           = Fallback(theme.BackgroundColor, Color.White),
                SurfaceVariant    = Fallback(theme.SurfaceColor, Color.FromArgb(247, 247, 252)),
                SurfaceContainer  = Blend(Fallback(theme.SurfaceColor, Color.White), primary, 0.05f),

                OnSurface         = Fallback(theme.ForeColor, Color.FromArgb(28, 27, 31)),
                OnSurfaceVariant  = Fallback(theme.SecondaryTextColor, Color.FromArgb(73, 69, 79)),
                Outline           = Fallback(theme.BorderColor, Color.FromArgb(121, 116, 126)),
                OutlineVariant    = Lighten(Fallback(theme.BorderColor, Color.FromArgb(202, 196, 208)), 0.4f),

                Primary           = primary,
                OnPrimary         = Fallback(theme.ButtonForeColor, Color.White),
                PrimaryContainer  = Blend(Color.White, primary, 0.12f),
                OnPrimaryContainer = Darken(primary, 0.3f),

                HoverStateLayer   = Color.FromArgb(8,  primary),
                FocusStateLayer   = Color.FromArgb(12, primary),
                PressStateLayer   = Color.FromArgb(12, primary),

                Error             = Fallback(theme.ErrorColor, Color.FromArgb(179, 38, 30)),
                Disabled          = Fallback(theme.DisabledForeColor, Color.FromArgb(158, 157, 162)),
                DisabledContainer = Color.FromArgb(30, Color.FromArgb(158, 157, 162))
            };
        }

        /// <summary>
        /// Creates a token set from <see cref="StyleColors"/> only (no theme).
        /// </summary>
        public static RadioGroupColorTokens FromStyleColors(BeepControlStyle style)
        {
            var primary = StyleColors.GetPrimary(style);
            return new RadioGroupColorTokens
            {
                Surface           = StyleColors.GetBackground(style),
                SurfaceVariant    = Lighten(StyleColors.GetBackground(style), 0.03f),
                SurfaceContainer  = Blend(StyleColors.GetBackground(style), primary, 0.05f),

                OnSurface         = StyleColors.GetForeground(style),
                OnSurfaceVariant  = Blend(StyleColors.GetForeground(style), Color.Gray, 0.4f),
                Outline           = StyleColors.GetBorder(style),
                OutlineVariant    = Lighten(StyleColors.GetBorder(style), 0.4f),

                Primary           = primary,
                OnPrimary         = Color.White,
                PrimaryContainer  = Blend(Color.White, primary, 0.12f),
                OnPrimaryContainer = Darken(primary, 0.3f),

                HoverStateLayer   = Color.FromArgb(8,  primary),
                FocusStateLayer   = Color.FromArgb(12, primary),
                PressStateLayer   = Color.FromArgb(12, primary),

                Error             = Color.FromArgb(179, 38, 30),
                Disabled          = Color.FromArgb(158, 157, 162),
                DisabledContainer = Color.FromArgb(30, Color.FromArgb(158, 157, 162))
            };
        }

        /// <summary>
        /// Creates a token set that maps to Windows system colors for High Contrast mode.
        /// All custom colors are replaced with system colors — no alpha blending.
        /// </summary>
        public static RadioGroupColorTokens ForHighContrast()
        {
            return new RadioGroupColorTokens
            {
                Surface           = SystemColors.Window,
                SurfaceVariant    = SystemColors.Window,
                SurfaceContainer  = SystemColors.Window,

                OnSurface         = SystemColors.ControlText,
                OnSurfaceVariant  = SystemColors.GrayText,
                Outline           = SystemColors.ControlText,
                OutlineVariant    = SystemColors.GrayText,

                Primary           = SystemColors.Highlight,
                OnPrimary         = SystemColors.HighlightText,
                PrimaryContainer  = SystemColors.Highlight,
                OnPrimaryContainer = SystemColors.HighlightText,

                HoverStateLayer   = Color.FromArgb(32, SystemColors.Highlight),
                FocusStateLayer   = Color.FromArgb(48, SystemColors.Highlight),
                PressStateLayer   = Color.FromArgb(48, SystemColors.Highlight),

                Error             = SystemColors.ControlText,
                Disabled          = SystemColors.GrayText,
                DisabledContainer = SystemColors.Control
            };
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private colour math utilities
        // ─────────────────────────────────────────────────────────────────────

        private static Color Fallback(Color candidate, Color fallback)
            => candidate == Color.Empty ? fallback : candidate;

        /// <summary>Linearly blends <paramref name="a"/> toward <paramref name="b"/> by <paramref name="t"/>.</summary>
        private static Color Blend(Color a, Color b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            return Color.FromArgb(
                (int)(a.A + (b.A - a.A) * t),
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }

        private static Color Lighten(Color c, float amount)
            => Blend(c, Color.White, amount);

        private static Color Darken(Color c, float amount)
            => Blend(c, Color.Black, amount);
    }
}
