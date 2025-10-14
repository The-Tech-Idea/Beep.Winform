using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Modules
{
    // this suppose to be implemented by a form that can display popup controls
    // like BeepPopupForm or BeepPopupModalForm
    /// it defines methods to show and close popup controls
    /// it can behave like a container for popup controls
    /// it can behave like a dialog container for modal popup controls
    /// 
    /// <summary>
    /// Interface for a container that can display popup controls.
    /// </summary>
    public interface IPopupDisplayContainer
    {
        /// this suppose to be implemented by a form that can display popup controls
        /// <summary>
        /// like BeepPopupForm or BeepPopupModalForm
        /// </summary>
        /// it defines methods to show and close popup controls
        /// it can behave like a container for popup controls
        /// it can behave like a dialog container for modal popup controls
        
        
        DialogReturn Result { get; set; }
     
        string DisplayName { get; }

        IBeepUIComponent TriggerControl { get; set; }
        event EventHandler OnLeave;


        DialogReturn ShowPopup(IBeepUIComponent triggerControl, BeepPopupFormPosition position);
        DialogReturn ShowPopup(IBeepUIComponent triggerControl, BeepPopupFormPosition position, int width, int height);
        DialogReturn ShowPopup(IBeepUIComponent triggerControl, Point location);
        DialogReturn ClosePopup();
        bool IsMouseOverControl(IBeepUIComponent control);
    }
}
