using System;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IDisplayContainer
    {
        bool AddControl(string TitleText, IDM_Addin control, ContainerTypeEnum pcontainerType);
        bool RemoveControl(string TitleText, IDM_Addin control);
        bool RemoveControlByGuidTag(string guidid);
        bool RemoveControlByName(string guidid);
        bool ShowControl(string TitleText, IDM_Addin control);
        bool IsControlExit(IDM_Addin control);

        //IAppManager VisManager { get; set; }
        //IDMEEditor Editor { get; set; }
        void Clear();
        event EventHandler<ContainerEvents> AddinAdded;
        event EventHandler<ContainerEvents> AddinRemoved;
        event EventHandler<ContainerEvents> AddinMoved;
        event EventHandler<ContainerEvents> AddinChanging;
        event EventHandler<ContainerEvents> AddinChanged;
        event EventHandler<IPassedArgs> PreCallModule;
        event EventHandler<IPassedArgs> PreShowItem;

        event EventHandler<KeyCombination> KeyPressed;
        IErrorsInfo PressKey(KeyCombination keyCombination);
    }
    public class ContainerEvents : EventArgs
    {
        public ContainerEvents() { }
       public ContainerTypeEnum ContainerType { get; set; }
        public string TitleText { get; set; }   
        public IDM_Addin Control { get; set; }
        public string Guidid { get; set; }
        public int Id { get; set; }

    }
}
