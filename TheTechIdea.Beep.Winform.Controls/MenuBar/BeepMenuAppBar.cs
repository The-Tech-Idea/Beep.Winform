
using Autofac.Core;
using System.ComponentModel;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Shared;


namespace TheTechIdea.Beep.Winform.Controls.MenuBar
{
    [ToolboxItem(true)]
    [DisplayName("Beep Menu App Bar")]
    [Category("Beep Controls")]
    [Description("A menu bar control that displays a list of items.")]
    public partial class BeepMenuAppBar:BeepMenuBar
    {

        IBeepService _beepServices;
        IDMEEditor DMEEditor;
        public IBeepService beepServices { 
            get { return _beepServices; } 
            set { _beepServices = value;
                if (value != null) 
                    { DMEEditor = _beepServices.DMEEditor; 
                      AssemblyClassDefinitionManager.DMEEditor = DMEEditor; 
                    }
                }
        }

        public string? ObjectType { get; private set; }

        public BeepMenuAppBar(IBeepService beepServices)
        {
            _beepServices = beepServices;
            DMEEditor = _beepServices.DMEEditor;
        }

        public BeepMenuAppBar():base()
        {

        }
        public IErrorsInfo CreateMenuItems()
        {
            try
            {
                foreach (var item in DynamicMenuManager.CreateMenuItems(DMEEditor, "Beep"))
                {
                    MenuItems.Add(item);
                }
                InitMenu();

                return DMEEditor.ErrorObject;
            }
            catch (Exception ex)
            {

                return DMEEditor.ErrorObject;
            }
        }
      
    }
}
