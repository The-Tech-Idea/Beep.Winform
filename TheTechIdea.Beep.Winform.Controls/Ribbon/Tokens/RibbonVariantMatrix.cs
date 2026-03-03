using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Tokens
{
    public sealed class RibbonVariantSelection
    {
        public RibbonLayoutMode LayoutMode { get; set; } = RibbonLayoutMode.Classic;
        public RibbonDensity Density { get; set; } = RibbonDensity.Comfortable;
        public bool IsMinimized { get; set; }
        public bool DarkMode { get; set; }
        public bool UseSuperToolTips { get; set; } = true;
        public bool SearchIncludeBackstage { get; set; }
    }

    public sealed class RibbonVariantMatrix
    {
        private readonly Dictionary<string, RibbonVariantSelection> _variants = new(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<string, RibbonVariantSelection> Variants => _variants;

        public void Register(string name, RibbonVariantSelection selection)
        {
            if (string.IsNullOrWhiteSpace(name) || selection == null)
            {
                return;
            }

            _variants[name.Trim()] = selection;
        }

        public bool TryGet(string name, out RibbonVariantSelection selection)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                selection = new RibbonVariantSelection();
                return false;
            }

            return _variants.TryGetValue(name.Trim(), out selection!);
        }

        public void Remove(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            _variants.Remove(name.Trim());
        }

        public static RibbonVariantMatrix CreateDefault()
        {
            var matrix = new RibbonVariantMatrix();
            matrix.Register("classic-default", new RibbonVariantSelection
            {
                LayoutMode = RibbonLayoutMode.Classic,
                Density = RibbonDensity.Comfortable,
                IsMinimized = false,
                DarkMode = false,
                UseSuperToolTips = true,
                SearchIncludeBackstage = false
            });
            matrix.Register("simplified-compact", new RibbonVariantSelection
            {
                LayoutMode = RibbonLayoutMode.Simplified,
                Density = RibbonDensity.Compact,
                IsMinimized = false,
                DarkMode = false,
                UseSuperToolTips = true,
                SearchIncludeBackstage = true
            });
            matrix.Register("touch-presenter", new RibbonVariantSelection
            {
                LayoutMode = RibbonLayoutMode.Classic,
                Density = RibbonDensity.Touch,
                IsMinimized = false,
                DarkMode = false,
                UseSuperToolTips = true,
                SearchIncludeBackstage = true
            });
            return matrix;
        }
    }
}
