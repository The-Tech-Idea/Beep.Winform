using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls.Design
{
    public class TabControlWithoutTabsDesigner : ParentControlDesigner
    {
        public override bool CanParent(Control control)
        {
            // Allow only TabPages to be added as children
            return control is TabPage || base.CanParent(control);
        }

        protected override bool GetHitTest(System.Drawing.Point point)
        {
            // Allow dragging onto the client area
            var tabControl = (TabControlWithoutHeader)Control;
            var clientRect = tabControl.DisplayRectangle;
            return clientRect.Contains(tabControl.PointToClient(point));
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            if (component is TabControlWithoutHeader tabControl)
            {
                EnableDesignMode(tabControl, "TabPages");
            }
        }
    }
}
