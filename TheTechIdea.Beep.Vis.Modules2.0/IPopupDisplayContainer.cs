using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IPopupDisplayContainer
    {
        IBeepUIComponent TriggerControl { get; set; }
        event EventHandler OnLeave;

        void ShowPopup(IBeepUIComponent triggerControl, BeepPopupFormPosition position);
        void ShowPopup(IBeepUIComponent triggerControl, BeepPopupFormPosition position, int width, int height);
        void ShowPopup(IBeepUIComponent triggerControl, Point location);
        void ClosePopup();
        bool IsMouseOverControl(IBeepUIComponent control);
    }
}
