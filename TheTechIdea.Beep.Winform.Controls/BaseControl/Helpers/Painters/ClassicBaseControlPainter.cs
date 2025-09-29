using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Adapter painter that renders using the legacy ControlPaintHelper (classic mode).
    /// </summary>
    internal sealed class ClassicBaseControlPainter : IBaseControlPainter
    {
        public void UpdateLayout(Base.BaseControl owner)
        {
            owner?._paint?.UpdateRects();
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null) return;
            owner?._paint?.EnsureUpdated();
            owner?._paint?.Draw(g);
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            // Classic mode: rely on owner's existing hit-test registrations (no-op)
        }
    }
}
