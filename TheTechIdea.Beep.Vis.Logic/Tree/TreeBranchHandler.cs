using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.DataView;
using TheTechIdea.Beep.Vis.Logic;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Tree;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Addin;

namespace TheTechIdea.Beep.Vis.Tree
{
    public class TreeBranchHandler : ITreeBranchHandler
    {
        public TreeBranchHandler(IBeepService beepService, ITree ptreeControl)
        {
            BeepService = beepService;
            DMEEditor = beepService.DMEEditor;
            VisManager = beepService.vis;
            Tree = ptreeControl;
        }
        public IBeepService BeepService { get; }
        public IVisManager VisManager { get; }
        public IDMEEditor DMEEditor { get ; set ; }
        public ITree Tree { get; private set; }
        public IErrorsInfo AddBranch(IBranch ParentBranch, IBranch Branch)
        {
            try
            {
                AssemblyClassDefinition cls = DMEEditor.ConfigEditor.BranchesClasses.Where(x => x.PackageName == Branch.ToString()).FirstOrDefault()!;
                Branch.Name = cls.PackageName;
                Branch.BranchID = Branch.ID;
                Branch.TreeEditor = Tree;
                Branch.ParentBranchID = ParentBranch.BranchID;
                Branch.Level = ParentBranch.Level + 1;
                Branch.Visutil = ParentBranch.Visutil;
                DMEEditor.AddLogMessage($"AddBranch -{Branch.BranchText}");
                MethodHandler.CreateMenuMethods(Branch);
                MethodHandler.CreateGlobalMenu(Branch);
                Branch.DMEEditor = DMEEditor;
                Tree.Branches.Add(Branch);
                if (!DMEEditor.ConfigEditor.objectTypes.Any(i => i.ObjectType == Branch.BranchClass && i.ObjectName == Branch.BranchType.ToString() + "_" + Branch.BranchClass))
                {
                    DMEEditor.ConfigEditor.objectTypes.Add(new TheTechIdea.Beep.Workflow.ObjectTypes { ObjectType = Branch.BranchClass, ObjectName = Branch.BranchType.ToString() + "_" + Branch.BranchClass });
                }
            }
            catch (Exception ex)
            {
                string mes = "Could not Add Branch to " + ParentBranch.BranchText;
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        public IErrorsInfo AddCategory(IBranch Rootbr, string foldername)
        {
            try
            {
                if (DMEEditor.Passedarguments == null)
                {
                    DMEEditor.Passedarguments = new PassedArgs();
                }
                if (foldername != null)
                {
                    if (foldername.Length > 0)
                    {
                        if (!DMEEditor.ConfigEditor.CategoryFolders.Where(p => p.RootName.Equals(Rootbr.BranchClass, StringComparison.InvariantCultureIgnoreCase) && p.ParentName.Equals(Rootbr.BranchText, StringComparison.InvariantCultureIgnoreCase) && p.FolderName.Equals(foldername, StringComparison.InvariantCultureIgnoreCase)).Any())
                        {
                            CategoryFolder x = DMEEditor.ConfigEditor.AddFolderCategory(foldername, Rootbr.BranchClass, Rootbr.BranchText);
                            Rootbr.CreateCategoryNode(x);
                            DMEEditor.ConfigEditor.SaveCategoryFoldersValues();
                        }
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
        public string CheckifBranchExistinCategory(string BranchName, string pRootName)
        {
            List<CategoryFolder> ls = DMEEditor.ConfigEditor.CategoryFolders.Where(x => x.RootName == pRootName ).ToList();
            foreach (CategoryFolder item in ls)
            {
                foreach (string f in item.items)
                {
                    if (f == BranchName)
                    {
                        return item.FolderName;
                    }
                }
            }
            return null;
        }
        public IErrorsInfo CreateBranch(IBranch Branch)
        {
            Tree.Branches.Add(Branch);
            return DMEEditor.ErrorObject;
        }
        public IBranch GetBranch(int pID)
        {
            return Tree.Branches.Where(c => c.BranchID == pID).FirstOrDefault();
        }
        public IBranch GetBranchByMiscID(int pID)
        {
            return Tree.Branches.Where(c => c.MiscID == pID).FirstOrDefault();
        }
        public IErrorsInfo MoveBranchToCategory(IBranch CategoryBranch, IBranch CurrentBranch)
        {
            try
            {
                IBranch CategoryBranchNode = (IBranch)Tree.GetTreeNodeByID(CategoryBranch.BranchID);
                IBranch ParentBranchNode = (IBranch)Tree.GetTreeNodeByID(CurrentBranch.ParentBranchID);
                IBranch CurrentBranchNode = (IBranch)Tree.GetTreeNodeByID(CurrentBranch.BranchID);
                string currentParentFoelder = CheckifBranchExistinCategory(CurrentBranch.BranchText, CurrentBranch.BranchClass);
                IBranch ParentBranch = (IBranch)ParentBranchNode;
                if (currentParentFoelder != null)
                {

                    RemoveEntityFromCategory(ParentBranch.BranchClass, currentParentFoelder, CurrentBranch.BranchText);
                }
                Tree.Branches.Remove(CurrentBranchNode);
                CategoryFolder CurFodler = DMEEditor.ConfigEditor.CategoryFolders.Where(y => y.RootName == CategoryBranch.BranchClass && y.FolderName == CategoryBranch.BranchText).FirstOrDefault();
                if (CurFodler != null)
                {
                    if (CurFodler.items.Contains(CurrentBranch.BranchText) == false)
                    {
                        CurFodler.items.Remove(CurrentBranch.BranchText);
                    }
                }

                CategoryFolder NewFolder = DMEEditor.ConfigEditor.CategoryFolders.Where(y => y.FolderName == CategoryBranch.BranchText && y.RootName == CategoryBranch.BranchClass).FirstOrDefault();
                if (NewFolder != null)
                {
                    if (NewFolder.items.Contains(CurrentBranch.BranchText) == false)
                    {
                        NewFolder.items.Add(CurrentBranch.BranchText);
                    }
                }
                if ((CategoryBranch.BranchType == TheTechIdea.Beep.Vis.EnumPointType.DataPoint) && (CategoryBranch.BranchClass == "VIEW" && CurrentBranch.BranchClass == "VIEW") && (CategoryBranch.DataSourceName == CurrentBranch.DataSourceName))
                {
                    DataViewDataSource vds = (DataViewDataSource)DMEEditor.GetDataSource(CurrentBranch.DataSourceName);
                    if (vds.Entities[vds.EntityListIndex(CategoryBranch.MiscID)].Id == vds.Entities[vds.EntityListIndex(CurrentBranch.MiscID)].ParentId)
                    {

                    }
                    else
                    {
                        vds.Entities[vds.EntityListIndex(CurrentBranch.MiscID)].ParentId = vds.Entities[vds.EntityListIndex(CategoryBranch.MiscID)].Id;
                    }


                }

                CategoryBranchNode.ChildBranchs.Add(CurrentBranchNode);

                DMEEditor.ConfigEditor.SaveCategoryFoldersValues();


                DMEEditor.AddLogMessage("Success", "Moved Branch successfully", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Moved Branch";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        public IErrorsInfo MoveBranchToParent(IBranch ParentBranch, IBranch CurrentBranch)
        {
            try
            {
               
                string foldername = CheckifBranchExistinCategory(CurrentBranch.BranchText, CurrentBranch.BranchClass);
                if (foldername != null)
                {
                    RemoveEntityFromCategory(ParentBranch.BranchClass, foldername, CurrentBranch.BranchText);
                }
                if (CurrentBranch != null)
                {
                    Tree.Branches.Remove(CurrentBranch);

                }

                CategoryFolder CurFodler = DMEEditor.ConfigEditor.CategoryFolders.Where(y => y.RootName == ParentBranch.BranchClass).FirstOrDefault();
                if (CurFodler != null)
                {
                    if (CurFodler.items.Contains(CurrentBranch.BranchText) == false)
                    {
                        CurFodler.items.Remove(CurrentBranch.BranchText);
                    }
                }

                CategoryFolder NewFolder = DMEEditor.ConfigEditor.CategoryFolders.Where(y => y.FolderName == ParentBranch.BranchText && y.RootName == ParentBranch.BranchClass).FirstOrDefault();
                if (NewFolder != null)
                {
                    if (NewFolder.items.Contains(CurrentBranch.BranchText) == false)
                    {
                        NewFolder.items.Add(CurrentBranch.BranchText);
                    }
                }
                if ((ParentBranch.BranchType == TheTechIdea.Beep.Vis.EnumPointType.DataPoint) && (ParentBranch.BranchClass == "VIEW" && CurrentBranch.BranchClass == "VIEW") && (ParentBranch.DataSourceName == CurrentBranch.DataSourceName))
                {
                    DataViewDataSource vds = (DataViewDataSource)DMEEditor.GetDataSource(CurrentBranch.DataSourceName);
                    if (vds.Entities[vds.EntityListIndex(ParentBranch.MiscID)].Id == vds.Entities[vds.EntityListIndex(CurrentBranch.MiscID)].ParentId)
                    {

                    }
                    else
                    {
                        vds.Entities[vds.EntityListIndex(CurrentBranch.MiscID)].ParentId = vds.Entities[vds.EntityListIndex(ParentBranch.MiscID)].Id;
                    }


                }

                ParentBranch.ChildBranchs.Add(CurrentBranch);

                DMEEditor.ConfigEditor.SaveCategoryFoldersValues();

                DMEEditor.AddLogMessage("Success", "Moved Branch successfully", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not Moved Branch";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        public IErrorsInfo RemoveBranch(IBranch Branch)
        {
            try
            {
                
                string foldername = CheckifBranchExistinCategory(Branch.BranchText, Branch.BranchClass);
                if (foldername != null)
                {
                    RemoveEntityFromCategory(Branch.BranchClass, foldername, Branch.BranchText);
                }
                RemoveChildBranchs(Branch);
                Tree.Branches.Remove(Branch);
                
                // DMEEditor.AddLogMessage("Success", "removed node and childs", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not  remove node and childs";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        public IErrorsInfo RemoveBranch(int id)
        {
            try
            {
                RemoveBranch(Tree.Branches.Where(x => x.BranchID == id).FirstOrDefault());
            }
            catch (Exception ex)
            {
                string mes = "Could not  remove node and childs";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
           
        }
        public IErrorsInfo RemoveCategoryBranch(int id)
        {
            try
            {
                IBranch CategoryBranch = GetBranch(id);
                IBranch RootBranch = GetBranch(CategoryBranch.ParentBranchID);
               
                var ls = Tree.Branches.Where(x => x.ParentBranchID == id).ToList();
                if (ls.Count() > 0)
                {
                    foreach (IBranch f in ls)
                    {
                        MoveBranchToParent(RootBranch, f);
                    }
                }

                Tree.Branches.Remove(CategoryBranch);
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
        public IErrorsInfo RemoveChildBranchs(IBranch branch)
        {
            try
            {
                if (branch.ChildBranchs != null)
                {
                    if (branch.ChildBranchs.Count > 0)
                    {
                        foreach (IBranch item in branch.ChildBranchs)
                        {
                            if (branch.ChildBranchs.Count > 0)
                            {
                                RemoveBranch(item);
                            }
                            if (Tree.SelectedBranchs.Contains(item.BranchID))
                            {
                                Tree.SelectedBranchs.Remove(item.BranchID);
                            }
                            Tree.Branches.Remove(item);
                        }

                        branch.ChildBranchs.Clear();

                    }

                }
                //  DMEEditor.AddLogMessage("Success", "removed childs", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not  remove   childs";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
        public bool RemoveEntityFromCategory(string root, string foldername, string entityname)
        {
           
                try
                {
                    CategoryFolder f = DMEEditor.ConfigEditor.CategoryFolders.Where(x => x.RootName == root && x.FolderName == foldername).FirstOrDefault();
                    if (f != null)
                    {
                        f.items.Remove(entityname);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    string mes = "";
                    DMEEditor.AddLogMessage(ex.Message, "Could not remove entity from category" + mes, DateTime.Now, -1, mes, Errors.Failed);
                    return false;
                };
          
        }
        public IErrorsInfo SendActionFromBranchToBranch(IBranch ToBranch, IBranch CurrentBranch, string ActionType)
        {
            string targetBranchClass = ToBranch.GetType().Name;
            string dragedBranchClass = CurrentBranch.GetType().Name;


            try
            {

                var functionAction = DMEEditor.ConfigEditor.Function2Functions.Where(x => x.FromClass == dragedBranchClass && x.ToClass == targetBranchClass && x.ToMethod == ActionType).FirstOrDefault();
                if (functionAction != null)
                {
                    Tree.RunMethod(ToBranch, ActionType);
                }
                //   DMEEditor.AddLogMessage("Success", "Added Database Connection", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                string mes = "Could not send action to branch";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            };
            return DMEEditor.ErrorObject;
        }
    }
}
