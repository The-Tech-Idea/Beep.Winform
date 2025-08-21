using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Advanced.Helpers
{
    internal class ControlInputHelper
    {
        private readonly BeepControlAdvanced _owner;
        private readonly ControlEffectHelper _effects;
        private readonly ControlHitTestHelper _hitTest;

        public ControlInputHelper(BeepControlAdvanced owner, ControlEffectHelper effects, ControlHitTestHelper hitTest)
        {
            _owner = owner;
            _effects = effects;
            _hitTest = hitTest;
        }

        public void OnMouseEnter() { _owner.Invalidate(); }
        public void OnMouseLeave() { _owner.Invalidate(); }
        public void OnMouseMove(Point location) { /* extend as needed */ }
        public void OnMouseDown(MouseEventArgs e) { _owner.Invalidate(); }
        public void OnMouseUp(MouseEventArgs e) { _owner.Invalidate(); }
        public void OnMouseHover() { /* extend */ }
        public void OnClick() { /* extend */ }

        public void ReceiveMouseEvent(HitTestEventArgs eventArgs)
        {
            switch (eventArgs.MouseEvent)
            {
                case MouseEventType.MouseEnter: OnMouseEnter(); break;
                case MouseEventType.MouseLeave: OnMouseLeave(); break;
                case MouseEventType.MouseMove: OnMouseMove(eventArgs.Location); break;
                case MouseEventType.MouseDown: OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, eventArgs.Location.X, eventArgs.Location.Y, 0)); break;
                case MouseEventType.MouseUp: OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, eventArgs.Location.X, eventArgs.Location.Y, 0)); break;
                case MouseEventType.Click: /* intentionally no direct call to OnClick */ break;
            }
        }
    }
}
