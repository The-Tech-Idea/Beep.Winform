using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDisplayContainer : BeepControl, IDisplayContainer
    {
        public BeepDisplayContainer()
        {
            
        }
      
        public event EventHandler<ContainerEvents> AddinAdded;
        public event EventHandler<ContainerEvents> AddinRemoved;
        public event EventHandler<ContainerEvents> AddinMoved;
        public event EventHandler<ContainerEvents> AddinChanging;
        public event EventHandler<ContainerEvents> AddinChanged;
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;
        public event EventHandler<KeyCombination> KeyPressed;

        public bool AddControl(string TitleText, IDM_Addin control, ContainerTypeEnum pcontainerType)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool IsControlExit(IDM_Addin control)
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo PressKey(KeyCombination keyCombination)
        {
            throw new NotImplementedException();
        }

        public bool RemoveControl(string TitleText, IDM_Addin control)
        {
            throw new NotImplementedException();
        }

        public bool RemoveControlByGuidTag(string guidid)
        {
            throw new NotImplementedException();
        }

        public bool RemoveControlByName(string guidid)
        {
            throw new NotImplementedException();
        }

        public bool ShowControl(string TitleText, IDM_Addin control)
        {
            throw new NotImplementedException();
        }
    }
}
