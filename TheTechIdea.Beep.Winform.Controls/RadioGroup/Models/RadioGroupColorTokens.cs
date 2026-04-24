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
            var background = Fallback(theme.BackgroundColor, StyleColors.GetBackground(style));
            var foreground = Fallback(theme.ForeColor, StyleColors.GetForeground(style));
            var surface = Fallback(theme.SurfaceColor, background);
            var borderColor = Fallback(theme.BorderColor, StyleColors.GetBorder(style));
            var secondaryText = Fallback(theme.SecondaryTextColor, StyleColors.GetForeground(style));

            return new RadioGroupColorTokens
            {
                Surface           = background,
                SurfaceVariant    = Lighten(surface, 0.03f),
                SurfaceContainer  = Blend(surface, primary, 0.05f),

                OnSurface         = foreground,
                OnSurfaceVariant  = Blend(secondaryText, Color.Gray, 0.4f),
                Outline           = borderColor,
                OutlineVariant    = Lighten(borderColor, 0.4f),

                Primary           = primary,
                OnPrimary         = Fallback(theme.ButtonForeColor, Color.White),
                PrimaryContainer  = Blend(surface, primary, 0.12f),
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
            var background = StyleColors.GetBackground(style);
            var foreground = StyleColors.GetForeground(style);
            var border = StyleColors.GetBorder(style);

            return new RadioGroupColorTokens
            {
                Surface           = background,
                SurfaceVariant    = Lighten(background, 0.03f),
                SurfaceContainer  = Blend(background, primary, 0.05f),

                OnSurface         = foreground,
                OnSurfaceVariant  = Blend(foreground, Color.Gray, 0.4f),
                Outline           = border,
                OutlineVariant    = Lighten(border, 0.4f),

                Primary           = primary,
                OnPrimary         = Luminance(primary) > 0.5f ? Color.FromArgb(28, 27, 31) : Color.White,
                PrimaryContainer  = Blend(background, primary, 0.12f),
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

                OnSurface         = SystemColors.WindowText,
                OnSurfaceVariant  = SystemColors.WindowText,
                Outline           = SystemColors.WindowText,
                OutlineVariant    = SystemColors.WindowText,

                Primary           = SystemColors.Highlight,
                OnPrimary         = SystemColors.HighlightText,
                PrimaryContainer  = SystemColors.Highlight,
                OnPrimaryContainer = SystemColors.HighlightText,

                HoverStateLayer   = SystemColors.Highlight,
                FocusStateLayer   = SystemColors.Highlight,
                PressStateLayer   = SystemColors.Highlight,

                Error             = Color.Red,
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

        private static float Luminance(Color c)
        {
            float r = c.R / 255f;
            float g = c.G / 255f;
            float b = c.B / 255f;
            return (0.299f * r + 0.587f * g + 0.114f * b);
        }
    }
}
