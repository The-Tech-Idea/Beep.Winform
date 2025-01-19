using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls.Design
{
    public class BeepDisplayContainerDesigner : ParentControlDesigner
    {
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            var control = component as BeepDisplayContainer;

            if (control != null)
            {
                EnableDesignMode(control, "SelectedTab");
            }
        }

        public override SelectionRules SelectionRules => SelectionRules.Visible;
    }
}
