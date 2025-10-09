using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Caption
{
    /// <summary>
    /// Visual preferences for each caption renderer including border, shadow, and glow settings.
    /// These preferences are automatically applied when switching form styles.
    /// </summary>
    public class CaptionRendererPreference
    {
        public int BorderThickness { get; set; }
        public int BorderRadius { get; set; }
        public int ShadowDepth { get; set; }
        public bool EnableGlow { get; set; }
        public float GlowSpread { get; set; }
        public int CaptionHeight { get; set; } = 36;
    }

    /// <summary>
    /// Maps each BeepFormStyle to its optimal visual settings for caption renderers.
    /// </summary>
    internal static class CaptionRendererPreferences
    {
        public static readonly Dictionary<BeepFormStyle, CaptionRendererPreference> Map = new()
        {
            // Standard Windows-like styles
            [BeepFormStyle.Modern] = new CaptionRendererPreference 
            { 
                BorderThickness = 1, 
                BorderRadius = 8, 
                ShadowDepth = 6, 
                EnableGlow = false, 
                GlowSpread = 8,
                CaptionHeight = 36
            },

            [BeepFormStyle.Metro] = new CaptionRendererPreference 
            { 
                BorderThickness = 2, 
                BorderRadius = 0, // Flat design
                ShadowDepth = 0, 
                EnableGlow = false, 
                GlowSpread = 0,
                CaptionHeight = 32
            },

            [BeepFormStyle.Office] = new CaptionRendererPreference 
            { 
                BorderThickness = 2, 
                BorderRadius = 4, 
                ShadowDepth = 4, 
                EnableGlow = false, 
                GlowSpread = 0,
                CaptionHeight = 36
            },

            [BeepFormStyle.Glass] = new CaptionRendererPreference 
            { 
                BorderThickness = 1, 
                BorderRadius = 12, 
                ShadowDepth = 8, 
                EnableGlow = true, 
                GlowSpread = 12,
                CaptionHeight = 36
            },

            [BeepFormStyle.ModernDark] = new CaptionRendererPreference 
            { 
                BorderThickness = 2, 
                BorderRadius = 8, 
                ShadowDepth = 8, 
                EnableGlow = true, 
                GlowSpread = 10,
                CaptionHeight = 36
            },

            // Linux-inspired styles
            [BeepFormStyle.Gnome] = new CaptionRendererPreference 
            { 
                BorderThickness = 1, 
                BorderRadius = 6, 
                ShadowDepth = 0, 
                EnableGlow = false, 
                GlowSpread = 0,
                CaptionHeight = 34
            },

            [BeepFormStyle.Kde] = new CaptionRendererPreference 
            { 
                BorderThickness = 2, 
                BorderRadius = 8, 
                ShadowDepth = 8, 
                EnableGlow = true, 
                GlowSpread = 8,
                CaptionHeight = 36
            },

            [BeepFormStyle.Cinnamon] = new CaptionRendererPreference 
            { 
                BorderThickness = 3, // Thicker for Linux feel
                BorderRadius = 10, 
                ShadowDepth = 8, 
                EnableGlow = true, 
                GlowSpread = 10,
                CaptionHeight = 38
            },

            [BeepFormStyle.Elementary] = new CaptionRendererPreference 
            { 
                BorderThickness = 1, // Clean minimal
                BorderRadius = 10, 
                ShadowDepth = 4, 
                EnableGlow = false, 
                GlowSpread = 0,
                CaptionHeight = 40
            },

            // Modern design styles
            [BeepFormStyle.Fluent] = new CaptionRendererPreference 
            { 
                BorderThickness = 1, 
                BorderRadius = 12, // Large radius for modern look
                ShadowDepth = 10, 
                EnableGlow = true, 
                GlowSpread = 14,
                CaptionHeight = 36
            },

            [BeepFormStyle.Material] = new CaptionRendererPreference 
            { 
                BorderThickness = 1, 
                BorderRadius = 12, 
                ShadowDepth = 5, 
                EnableGlow = true, 
                GlowSpread = 15, // Material elevation
                CaptionHeight = 36
            },

            [BeepFormStyle.NeoBrutalist] = new CaptionRendererPreference 
            { 
                BorderThickness = 4, // Very thick borders
                BorderRadius = 0, // Sharp corners
                ShadowDepth = 0, 
                EnableGlow = false, 
                GlowSpread = 0,
                CaptionHeight = 32
            },

            [BeepFormStyle.Minimal] = new CaptionRendererPreference 
            { 
                BorderThickness = 1, 
                BorderRadius = 0, 
                ShadowDepth = 0, 
                EnableGlow = false, 
                GlowSpread = 0,
                CaptionHeight = 32
            },

            [BeepFormStyle.Classic] = new CaptionRendererPreference 
            { 
                BorderThickness = 2, 
                BorderRadius = 0, 
                ShadowDepth = 0, 
                EnableGlow = false, 
                GlowSpread = 0,
                CaptionHeight = 32
            },

            // Specialty themed styles
            [BeepFormStyle.Neon] = new CaptionRendererPreference 
            { 
                BorderThickness = 1, 
                BorderRadius = 15, // Smooth curves
                ShadowDepth = 12, // Deep shadow for neon glow
                EnableGlow = true, 
                GlowSpread = 20, // Strong glow effect
                CaptionHeight = 38
            },

            [BeepFormStyle.Retro] = new CaptionRendererPreference 
            { 
                BorderThickness = 3, // Bold 80s style
                BorderRadius = 6, 
                ShadowDepth = 6, 
                EnableGlow = true, 
                GlowSpread = 12, // Miami Vice glow
                CaptionHeight = 42
            },

            [BeepFormStyle.Gaming] = new CaptionRendererPreference 
            { 
                BorderThickness = 3, // Strong borders
                BorderRadius = 2, // Sharp angular
                ShadowDepth = 10, 
                EnableGlow = true, 
                GlowSpread = 16, // RGB glow
                CaptionHeight = 36
            },

            [BeepFormStyle.Corporate] = new CaptionRendererPreference 
            { 
                BorderThickness = 1, // Minimal professional
                BorderRadius = 4, 
                ShadowDepth = 3, 
                EnableGlow = false, 
                GlowSpread = 0,
                CaptionHeight = 32
            },

            [BeepFormStyle.Artistic] = new CaptionRendererPreference 
            { 
                BorderThickness = 2, 
                BorderRadius = 20, // Very rounded
                ShadowDepth = 15, 
                EnableGlow = true, 
                GlowSpread = 25, // Dramatic effects
                CaptionHeight = 44
            },

            [BeepFormStyle.HighContrast] = new CaptionRendererPreference 
            { 
                BorderThickness = 3, // High visibility
                BorderRadius = 0, // Sharp for clarity
                ShadowDepth = 0, 
                EnableGlow = false, 
                GlowSpread = 0,
                CaptionHeight = 36
            },

            [BeepFormStyle.Soft] = new CaptionRendererPreference 
            { 
                BorderThickness = 1, 
                BorderRadius = 16, // Very rounded neumorphic
                ShadowDepth = 8, 
                EnableGlow = true, 
                GlowSpread = 12,
                CaptionHeight = 38
            },

            [BeepFormStyle.Industrial] = new CaptionRendererPreference 
            { 
                BorderThickness = 4, // Heavy industrial
                BorderRadius = 2, // Minimal rounding
                ShadowDepth = 12, // Deep shadows
                EnableGlow = true, 
                GlowSpread = 8,
                CaptionHeight = 34
            },

            [BeepFormStyle.Custom] = new CaptionRendererPreference 
            { 
                BorderThickness = 1, 
                BorderRadius = 8, 
                ShadowDepth = 6, 
                EnableGlow = true, 
                GlowSpread = 8,
                CaptionHeight = 36
            },
        };

        /// <summary>
        /// Gets the visual preferences for a specific BeepFormStyle.
        /// Returns default Modern preferences if not found.
        /// </summary>
        public static CaptionRendererPreference GetPreference(BeepFormStyle style)
        {
            return Map.TryGetValue(style, out var pref) 
                ? pref 
                : Map[BeepFormStyle.Modern];
        }
    }
}
