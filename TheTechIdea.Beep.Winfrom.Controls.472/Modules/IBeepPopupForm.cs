

using System;
using System.Drawing;
using System.Threading.Tasks;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IBeepPopupForm
    {
     
        bool AutoClose { get; set; }
      
        bool CloseOnSelection { get; set; }
        string DisplayName { get; }
        bool InPopMode { get; set; }
 
        DialogReturn Result { get; set; }
     

        event EventHandler OnClose;
        event EventHandler OnLeave;
        event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        void CloseCascade();
     
     
        void NotifySelectedItemChanged(SimpleItem selectedItem);
        void SetAsActive();
      
        void ShowPopup(IDM_Addin triggerControl, BeepPopupFormPosition position);
        void ShowPopup(IDM_Addin triggerControl, BeepPopupFormPosition position, int width, int height);
        void ShowPopup(IDM_Addin triggerControl, BeepPopupFormPosition position, Point adjustment);
        void ShowPopup(IDM_Addin triggerControl, Point location);
        void ShowPopup(Point anchorPoint, BeepPopupFormPosition position);
        void ShowPopup(Point anchorPoint, BeepPopupFormPosition position, Point adjustment);
        Task ShowPopupAsync(IDM_Addin triggerControl, BeepPopupFormPosition position);
        Task ShowPopupAsync(IDM_Addin triggerControl, BeepPopupFormPosition position, int width, int height);
        Task ShowPopupAsync(IDM_Addin triggerControl, BeepPopupFormPosition position, Point adjustment);
        Task ShowPopupAsync(IDM_Addin triggerControl, Point location);
        Task ShowPopupAsync(Point anchorPoint, BeepPopupFormPosition position);
        Task ShowPopupAsync(Point anchorPoint, BeepPopupFormPosition position, Point adjustment);
    }
}