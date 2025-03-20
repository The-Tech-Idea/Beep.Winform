using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Vis;

using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;

using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Winform.Controls.FunctionsandExtensions
{
    [AddinAttribute(Caption = "Data Menu", Name = "DataSourceMenuFunctions", misc = "IFunctionExtension", menu = "Beep", ObjectType = "Beep", addinType = AddinType.Class, iconimage = "datasources.png", order = 3, Showin = ShowinType.HorZToolbar)]
    public class HorizantalFunctions : IFunctionExtension
    {
        public IDMEEditor DMEEditor { get; set; }
        public IPassedArgs Passedargs { get; set; }
        public ErrorsInfo ErrorsandMesseges { get; set; } = new ErrorsInfo();


        private FunctionandExtensionsHelpers ExtensionsHelpers;
        public HorizantalFunctions(IDMEEditor pdMEEditor, Vis.Modules.IAppManager pvisManager, ITree ptreeControl)
        {
            DMEEditor = pdMEEditor;

            ExtensionsHelpers = new FunctionandExtensionsHelpers(DMEEditor, pvisManager, ptreeControl);
        }



        [CommandAttribute(Caption = "Refresh", Name = "Refresh", Click = true, iconimage = "refresh.png", ObjectType = "Beep", PointType = EnumPointType.Global, Showin = ShowinType.HorZToolbar, IsLeftAligned = false)]
        public IErrorsInfo Refresh(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            try
            {
                ExtensionsHelpers.GetValues(Passedarguments);
                if (ExtensionsHelpers.Vismanager != null)
                {

                    try
                    {
                        if (ExtensionsHelpers.Vismanager.CurrentDisplayedAddin == null)
                        {
                            return ErrorsandMesseges;
                        }
                        else
                        {
                            IDM_Addin dM_Addin = ExtensionsHelpers.Vismanager.CurrentDisplayedAddin;
                            dM_Addin.Run(Passedarguments);
                        }


                    }
                    catch (Exception ex)
                    {
                        ErrorsandMesseges.Flag = Errors.Failed;
                        ErrorsandMesseges.Message = ex.Message;
                        ErrorsandMesseges.Ex = ex;
                        DMEEditor.AddLogMessage("Beep", $" {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
                        ExtensionsHelpers.Vismanager.DialogManager.ShowMessege("DHUB", " Error in Saving  _targetGrid Layout", null);

                    }
                    return ErrorsandMesseges;
                }



                //   DMEEditor.AddLogMessage("Success", $"Turn on/off entities", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep", $" {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;

        }
    }
}
