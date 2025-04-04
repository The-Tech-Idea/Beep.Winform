﻿
//using TheTechIdea.Beep.Vis.Modules;
//using TheTechIdea.Beep.ConfigUtil;
//using TheTechIdea.Beep.Editor;
//using TheTechIdea.Beep.Addin;

//using TheTechIdea.Beep.DataBase;
//using TheTechIdea.Beep.Vis;
//using TheTechIdea.Beep.Utilities;
//using Point = System.Drawing.Point;

//namespace TheTechIdea.Beep.Winform.Controls.ITrees.FormsTreeView
//{
//    public class TreeNodeDragandDropHandler
//    {
//        public TreeNodeDragandDropHandler(IDMEEditor pDMEEditor, TreeViewControl ptreeControl)
//        {
//            Editor = pDMEEditor;
//            treeControl = ptreeControl;
//            TreeV = ptreeControl.TreeV;


//            TreeV.DragDrop += Tree_DragDrop;
//            TreeV.DragEnter += Tree_DragEnter;
//            TreeV.DragLeave += Tree_DragLeave;
//            TreeV.ItemDrag += Tree_ItemDrag;
//            TreeV.DragOver += Tree_DragOver;

//        }
//        public IDMEEditor Editor { get; set; }
//        public TreeViewControl treeControl { get; set; }

//        public System.Windows.Forms.TreeView TreeV { get; set; }
//        #region "Drag and Drop"
//        // Determine whether one node is a parent 
//        // or ancestor of a second node.
//        private bool ContainsNode(TreeNode node1, TreeNode node2)
//        {
//            // Check the parent node of the second node.
//            if (node2.Parent == null) return false;
//            if (node2.Parent.Equals(node1)) return true;

//            // If the parent node !=null or equal to the first node, 
//            // call the ContainsNode method recursively using the parent of 
//            // the second node.
//            return ContainsNode(node1, node2.Parent);
//        }
//        //------------ Drag and Drop -----------------
//        private void Tree_DragLeave(object sender, EventArgs e)
//        {
//            // null;  //throw new NotImplementedException();

//        }

//        private void Tree_DragEnter(object sender, DragEventArgs e)
//        {
//            e.Effect = e.AllowedEffect;
//        }

//        private void Tree_DragDrop(object sender, DragEventArgs e)
//        {
//            // Retrieve the client coordinates of the drop location.
//            Point targetPoint = TreeV.PointToClient(new Point(e.X, e.Y));

//            // Retrieve the node at the drop location.
//            TreeNode targetNode = TreeV.GetNodeAt(targetPoint);
//            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
//            IBranch dragedBranch = (IBranch)draggedNode.Tag;
//            if (targetNode != null)
//            {
//                if (!e.Data.GetDataPresent(typeof(TreeNode)))
//                {
//                    return;
//                }
//                IBranch targetBranch = (IBranch)targetNode.Tag;
//                // Retrieve the node that was dragged.


//                string targetBranchClass = targetBranch.GetType().Name;
//                string dragedBranchClass = dragedBranch.GetType().Name;
//                Function2FunctionAction functionAction = Editor.ConfigEditor.Function2Functions.Where(x => x.FromClass == dragedBranchClass && x.ToClass == targetBranchClass && x.Event == "DragandDrop").FirstOrDefault();
//                //---------------------------------------------------------
//                if (targetBranch.BranchClass == dragedBranch.BranchClass)
//                {
//                    switch (targetBranch.BranchType)
//                    {
//                        case EnumPointType.Root:
//                            if (treeControl.Treebranchhandler.CheckifBranchExistinCategory(dragedBranch.BranchText, dragedBranch.BranchClass) != null)
//                            {
//                                if (dragedBranch.BranchType == EnumPointType.DataPoint)
//                                {
//                                    treeControl.Treebranchhandler.MoveBranchToParent(targetBranch, dragedBranch);
//                                }
//                            }

//                            break;


//                        case EnumPointType.Category:
//                            //  if (dragedBranch.BranchClass == "VIEW")
//                            // {
//                            if (dragedBranch.BranchType == EnumPointType.DataPoint)
//                            {
//                                treeControl.Treebranchhandler.MoveBranchToCategory(targetBranch, dragedBranch);
//                            };
//                            // };
//                            break;
//                        case EnumPointType.DataPoint:
//                        case EnumPointType.Entity:
//                            if (dragedBranch.BranchClass == "VIEW")
//                            {
//                                if (dragedBranch.BranchType == EnumPointType.Entity && dragedBranch.DataSourceName == targetBranch.DataSourceName)
//                                {
//                                    treeControl.Treebranchhandler.MoveBranchToParent(targetBranch, dragedBranch);
//                                }
//                            }


//                            break;
//                        default:
//                            break;
//                    }

//                }
//                // Check if Function connected to this event
//                if (functionAction != null) //functionAction
//                {
//                    switch (targetBranch.BranchType)
//                    {
//                        case EnumPointType.Root:
//                            Editor.Passedarguments = new PassedArgs
//                            {
//                                ObjectName = "DATABASE",
//                                ObjectType = "TABLE",
//                                EventType = "DragandDrop",
//                                ParameterString1 = "Create View using Table",
//                                Objects = new List<ObjectItem> { new ObjectItem { Name = "Branch", obj = dragedBranch } }
//                            };



//                            SendActionFromBranchToBranch(targetBranch, dragedBranch, functionAction.ToMethod);
//                            break;
//                        case EnumPointType.DataPoint:
//                            Editor.Passedarguments = new PassedArgs
//                            {
//                                ObjectName = "DATABASE",
//                                ObjectType = "TABLE",
//                                EventType = "DragandDrop",
//                                ParameterString1 = "Add Entity Child",
//                                DataBindingSource = dragedBranch.DataBindingSource,
//                                DatasourceName = dragedBranch.DataSourceName,
//                                CurrentEntity = dragedBranch.BranchText,
//                                Id = dragedBranch.BranchID,
//                                Objects = new List<ObjectItem> { new ObjectItem { Name = "ChildBranch", obj = dragedBranch } }
//                            };
//                            SendActionFromBranchToBranch(targetBranch, dragedBranch, functionAction.ToMethod);
//                            break;
//                        case EnumPointType.Category:
//                            if (dragedBranch.BranchType == EnumPointType.DataPoint)
//                            {
//                                treeControl.Treebranchhandler.MoveBranchToParent(targetBranch, dragedBranch);
//                            }

//                            break;
//                        case EnumPointType.Entity:
//                            IDataSource ds = Editor.GetDataSource(dragedBranch.DataSourceName);
//                            EntityStructure ent = ds.GetEntityStructure(dragedBranch.BranchText, true);
//                            Editor.Passedarguments = new PassedArgs
//                            {
//                                ObjectName = "DATABASE",
//                                ObjectType = "TABLE",
//                                EventType = "COPYENTITY",
//                                ParameterString1 = "COPYENTITY",

//                                DataBindingSource = dragedBranch.DataBindingSource,
//                                DatasourceName = dragedBranch.DataSourceName,
//                                CurrentEntity = dragedBranch.BranchText,
//                                Id = dragedBranch.BranchID,
//                                Objects = new List<ObjectItem> { new ObjectItem { Name = "Branch", obj = dragedBranch }, new ObjectItem { Name = "Entity", obj = ent } }
//                            };



//                            SendActionFromBranchToBranch(targetBranch, dragedBranch, functionAction.ToMethod);


//                            break;
//                        default:
//                            break;
//                    }
//                }
//            }


//            //if (targetBranch.BranchType == EnumBranchType.Root)
//            //{
//            //    // Confirm that the node at the drop location is not 
//            //    // the dragged node or a descendant of the dragged node.
//            //    IDMDataView v = DME.viewEditor.GetView(Visutil.GetNodeID(targetNode).NodeIndex);
//            //    IRDBSource ds = (IRDBSource)DME.GetDataSource(v.MainDataSourceID);
//            //    if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
//            //    {
//            //        // If it is a move operation, remove the node from its current 
//            //        // location and add it to the node at the drop location.
//            //        if (e.Effect == DragDropEffects.Move)
//            //        {

//            //            int tabid = DME.viewEditor.AddEntitytoDataView(ds, draggedNode.Text.ToUpper(),ds.GetSchemaName(), "", v.id);
//            //            //  Visutil.ShowTableonTree(MainNode, v.id, tabid, true);

//            //            //draggedNode.Remove();
//            //            //targetNode.NodesControls.Add(draggedNode);
//            //        }

//            //        // If it is a copy operation, clone the dragged node 
//            //        // and add it to the node at the drop location.
//            //        //else if (e.Effect == DragDropEffects.Copy)
//            //        //{
//            //        //    targetNode.NodesControls.Add((TreeNode)draggedNode.Clone());
//            //        //}

//            //        // Expand the node at the location 
//            //        // to show the dropped node.
//            //        targetNode.Expand();
//            //    }
//            //}

//        }
//        public IErrorsInfo SendActionFromBranchToBranch(IBranch ParentBranch, IBranch CurrentBranch, string ActionType)
//        {
//            return Editor.ErrorObject;
//        }
//        private void Tree_ItemDrag(object sender, ItemDragEventArgs e)

//        {
//            //if (CurrentNode != null)
//            //{counties.xls,counties
//            //    if (CurrentNode.nodeType == "EN")
//            //    {
//            // Move the dragged node when the left mouse button is used.
//            IDataObject x = new DataObject();

//            TreeNode n = (TreeNode)e.Item;
//            if (e.Button == MouseButtons.Left)
//            {
//                IBranch branch = (IBranch)n.Tag;
//                x.SetData(branch);
//                switch (branch.BranchType)
//                {
//                    case EnumPointType.Root:
//                        break;
//                    case EnumPointType.DataPoint:
//                        TreeV.DoDragDrop(e.Item, DragDropEffects.Move);
//                        break;
//                    case EnumPointType.Category:
//                        break;
//                    case EnumPointType.Entity:
//                        TreeV.DoDragDrop(e.Item, DragDropEffects.Move);
//                        break;
//                    default:
//                        break;
//                }
//            }




//            // Copy the dragged node when the right mouse button is used.
//            //else if (e.Button == MouseButtons.Right)
//            //{
//            //    StandardTree.DoDragDrop(e.Item, DragDropEffects.Copy);
//            //}
//            //  }

//            //}


//        }

//        private void Tree_DragOver(object sender, DragEventArgs e)
//        {
//            // Retrieve the client coordinates of the mouse position.
//            Point targetPoint = TreeV.PointToClient(new Point(e.X, e.Y));

//            // Select the node at the mouse position.
//            TreeV.SelectedNode = TreeV.GetNodeAt(targetPoint);
//        }
//        #endregion

//    }
//}
