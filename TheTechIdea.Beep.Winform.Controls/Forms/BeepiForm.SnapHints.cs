namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm
    {
        private bool _showSnapHints = true;

        public bool ShowSnapHints
        {
            get => _showSnapHints;
            set { _showSnapHints = value; Invalidate(); }
        }

        private Rectangle _snapLeft, _snapRight, _snapTop;
        private bool _showSnapOverlay;

        partial void InitializeSnapHintsFeature()
        {
            RegisterMouseMoveHandler(SnapHints_OnMouseMove);
            RegisterMouseLeaveHandler(SnapHints_OnMouseLeave);
            RegisterOverlayPainter(SnapHints_OnPaintOverlay);
        }

        private void SnapHints_OnMouseMove(MouseEventArgs e)
        {
            if (!_showSnapHints) return;
            if (WindowState == FormWindowState.Normal && (MouseButtons & MouseButtons.Left) == MouseButtons.Left)
            {
                var screen = Screen.FromPoint(Cursor.Position).WorkingArea;
                int thickness = 8;
                _snapLeft = new Rectangle(screen.Left, screen.Top, thickness, screen.Height);
                _snapRight = new Rectangle(screen.Right - thickness, screen.Top, thickness, screen.Height);
                _snapTop = new Rectangle(screen.Left, screen.Top, screen.Width, thickness);
                _showSnapOverlay = true;
                Invalidate();
            }
            else if (_showSnapOverlay)
            {
                _showSnapOverlay = false;
                Invalidate();
            }
        }

        private void SnapHints_OnMouseLeave()
        {
            if (_showSnapOverlay)
            {
                _showSnapOverlay = false;
                Invalidate();
            }
        }

        private void SnapHints_OnPaintOverlay(Graphics g)
        {
            if (!_showSnapOverlay) return;
            using var br = new SolidBrush(Color.FromArgb(40, 0, 120, 215));
            g.FillRectangle(br, _snapLeft);
            g.FillRectangle(br, _snapRight);
            g.FillRectangle(br, _snapTop);
        }
    }
}
