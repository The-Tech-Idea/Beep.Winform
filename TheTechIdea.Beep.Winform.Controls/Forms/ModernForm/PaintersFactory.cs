using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    /// <summary>
    /// Factory for creating and caching form painter instances.
    /// Painters are cached by FormStyle to avoid repeated allocations.
    /// This significantly improves performance when switching styles or creating multiple forms.
    /// </summary>
    public static class PaintersFactory
    {
        private static readonly Dictionary<FormStyle, IFormPainter> _painterCache = new Dictionary<FormStyle, IFormPainter>();
        private static readonly object _cacheLock = new object();

        /// <summary>
        /// Gets a cached painter instance for the specified form style.
        /// Creates a new instance only if one doesn't exist in the cache.
        /// </summary>
        /// <param name="style">The form style to get a painter for.</param>
        /// <returns>A cached or newly created painter instance.</returns>
        public static IFormPainter GetPainter(FormStyle style)
        {
            lock (_cacheLock)
            {
                if (_painterCache.TryGetValue(style, out var cached))
                    return cached;

                var painter = CreatePainter(style);
                _painterCache[style] = painter;
                return painter;
            }
        }

        /// <summary>
        /// Creates a new painter instance for the specified form style.
        /// This method is called internally when a painter is not in the cache.
        /// </summary>
        private static IFormPainter CreatePainter(FormStyle style)
        {
            return style switch
            {
                FormStyle.Terminal => new TerminalFormPainter(),
                FormStyle.Modern => new ModernFormPainter(),
                FormStyle.Minimal => new MinimalFormPainter(),
                FormStyle.MacOS => new MacOSFormPainter(),
                FormStyle.Fluent => new FluentFormPainter(),
                FormStyle.Material => new MaterialFormPainter(),
                FormStyle.Cartoon => new CartoonFormPainter(),
                FormStyle.ChatBubble => new ChatBubbleFormPainter(),
                FormStyle.Glass => new GlassFormPainter(),
                FormStyle.Metro => new MetroFormPainter(),
                FormStyle.Metro2 => new Metro2FormPainter(),
                FormStyle.GNOME => new GNOMEFormPainter(),
                FormStyle.NeoMorphism => new NeoMorphismFormPainter(),
                FormStyle.Glassmorphism => new GlassmorphismFormPainter(),
                FormStyle.iOS => new iOSFormPainter(),
                FormStyle.Nordic => new NordicFormPainter(),
                FormStyle.Paper => new PaperFormPainter(),
                FormStyle.Ubuntu => new UbuntuFormPainter(),
                FormStyle.KDE => new KDEFormPainter(),
                FormStyle.ArcLinux => new ArcLinuxFormPainter(),
                FormStyle.Dracula => new DraculaFormPainter(),
                FormStyle.Solarized => new SolarizedFormPainter(),
                FormStyle.OneDark => new OneDarkFormPainter(),
                FormStyle.GruvBox => new GruvBoxFormPainter(),
                FormStyle.Nord => new NordFormPainter(),
                FormStyle.Tokyo => new TokyoFormPainter(),
                FormStyle.Brutalist => new BrutalistFormPainter(),
                FormStyle.Retro => new RetroFormPainter(),
                FormStyle.Cyberpunk => new CyberpunkFormPainter(),
                FormStyle.Neon => new NeonFormPainter(),
                FormStyle.Holographic => new HolographicFormPainter(),
                FormStyle.Shadcn => new ShadcnFormPainter(),
                FormStyle.RadixUI => new RadixUIFormPainter(),
                FormStyle.NextJS => new NextJSFormPainter(),
                FormStyle.Linear => new LinearFormPainter(),
                FormStyle.MaterialYou => new MaterialYouFormPainter(),
                FormStyle.Custom => new CustomFormPainter(),
                _ => new MinimalFormPainter()
            };
        }

        /// <summary>
        /// Clears the painter cache.
        /// Call this when theme changes to force painters to be recreated with new theme colors.
        /// </summary>
        public static void ClearCache()
        {
            lock (_cacheLock)
            {
                // Dispose painters if they implement IDisposable
                foreach (var painter in _painterCache.Values)
                {
                    if (painter is IDisposable disposable)
                    {
                        try { disposable.Dispose(); } catch { }
                    }
                }
                _painterCache.Clear();
            }
        }

        /// <summary>
        /// Gets the number of cached painters.
        /// Useful for debugging and monitoring.
        /// </summary>
        public static int CachedPainterCount
        {
            get
            {
                lock (_cacheLock)
                {
                    return _painterCache.Count;
                }
            }
        }

        /// <summary>
        /// Checks if a painter for the specified style is cached.
        /// </summary>
        public static bool IsCached(FormStyle style)
        {
            lock (_cacheLock)
            {
                return _painterCache.ContainsKey(style);
            }
        }

        /// <summary>
        /// Removes a specific painter from the cache.
        /// Useful when you want to force recreation of a specific painter.
        /// </summary>
        public static void RemoveFromCache(FormStyle style)
        {
            lock (_cacheLock)
            {
                if (_painterCache.TryGetValue(style, out var painter))
                {
                    if (painter is IDisposable disposable)
                    {
                        try { disposable.Dispose(); } catch { }
                    }
                    _painterCache.Remove(style);
                }
            }
        }
    }
}

