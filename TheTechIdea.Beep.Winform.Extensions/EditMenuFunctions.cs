﻿
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;


namespace TheTechIdea.Beep.Winform.Extensions
{
    [AddinAttribute(Caption = "Edit", Name = "EditMenuFunctions", ObjectType = "Beep", misc = "IFunctionExtension", menu = "Beep", addinType = AddinType.Class, iconimage = "edit.svg", order = 2, Showin = ShowinType.Menu)]
    public class EditMenuFunctions : IFunctionExtension
    {
        public IDMEEditor DMEEditor { get; set; }
        public IPassedArgs Passedargs { get; set; }

        public IFunctionandExtensionsHelpers ExtensionsHelpers { get; set; }
        public EditMenuFunctions(IAppManager pvisManager)
        {
            DMEEditor = pvisManager.DMEEditor;
            if (pvisManager.Tree != null)
            {
                tree = (ITree)pvisManager.Tree;
                ExtensionsHelpers = tree.ExtensionsHelpers;
            }
        }
        private ITree tree;
        //public EditMenuFunctions(IDMEEditor pdMEEditor, Vis.Modules.IAppManager pvisManager, ITree ptreeControl)
        //{
        //    DMEEditor = pdMEEditor;

        //    ExtensionsHelpers = new FunctionandExtensionsHelpers(DMEEditor, pvisManager, ptreeControl);
        //}

        //[CommandAttribute(Caption = "Config Log", Name = "Showlog", Click = true, iconimage = "log.png", ObjectType = "Beep", PointType = EnumPointType.Global, Showin = ShowinType.Both)]
        //public IErrorsInfo showlog(IPassedArgs Passedarguments)
        //{
        //    DMEEditor.ErrorObject.Flag = Errors.Ok;
        //    try
        //    {
        //        ExtensionsHelpers.GetValues(Passedarguments);
        //        ExtensionsHelpers.Vismanager.IsLogOn=!ExtensionsHelpers.Vismanager.IsLogOn;
        //        ExtensionsHelpers.Vismanager.ShowLogWindow = !ExtensionsHelpers.Vismanager.ShowLogWindow;

        //        //  DMEEditor.AddLogMessage("Success", $"Turn on/off entities", DateTime.Now, 0, null, Errors.Ok);
        //    }
        //    catch (Exception ex)
        //    {
        //        DMEEditor.AddLogMessage("Fail", $"Could not turn on/offf entities {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
        //    }
        //    return DMEEditor.ErrorObject;

        //}

        [CommandAttribute(Caption = "Config/Hide Side Panel", Name = "SidePanelCollapse", Click = true, iconimage = "treeexpand.png", ObjectType = "Beep", PointType = EnumPointType.Global, Showin = ShowinType.Menu,Key =BeepKeys.Q,Alt =true)]
        public IErrorsInfo SidePanelCollapse(IPassedArgs Passedarguments)
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            try
            {
                ExtensionsHelpers.GetValues(Passedarguments);
                ExtensionsHelpers.Vismanager.ShowTreeWindow = !ExtensionsHelpers.Vismanager.ShowTreeWindow;

                //  DMEEditor.AddLogMessage("Success", $"Turn on/off entities", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Fail", $"Could not turn on/offf entities {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;

        }
        [CommandAttribute(Caption = "Config/Hide Log Panel", Name = "LogPanelCollapse", Click = true, iconimage = "expandlog.png", ObjectType = "Beep", PointType = EnumPointType.Global, Showin = ShowinType.Menu, Key = BeepKeys.L, Alt = true)]
        public IErrorsInfo LogPanelCollapse()
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            try
            {
                ExtensionsHelpers.GetValues();
                ExtensionsHelpers.Vismanager.ShowLogWindow = !ExtensionsHelpers.Vismanager.ShowLogWindow;

                //  DMEEditor.AddLogMessage("Success", $"Turn on/off entities", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Fail", $"Could not turn on/offf entities {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;

        }
        [CommandAttribute(Caption = "Turnon/Off CheckBox's", Name = "Turnon/Off CheckBox", Click = true, iconimage = "check.png", ObjectType = "Beep", PointType = EnumPointType.Global, Showin = ShowinType.Both,Key = BeepKeys.T,Alt =true)]
        public IErrorsInfo TurnonOffCheckBox()
        {
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            try
            {
                ExtensionsHelpers.GetValues();
                ExtensionsHelpers.TreeEditor.TurnonOffCheckBox(!ExtensionsHelpers.TreeEditor.IsCheckBoxon);
                //ExtensionsHelpers.TreeEditor.SelectedBranchs.Clear();
                //  DMEEditor.AddLogMessage("Success", $"Turn on/off entities", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Fail", $"Could not turn on/offf entities {ex.Message}", DateTime.Now, 0, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;

        }
        [CommandAttribute(Name = "EditDefaults", Caption = "Edit Default", Click = true, iconimage = "editdefaults.png", ObjectType = "Beep", PointType = EnumPointType.DataPoint, Showin = ShowinType.Menu)]
        public IErrorsInfo EditDefault(IPassedArgs Passedarguments)
        {

            DMEEditor.ErrorObject.Flag = Errors.Ok;
            //  DMEEditor.Logger.WriteLog($"Filling Database Entites ) ");
            try
            {
                ExtensionsHelpers.GetValues(Passedarguments);
                if (ExtensionsHelpers.CurrentBranch == null)
                {
                    return DMEEditor.ErrorObject;
                }
                if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.DataPoint)
                {
                   
                    List<DefaultValue> defaults = DMEEditor.ConfigEditor.DataConnections[DMEEditor.ConfigEditor.DataConnections.FindIndex(i => i.ConnectionName == Passedarguments.DatasourceName)].DatasourceDefaults;
                    if (defaults != null)
                    {
                        string[] args = { "CopyDefaults", null, null };
                        List<ObjectItem> ob = new List<ObjectItem>(); ;
                        ObjectItem it = new ObjectItem();
                        it.obj = defaults;
                        it.Name = "Defaults";
                        ob.Add(it);
                        Passedarguments.CurrentEntity = Passedarguments.DatasourceName;
                        Passedarguments.Id = 0;
                        Passedarguments.ObjectType = "COPYDEFAULTS";
                        Passedarguments.ObjectName = Passedarguments.DatasourceName;
                        Passedarguments.Objects = ob;
                        Passedarguments.DatasourceName = Passedarguments.DatasourceName;
                        Passedarguments.EventType = "COPYDEFAULTS";

                        //  TreeEditor.args = (PassedArgs)Passedarguments;
                        DMEEditor.Passedarguments = Passedarguments;
                     //   DMEEditor.AddLogMessage("Success", $"Edit Defaults", DateTime.Now, 0, null, Errors.Ok);
                        ExtensionsHelpers.Vismanager.ShowPage("uc_datasourceDefaults", (PassedArgs)Passedarguments);
                    }
                    else
                    {
                        string mes = "Could not get Defaults";
                        DMEEditor.AddLogMessage("Failed", mes, DateTime.Now, -1, mes, Errors.Failed);
                    }
                }


            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error getting defaults ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Name = "CopyDefaults", Caption = "Copy Default", Click = true, iconimage = "copydefaults.png", ObjectType = "Beep", PointType = EnumPointType.DataPoint, Showin = ShowinType.Menu,Key =BeepKeys.C,Alt =true)]
        public IErrorsInfo CopyDefault(IPassedArgs Passedarguments)
        {

            DMEEditor.ErrorObject.Flag = Errors.Ok;
            //  DMEEditor.Logger.WriteLog($"Filling Database Entites ) ");
            try
            {
                ExtensionsHelpers.GetValues(Passedarguments);
                if (ExtensionsHelpers.CurrentBranch == null)
                {
                    return DMEEditor.ErrorObject;
                }
               

                if (ExtensionsHelpers.CurrentBranch.BranchType == EnumPointType.DataPoint)
                {
                   
                    List<DefaultValue> defaults = DMEEditor.ConfigEditor.DataConnections[DMEEditor.ConfigEditor.DataConnections.FindIndex(i => i.ConnectionName == Passedarguments.DatasourceName)].DatasourceDefaults;
                    if (defaults != null)
                    {
                        string[] args = { "CopyDefaults", null, null };
                        List<ObjectItem> ob = new List<ObjectItem>(); ;
                        ObjectItem it = new ObjectItem();
                        it.obj = defaults;
                        it.Name = "Defaults";
                        ob.Add(it);
                        Passedarguments.CurrentEntity = Passedarguments.DatasourceName;
                        Passedarguments.Id = 0;
                        Passedarguments.ObjectType = "COPYDEFAULTS";
                        Passedarguments.ObjectName = Passedarguments.DatasourceName;
                        Passedarguments.Objects = ob;
                        Passedarguments.DatasourceName = Passedarguments.DatasourceName;
                        Passedarguments.EventType = "COPYDEFAULTS";

                        DMEEditor.AddLogMessage("Success", $"Copy Defaults", DateTime.Now, 0, null, Errors.Ok);
                        ExtensionsHelpers.Vismanager.DialogManager.MsgBox("Beep", "Defaults Copied Successfully");
                        DMEEditor.Passedarguments = Passedarguments;
                    }
                    else
                    {
                        string mes = "Could not get Defaults";
                        DMEEditor.AddLogMessage("Failed", mes, DateTime.Now, -1, mes, Errors.Failed);
                    }
                }


            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error getting defaults ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Name = "PasteDefaults", Caption = "Paste Default", Click = true, iconimage = "pastedefaults.png", ObjectType = "Beep", PointType = EnumPointType.DataPoint, Showin = ShowinType.Menu,Key = BeepKeys.V, Alt = true)]
        public IErrorsInfo PasteDefault(IPassedArgs Passedarguments)
        {
            ExtensionsHelpers.GetValues(Passedarguments);
            if (ExtensionsHelpers.CurrentBranch == null)
            {
                return DMEEditor.ErrorObject;
            }
            DMEEditor.ErrorObject.Flag = Errors.Ok;
            //  DMEEditor.Logger.WriteLog($"Filling Database Entites ) ");
            try
            {
                if (DMEEditor.Passedarguments != null)
                {
                    if (DMEEditor.Passedarguments.ObjectType == "COPYDEFAULTS")
                    {
                        if (Passedarguments.Objects.Where(o => o.Name == "Defaults").Any())
                        {
                            List<DefaultValue> defaults = (List<DefaultValue>)Passedarguments.Objects.Where(o => o.Name == "Defaults").FirstOrDefault().obj;
                            DMEEditor.ConfigEditor.DataConnections[DMEEditor.ConfigEditor.DataConnections.FindIndex(i => i.ConnectionName == ExtensionsHelpers.DataSource.DatasourceName)].DatasourceDefaults = defaults;
                            DMEEditor.ConfigEditor.SaveDataconnectionsValues();
                            DMEEditor.AddLogMessage("Success", $"Paste Defaults", DateTime.Now, 0, null, Errors.Ok);
                            ExtensionsHelpers.Vismanager.DialogManager.MsgBox("Beep", "Pasted Defaults Successfully");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error in  pasting Defaults ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }
            return DMEEditor.ErrorObject;

        }
        [CommandAttribute(Name = "WorkFlowEditor", Caption = "WorkFlow Editor", Click = true, iconimage = "workfloweditor.png", ObjectType = "Beep", PointType = EnumPointType.Global, Showin = ShowinType.Menu)]
        public IErrorsInfo WorkFlowEditor(IPassedArgs Passedarguments)
        {

            DMEEditor.ErrorObject.Flag = Errors.Ok;
            //  DMEEditor.Logger.WriteLog($"Filling Database Entites ) ");
            try
            {

                ExtensionsHelpers.GetValues(Passedarguments);
                     //  TreeEditor.args = (PassedArgs)Passedarguments;
                        DMEEditor.Passedarguments = Passedarguments;
                //  DMEEditor.AddLogMessage("Success", $"Edit Defaults", DateTime.Now, 0, null, Errors.Ok);
                ExtensionsHelpers.Vismanager.ShowPage("uc_WorkflowEditor", (PassedArgs)Passedarguments);
            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($"Error getting defaults ({ex.Message}) ");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Name = "AddCategory", Caption = "Add Category", Click = true, iconimage = "category.png", ObjectType = "Beep", PointType = EnumPointType.Root, Showin = ShowinType.Menu,Key = BeepKeys.M, Alt = true)]
        public IErrorsInfo AddCategory(IPassedArgs Passedarguments)
        {

            try
            {
                ExtensionsHelpers.GetValues(Passedarguments);
                if (ExtensionsHelpers.CurrentBranch == null)
                {
                    return DMEEditor.ErrorObject;
                }
                if (ExtensionsHelpers.CurrentBranch.BranchType != EnumPointType.Root)
                {
                    return DMEEditor.ErrorObject;
                }
                IBranch Rootbr = ExtensionsHelpers.RootBranch;
                string foldername = "";
                ExtensionsHelpers.Vismanager.DialogManager.InputBox("Enter Category Name", "What Category you want to Add", ref foldername);
                if (foldername != null)
                {
                    if (foldername.Length > 0)
                    {
                        CategoryFolder x = DMEEditor.ConfigEditor.AddFolderCategory(foldername, Rootbr.BranchClass, Rootbr.BranchText);
                        Rootbr.CreateCategoryNode(x);
                        DMEEditor.ConfigEditor.SaveCategoryFoldersValues();

                    }
                }
                DMEEditor.AddLogMessage("Success", "Added Category", DateTime.Now, 0, null, Errors.Failed);
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Category";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        [CommandAttribute(Name = "RemoveCategory", Caption = "Remove Category", Click = true, iconimage = "removecategory.png", ObjectType = "Beep", PointType = EnumPointType.Category, Showin = ShowinType.Menu,Key = BeepKeys.Delete, Alt = true)]
        public IErrorsInfo RemoveCategoryBranch(IPassedArgs Passedarguments)
        {

            try
            {
                ExtensionsHelpers.GetValues(Passedarguments);
                if (ExtensionsHelpers.CurrentBranch == null)
                {
                    return DMEEditor.ErrorObject;
                }
                if (ExtensionsHelpers.CurrentBranch.BranchType != EnumPointType.Category)
                {
                    return DMEEditor.ErrorObject;
                }
                int id = ExtensionsHelpers.CurrentBranch.ID;
                IBranch CategoryBranch = ExtensionsHelpers.CurrentBranch;
                IBranch RootBranch = ExtensionsHelpers.RootBranch;
                IBranch CategoryBranchNode = (IBranch)ExtensionsHelpers.TreeEditor.GetTreeNodeByID(CategoryBranch.BranchID);
                var ls = ExtensionsHelpers.TreeEditor.Branches.Where(x => x.ParentBranchID == id).ToList();
                if (ls.Count() > 0)
                {
                    foreach (IBranch f in ls)
                    {
                        ExtensionsHelpers.TreeEditor.Treebranchhandler.MoveBranchToParent(RootBranch, f);
                    }
                }

                ExtensionsHelpers.TreeEditor.RemoveNode(CategoryBranch.BranchID);
                CategoryFolder Folder = DMEEditor.ConfigEditor.CategoryFolders.Where(y => y.FolderName == CategoryBranch.BranchText && y.RootName == CategoryBranch.BranchClass).FirstOrDefault();
                DMEEditor.ConfigEditor.CategoryFolders.Remove(Folder);

                DMEEditor.ConfigEditor.SaveCategoryFoldersValues();
                DMEEditor.AddLogMessage("Success", "Removed Branch successfully", DateTime.Now, 0, null, Errors.Ok);

            }
            catch (Exception ex)
            {
                string mes = "";
                DMEEditor.AddLogMessage(ex.Message, "Could not remove category" + mes, DateTime.Now, -1, mes, Errors.Failed);

            };
            return DMEEditor.ErrorObject;

        }
    }
}

