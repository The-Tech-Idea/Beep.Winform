//using TheTechIdea.Beep.Vis.Modules;
//using TheTechIdea.Beep.Vis;
//using TheTechIdea.Beep.Utilities;
//using TheTechIdea.Beep.Editor;
//using TheTechIdea.Beep.Addin;
//using TheTechIdea.Beep.ConfigUtil;

//namespace TheTechIdea.Beep.Winform.Controls.ITrees.FormsTreeView
//{
//    public class TreeNodeEvents:EventArgs
//    {
//        string TreeEvent { get; set; }
//        string TreeOP { get; set; }
//        public TreeNode SelectedNode { get; set; }
//        public TreeNode LastSelectedNode { get; set; }
//        public Color SelectBackColor { get; set; } = Color.Red;

//        public int StartselectBranchID { get; set; } = 0;
//        public int SelectBranchID { get; set; } = 0;
//        public int SelectedBranchID { get; set; } = 0;
//        public TreeNodeEvents(IDMEEditor pDMEEditor, TreeViewControl ptreeControl)
//        {
//            Editor = pDMEEditor;
//            treeControl = ptreeControl;
//            StandardTree = ptreeControl;
//            Treecontrol = ptreeControl;
//            AppManager = Treecontrol.VisManager;
//            TreeV = Treecontrol.TreeV;
//            TreeV.AllowDrop = true;
//            TreeV.NodeMouseClick += TreeView1_NodeMouseClick;
//            TreeV.NodeMouseDoubleClick += TreeView1_NodeMouseDoubleClick;
//            TreeV.AfterCheck += TreeView1_AfterCheck;

//            //   TreeV.DrawNode += TreeV_DrawNode;
//            TreeV.AfterSelect += TreeV_AfterSelect;

//        }
//        public IDMEEditor Editor { get; set; }
//        public TreeViewControl treeControl { get; set; }
//        private ITree StandardTree { get; set; }
//        private TreeViewControl Treecontrol { get; set; }
//        private IAppManager AppManager { get; set; }
//        public System.Windows.Forms.TreeView TreeV { get; set; }
//        private bool IsSelecting = false;

//        #region "Node Handling Functions"

//        private void TreeV_KeyDown(object sender, KeyEventArgs e)
//        {
//            if (e.KeyCode == Keys.LControlKey)
//            {
//                TreeOP = "UnSelect";
//                StartselectBranchID = 0;
//                IBranch br = (IBranch)TreeV.SelectedNode.Tag;
//                int BranchID = br.ID;
//                TreeV.BeginUpdate();
//                if (TreeV.SelectedNode.BackColor == SelectBackColor)
//                {
//                    TreeV.SelectedNode.BackColor = Color.White;
//                    Treecontrol.SelectedBranchs.Remove(BranchID);

//                }
//                else
//                {
//                    TreeV.SelectedNode.BackColor = SelectBackColor;
//                    Treecontrol.SelectedBranchs.Add(BranchID);
//                }
//                TreeV.EndUpdate();

//            }
//            if (e.KeyCode == Keys.LShiftKey) //|| !Startselect
//            {

//                IsSelecting = true;
//                TreeOP = "StartSelect";
//                if (StartselectBranchID == 0)
//                {
//                    StartselectBranchID = SelectedBranchID;
//                }
//                if (SelectedBranchID != StartselectBranchID)
//                {

//                    IBranch startbr = StandardTree.Branches.Where(x => x.BranchID == StartselectBranchID).FirstOrDefault();
//                    IBranch endbr = StandardTree.Branches.Where(x => x.BranchID == SelectedBranchID).FirstOrDefault();
//                    if (startbr != endbr || startbr.ParentBranchID == endbr.ParentBranchID || startbr.BranchClass == endbr.BranchClass)
//                    {

//                        TreeNode startnode;
//                        TreeNode endnode;
//                        bool found = false;

//                        if (SelectedBranchID > StartselectBranchID)
//                        {
//                            startnode = Treecontrol.GetTreeNodeByID(StartselectBranchID, TreeV.Nodes);
//                            endnode = Treecontrol.GetTreeNodeByID(SelectedBranchID, TreeV.Nodes);
//                        }
//                        else
//                        {
//                            startnode = Treecontrol.GetTreeNodeByID(SelectedBranchID, TreeV.Nodes);
//                            endnode = Treecontrol.GetTreeNodeByID(StartselectBranchID, TreeV.Nodes);
//                        }
//                        TreeNode n = startnode;
//                        while (!found)
//                        {
//                            TreeV.BeginUpdate();
//                            n.BackColor = SelectBackColor;
//                            IBranch nbr = (IBranch)n.Tag;
//                            Treecontrol.SelectedBranchs.Add(nbr.ID);
//                            if (n == endnode)
//                            {
//                                found = true;
//                            }
//                            else
//                            {
//                                n = n.NextNode;
//                            }
//                            TreeV.EndUpdate();
//                        }
//                    }
//                }
//            }
//            if (e.KeyCode == Keys.RShiftKey) //|| !Startselect
//            {

//                if (SelectedBranchID != StartselectBranchID)
//                {
//                    TreeOP = "StartSelect";
//                    if (StartselectBranchID == 0)
//                    {
//                        StartselectBranchID = SelectedBranchID;
//                    }
//                    IBranch startbr = StandardTree.Branches.Where(x => x.BranchID == StartselectBranchID).FirstOrDefault();
//                    IBranch endbr = StandardTree.Branches.Where(x => x.BranchID == SelectedBranchID).FirstOrDefault();
//                    if (startbr != endbr || startbr.ParentBranchID == endbr.ParentBranchID || startbr.BranchClass == endbr.BranchClass)
//                    {

//                        TreeNode startnode;
//                        TreeNode endnode;
//                        bool found = false;

//                        if (SelectedBranchID > StartselectBranchID)
//                        {
//                            startnode = Treecontrol.GetTreeNodeByID(StartselectBranchID, TreeV.Nodes);
//                            endnode = Treecontrol.GetTreeNodeByID(SelectedBranchID, TreeV.Nodes);
//                        }
//                        else
//                        {
//                            startnode = Treecontrol.GetTreeNodeByID(SelectedBranchID, TreeV.Nodes);
//                            endnode = Treecontrol.GetTreeNodeByID(StartselectBranchID, TreeV.Nodes);
//                        }
//                        TreeNode n = startnode;
//                        while (!found)
//                        {
//                            TreeV.BeginUpdate();
//                            n.BackColor = Color.White;
//                            IBranch nbr = (IBranch)n.Tag;
//                            StandardTree.SelectedBranchs.Remove(nbr.ID);
//                            if (n == endnode)
//                            {
//                                found = true;
//                            }
//                            else
//                            {
//                                n = n.NextNode;
//                            }
//                            TreeV.EndUpdate();
//                        }

//                    }
//                }


//            }
//        }

//        // Returns the bounds of the specified node, including the region 
//        // occupied by the node label and any node tag displayed.

//        private void TreeV_AfterSelect(object sender, TreeViewEventArgs e)
//        {
//            // Vary the response depending on which TreeViewAction
//            // triggered the event. 
//            //switch (e.Action)
//            //{
//            //    case TreeViewAction.ByKeyboard:
//            LastSelectedNode = e.Node;
//            if (TreeOP != "StartSelect")
//            {
//                IBranch nbr = (IBranch)e.Node.Tag;
//                StartselectBranchID = nbr.ID;
//            }
//            //        break;
//            //    case TreeViewAction.ByMouse:
//            //        LastSelectedNode = e.Node;
//            //        if (TreeOP != "StartSelect")
//            //        {
//            //            StartselectBranchID = Convert.ToInt32(LastSelectedNode.Tag);
//            //        }


//            //        break;
//            //}
//        }
//        public void NodeEvent(TreeNodeMouseClickEventArgs e)
//        {
//            TreeV.SelectedNode = e.Node;
//            IBranch br = (IBranch)e.Node.Tag;
//            int BranchID = br.ID;
//            string BranchText = br.BranchText;

//            SelectedBranchID = BranchID;
//            if (br != null)
//            {
//                if (e.Button == MouseButtons.Left)
//                {

//                    Editor.Passedarguments = new PassedArgs
//                    {
//                        Addin = null,
//                        AddinName = br.BranchText,
//                        AddinType = "",
//                        DMView = null,
//                        Id = BranchID,
//                        CurrentEntity = BranchText,
//                        DataBindingSource = null,
//                        EventType = TreeEvent
//                    };

//                }
//                else
//                {


//                    StartselectBranchID = 0;
//                    TreeOP = "NONE";
//                }

//                if (e.Button == MouseButtons.Right)
//                {
//                    Treecontrol.Nodemenu_MouseClick(e);
//                }

//            }


//        }
//        private void TreeView1_AfterCheck(object sender, TreeViewEventArgs e)
//        {

//            try
//            {
//                IBranch br = (IBranch)e.Node.Tag;

//                CheckNodes(e.Node, e.Node.Checked);
//                if (br.BranchType == EnumPointType.Entity)
//                {

//                    if (e.Node.Checked)
//                    {
//                        Treecontrol.SelectedBranchs.Add(br.BranchID);
//                    }
//                    else
//                        Treecontrol.SelectedBranchs.Remove(br.BranchID);

//                }
//                else
//                {
//                    e.Node.Checked = false;
//                }

//            }
//            catch (Exception ex)
//            {

//                Editor.AddLogMessage("Fail", $"Error in Showing View on StandardTree ({ex.Message}) ", DateTime.Now, 0, null, Errors.Failed);

//            }


//        }
//        private void CheckNodes(TreeNode node, bool check)
//        {
//            try
//            {
//                SetChildrenChecked(node, node.Checked);
//            }
//            catch (Exception ex)
//            {
//                Editor.AddLogMessage("Fail", $"Error in Setting Check for Node StandardTree ({ex.Message}) ", DateTime.Now, 0, null, Errors.Failed);
//            }



//        }
//        private void SetChildrenChecked(TreeNode treeNode, bool checkedState)
//        {
//            foreach (TreeNode item in treeNode.Nodes)
//            {
//                if (item.Checked != checkedState)
//                {

//                    // int vitem = Convert.ToInt32(item.Tag.ToString().Substring(item.Tag.ToString().IndexOf('-') + 1));
//                    item.Checked = checkedState;
//                    IBranch br = (IBranch)item.Tag;
//                    if (item.Checked)
//                    {
//                        Treecontrol.SelectedBranchs.Add(br.ID);
//                    }
//                    else
//                        Treecontrol.SelectedBranchs.Remove(br.ID);
//                }
//                SetChildrenChecked(item, item.Checked);
//            }
//        }
//        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
//        {
//            TreeEvent = "MouseDoubleClick";
//            Nodeclickhandler(e);

//        }
//        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
//        {
//            TreeEvent = "MouseClick";
//            Nodeclickhandler(e);
//        }
//        private void Nodeclickhandler(TreeNodeMouseClickEventArgs e)
//        {
//            SelectedNode = e.Node;
//            IBranch br = (IBranch)e.Node.Tag;
//            SelectedBranchID = br.ID;
//            NodeEvent(e);
//        }
//        #endregion
//    }
//}
