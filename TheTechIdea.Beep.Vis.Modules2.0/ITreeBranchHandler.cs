﻿
using TheTechIdea.Beep;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface ITreeBranchHandler
    {
        IDMEEditor DMEEditor { get; set; }

        IErrorsInfo AddBranch(IBranch ParentBranch, IBranch Branch);
        IErrorsInfo AddCategory(IBranch Rootbr,string foldername);
        string CheckifBranchExistinCategory(string BranchName, string pRootName);
        IErrorsInfo CreateBranch(IBranch Branch);
        IBranch GetBranch(int pID);
        IBranch GetBranchByMiscID(int pID);
        IErrorsInfo MoveBranchToParent(IBranch ParentBranch, IBranch CurrentBranch);
        IErrorsInfo MoveBranchToCategory(IBranch CategoryBranch, IBranch CurrentBranch);
        IErrorsInfo RemoveBranch(IBranch Branch);
        IErrorsInfo RemoveBranch(int id);
        IErrorsInfo RemoveCategoryBranch(int id);
        IErrorsInfo RemoveChildBranchs(IBranch branch);
        bool RemoveEntityFromCategory(string root, string foldername, string entityname);
        IErrorsInfo SendActionFromBranchToBranch(IBranch ToBranch, IBranch CurrentBranch, string ActionType);
    }
}