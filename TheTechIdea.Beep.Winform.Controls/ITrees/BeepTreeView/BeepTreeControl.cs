using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;


namespace TheTechIdea.Beep.Winform.Controls.ITrees.BeepTreeView
{
    public partial class BeepTreeControl : BeepTree,ITree
    {
        public BeepTreeControl(IBeepService service)
        {
            BeepService = service;
            DMEEditor = service.DMEEditor;
            VisManager = service.vis;
            treeBranchHandler = new BeepTreeBranchHandler(service, this);
            DropHandler = new BeepTreeNodeDragandDropHandler(service, this);
        }
        #region "Properties"
        public string CategoryIcon { get; set; }= "Category.svg";
        public string SelectIcon { get; set; } = "Select.svg";
        public string TreeType { get; set; } = "Beep";
        public IBranch CurrentBranch { get; set; }
        public IBeepService BeepService { get; }
        public IDMEEditor DMEEditor { get; set; }
        public List<int> SelectedBranchs { get; set; } = new List<int>();
        public PassedArgs args { get; set; }
        public int SeqID { get; set; }
        public List<IBranch> Branches { get; set; }=new List<IBranch>();
        public List<Tuple<IBranch, string>> GenerBranchs { get; set; } = new List<Tuple<IBranch, string>>();
        public List<MenuList> Menus { get; set; } = new List<MenuList>();
        public IVisManager VisManager { get; set; }
        public int SelectedBranchID { get; set; }
        public string Filterstring { get; set; }
        #endregion "Properties"
        #region "Events"
        public event EventHandler<IPassedArgs> PreCallModule;
        public event EventHandler<IPassedArgs> PreShowItem;
        public event EventHandler<IBranch> RefreshBranch;
        public event EventHandler<IBranch> RefreshChildBranchs;
        public event EventHandler<IBranch> RefreshParentBranch;
        public event EventHandler<IBranch> RefreshBranchIcon;
        public event EventHandler<IBranch> RefreshBranchText;
        #endregion "Events"
        #region "Handlers"
        public IErrorsInfo RunMethod(object branch, string MethodName)
        {
            try
            {
                ControlExtensions.RunMethodFromBranch(this, branch, MethodName);

            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", ex.Message, DateTime.Now, 0, "", Errors.Failed);
            }
            return DMEEditor.ErrorObject;
           
        }
        public ITreeBranchHandler treeBranchHandler { get; set; }
        public BeepTreeNodeDragandDropHandler DropHandler { get; }

        #endregion "Handlers"
        #region "Change Branch Properties"
        public void ChangeBranchIcon(int branchid, string iconname)
        {
            
        }

        public void ChangeBranchIcon(string branchname, string iconname)
        {
            throw new NotImplementedException();
        }

        public void ChangeBranchIcon(IBranch branch, string iconname)
        {
            throw new NotImplementedException();
        }

        public void ChangeBranchText(int branchid, string text)
        {
            throw new NotImplementedException();
        }

        public void ChangeBranchText(string branchname, string text)
        {
            throw new NotImplementedException();
        }

        public void ChangeBranchText(IBranch branch, string text)
        {
            throw new NotImplementedException();
        }
        public void RemoveNode(int id)
        {
            throw new NotImplementedException();
        }
        public IErrorsInfo TurnonOffCheckBox(IPassedArgs Passedarguments)
        {
            throw new NotImplementedException();
        }
        #endregion "Change Branch Properties"
        #region "Create Branches"
        public IErrorsInfo CreateFunctionExtensions(MethodsClass item)
        {
            try
            {
                ControlExtensions.CreateFunctionExtensions(this,item);

            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", ex.Message, DateTime.Now, 0, "", Errors.Failed);
            }
            return DMEEditor.ErrorObject;
           
        }
        public IErrorsInfo CreateRootTree()
        {

            try
            {
                ControlExtensions.GetBranchs(this, DMEEditor);

            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", ex.Message, DateTime.Now, 0, "", Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }
        #endregion "Create Branches"
        #region "Get Branches"
        public IBranch GetBranchByEntityGuidID(string guidid)
        {
            return Branches.Where(c => c.EntityGuidID == guidid).FirstOrDefault();
        }

        public IBranch GetBranchByGuidID(string guidid)
        {
            return Branches.Where(c => c.GuidID == guidid).FirstOrDefault();
        }

        public IBranch GetBranchByMiscGuidID(string guidid)
        {
           return Branches.Where(c => c.MiscStringID == guidid).FirstOrDefault();
        }

        public object GetTreeNodeByID(int id)
        {
            return Branches.Where(c => c.ID == id).FirstOrDefault();
        }
        #endregion "Get Branches"
        #region "Refresh Tree"
        public void RefreshImageList()
        {
            throw new NotImplementedException();
        }

        public void RefreshTree()
        {
            throw new NotImplementedException();
        }

        public void RefreshTree(IBranch branch)
        {
            throw new NotImplementedException();
        }

        public void RefreshTree(int branchid)
        {
            throw new NotImplementedException();
        }

        public void RefreshTree(string branchname)
        {
            throw new NotImplementedException();
        }
        #endregion "Refresh Tree"

      
    }
}
