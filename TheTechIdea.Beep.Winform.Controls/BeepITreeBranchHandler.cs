
using System.ComponentModel;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(false)]
    public class BeepITreeBranchHandler : ITreeBranchHandler
    {
        public IDMEEditor DMEEditor { get  ; set  ; }

        public IErrorsInfo AddBranch(IBranch ParentBranch, IBranch Branch)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }

        public IErrorsInfo AddCategory(IBranch Rootbr, string foldername)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }

        public string CheckifBranchExistinCategory(string BranchName, string pRootName)
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo CreateBranch(IBranch Branch)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }

        public IBranch GetBranch(int pID)
        {
            IBranch retval = null;
            try
            {

            }
            catch (Exception ex)
            {

            }
            return retval;
        }

        public IBranch GetBranchByMiscID(int pID)
        {
            IBranch retval = null;
            try
            {

            }
            catch (Exception ex)
            {

            }
            return retval;
        }

        public IErrorsInfo MoveBranchToCategory(IBranch CategoryBranch, IBranch CurrentBranch)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }

        public IErrorsInfo MoveBranchToParent(IBranch ParentBranch, IBranch CurrentBranch)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }

        public IErrorsInfo RemoveBranch(IBranch Branch)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }

        public IErrorsInfo RemoveBranch(int id)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }

        public IErrorsInfo RemoveCategoryBranch(int id)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }

        public IErrorsInfo RemoveChildBranchs(IBranch branch)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }

        public bool RemoveEntityFromCategory(string root, string foldername, string entityname)
        {
            bool retval =false;
            try
            {

            }
            catch (Exception ex)
            {
                retval = false;

            }
            return retval;
        }

        public IErrorsInfo SendActionFromBranchToBranch(IBranch ToBranch, IBranch CurrentBranch, string ActionType)
        {
            IErrorsInfo retval = new ErrorsInfo();
            try
            {

            }
            catch (Exception ex)
            {
                retval.Message = ex.Message;
                retval.Flag = Errors.Failed;

            }
            return retval;
        }
    }
}
