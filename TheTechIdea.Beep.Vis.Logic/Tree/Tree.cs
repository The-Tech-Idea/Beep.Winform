using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Vis.Tree;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Vis.Logic.Tree
{
    public class Tree : ITree
    {
        public Tree(IBeepService beepService)
        {
            BeepService = beepService;
            DMEEditor = beepService.DMEEditor;
            VisManager= beepService.vis;
            Treebranchhandler = new TreeBranchHandler(beepService,this);
            DropHandler= new TreeNodeDragandDropHandler(beepService, this);
        }
        #region "Other Properties"
        public IBeepService BeepService { get; }
        TreeNodeDragandDropHandler DropHandler;
        bool isTreeCreate = false;

        #endregion
        #region "Properties"
        public List<MenuList> Menus { get; set; } = new List<MenuList>();
        public List<Tuple<IBranch, string>> GenerBranchs { get; set; } = new List<Tuple<IBranch, string>>();
        public string CategoryIcon { get; set; }="Category.png";
        public string SelectIcon { get; set; }="Select.png";
        public string TreeType { get; set; }="Beep";
        public IBranch CurrentBranch { get; set; }
        public IDMEEditor DMEEditor { get; set; }
        public string ObjectType { get; set; }= "Beep";
        public List<int> SelectedBranchs { get; set; } = new List<int>();
        public PassedArgs args { get; set; }
        int _Seqid = 0;
        public int SeqID { get { return _Seqid++; } set { } }
        public List<IBranch> Branches { get; set; } = new List<IBranch>();
        public IVisHelper visHelper { get; set; }
        public IVisManager VisManager { get; set; }
        public int SelectedBranchID { get; set; }
        public ITreeBranchHandler Treebranchhandler { get; set; }
        public string Filterstring { set; get; }
        #endregion
        #region "Events"
        public event EventHandler<IBranch> RefreshBranch;
        public event EventHandler<IBranch> RefreshChildBranchs;
        public event EventHandler<IBranch> RefreshParentBranch;
        public event EventHandler<IBranch> RefreshBranchIcon;
        public event EventHandler<IBranch> RefreshBranchText;
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;
        #endregion
        #region "Methods"
        public IErrorsInfo CreateFunctionExtensions(MethodsClass item)
        {
            try
            {
                MethodHandler.CreateFunctionExtensions(this,item);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep", $"Error Creating Root Tree {ex.Message} ", DateTime.Now, -1, "", Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo CreateRootTree()
        {
            try
            {
                MethodHandler.CreateRootTree(this);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep",$"Error Creating Root Tree {ex.Message} ", DateTime.Now, -1, "", Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        public object GetTreeNodeByID(int id)
        {
           return Branches.FirstOrDefault(CurrentBranch => CurrentBranch.ID == id   );
        }
      
        public void RefreshImageList()
        {
            try
            {
               
               // MethodHandler.RunMethodFromBranch((IBranch)branch, MethodName);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep", $"Error refersh Image List    {ex.Message} ", DateTime.Now, -1, "", Errors.Failed);
            }
            
        }
        public void RefreshTree()
        {
           CreateRootTree();
        }
        public void RefreshTree(IBranch branch)
        {
           int idx=Branches.FindIndex(x => x.ID == branch.ID);
            if (idx > -1)
            {
                Branches[idx] = branch;
            }
        }
        public void RefreshTree(int branchid)
        {
            int idx = Branches.FindIndex(x => x.ID == branchid);
            if (idx > -1)
            {
               
            }
        }
        public void RefreshTree(string branchname)
        {
            int idx = Branches.FindIndex(x => x.BranchText == branchname);
            if (idx > -1)
            {

            }
        }
        public void RemoveNode(int id)
        {
            int idx = Branches.FindIndex(x => x.ID == id);
            if (idx > -1)
            {
                Branches.RemoveAt(idx);
            }
        }
        public IErrorsInfo RunMethod(object branch, string MethodName)
        {
            try
            {
                MethodHandler.RunMethodFromBranch((IBranch)branch, MethodName);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Beep", $"Error Running Method  {MethodName}- {ex.Message} ", DateTime.Now, -1, "", Errors.Failed);
            }
            return DMEEditor.ErrorObject;
           
        }
        public IErrorsInfo TurnonOffCheckBox(IPassedArgs Passedarguments)
        {
            throw new NotImplementedException();
        }
        public void ChangeBranchIcon(int branchid, string iconname)
        {
            IBranch br = Branches.Where(p => p.ID == branchid).FirstOrDefault();
            if (br != null)
            {
                ChangeBranchIcon(br, iconname);
            }
        }
        public void ChangeBranchIcon(string branchname, string iconname)
        {
            IBranch br = Branches.Where(p => p.BranchText == branchname).FirstOrDefault();
            if (br != null)
            {
                ChangeBranchIcon(br, iconname);
            }
        }
        public void ChangeBranchIcon(IBranch branch, string iconname)
        {
           
            
        }
        public void ChangeBranchText(int branchid, string text)
        {
            IBranch br = Branches.Where(p => p.ID == branchid).FirstOrDefault();
            if (br != null)
            {
                ChangeBranchText(br, text);
            }
        }
        public void ChangeBranchText(string branchname, string text)
        {
            IBranch br = Branches.Where(p => p.BranchText == branchname).FirstOrDefault();
            if (br != null)
            {
                ChangeBranchText(br, text);
            }
        }
        public void ChangeBranchText(IBranch branch, string text)
        {
            throw new NotImplementedException();
        }
        public IBranch GetBranchByGuidID(string guidid)
        {
            int idx = Branches.FindIndex(x => x.GuidID == guidid);
            if (idx > -1)
            {
                return Branches[idx];
            }
            else
            {
                return null;
            }
        }
        public IBranch GetBranchByEntityGuidID(string guidid)
        {
            int idx = Branches.FindIndex(x => x.EntityGuidID == guidid);
            if (idx > -1)
            {
                return Branches[idx];
            }
            else
            {
                return null;
            }
        }
        public IBranch GetBranchByMiscGuidID(string guidid)
        {
            int idx = Branches.FindIndex(x => x.MiscStringID == guidid);
            if (idx > -1)
            {
                return Branches[idx];
            }
            else
            {
                return null;
            }
        }

        public bool AddBranchToParentInBranchsOnly(IBranch ParentBranch, IBranch br)
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo AddBranch(IBranch ParentBranch, IBranch br)
        {
            throw new NotImplementedException();
        }

        public void CreateNode(int id, IBranch br)
        {
            throw new NotImplementedException();
        }

        public void RemoveNode(IBranch br)
        {
            throw new NotImplementedException();
        }

        public void RemoveNode(string branchname)
        {
            throw new NotImplementedException();
        }

        public void RemoveNodeByGuidID(string guidid)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
