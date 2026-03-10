using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup
{
    public partial class BeepRadioGroup
    {
        #region Mouse Handling
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (DesignMode || !Enabled) return;
            
            // Convert mouse location to be relative to DrawingRect
            var adjustedLocation = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );
            
            _hitTestHelper.HandleMouseMove(adjustedLocation);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (DesignMode) return;
            
            _hitTestHelper.HandleMouseLeave();
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
