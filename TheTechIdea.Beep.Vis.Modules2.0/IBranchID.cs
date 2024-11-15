using System;
using System.Collections.Generic;
using System.Text;
using TheTechIdea.Beep.Vis;

namespace TheTechIdea.Beep.Vis.Modules
{
    public interface IBranchID
    {
        string GuidID { get; set; }
        string ParentGuidID { get; set; }
        string DataSourceConnectionGuidID { get; set; } 
        string EntityGuidID { get; set; }   
        string MiscStringID { get; set; }   
        string Name { get; set; }
        string BranchText { get; set; }
        int Level { get; set; }
        EnumPointType BranchType { get; set; }
        int BranchID { get; set; }
        string IconImageName { get; set; }
        string BranchStatus { get; set; }
        int ParentBranchID { get; set; }
        string BranchDescription { get; set; }
        string BranchClass { get; set; }
    }
}
