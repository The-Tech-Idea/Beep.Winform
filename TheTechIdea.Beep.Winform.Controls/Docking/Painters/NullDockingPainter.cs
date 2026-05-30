using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// No-op theme/metrics provider used at design time so that the manager's painter is never
    /// <c>null</c>. Color/font/size properties return safe system defaults.
    /// Safe to use in the Visual Studio designer process.
    /// </summary>
    internal sealed class NullDockingPainter : IDockingPainter
    {
        public static readonly NullDockingPainter Instance = new();

        private NullDockingPainter() { }

        // ── Colors ───────────────────────────────────────────────────────────────
        public Color BackgroundColor => SystemColors.Control;
        public Color ForegroundColor => SystemColors.ControlText;
        public Color BorderColor     => SystemColors.ControlDark;
        public Color HoverColor      => SystemColors.ControlLight;
        public Color SelectedColor   => SystemColors.Highlight;
        public Color DisabledColor   => SystemColors.GrayText;

        // ── Fonts ────────────────────────────────────────────────────────────────
        public Font UIFont  => BeepFontManager.DefaultFont;
        public Font TabFont => BeepFontManager.DefaultFont;

        // ── Metrics ──────────────────────────────────────────────────────────────
        public int TabStripHeight => 24;
        public int SplitterWidth  => 4;

        public void InvalidateCache() { }

        public void Dispose() { }
    }
}
