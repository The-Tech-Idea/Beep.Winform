using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Modules
{
    public class ControlHitTest
    {
        public string Name { get; set; }
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public Rectangle TargetRect { get; set; }
        public Action HitAction { get; set; }
        public string ActionName { get; set; }
        public bool IsHit
        {
            get; set;
        }
        public bool IsVisible { get; set; } = true;
        public bool IsEnabled { get; set; } = true;
        public bool IsSelected { get; set; }
        public bool IsPressed { get; set; }
        public bool IsHovered { get; set; }
        public bool IsFocused { get; set; }

        public ControlHitTest()
        {
        }
        public ControlHitTest(Rectangle rect)
        {
            TargetRect = rect;
        }
        public ControlHitTest(Rectangle rect, Point location)
        {
            TargetRect = rect;
            IsHit = TargetRect.Contains(location);
        }


    }
    public class ControlHitTestArgs : EventArgs
    {
        public ControlHitTest HitTest { get; set; }
        public ControlHitTestArgs(ControlHitTest hitTest)
        {
            HitTest = hitTest;
        }

    }
}
