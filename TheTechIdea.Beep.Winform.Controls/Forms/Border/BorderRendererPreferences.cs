using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Border
{
    internal sealed class BorderRendererPreference
    {
        public int BorderThickness { get; set; }
        public int BorderRadius { get; set; }
        public int ShadowDepth { get; set; }
        public bool EnableGlow { get; set; }
    }

    internal static class BorderRendererPreferences
    {
        public static readonly Dictionary<BeepFormStyle, BorderRendererPreference> Map = new()
        {
            [BeepFormStyle.Modern]        = new() { BorderThickness = 1, BorderRadius = 8,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.ModernDark]    = new() { BorderThickness = 2, BorderRadius = 8,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Metro]         = new() { BorderThickness = 2, BorderRadius = 0,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Windows]       = new() { BorderThickness = 1, BorderRadius = 8,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Office]        = new() { BorderThickness = 2, BorderRadius = 4,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Fluent]        = new() { BorderThickness = 1, BorderRadius = 12, ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Material]      = new() { BorderThickness = 1, BorderRadius = 12, ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.NeoBrutalist]  = new() { BorderThickness = 4, BorderRadius = 0,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Minimal]       = new() { BorderThickness = 1, BorderRadius = 0,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Classic]       = new() { BorderThickness = 2, BorderRadius = 0,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Glass]         = new() { BorderThickness = 1, BorderRadius = 12, ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Gnome]         = new() { BorderThickness = 1, BorderRadius = 6,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Kde]           = new() { BorderThickness = 2, BorderRadius = 8,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Cinnamon]      = new() { BorderThickness = 3, BorderRadius = 10, ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Elementary]    = new() { BorderThickness = 1, BorderRadius = 10, ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Neon]          = new() { BorderThickness = 1, BorderRadius = 15, ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Retro]         = new() { BorderThickness = 3, BorderRadius = 6,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Gaming]        = new() { BorderThickness = 3, BorderRadius = 2,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Corporate]     = new() { BorderThickness = 1, BorderRadius = 4,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Artistic]      = new() { BorderThickness = 2, BorderRadius = 20, ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.HighContrast]  = new() { BorderThickness = 4, BorderRadius = 0,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Soft]          = new() { BorderThickness = 1, BorderRadius = 16, ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Industrial]    = new() { BorderThickness = 4, BorderRadius = 2,  ShadowDepth = 0,  EnableGlow = false },
            [BeepFormStyle.Custom]        = new() { BorderThickness = 1, BorderRadius = 8,  ShadowDepth = 0,  EnableGlow = false },
        };

        public static BorderRendererPreference Get(BeepFormStyle style)
        {
            return Map.TryGetValue(style, out var pref) ? pref : Map[BeepFormStyle.Modern];
        }
    }
}
