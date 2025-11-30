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
            if (DesignMode) return;
            
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
            if (DesignMode) return;
            
            // Convert mouse location to be relative to DrawingRect
            var adjustedLocation = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );
            
            _hitTestHelper.HandleMouseClick(adjustedLocation, e.Button);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (DesignMode) return;
            
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
            if (DesignMode) return;
            
            if (_hitTestHelper.HandleKeyDown(e.KeyCode, Orientation))
            {
                e.Handled = true;
            }
        }

        protected override bool ProcessTabKey(bool forward)
        {
            if (DesignMode) return base.ProcessTabKey(forward);
            
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
