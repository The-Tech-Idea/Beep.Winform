using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers
{
    // === OVERLAY (dimmed background) ===
    internal sealed class _OverlayPanel : Panel
    {
        public float Opacity { get; set; } = 0f; // 0..1
        public _OverlayPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Dock = DockStyle.None; // we set Bounds precisely to the adjusted rectangle
            Cursor = Cursors.Default;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var br = new SolidBrush(Color.FromArgb((int)(Opacity * 255), 0, 0, 0));
            e.Graphics.FillRectangle(br, ClientRectangle);
        }
    }
}
