using System;
using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Maintains a list of overlay painters invoked after core painting.
    /// </summary>
    internal sealed class FormOverlayPainterRegistry
    {
        private readonly List<Action<Graphics>> _painters = new();

        public void Add(Action<Graphics> painter)
        {
            if (painter == null) return;
            if (!_painters.Contains(painter)) _painters.Add(painter);
        }

        public void Remove(Action<Graphics> painter)
        {
            _painters.Remove(painter);
        }

        public void PaintOverlays(Graphics g)
        {
            foreach (var p in _painters)
            {
                try { p(g); } catch { /* swallow individual painter errors */ }
            }
        }
    }
}
