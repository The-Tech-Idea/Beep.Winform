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
        public int RibbonHeight { get; set; }
        public int CaptionButtonSize { get; set; }
    }

    // Source of truth for per-style metrics (used by ApplyMetrics)
    internal static class BeepFormStyleMetricsDefaults
    {
        public static readonly Dictionary<BeepFormStyle, BeepFormStyleMetrics> Map = new()
        {
            [BeepFormStyle.Modern] = new BeepFormStyleMetrics { BorderRadius = 8, BorderThickness = 1, ShadowDepth = 6, EnableGlow = true, GlowSpread = 8, CaptionHeight = 36, RibbonHeight = 80, CaptionButtonSize = 28 },
            [BeepFormStyle.Metro] = new BeepFormStyleMetrics { BorderRadius = 0, BorderThickness = 1, ShadowDepth = 0, EnableGlow = false, GlowSpread = 0, CaptionHeight = 32, RibbonHeight = 72, CaptionButtonSize = 26 },
            [BeepFormStyle.Glass] = new BeepFormStyleMetrics { BorderRadius = 12, BorderThickness = 1, ShadowDepth = 8, EnableGlow = true, GlowSpread = 12, CaptionHeight = 36, RibbonHeight = 84, CaptionButtonSize = 28 },
            [BeepFormStyle.Office] = new BeepFormStyleMetrics { BorderRadius = 4, BorderThickness = 1, ShadowDepth = 4, EnableGlow = false, GlowSpread = 0, CaptionHeight = 34, RibbonHeight = 84, CaptionButtonSize = 28 },
            [BeepFormStyle.ModernDark] = new BeepFormStyleMetrics { BorderRadius = 8, BorderThickness = 1, ShadowDepth = 8, EnableGlow = true, GlowSpread = 10, CaptionHeight = 36, RibbonHeight = 80, CaptionButtonSize = 28 },

            // Adjusted: Material uses larger radius with 0 border thickness
            [BeepFormStyle.Material] = new BeepFormStyleMetrics { BorderRadius = 8, BorderThickness = 0, ShadowDepth = 5, EnableGlow = true, GlowSpread = 15, CaptionHeight = 36, RibbonHeight = 72, CaptionButtonSize = 26 },

            [BeepFormStyle.Minimal] = new BeepFormStyleMetrics { BorderRadius = 0, BorderThickness = 1, ShadowDepth = 0, EnableGlow = false, GlowSpread = 0, CaptionHeight = 30, RibbonHeight = 64, CaptionButtonSize = 24 },
            [BeepFormStyle.Classic] = new BeepFormStyleMetrics { BorderRadius = 0, BorderThickness = 1, ShadowDepth = 0, EnableGlow = false, GlowSpread = 0, CaptionHeight = 30, RibbonHeight = 64, CaptionButtonSize = 24 },
            [BeepFormStyle.Custom] = new BeepFormStyleMetrics { BorderRadius = 8, BorderThickness = 1, ShadowDepth = 6, EnableGlow = true, GlowSpread = 8, CaptionHeight = 36, RibbonHeight = 80, CaptionButtonSize = 28 },
        };
    }
}