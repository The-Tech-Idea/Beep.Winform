using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Advanced.Helpers
{
    internal class ControlHitTestHelper
    {
        private readonly Control _owner;

        public ControlHitTestHelper(Control owner)
        {
            _owner = owner;
        }

        public void SendMouseEvent(IBeepUIComponent targetControl, MouseEventType eventType, Point screenLocation)
        {
            if (targetControl == null) return;
            Point clientPoint = screenLocation;
            if (targetControl is Control control)
            {
                clientPoint = control.PointToClient(screenLocation);
            }
            var args = new HitTestEventArgs(eventType, clientPoint);
            targetControl.ReceiveMouseEvent(args);
        }
    }
}
