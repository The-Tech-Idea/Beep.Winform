using TheTechIdea.Beep.Vis.Modules;
using System.Drawing;

using TheTechIdea.Beep.Editor;
using System;

namespace TheTechIdea.Beep.Vis.Tree
{
    public class TreeNodeEvents:EventArgs
    {
        string TreeEvent { get; set; }
        string TreeOP { get; set; }
        public IBranch SelectedNode { get; set; }
        public IBranch LastSelectedNode { get; set; }
        public Color SelectBackColor { get; set; } = Color.Red;

        public int StartselectBranchID { get; set; } = 0;
        public int SelectBranchID { get; set; } = 0;
        public int SelectedBranchID { get; set; } = 0;
        public TreeNodeEvents(IDMEEditor pDMEEditor, ITree ptreeControl)
        {
            DMEEditor = pDMEEditor;
            
            Tree = ptreeControl;
            Treecontrol = ptreeControl;
            visManager = Treecontrol.VisManager;
           

        }
        public IDMEEditor DMEEditor { get; set; }
      
        private ITree Tree { get; set; }
        private ITree Treecontrol { get; set; }
        private IVisManager visManager { get; set; }
     
        private bool IsSelecting = false;

        #region "Node Handling Functions"
       
       
        #endregion
    }
}
