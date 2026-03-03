using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Drag-to-reorder support for <see cref="BeepListBox"/>.
    /// Activated when <see cref="AllowItemReorder"/> is <see langword="true"/>.
    /// </summary>
    public partial class BeepListBox
    {
        // ── Drag state ───────────────────────────────────────────────────────────

        private Point      _dragAnchorPoint  = Point.Empty;
        private SimpleItem? _dragItem;
        private int        _dragSourceIndex  = -1;
        private bool       _isDragging;
        private int        _insertIndex      = -1;   // 0 = before first, Count = after last
        private Form?      _dragGhost;

        private const int DragThresholdPx = 4;

        // ── Intent recording (called from Keyboard.cs OnMouseDown) ──────────────

        /// <summary>
        /// Records the drag anchor point and source item when the left button is pressed.
        /// Called by <see cref="BeepListBox.Keyboard"/> partial OnMouseDown.
        /// </summary>
        internal void BeginDragCheck(MouseEventArgs e)
        {
            if (!AllowItemReorder || e.Button != MouseButtons.Left) return;

            var layoutCache = _layoutHelper?.GetCachedLayout();
            if (layoutCache == null) return;

            foreach (var info in layoutCache)
            {
                if (info.RowRect.Contains(e.Location))
                {
                    _dragAnchorPoint = e.Location;
                    _dragItem        = info.Item;
                    _dragSourceIndex = _listItems?.IndexOf(info.Item) ?? -1;
                    return;
                }
            }

            CancelDrag();   // clicked empty area – clear any stale intent
        }

        // ── Mouse override ───────────────────────────────────────────────────────

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_isDragging && e.Button == MouseButtons.Left)
                CommitDragReorder();

            CancelDrag();
        }

        // ── Drag move helper (called from Events.cs OnMouseMove) ─────────────────

        private void HandleDragMove(MouseEventArgs e)
        {
            if (!AllowItemReorder || _dragItem == null || e.Button != MouseButtons.Left) return;

            if (!_isDragging)
            {
                // Enforce drag threshold before starting the visual feedback
                int dx = Math.Abs(e.X - _dragAnchorPoint.X);
                int dy = Math.Abs(e.Y - _dragAnchorPoint.Y);
                if (dx < DragThresholdPx && dy < DragThresholdPx) return;

                _isDragging = true;
                _dragGhost  = CreateDragGhost(_dragItem);
                _dragGhost.Show(this);
            }

            // Keep ghost window under the cursor
            if (_dragGhost != null)
            {
                var screen = PointToScreen(e.Location);
                _dragGhost.Location = new Point(screen.X + 14, screen.Y + 4);
            }

            // Recalculate insertion slot
            int newInsert = CalculateInsertIndex(e.Y);
            if (newInsert != _insertIndex)
            {
                _insertIndex = newInsert;
                Invalidate();
            }
        }

        // ── Drawing (called at the end of Drawing.cs Paint) ─────────────────────

        private void DrawDragIndicator(Graphics g)
        {
            if (!_isDragging || _insertIndex < 0) return;

            var layoutCache = _layoutHelper?.GetCachedLayout();
            if (layoutCache == null || layoutCache.Count == 0) return;

            int lineY;
            var clientArea = GetClientArea();

            if (_insertIndex == 0)
                lineY = layoutCache[0].RowRect.Top;
            else if (_insertIndex >= layoutCache.Count)
                lineY = layoutCache[layoutCache.Count - 1].RowRect.Bottom;
            else
                lineY = layoutCache[_insertIndex - 1].RowRect.Bottom;

            var accentColor = (_currentTheme != null)
                ? _currentTheme.PrimaryColor
                : Color.DodgerBlue;

            // 2-px insertion line
            using var pen   = new Pen(accentColor, 2f);
            int x1 = clientArea.Left  + 8;
            int x2 = clientArea.Right - 8;
            g.DrawLine(pen, x1, lineY, x2, lineY);

            // Small circular tips at both ends
            const int r = 4;
            using var tipBrush = new SolidBrush(accentColor);
            g.FillEllipse(tipBrush, x1 - r, lineY - r, r * 2, r * 2);
            g.FillEllipse(tipBrush, x2 - r, lineY - r, r * 2, r * 2);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private int CalculateInsertIndex(int mouseY)
        {
            var layoutCache = _layoutHelper?.GetCachedLayout();
            if (layoutCache == null || layoutCache.Count == 0) return 0;

            for (int i = 0; i < layoutCache.Count; i++)
            {
                int midY = layoutCache[i].RowRect.Top + layoutCache[i].RowRect.Height / 2;
                if (mouseY < midY) return i;
            }

            return layoutCache.Count;   // after last item
        }

        private void CommitDragReorder()
        {
            if (_listItems == null || _dragItem == null || _dragSourceIndex < 0) return;

            int target = _insertIndex;

            // When we remove the source item the indices above it shift down by 1
            if (target > _dragSourceIndex) target--;

            // Nothing to do when dropping back to same position
            if (target == _dragSourceIndex) return;

            target = Math.Max(0, Math.Min(target, _listItems.Count - 1));

            _listItems.RemoveAt(_dragSourceIndex);
            _listItems.Insert(target, _dragItem);

            // Notify accessibility and raise event
            NotifyA11yOrderChanged();
            OnItemReordered(_dragSourceIndex, target, _dragItem);

            // Trigger layout recalculation
            _needsLayoutUpdate = true;
            RequestDelayedInvalidate();
        }

        /// <summary>
        /// Creates a lightweight semi-transparent ghost window that follows the cursor
        /// while the drag is in progress.
        /// </summary>
        private Form CreateDragGhost(SimpleItem item)
        {
            int ghostH = DpiScalingHelper.ScaleValue(32, this);
            int ghostW = Math.Max(120, Width / 2);
            string label = item?.Text ?? string.Empty;

            var ghost = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar   = false,
                TopMost         = true,
                Size            = new Size(ghostW, ghostH),
                Opacity         = 0.72,
                BackColor       = Color.FromArgb(45, 45, 65),
                StartPosition   = FormStartPosition.Manual
            };

            var lbl = new Label
            {
                Text      = label,
                ForeColor = Color.White,
                Font      = Font ?? SystemFonts.DefaultFont,
                AutoSize  = false,
                Dock      = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding   = new Padding(8, 0, 0, 0)
            };

            ghost.Controls.Add(lbl);

            // Rounded region
            ghost.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, ghostW, ghostH, 6, 6));

            return ghost;
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);

        private void CancelDrag()
        {
            _dragItem        = null;
            _dragSourceIndex = -1;
            _isDragging      = false;
            _insertIndex     = -1;
            _dragAnchorPoint = Point.Empty;

            if (_dragGhost != null)
            {
                _dragGhost.Close();
                _dragGhost.Dispose();
                _dragGhost = null;
            }

            Invalidate();
        }
    }
}
