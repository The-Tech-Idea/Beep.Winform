using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Cards.Helpers
{
    /// <summary>
    /// Process-lifetime cache of single-color GDI brushes and simple pens shared by all
    /// card painters. Card painters are deliberately self-contained (no base class), so this
    /// static helper gives them allocation-free fills/strokes in the per-paint
    /// <c>DrawBackground</c> / <c>DrawForegroundAccents</c> hot paths.
    ///
    /// Rules:
    /// - UI-thread only (WinForms painting). A plain dictionary is safe here.
    /// - NEVER dispose the returned instances — they are shared and live for the process.
    /// - Only for solid fills and simple solid strokes. Callers that mutate a pen
    ///   (StartCap/EndCap/DashStyle/etc.) MUST allocate their own <c>using</c> pen instead,
    ///   because mutating a cached pen would corrupt it for every other caller.
    /// </summary>
    internal static class CardPaintCache
    {
        private static readonly Dictionary<int, SolidBrush> _brushes = new();
        private static readonly Dictionary<(int argb, float width), Pen> _pens = new();

        /// <summary>Returns a shared solid brush for the given color. Do not dispose.</summary>
        public static SolidBrush Brush(Color color)
        {
            int key = color.ToArgb();
            if (_brushes.TryGetValue(key, out var b) && b != null)
                return b;
            b = new SolidBrush(color);
            _brushes[key] = b;
            return b;
        }

        /// <summary>Returns a shared simple pen for the given color/width. Do not dispose or mutate.</summary>
        public static Pen Pen(Color color, float width = 1f)
        {
            var key = (color.ToArgb(), width);
            if (_pens.TryGetValue(key, out var p) && p != null)
                return p;
            p = new Pen(color, width);
            _pens[key] = p;
            return p;
        }
    }
}
