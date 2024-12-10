using System;
using System.Collections.Generic;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;


namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IBranch: IBranchID
    {
        int ID { get; set; }
        bool Visible { get; set; }
        
        IDMEEditor DMEEditor { get; set; }
        IDataSource DataSource { get; set; }
        string DataSourceName { get; set; }
        List<IBranch> ChildBranchs { get; set; }
        IBranch ParentBranch { get;set; }
        ITree TreeEditor { get; set; }
        IVisManager Visutil { get; set; }
        List<string> BranchActions { get; set; }
        EntityStructure EntityStructure { get; set; }
        string ObjectType { get; set; }
        int MiscID { get; set; }
        bool IsDataSourceNode { get; set; }
        IErrorsInfo ExecuteBranchAction(string ActionName);
        IErrorsInfo SetConfig(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename);
        IErrorsInfo CreateChildNodes();
        IErrorsInfo RemoveChildNodes();
        IErrorsInfo MenuItemClicked(string ActionNam);
        IBranch CreateCategoryNode(CategoryFolder p);
        string MenuID { get; set; }



    }
}
