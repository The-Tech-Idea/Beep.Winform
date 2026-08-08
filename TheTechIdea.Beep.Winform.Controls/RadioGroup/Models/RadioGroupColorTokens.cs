using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
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
    [TypeConverter(typeof(RadioGroupColorTokensConverter))]
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
        /// <summary>Elevation shadow base - painters apply their own alpha</summary>
        public Color Shadow           { get; init; }

        // ─────────────────────────────────────────────────────────────────────
        // Factory methods
        // ─────────────────────────────────────────────────────────────────────

        public static RadioGroupColorTokens FromTheme(IBeepTheme theme)
        {
            if (SystemInformation.HighContrast)
                return ForHighContrast();

            // One slot per role (the settled end-state). The previous version blended and
            // lightened derived roles out of 4-5 base slots and kept a whole parallel
            // StyleColors palette for a themeless mode that cannot exist - there is always
            // a theme. State layers stay alpha veils of the Primary slot.
            var t = theme ?? BeepThemesManager.CurrentTheme;
            return new RadioGroupColorTokens
            {
                Surface            = t.SurfaceColor,
                SurfaceVariant     = t.PanelBackColor,
                SurfaceContainer   = t.CardBackColor,

                OnSurface          = t.ForeColor,
                OnSurfaceVariant   = t.SecondaryTextColor,
                Outline            = t.BorderColor,
                OutlineVariant     = t.InactiveBorderColor,

                Primary            = t.PrimaryColor,
                OnPrimary          = t.OnPrimaryColor,
                PrimaryContainer   = t.ButtonSelectedBackColor,
                OnPrimaryContainer = t.ButtonSelectedForeColor,

                HoverStateLayer    = Color.FromArgb(8,  t.PrimaryColor),
                FocusStateLayer    = Color.FromArgb(12, t.PrimaryColor),
                PressStateLayer    = Color.FromArgb(12, t.PrimaryColor),

                Shadow             = t.ShadowColor,
                Error              = t.ErrorColor,
                Disabled           = t.DisabledForeColor,
                DisabledContainer  = t.DisabledBackColor
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

                Shadow            = Color.Transparent, // no fake depth in high contrast
                Error             = SystemColors.MenuHighlight,
                Disabled          = SystemColors.GrayText,
                DisabledContainer = SystemColors.Control
            };
        }

    }
}
