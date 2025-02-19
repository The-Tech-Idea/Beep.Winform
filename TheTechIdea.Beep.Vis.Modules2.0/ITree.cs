using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Vis.Modules
{
    public interface ITree
    {
        string CategoryIcon { get; set; }
        string SelectIcon { get; set; }
        string TreeType { get; set; }
        string ObjectType { get; set; }
        IBranch CurrentBranch { get; set; }
        IDMEEditor DMEEditor { get; set; }
        List<int> SelectedBranchs { get; set; }
        IBranch SelectedBranch { get; set; }
        PassedArgs args { get; set; }
        int SeqID { get; set; }
        List<IBranch> Branches { get; set; }
        List<Tuple<IBranch, string>> GenerBranchs { get; set;   }
        List<MenuList> Menus { get; set; }
        IAppManager VisManager { get; set; }
        int SelectedBranchID { get; set; }
        void RefreshTree();
        void RefreshTree(IBranch branch);
        void RefreshTree(int branchid);

        void ChangeBranchIcon(int branchid, string iconname);
        void ChangeBranchIcon(string branchname, string iconname);
        void ChangeBranchIcon(IBranch branch, string iconname);
        void ChangeBranchText(int branchid, string text);
        void ChangeBranchText(string branchname, string text);
        void ChangeBranchText(IBranch branch, string text);

        IBranch GetBranchByGuidID(string guidid);
        IBranch GetBranchByEntityGuidID(string guidid);
        IBranch GetBranchByMiscGuidID(string guidid);
        IBranch GetBranch(string text);
        IBranch GetBranch(int id);
        IBranch GetBranch(string text, string parent);
        IBranch GetBranch(string text, EnumPointType branchtype);
        ITreeBranchHandler Treebranchhandler { get; set; }
        IErrorsInfo RunMethod(object branch, string MethodName);
        bool AddBranchToParentInBranchsOnly(IBranch ParentBranch, IBranch br);
        IErrorsInfo AddBranch(IBranch ParentBranch, IBranch br);
        void CreateNode(int id, IBranch br);
        IErrorsInfo CreateRootTree();
        IErrorsInfo CreateFunctionExtensions(MethodsClass item);
        string Filterstring { get; set; }
        void TurnonOffCheckBox(bool val);
        object GetTreeNodeByID(int id);
        void RemoveNode(int id);
        void RemoveNode(IBranch br);
        void RemoveNode(string branchname);
        void RemoveNodeByGuidID(string guidid);

        event EventHandler<IPassedArgs> PreCallModule;
        event EventHandler<IPassedArgs> PreShowItem;
        event EventHandler<IBranch> RefreshBranch;
        event EventHandler<IBranch> RefreshChildBranchs;
        event EventHandler<IBranch> RefreshParentBranch;
        event EventHandler<IBranch> RefreshBranchIcon;
        event EventHandler<IBranch> RefreshBranchText;
        

    }
}
