using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup
{
    public partial class BeepRadioGroup
    {
        #region Mouse Handling
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (DesignMode || !Enabled) return;
            // Cursor will be set to Hand on the next OnMouseMove once an item is hit.
            // Setting it now ensures the user sees an interactive cursor immediately.
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (DesignMode || !Enabled) return;

            // Convert mouse location to be relative to DrawingRect, then add _scrollOffset
            // when virtualized (the renderer shifts the visible window up by that many
            // pixels, so hit-testing must look at the un-translated rectangles).
            var drawingRelative = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );
            var hitTestPoint = TranslateMouseForHitTest(drawingRelative);

            _hitTestHelper.HandleMouseMove(hitTestPoint);
            Cursor = _hitTestHelper.HoveredIndex >= 0 ? Cursors.Hand : Cursors.Default;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (DesignMode) return;
            
            _hitTestHelper.HandleMouseLeave();
            Cursor = Cursors.Default;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (DesignMode || !Enabled) return;
            
            // Convert mouse location to be relative to DrawingRect, then shift for scroll.
            var drawingRelative = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );
            var hitTestPoint = TranslateMouseForHitTest(drawingRelative);
            
            _hitTestHelper.HandleMouseClick(hitTestPoint, e.Button);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (DesignMode || !Enabled) return;

            // Take focus on left-click only (matches standard Windows behaviour).
            // Right-clicks should not steal keyboard focus from another control.
            // The hit-test helper already filters non-Left buttons, but Focus()
            // runs before that filter so it needs its own gate.
            if (e.Button == MouseButtons.Left && !Focused && CanFocus) Focus();

            var drawingRelative = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );
            var hitTestPoint = TranslateMouseForHitTest(drawingRelative);

            _hitTestHelper.HandleMouseDown(hitTestPoint, e.Button);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (DesignMode || !Enabled) return;

            var drawingRelative = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );
            var hitTestPoint = TranslateMouseForHitTest(drawingRelative);

            _hitTestHelper.HandleMouseUp(hitTestPoint, e.Button);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (DesignMode || !Enabled) return;

            var drawingRelative = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );
            var hitTestPoint = TranslateMouseForHitTest(drawingRelative);

            _hitTestHelper.HandleMouseDoubleClick(hitTestPoint, e.Button);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (DesignMode || !Enabled) return;
            if (!IsVirtualized || _vScroll == null)
            {
                // Not virtualized: nothing for us to scroll.  Let the event bubble up
                // to the parent form so other scrollable surfaces can react.
                base.OnMouseWheel(e);
                return;
            }

            // Virtualized: scroll our own VScrollBar.  Do NOT call base when handling
            // (the base bubbles the event to the parent form and may scroll a
            // different surface, which is a confusing user experience).
            int newValue = Math.Max(_vScroll.Minimum,
                Math.Min(_vScroll.Maximum, _vScroll.Value - e.Delta / 120 * SystemInformation.MouseWheelScrollLines));
            if (newValue == _vScroll.Value) return;
            _vScroll.Value = newValue;
            // MarkLayoutDirty + Invalidate are wired by the VScrollBar.Scroll event handler.
        }
        #endregion

        #region Keyboard Handling
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (DesignMode || !Enabled) return;
            
            if (_hitTestHelper.HandleKeyDown(e.KeyCode, Orientation, ColumnCount))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        protected override bool ProcessTabKey(bool forward)
        {
            if (DesignMode || !Enabled) return base.ProcessTabKey(forward);
            
            if (forward)
            {
                return _hitTestHelper.MoveFocusNext();
            }
            else
            {
                return _hitTestHelper.MoveFocusPrevious();
            }
        }
        #endregion
    }
}
