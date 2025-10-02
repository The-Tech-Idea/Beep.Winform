namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepFormStyleMetrics
    {
        public int BorderRadius { get; set; }
        public int BorderThickness { get; set; }
        public int ShadowDepth { get; set; }
        public bool EnableGlow { get; set; }
        public float GlowSpread { get; set; }
        public int CaptionHeight { get; set; }
        public int CaptionButtonSize { get; set; }
    }

    // Source of truth for per-style metrics (used by ApplyMetrics)
    internal static class BeepFormStyleMetricsDefaults
    {
        public static readonly Dictionary<BeepFormStyle, BeepFormStyleMetrics> Map = new()
        {
            [BeepFormStyle.Modern] = new BeepFormStyleMetrics { BorderRadius = 8, BorderThickness = 2, ShadowDepth = 6, EnableGlow = true, GlowSpread = 8, CaptionHeight = 36, CaptionButtonSize = 28 },
            [BeepFormStyle.Metro] = new BeepFormStyleMetrics { BorderRadius = 0, BorderThickness = 2, ShadowDepth = 0, EnableGlow = false, GlowSpread = 0, CaptionHeight = 32, CaptionButtonSize = 26 },
            [BeepFormStyle.Glass] = new BeepFormStyleMetrics { BorderRadius = 12, BorderThickness = 2, ShadowDepth = 8, EnableGlow = true, GlowSpread = 12, CaptionHeight = 36, CaptionButtonSize = 28 },
            [BeepFormStyle.Office] = new BeepFormStyleMetrics { BorderRadius = 4, BorderThickness = 2, ShadowDepth = 4, EnableGlow = false, GlowSpread = 0, CaptionHeight = 34, CaptionButtonSize = 28 },
            [BeepFormStyle.ModernDark] = new BeepFormStyleMetrics { BorderRadius = 8, BorderThickness = 2, ShadowDepth = 8, EnableGlow = true, GlowSpread = 10, CaptionHeight = 36, CaptionButtonSize = 28 },

            // Material uses thin borders with larger radius for a softer look
            [BeepFormStyle.Material] = new BeepFormStyleMetrics { BorderRadius = 12, BorderThickness = 2, ShadowDepth = 5, EnableGlow = true, GlowSpread = 15, CaptionHeight = 36, CaptionButtonSize = 26 },

            [BeepFormStyle.Minimal] = new BeepFormStyleMetrics { BorderRadius = 0, BorderThickness = 2, ShadowDepth = 0, EnableGlow = false, GlowSpread = 0, CaptionHeight = 30, CaptionButtonSize = 24 },
            [BeepFormStyle.Classic] = new BeepFormStyleMetrics { BorderRadius = 0, BorderThickness = 2, ShadowDepth = 0, EnableGlow = false, GlowSpread = 0, CaptionHeight = 30, CaptionButtonSize = 24 },

            // Linux-like styles with distinctive border patterns
            [BeepFormStyle.Gnome] = new BeepFormStyleMetrics { BorderRadius = 6, BorderThickness = 1, ShadowDepth = 0, EnableGlow = false, GlowSpread = 0, CaptionHeight = 34, CaptionButtonSize = 26 },
            [BeepFormStyle.Kde] = new BeepFormStyleMetrics { BorderRadius = 8, BorderThickness = 2, ShadowDepth = 8, EnableGlow = true, GlowSpread = 8, CaptionHeight = 36, CaptionButtonSize = 28 },
            [BeepFormStyle.Cinnamon] = new BeepFormStyleMetrics { BorderRadius = 10, BorderThickness = 3, ShadowDepth = 8, EnableGlow = true, GlowSpread = 10, CaptionHeight = 38, CaptionButtonSize = 30 },
            [BeepFormStyle.Elementary] = new BeepFormStyleMetrics { BorderRadius = 10, BorderThickness = 2, ShadowDepth = 4, EnableGlow = false, GlowSpread = 0, CaptionHeight = 40, CaptionButtonSize = 30 },

            // Extra visual styles with more dramatic differences
            [BeepFormStyle.Fluent] = new BeepFormStyleMetrics { BorderRadius = 12, BorderThickness = 1, ShadowDepth = 10, EnableGlow = true, GlowSpread = 14, CaptionHeight = 40, CaptionButtonSize = 30 },
            [BeepFormStyle.NeoBrutalist] = new BeepFormStyleMetrics { BorderRadius = 0, BorderThickness = 4, ShadowDepth = 0, EnableGlow = false, GlowSpread = 0, CaptionHeight = 34, CaptionButtonSize = 26 },

            // New styles for enhanced visual variety
            [BeepFormStyle.Neon] = new BeepFormStyleMetrics { BorderRadius = 15, BorderThickness = 1, ShadowDepth = 12, EnableGlow = true, GlowSpread = 20, CaptionHeight = 38, CaptionButtonSize = 30 },
            [BeepFormStyle.Retro] = new BeepFormStyleMetrics { BorderRadius = 6, BorderThickness = 3, ShadowDepth = 6, EnableGlow = true, GlowSpread = 12, CaptionHeight = 42, CaptionButtonSize = 32 },
            [BeepFormStyle.Gaming] = new BeepFormStyleMetrics { BorderRadius = 2, BorderThickness = 3, ShadowDepth = 10, EnableGlow = true, GlowSpread = 16, CaptionHeight = 36, CaptionButtonSize = 28 },
            [BeepFormStyle.Corporate] = new BeepFormStyleMetrics { BorderRadius = 4, BorderThickness = 1, ShadowDepth = 3, EnableGlow = false, GlowSpread = 0, CaptionHeight = 32, CaptionButtonSize = 26 },
            [BeepFormStyle.Artistic] = new BeepFormStyleMetrics { BorderRadius = 20, BorderThickness = 2, ShadowDepth = 15, EnableGlow = true, GlowSpread = 25, CaptionHeight = 44, CaptionButtonSize = 32 },
            [BeepFormStyle.HighContrast] = new BeepFormStyleMetrics { BorderRadius = 0, BorderThickness = 3, ShadowDepth = 0, EnableGlow = false, GlowSpread = 0, CaptionHeight = 36, CaptionButtonSize = 30 },
            [BeepFormStyle.Soft] = new BeepFormStyleMetrics { BorderRadius = 16, BorderThickness = 1, ShadowDepth = 8, EnableGlow = true, GlowSpread = 12, CaptionHeight = 38, CaptionButtonSize = 28 },
            [BeepFormStyle.Industrial] = new BeepFormStyleMetrics { BorderRadius = 2, BorderThickness = 4, ShadowDepth = 12, EnableGlow = true, GlowSpread = 8, CaptionHeight = 34, CaptionButtonSize = 28 },

            [BeepFormStyle.Custom] = new BeepFormStyleMetrics { BorderRadius = 8, BorderThickness = 1, ShadowDepth = 6, EnableGlow = true, GlowSpread = 8, CaptionHeight = 36, CaptionButtonSize = 28 },
        };
    }
}