
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Utilities;

using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Winform.Controls.FunctionsandExtensions
{
    [AddinAttribute(Caption = "File", Name = "FileMenuFunctions", ObjectType = "Beep", menu = "Beep", misc = "IFunctionExtension", addinType = AddinType.Class,iconimage ="File.svg",order =1, Showin = ShowinType.Menu)]
    public class FileMenuFunctions : IFunctionExtension
    {
        public IDMEEditor DMEEditor { get ; set ; }
        public IPassedArgs Passedargs { get; set; }
     
        private FunctionandExtensionsHelpers ExtensionsHelpers;

       
        public FileMenuFunctions(IDMEEditor pdMEEditor, Vis.Modules.IAppManager pvisManager, ITree ptreeControl)
        {
            DMEEditor = pdMEEditor;
           
            ExtensionsHelpers = new FunctionandExtensionsHelpers(DMEEditor, pvisManager, ptreeControl);
        }
      
        [CommandAttribute(Caption = "Data Connection", Name = "dataconnection", Click = true, iconimage = "datasources.svg", ObjectType = "Beep", PointType = EnumPointType.Global, Showin = ShowinType.Menu)]
        public IErrorsInfo dataconnection(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            try
            {
               
                ExtensionsHelpers.GetValues(Passedarguments);
                ExtensionsHelpers.Vismanager.ShowPage("uc_dsList", null);
                // DMEEditor.AddLogMessage("Success", $"Open Data Connection", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Fail", $"Could not show data connection {ex.Message}", DateTime.Now, 0, Passedarguments.DatasourceName, Errors.Failed);
            }
            return DMEEditor.ErrorObject;

        }
        //[CommandAttribute(Caption = "New Project", Name = "NewProject", Click = true, iconimage = "newproject.ico", ObjectType = "Beep", PointType = EnumPointType.Global, Showin = ShowinType.Menu)]
        //public IErrorsInfo NewProject(IPassedArgs Passedarguments)
        //{
        //    DMEEditor.ErrorObject.Flag = Errors.Ok;
        //    try
        //    {
               
        //        ExtensionsHelpers.GetValues(Passedarguments);
        //        ExtensionsHelpers.Vismanager.ShowPage("uc_DataConnection", null);
        //        // DMEEditor.AddLogMessage("Success", $"Open Data Connection", DateTime.Now, 0, null, Errors.Ok);
        //    }
        //    catch (Exception ex)
        //    {
        //        DMEEditor.AddLogMessage("Fail", $"Could not create new project {ex.Message}", DateTime.Now, 0, Passedarguments.DatasourceName, Errors.Failed);
        //    }
        //    return DMEEditor.ErrorObject;

        //}
        //[CommandAttribute(Caption = "Save Project", Name = "SaveProject", Click = true, iconimage = "saveproject.ico", ObjectType = "Beep", PointType = EnumPointType.Global)]
        //public IErrorsInfo SaveProject(IPassedArgs Passedarguments)
        //{
        //    DMEEditor.ErrorObject.Flag = Errors.Ok;
        //    try
        //    {
               
        //        ExtensionsHelpers.GetValues(Passedarguments);
        //        ExtensionsHelpers.Vismanager.ShowPage("uc_DataConnection", null);
        //        // DMEEditor.AddLogMessage("Success", $"Open Data Connection", DateTime.Now, 0, null, Errors.Ok);
        //    }
        //    catch (Exception ex)
        //    {
        //        DMEEditor.AddLogMessage("Fail", $"Could not save project {ex.Message}", DateTime.Now, 0, Passedarguments.DatasourceName, Errors.Failed);
        //    }
        //    return DMEEditor.ErrorObject;

        //}

    }
}
