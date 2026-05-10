using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes.Painters
{
    /// <summary>
    /// Factory for creating checkbox painters based on style.
    /// </summary>
    /// <remarks>
    /// Painter reuse policy (BCHK-P5-004):
    ///   All concrete painters are stateless — they hold no GDI resources and receive all
    ///   rendering inputs through <see cref="CheckBoxRenderOptions"/> on each call.
    ///   A static per-style cache is therefore safe and eliminates repeated allocations when
    ///   <see cref="BeepCheckBox{T}.CheckBoxStyle"/> is set (e.g., during data binding or
    ///   designer-preset application). The cache is never cleared because painters have no
    ///   resources to release.
    ///   If a future painter needs per-instance state, remove it from the cache and document why.
    /// </remarks>
    public static class CheckBoxPainterFactory
    {
        private static readonly Dictionary<CheckBoxStyle, ICheckBoxPainter> _cache =
            new Dictionary<CheckBoxStyle, ICheckBoxPainter>();

        /// <summary>
        /// Returns the shared (cached) painter instance for the specified style.
        /// </summary>
        public static ICheckBoxPainter GetPainter(CheckBoxStyle style)
        {
            BeepCheckBoxDiagnostics.RecordPainterFetch();

            if (_cache.TryGetValue(style, out var cached))
                return cached;

            ICheckBoxPainter painter = style switch
            {
                CheckBoxStyle.Material3 => new Material3CheckBoxPainter(),
                CheckBoxStyle.Modern    => new ModernCheckBoxPainter(),
                CheckBoxStyle.Classic   => new ClassicCheckBoxPainter(),
                CheckBoxStyle.Minimal   => new MinimalCheckBoxPainter(),
                CheckBoxStyle.iOS       => new iOSCheckBoxPainter(),
                CheckBoxStyle.Fluent2   => new Fluent2CheckBoxPainter(),
                CheckBoxStyle.Switch    => new SwitchCheckBoxPainter(),
                CheckBoxStyle.Button    => new ButtonCheckBoxPainter(),
                _                      => new Material3CheckBoxPainter()
            };

            _cache[style] = painter;
            return painter;
        }
    }
}
