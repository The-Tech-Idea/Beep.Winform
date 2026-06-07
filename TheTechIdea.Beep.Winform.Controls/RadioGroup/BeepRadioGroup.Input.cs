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

            var adjustedLocation = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );

            _hitTestHelper.HandleMouseMove(adjustedLocation);
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
            
            // Convert mouse location to be relative to DrawingRect
            var adjustedLocation = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );
            
            _hitTestHelper.HandleMouseClick(adjustedLocation, e.Button);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (DesignMode || !Enabled) return;

            // Take focus so the keyboard navigation handlers (OnKeyDown / ProcessTabKey)
            // can drive the focused item.  Without this, clicking an item moves the
            // selection but Tab / arrow keys have no effect because the control never
            // owns focus.
            if (!Focused && CanFocus) Focus();

            var adjustedLocation = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );

            _hitTestHelper.HandleMouseDown(adjustedLocation, e.Button);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (DesignMode || !Enabled) return;

            var adjustedLocation = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );

            _hitTestHelper.HandleMouseUp(adjustedLocation, e.Button);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (DesignMode || !Enabled) return;

            // Convert mouse location to be relative to DrawingRect
            var adjustedLocation = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );

            _hitTestHelper.HandleMouseDoubleClick(adjustedLocation, e.Button);
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
