using System.ComponentModel;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Vis.Modules;
using Timer = System.Windows.Forms.Timer;
using System.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep StandardTree")]
    [Category("Beep Controls")]
    [Description("A control that displays hierarchical data in a tree format.")]
    public class BeepTree : BeepControl
    {
        #region "Events"

        public EventHandler<BeepMouseEventArgs> LeftButtonClicked;
        public EventHandler<BeepMouseEventArgs> RighButtonClicked;
        public EventHandler<BeepMouseEventArgs> MiddleButtonClicked;
        public EventHandler<BeepMouseEventArgs> NodeDoubleClicked;
        public EventHandler<BeepMouseEventArgs> NodeSelected;
        public EventHandler<BeepMouseEventArgs> NodeDeselected;
        public EventHandler<BeepMouseEventArgs> NodeExpanded;
        public EventHandler<BeepMouseEventArgs> NodeCollapsed;
        public EventHandler<BeepMouseEventArgs> NodeChecked;
        public EventHandler<BeepMouseEventArgs> NodeUnchecked;
        public EventHandler<BeepMouseEventArgs> NodeVisible;
        public EventHandler<BeepMouseEventArgs> NodeInvisible;
        public EventHandler<BeepMouseEventArgs> NodeReadOnly;
        public EventHandler<BeepMouseEventArgs> NodeEditable;
        public EventHandler<BeepMouseEventArgs> NodeDeletable;
        public EventHandler<BeepMouseEventArgs> NodeDirty;
        public EventHandler<BeepMouseEventArgs> NodeModified;
        public EventHandler<BeepMouseEventArgs> NodeDeleted;
        public EventHandler<BeepMouseEventArgs> NodeAdded;
        public EventHandler<BeepMouseEventArgs> NodeExpandedAll;
        public EventHandler<BeepMouseEventArgs> NodeCollapsedAll;
        public EventHandler<BeepMouseEventArgs> NodeCheckedAll;
        public EventHandler<BeepMouseEventArgs> NodeUncheckedAll;
        public EventHandler<BeepMouseEventArgs> NodeVisibleAll;
        public EventHandler<BeepMouseEventArgs> NodeInvisibleAll;
        public EventHandler<BeepMouseEventArgs> NodeRightClicked;
        public EventHandler<BeepMouseEventArgs> NodeLeftClicked;
        public EventHandler<BeepMouseEventArgs> NodeMiddleClicked;
        public EventHandler<BeepMouseEventArgs> NodeMouseEnter;
        public EventHandler<BeepMouseEventArgs> NodeMouseLeave;
        public EventHandler<BeepMouseEventArgs> NodeMouseHover;
        public EventHandler<BeepMouseEventArgs> NodeMouseWheel;
        public EventHandler<BeepMouseEventArgs> NodeMouseUp;
        public EventHandler<BeepMouseEventArgs> NodeMouseDown;
        public EventHandler<BeepMouseEventArgs> NodeMouseMove;
        public EventHandler<SelectedItemChangedEventArgs> MenuItemSelected;
        public EventHandler<BeepMouseEventArgs> ShowMenu;
        #endregion "Events"
        #region "Popup List Properties"
   

        private int depth;
        private bool _dontRearrange;

        private BeepTreeNode _lastSelectedNode = null;
        public int Nodeseq { get; set; } = 0;
        private bool _isUpdatingTree = false;
        // Define the number of nodes to keep rendered above and below the viewport
        private const int BufferSize = 20;

        // Keep track of visible nodes
        private List<BeepTreeNode> visibleNodes = new List<BeepTreeNode>();
        public int NodeWidth { get; set; } = 100;
        private List<SimpleItem> rootnodeitems = new List<SimpleItem>();
        private List<BeepTreeNode> _beeptreeRootnodes = new List<BeepTreeNode>();
        private Dictionary<int, Panel> _nodePanels = new Dictionary<int, Panel>();
        private bool _shownodeimage = true;
        private int defaultHeight = 100;

        private int _nodeHeight = 20;


        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }
        private bool _useScaledfont = false;
        [Browsable(true)]
        [Category("Appearance")]
        public bool UseScaledFont
        {
            get => _useScaledfont;
            set
            {
                _useScaledfont = value;
                ApplyTheme();
                Invalidate();  // Trigger repaint
            }
        }
        private Font _textFont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {

                _textFont = value;
                UseThemeFont = false;
                Font = value;
                ChangeFontSettings();
                Invalidate();
            }
        }
        private bool _useThemeFont = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("If true, the label's font is always set to the theme font.")]
        public new bool UseThemeFont
        {
            get => _useThemeFont;
            set
            {
                _useThemeFont = value;
                ApplyTheme();
                Invalidate();
            }
        }
        private ContentAlignment textAlign= ContentAlignment.MiddleLeft;

        [Browsable(true)]
        [Category("Appearance")]
        public ContentAlignment TextAlignment
        {
            get => textAlign;
            set
            {
                textAlign = value;
                ChangeTextAlignment();
                Invalidate();
            }
        }
      
        public BindingList<SimpleItem> CurrentMenutems { get; set; }
       
       
        #endregion "Popup List Properties"
        #region "Properties"
        #region "Nodes State"

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeMenuShown { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeClicked { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeRightClicked { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeSelected { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeDragged { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeDropped { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeDraggedOver { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeDraggedEnter { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeDraggedLeave { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeDraggedDrop { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeDraggedOverLeave { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeDraggedOverEnter { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeDraggedOverDrop { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTreeNode LastNodeDraggedOverLeaveEnter { get; set; }

        #endregion "Nodes State"

        public SimpleItem SelectedItem { get;  set; }
        private BeepFlyoutMenu BeepFlyoutMenu;

        //private Dictionary<string, SimpleItem> _menus = new Dictionary<string, SimpleItem>();
        //[Browsable(true)]
        //[Localizable(true)]
        //[MergableProperty(false)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        //public Dictionary<string, SimpleItem> ContextMenu
        //{
        //    get => _menus;
        //    set
        //    {
        //        _menus = value;
        //    }
        //}
        private int nodeimagesize = 20;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int NodeImageSize
        {
            get => nodeimagesize;
            set
            {
                nodeimagesize = value;
                ChangeNodeImageSettings();
            }
        }
      

        public int NodeHeight
        {
            get => _nodeHeight;
            set
            {
                if (_nodeHeight != value)
                {
                    _nodeHeight = value;
                    RearrangeTree(); // Trigger rearrangement
                }
            }
        }
        public bool AllowMultiSelect { get; set; }

      
        public List<BeepTreeNode> SelectedNodes { get; private set; }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BeepTreeNode SelectedNode
        {
            get
            {
                if (!AllowMultiSelect)
                {
                    // Return the only selected node or null if none
                    return SelectedNodes.Count > 0 ? SelectedNodes[0] : null;
                }
                else
                {
                    // Return the last clicked node that caused selection changes
                    return _lastSelectedNode;
                }
            }
            set
            {
                if (_lastSelectedNode != value)
                {
                    if (_lastSelectedNode != null)
                        _lastSelectedNode.IsSelected = false;  // Deselect previous node

                    _lastSelectedNode = value;

                    if (_lastSelectedNode != null)
                    {
                        _lastSelectedNode.IsSelected = true;
                        _lastSelectedNode.EnsureVisible();
                    }
                }
            }
        }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowNodeImage
        {
            get { return _shownodeimage; }
            set { _shownodeimage = value; ChangeNodeImageSettings(); }
        }
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        //  [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<SimpleItem> Nodes
        {
            get => rootnodeitems;
            set
            {
                rootnodeitems = value;
                InitializeTreeFromMenuItems();
            }
        }
      
        protected List<BeepTreeNode> NodesControls
        {
            get => _beeptreeRootnodes;
            set
            {
                _beeptreeRootnodes = value;
            }
        }
        public bool ShowCheckBox
        {
            get
            {
                return _beeptreeRootnodes.Any(n => n.ShowCheckBox);
            }
            set
            {
                SetAllNodesCheckBoxVisibility(value);
            }
        }

        public BeepTreeNode? ClickedNode { get;  set; }
        #endregion "Properties"
        #region "Constructors"
        public BeepTree()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            if (rootnodeitems == null)
            {
                rootnodeitems = new List<SimpleItem>();
            }

            _beeptreeRootnodes = new List<BeepTreeNode>();
            //rootnodeitems.ListChanged -= Items_ListChanged;
            //rootnodeitems.ListChanged += Items_ListChanged;

            ApplyThemeToChilds = false;
            InitLayout();
            // Enable double buffering to reduce flickering
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            Padding = new Padding(1);
            // Initialize scroll event handling for virtualization
            this.AutoScroll = true;
            this.VerticalScroll.Visible = true;
            SelectedNodes = new List<BeepTreeNode>();
           
        }
        protected override void InitLayout()
        {
            base.InitLayout();
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 200;
                Height = defaultHeight;
            }
            InitializeTreeFromMenuItems();
            RearrangeTree();
            
        }
        #endregion "Constructors"
        #region "Node Clicked"
        private void OnNodeRightClicked(object sender, BeepMouseEventArgs e)
        {
            ClickedNode = sender as BeepTreeNode;
            if (ClickedNode == null) return;
            NodeRightClicked?.Invoke(sender, e);
         
          
            SelectedItem = GetNodeByGuidID(ClickedNode.GuidID);
            var a = DynamicMenuManager.GetMenuItemsList(ClickedNode.NodeInfo);
            if (a == null) return;
            CurrentMenutems = new BindingList<SimpleItem>(a);
            if (CurrentMenutems.Count > 0)
            {

                TogglePopup();

            }

        }
        private void OnNodeDoubleClicked(object sender, BeepMouseEventArgs e)
        {
            ClickedNode = sender as BeepTreeNode;
            if (ClickedNode == null) return;
            CloseOpenPopupMenu();
            NodeDoubleClicked?.Invoke(sender, e);
        }
        private void OnNodeExpanded(object sender, BeepMouseEventArgs e)
        {
            NodeExpanded?.Invoke(sender, e);
        }
        private void OnNodeCollapsed(object sender, BeepMouseEventArgs e)
        {
            NodeCollapsed?.Invoke(sender, e);
        }
        private void OnNodeDeselected(object? sender, BeepMouseEventArgs e)
        {
            var node = sender as BeepTreeNode;
            if (node == null) return;

            // Remove from SelectedNodes if present
            if (SelectedNodes.Contains(node))
                SelectedNodes.Remove(node);
        }
        private void OnNodeChecked(object? sender, BeepMouseEventArgs e)
        {
            ClickedNode = sender as BeepTreeNode;
            if (ClickedNode == null) return;
            var node = ClickedNode;
            if (node == null) return;

            // Add to SelectedNodes if not already present
            if (!SelectedNodes.Contains(node))
                SelectedNodes.Add(node);
            NodeChecked?.Invoke(sender, e);
            if (!AllowMultiSelect)
            {
                // Single selection mode
                DeselectSelectdNodes();          // This will set IsSelected=false on previously selected nodes
                node.IsSelected = true; // This will raise NodeSelected
                _lastSelectedNode = node;
            }
         
        }
        private void OnNodeUnChecked(object? sender, BeepMouseEventArgs e)
        {
            ClickedNode = sender as BeepTreeNode;
            if (ClickedNode == null) return;
            var node = ClickedNode;
            if (node == null) return;

            // Remove from SelectedNodes if present
            if (SelectedNodes.Contains(node))
                SelectedNodes.Remove(node);
        }
        private void OnNodeSelected(object? sender, BeepMouseEventArgs e)
        {
            ClickedNode = sender as BeepTreeNode;
            if (ClickedNode == null) return;
            var node = ClickedNode;
            if (node == null) return;
            CloseOpenPopupMenu();
            // Add to SelectedNodes if not already present
            if (!SelectedNodes.Contains(node))
                SelectedNodes.Add(node);



            var clickedNode = sender as BeepTreeNode;
            if (clickedNode == null) return;
            // If checkboxes are used for selection, the node’s CheckBox_StateChanged event will handle it.
            // If not, handle selection here.

            if (!AllowMultiSelect)
            {
                // Single selection mode
                DeselectSelectdNodes();          // This will set IsSelected=false on previously selected nodes
                clickedNode.IsSelected = true; // This will raise NodeSelected
                _lastSelectedNode = clickedNode;
            }
            else
            {
                // Multi-selection mode with Ctrl/Shift support
                bool ctrlPressed = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                bool shiftPressed = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

                if (ctrlPressed)
                {
                    // Toggle selection
                    clickedNode.IsSelected = !clickedNode.IsSelected;
                    // If set to false, NodeDeselected will fire, removing it from SelectedNodes
                    // If set to true, NodeSelected will fire, adding it to SelectedNodes
                    // Do not update _lastSelectedNode if we just toggled
                }
                else if (shiftPressed && _lastSelectedNode != null)
                {
                    // Select a range of nodes between _lastSelectedNode and clickedNode
                    var allNodes = this.Traverse().ToList();
                    int startIndex = allNodes.IndexOf(_lastSelectedNode);
                    int endIndex = allNodes.IndexOf(clickedNode);

                    if (startIndex > -1 && endIndex > -1)
                    {
                        // First clear current selection
                        DeselectSelectdNodes();

                        // Ensure startIndex < endIndex
                        if (startIndex > endIndex)
                        {
                            var temp = startIndex;
                            startIndex = endIndex;
                            endIndex = temp;
                        }

                        // Select nodes in the specified range
                        for (int i = startIndex; i <= endIndex; i++)
                        {
                            allNodes[i].IsSelected = true; // Raises NodeSelected event
                        }
                    }
                }
                else
                {
                    // No modifier key pressed, treat it like single selection
                    DeselectSelectdNodes();
                    clickedNode.IsSelected = true; // Raises NodeSelected event
                    _lastSelectedNode = clickedNode;
                }
            }

        }
        private void OnNodeClicked(object sender, BeepMouseEventArgs e)
        {
            ClickedNode = sender as BeepTreeNode;
            if (ClickedNode == null) return;

            CloseOpenPopupMenu();

            // Refresh the UI if necessary
            Invalidate();
        }
        #endregion "Node Clicked"
        #region "Event Handlers"
        /// <summary>
        /// Selects the previous visible node (if any) in the tree.
        /// </summary>
        public void SelectPreviousNode()
        {
            if (SelectedNode == null)
                return;

            List<BeepTreeNode> visibleNodes = GetAllVisibleNodes();
            int currentIndex = visibleNodes.IndexOf(SelectedNode);

            if (currentIndex > 0)
            {
                SelectedNode = visibleNodes[currentIndex - 1];
            }
        }

        /// <summary>
        /// Selects the next visible node (if any) in the tree.
        /// </summary>
        public void SelectNextNode()
        {
            if (SelectedNode == null)
                return;

            List<BeepTreeNode> visibleNodes = GetAllVisibleNodes();
            int currentIndex = visibleNodes.IndexOf(SelectedNode);

            if (currentIndex < visibleNodes.Count - 1)
            {
                SelectedNode = visibleNodes[currentIndex + 1];
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (SelectedNode == null) return;
            if (e.KeyCode == Keys.Up)
            {
                SelectPreviousNode();
            }
            else if (e.KeyCode == Keys.Down)
            {
                SelectNextNode();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                NodeSelected?.Invoke(this, new BeepMouseEventArgs { EventName="KeyEnter" });
            }
        }

        private void ChangeTextAlignment()
        {
            // change the text alignment of all nodes
            foreach (var item in _beeptreeRootnodes)
            {
                item.TextAlignment = textAlign;

            }
        }
        private void ChangeFontSettings()
        {
            foreach (var item in _beeptreeRootnodes)
            {
                item.TextFont = TextFont;

            }
        }
        public void SetAllNodesCheckBoxVisibility(bool show)
        {
            // Iterate over all nodes that are directly within the _beeptreeRootnodes collection
            // These are top-level nodes or root nodes in the tree
            foreach (var node in _beeptreeRootnodes)
            {
                node.ShowCheckBox=show;
            }
            RearrangeTree(); // Adjust layout based on expanded/collapsed state
                             // Optionally, you can store this setting if needed for future reference
                             // For instance: 
                             //       this.ShowCheckBox = show;
                             // if you keep a similar property in the BeepTree itself.
        }

        /// <summary>
        /// Expands the root node and ensures all necessary child nodes are rendered to locate the target menu item.
        /// </summary>
        /// <param name="rootNode">The root node to start expansion from.</param>
        /// <param name="menuItem">The menu item to locate.</param>
        private void ExpandRootToFindNode(BeepTreeNode rootNode, SimpleItem menuItem)
        {
            // Recursively expand the tree and render child nodes
            void ExpandNode(BeepTreeNode currentNode, SimpleItem currentItem)
            {
                currentNode.IsExpanded = true;

                foreach (var childItem in currentItem.Children)
                {
                    var childNode = currentNode.NodesControls.FirstOrDefault(n => n.NodeInfo == childItem);
                    if (childNode == null)
                    {
                        // Create and add the child node if not yet rendered
                        childNode = CreateTreeNodeFromMenuItem(childItem, currentNode);
                        currentNode.AddNode(childNode);
                    }

                    // If this child contains the target item, expand it further
                    if (childItem == menuItem || childItem.Children.Any(c => c == menuItem))
                    {
                        ExpandNode(childNode, childItem);
                    }
                }
            }

            ExpandNode(rootNode, menuItem);
        }
        private void ChangeNodeImageSettings()
        {
            foreach (var item in _beeptreeRootnodes)
            {
                item.ShowNodeImage = _shownodeimage;
                item.MaxImageSize = nodeimagesize;
                if (nodeimagesize == 0)
                {
                    nodeimagesize = NodeHeight - 2;
                }
                if (nodeimagesize >= NodeHeight)
                {
                    NodeHeight = NodeHeight + 2;
                }
                item.ChangeNodeImageSettings(); // Ensure the node redraws with the updated setting
            }
        }
        private void LogMessage(string message)
        {
            try
            {
               // File.AppendAllText(@"C:\Logs\debug_log.txt", $"{DateTime.Now}: {message}{Environment.NewLine}");
                Console.WriteLine(message);
                Debug.WriteLine(message);
            }
            catch { /* Ignore logging errors */ }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RearrangeTree();
        }
        public void ExpandAll()
        {
            foreach (var panel in _nodePanels)
            {
                BeepTreeNode node = GetBeepTreeNodeFromPanel(panel.Key);
                Panel panel1 = panel.Value;
                if (node != null)
                {
                    node.IsExpanded = true;
                    node.RearrangeNode();
                    panel1.Height = node.Height; // Adjust panel height
                }
            }

            RearrangeTree();
        }
        public void ToggleNode(BeepTreeNode node)
        {
            node.IsExpanded = !node.IsExpanded;
            node.RearrangeNode();
            RearrangeTree(); // Adjust layout based on expanded/collapsed state
        }
        public void RearrangeTree()
        {
            SuspendLayout(); // Suspend layout updates for performance

            foreach (var node in _beeptreeRootnodes)
            {
                if (!_nodePanels.TryGetValue(node.NodeSeq, out var panel))
                    continue;

                // Ensure the node's layout is fully updated
                node.RearrangeNode();
                node.IsInvalidated = false; // Reset after rearranging

                // Sync panel dimensions with the node's calculated size
                panel.Width = DrawingRect.Width; // Match tree width
                panel.Height = node.Height; // Use full node height including children

                // No need to set Location.Y since Dock = DockStyle.Top handles vertical stacking
                // panel.Location = new Point(5, panel.Location.Y); // X offset optional, Y ignored by docking
            }

            ResumeLayout(); // Resume layout updates and let docking adjust positions
                            // Invalidate(); // Uncomment if redraw is needed
        }
   
        #endregion "Event Handlers"
        #region "Nodes Creation"
        public void SyncChildNodes(BeepTreeNode node)
        {
            if (node == null || node.Nodes == null)
                return;

            foreach (var childItem in node.Nodes)
            {
                // Sync child items to BeepTree's master list if necessary
                if (!Nodes.Contains(childItem))
                {
                    Nodes.Add(childItem);
                }
            }
        }
        //private void Items_ListChanged(object? sender, ListChangedEventArgs e)
        //{
        //    if (_isUpdatingTree)
        //    {
        //        //Console.WriteLine("Skipping ListChanged event due to ongoing update.");
        //        return;
        //    }

        //  //  Console.WriteLine($"ListChanged: Type={e.ListChangedType}, Index={e.NewIndex}");
        //    try
        //    {
        //        _isUpdatingTree = true;

        //        switch (e.ListChangedType)
        //        {
        //            case ListChangedType.ItemAdded:
        //                HandleItemAdded(e.NewIndex);
        //                break;

        //            case ListChangedType.ItemDeleted:
        //                HandleItemDeleted(e.NewIndex);
        //                break;

        //            case ListChangedType.ItemChanged:
        //                HandleItemChanged(e.NewIndex);
        //                break;

        //            case ListChangedType.Reset:
        //                if (rootnodeitems.Count == 0)
        //                {
        //                    ClearNodes();
        //                }
        //                else
        //                {
        //                    InitializeTreeFromMenuItems();
        //                }
        //                break;

        //            default:
        //                Console.WriteLine($"Unhandled ListChangedType: {e.ListChangedType}");
        //                break;
        //        }
        //    }
        //    finally
        //    {
        //        _isUpdatingTree = false;
        //    }
        //}
        //private void HandleItemAdded(int index)
        //{

        //    if (index < 0 || index >= rootnodeitems.Count)
        //    {
        //        LogMessage($"Invalid index for addition: {index}");
        //        return;
        //    }

        //    var menuItem = rootnodeitems[index];
        //    LogMessage($"Handling item addition for index {index}: {menuItem.Text}");

        //    // Commented out for debugging:
        //    var node = CreateTreeNodeFromMenuItem(menuItem, null);
        //    if (node != null)
        //    {
        //        _dontRearrange = true;
        //        Panel panel = AddRootNode(node);
        //        panel.Tag = node.GuidID;
        //        menuItem.ContainerGuidID = node.GuidID;
        //        menuItem.RootContainerGuidID = node.GuidID;
        //        node.Nodes = menuItem.Children;
        //        node.NodeInfo = menuItem;
               
        //    //    node.RearrangeNode();
        //        LogMessage($"Node added for item at index {index}: {menuItem.Text}");
        //     //   RearrangeTree();
        //        _dontRearrange = false;
        //    }
        //}
        //private void HandleItemDeleted(int index)
        //{
        //    if (index < 0 || index >= _beeptreeRootnodes.Count)
        //    {
        //        LogMessage($"Invalid index for deletion: {index}");
        //        return;
        //    }

        //    var node = GetNode(index);
        //    if (node != null)
        //    {
        //        RemoveNode(node);
        //        LogMessage($"Node removed for item at index {index}");
        //        RearrangeTree();
        //    }
        //}
        //private void HandleItemChanged(int index)
        //{
        //    if (index < 0 || index >= rootnodeitems.Count || index >= _beeptreeRootnodes.Count)
        //    {
        //        Console.WriteLine($"Invalid index for update: {index}");
        //        return;
        //    }

        //    var menuItem = rootnodeitems[index];
        //    var node = GetNode(index);
        //    if (node != null && menuItem != null)
        //    {
        //        node.Text = menuItem.Text;
        //        node.ImagePath = menuItem.ImagePath;
        //        node.Children = menuItem.Children; // Sync children
        //        //node.RearrangeNode();
        //        Console.WriteLine($"Node updated for item at index {index}: {menuItem.Text}");
        //        RearrangeTree();
        //    }
        //}
        /// <summary>
        /// Initializes the tree by traversing SimpleItems and their children, creating BeepTreeNodes recursively.
        /// </summary>
        public void InitializeTreeFromMenuItems()
        {
            try
            {
                SuspendLayout(); // Prevent layout updates during initialization
                _beeptreeRootnodes.Clear(); // Clear existing root nodes
                Controls.Clear(); // Clear existing UI controls

                // Traverse root-level SimpleItems and create root BeepTreeNodes
                foreach (var item in rootnodeitems)
                {
                    var rootNode = CreateTreeNodeFromMenuItem(item, null); // null parent for root nodes
                    if (rootNode != null)
                    {
                        rootNode.GuidID = item.GuidId; // Share GuidId
                        item.ContainerGuidID = rootNode.GuidID;
                        item.RootContainerGuidID = rootNode.GuidID; // Root node's own GuidID
                        item.IsDrawn = true; // Root nodes are drawn immediately

                        // Add the root node using AddRootNode
                        var panel = AddRootNode(rootNode);

                        // Recursively create child nodes (not drawn until expanded)
                        CreateChildNodesRecursively(rootNode, item);
                    }
                    else
                    {
                        LogMessage($"Failed to create root BeepTreeNode for item with GuidId {item.GuidId}.");
                    }
                }

                // RearrangeTree is called within AddRootNode unless _dontRearrange is true
                // No need to call it again here unless additional layout is required
            }
            catch (Exception ex)
            {
                LogMessage($"Error initializing tree from menu items: {ex.Message}");
            }
            finally
            {
                ResumeLayout(); // Resume layout updates
            }
        }

        /// <summary>
        /// Recursively creates child BeepTreeNodes for a SimpleItem’s children.
        /// </summary>
        /// <param name="parentNode">The parent BeepTreeNode.</param>
        /// <param name="parentItem">The parent SimpleItem.</param>
        private void CreateChildNodesRecursively(BeepTreeNode parentNode, SimpleItem parentItem)
        {
            if (parentItem.Children == null || parentItem.Children.Count == 0)
            {
                return;
            }

            foreach (var childItem in parentItem.Children)
            {
                var childNode = CreateTreeNodeFromMenuItem(childItem, parentNode);
                if (childNode != null)
                {
                    childNode.GuidID = childItem.GuidId; // Share GuidId
                    childItem.ContainerGuidID = childNode.GuidID;
                    childItem.RootContainerGuidID = parentNode.GuidID; // Set to parent's GuidID
                    childItem.IsDrawn = false; // Children are not drawn until expanded

                    parentNode.AddNode(childNode); // Add to parent’s NodesControls and Nodes
                    parentNode.RearrangeNode();
                    childNode.RearrangeNode();
                    // Recursively process deeper children
                    CreateChildNodesRecursively(childNode, childItem);
                }
                else
                {
                    LogMessage($"Failed to create child BeepTreeNode for item with GuidId {childItem.GuidId}.");
                }
            }
        }
        // function to generate a tree node from a parameters
        public BeepTreeNode CreateTreeNode(string name, string imagepath, string BranchGuid, string typename, BeepTreeNode parent)
        {
            try
            {
                int seq = -1;
                if (parent != null)
                {
                    seq = parent.ChildNodesSeq;
                }
                else
                {
                    seq = Nodeseq++; ;
                }
                //   LogMessage($"Creating Node: {menuItem.Text}, Depth: {depth}");
                Console.WriteLine("Creating Node: " + name);
                var node = new BeepTreeNode
                {
                    Name = name ?? $"Node{seq}",
                    NodeLevelID = parent == null ? seq.ToString() : parent.NodeLevelID + "_" + parent.ChildNodesSeq,
                    Text = name,
                    ImagePath = imagepath,
                    ParentNode = parent,
                    Tree = this,
                    NodeSeq = seq,
                    Height = NodeHeight,
                    IsBorderAffectedByTheme = false,
                    IsShadowAffectedByTheme = false,
                    IsFramless = true,
                    IsChild = true,
                    Size = new Size(this.Width, NodeHeight),
                    Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                    Theme = this.Theme,
                    SavedGuidID = BranchGuid,
                    NodeDataType = typename,
                    UseScaledFont = UseScaledFont,
                    UseThemeFont = this.UseThemeFont,
                    Level = parent?.Level + 1 ?? 1 // Increment level for child nodes
                };
                node.Text = name ?? $"Node{seq}";
                node.NodeLeftClicked += (sender, e) => NodeLeftClicked?.Invoke(sender, e);
                node.NodeRightClicked += (sender, e) => NodeRightClicked?.Invoke(sender, e);
                node.NodeDoubleClicked += (sender, e) => NodeDoubleClicked?.Invoke(sender, e);
                node.ShowMenu += (sender, e) => ShowFlyoutMenu(sender,e);
                node.NodeExpanded += (sender, e) => NodeExpanded?.Invoke(sender, e);
                node.NodeCollapsed += (sender, e) => NodeCollapsed?.Invoke(sender, e);
                node.NodeSelected += (sender, e) => NodeSelected?.Invoke(sender, e);
                node.NodeDeselected += (sender, e) => NodeDeselected?.Invoke(sender, e);
              //  node.MouseHover += (sender, e) => { node.HilightNode(); };
             //   node.MouseLeave += (sender, e) => { node.UnHilightNode(); };
                Console.WriteLine("Node Created: " + node.Text);

                return node;

            }
            catch (Exception ex)
            {
                //   LogMessage($"Erro in creating node:  {ex.Message} ");
                Console.WriteLine("Error: " + ex.Message);
                return new BeepTreeNode();
            }
        }

        // Function to Create a BeepTreeNode from a MenuItem
        public BeepTreeNode CreateTreeNodeFromMenuItem(SimpleItem menuItem, BeepTreeNode parent)
        {
            try
            {
                if (depth > 100) // Arbitrary limit to prevent infinite loops
                {
                    LogMessage("Recursion depth exceeded");
                    return null;
                }
                int seq = -1;
                if (parent != null)
                {
                    seq = parent.ChildNodesSeq;
                }
                else
                {
                    seq = Nodeseq++;
                }
                 
                LogMessage($"Creating Node: {menuItem.Text}, Depth: {depth}");
                Console.WriteLine("Creating Node: " + menuItem.Text);
                var node = new BeepTreeNode
                {
                    Name = menuItem.Text ?? $"Node{seq}",
                    NodeLevelID = parent == null ? seq.ToString() : parent.NodeLevelID + "_" + parent.ChildNodesSeq,
                    Text = menuItem.Text,
                    ImagePath =  menuItem.ImagePath,
                    ParentNode = parent,
                    Tree = this,
                    NodeSeq = Nodeseq,
                    Nodes = menuItem.Children,
                    Height = NodeHeight,
                    IsBorderAffectedByTheme = false,
                    IsShadowAffectedByTheme = false,
                    MaxImageSize= NodeImageSize,
                    IsFramless = true,
                    IsChild = false,
                    Size = new Size(this.Width, NodeHeight),
                    Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                    Theme = this.Theme,
                    NodeInfo = menuItem,
                    GuidID=menuItem.GuidId,
                    NodeDataType = menuItem.GetType().Name,
                    Level = parent?.Level + 1 ?? 1,
                    UseThemeFont = this.UseThemeFont,
                    UseScaledFont = this.UseScaledFont,
                    TextAlignment = textAlign,
                  
                };
                if(UseThemeFont == false)
                {
                    node.TextFont = TextFont;
                }
                
                node.Text = menuItem.Text ?? $"Node{seq}";
                node.NodeClicked += OnNodeClicked;
                node.NodeRightClicked += OnNodeRightClicked;
                node.NodeDoubleClicked += NodeDoubleClicked;
               // node.ShowBeepMenu += (sender, e) => ShowFlyoutMenu(node, new Point(node.Left, node.Top));
                node.NodeExpanded += OnNodeExpanded;
                node.NodeCollapsed += OnNodeCollapsed;
                node.NodeSelected += OnNodeSelected;
                node.NodeDeselected += OnNodeDeselected;
                node.NodeChecked += OnNodeChecked;
                node.NodeUnchecked += OnNodeUnChecked;
              //  node.MouseHover += (sender, e) => { node.HilightNode(); };
               // node.MouseLeave += (sender, e) => { node.UnHilightNode(); };
                node.ShowCheckBox=ShowCheckBox;
                Console.WriteLine("Node Created: " + node.Text);
                LogMessage($"Creating Node Childern: {menuItem.Text}, Depth: {depth}");
                return node;

            }
            catch (Exception ex)
            {
                LogMessage($"Erro in creating node:  {ex.Message} ");
                Console.WriteLine("Error: " + ex.Message);
                return new BeepTreeNode();
            }
        }
        // this function to used add root nodes to the tree
        private Panel AddRootNode(BeepTreeNode node)
        {
            var panel = new Panel
            {
                Width = Width,
                Height = node.Height,
                AutoSize = true,
                Dock = DockStyle.Top,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Tag = node.GuidID,
                Margin = new Padding(0),
                Padding = new Padding(0),
                BorderStyle = BorderStyle.None
            };
            
            _beeptreeRootnodes.Add(node);
            panel.Controls.Add(node); // Add the node to the panel
            Controls.Add(panel); // Add the panel to the tree
            _nodePanels.Add(node.NodeSeq, panel);
            // Subscribe to node's expansion and collapse events
            node.NodeExpanded += (s, e) => RearrangeTree();
            node.NodeCollapsed += (s, e) => RearrangeTree();
            //  node.RearrangeNode();
            if (!_dontRearrange)
                RearrangeTree();
            return panel;
        }

        /// <summary>
        /// Populates the tree from a hierarchical data structure.
        /// </summary>
        /// <param name="data">The data to populate the tree with.</param>
        public void PopulateTree(IEnumerable<SimpleItem> data)
        {
            Nodes.Clear();
            foreach (var item in data.Where(p => p.ParentItem == null))
            {
                Nodes.Add(item);
            }
        }
        public async Task LoadTreeAsync(IEnumerable<SimpleItem> data)
        {

            await Task.Run(() =>
            {
                _dontRearrange = true;
                SuspendLayout(); // Suspend layout updates
                // Perform data processing here
                var nodes = CreateTreeNodes(data);

                // Return the processed nodes
                return nodes;
            }).ContinueWith(nodes =>
            {


                ResumeLayout(); // Resume layout updates
                RearrangeTree();
                _dontRearrange = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }
        private List<BeepTreeNode> CreateTreeNodes(IEnumerable<SimpleItem> data)
        {
            List<BeepTreeNode> nodes = new List<BeepTreeNode>();
            foreach (var item in data.Where(p => p.ParentItem == null))
            {
                var node = CreateTreeNodeFromMenuItem(item, null);
                if (node != null)
                {
                    Nodes.Add(item);
                    //Panel panel = AddRootNode(node);
                    //panel.Tag = node.GuidID;
                    //item.ContainerGuidID = node.GuidID;
                    //item.RootContainerGuidID = node.GuidID;
                }
            }
            return nodes;
        }
        private BeepTreeNode GetBeepTreeNodeFromPanel(int seq)
        {

            return _beeptreeRootnodes.FirstOrDefault(p => p.NodeSeq == seq);
        }
        /// <summary>
        /// Adds a new SimpleItem to the specified parent's Children and draws the entire branch.
        /// If no valid parent is specified or found, creates a root node.
        /// </summary>
        /// <param name="newItem">The SimpleItem to add.</param>
        /// <param name="parentGuidId">The GuidID of the parent node (null or invalid for root).</param>
        /// <returns>The created BeepTreeNode, or null if the operation fails.</returns>
        public BeepTreeNode AddNodeWithBranch(SimpleItem newItem, string parentGuidId = null)
        {
            if (newItem == null)
            {
                LogMessage("Cannot add null SimpleItem.");
                return null;
            }

            try
            {
                BeepTreeNode parentNode = null;
                SimpleItem parentItem = null;
                BeepTreeNode newNode = null;

                // Ensure newItem has a GuidId
                if (string.IsNullOrEmpty(newItem.GuidId))
                {
                    newItem.GuidId = Guid.NewGuid().ToString();
                }

                // Handle root node case (null or invalid parentGuidId)
                if (string.IsNullOrEmpty(parentGuidId))
                {
                    // Create as a root node
                    newNode = CreateTreeNodeFromMenuItem(newItem, null);
                    if (newNode == null)
                    {
                        LogMessage("Failed to create root BeepTreeNode.");
                        return null;
                    }

                    newNode.GuidID = newItem.GuidId;
                    newItem.ContainerGuidID = newNode.GuidID;
                    newItem.RootContainerGuidID = newNode.GuidID;
                    newItem.IsDrawn = true;

                    var panel = AddRootNode(newNode);
                    if (panel == null)
                    {
                        LogMessage("Failed to add root node panel.");
                        return null;
                    }

                    panel.Tag = newNode.GuidID;
                    Nodes.Add(newItem); // Add to rootnodeitems for data consistency
                }
                else
                {
                    // Find the parent node by GuidID
                    parentNode = GetBeepTreeNodeByGuid(parentGuidId);
                    if (parentNode == null)
                    {
                        // Parent not found: treat as root node (assuming intent to always succeed)
                        LogMessage($"Parent node with GuidID {parentGuidId} not found. Adding as root node.");
                        newNode = CreateTreeNodeFromMenuItem(newItem, null);
                        if (newNode == null)
                        {
                            LogMessage("Failed to create root BeepTreeNode for invalid parent.");
                            return null;
                        }

                        newNode.GuidID = newItem.GuidId;
                        newItem.ContainerGuidID = newNode.GuidID;
                        newItem.RootContainerGuidID = newNode.GuidID;
                        newItem.IsDrawn = true;

                        var panel = AddRootNode(newNode);
                        if (panel == null)
                        {
                            LogMessage("Failed to add root node panel for invalid parent.");
                            return null;
                        }

                        panel.Tag = newNode.GuidID;
                        Nodes.Add(newItem);
                    }
                    else
                    {
                        // Parent exists: add as child
                        parentItem = parentNode.NodeInfo;
                        if (parentItem == null)
                        {
                            LogMessage($"Parent node {parentGuidId} has no associated SimpleItem.");
                            return null;
                        }

                        parentItem.Children.Add(newItem);
                        newItem.ParentItem = parentItem;

                        newNode = CreateTreeNodeFromMenuItem(newItem, parentNode);
                        if (newNode == null)
                        {
                            LogMessage("Failed to create child BeepTreeNode.");
                            return null;
                        }

                        newNode.GuidID = newItem.GuidId;
                        newItem.ContainerGuidID = newNode.GuidID;
                        newItem.RootContainerGuidID = parentNode.GuidID;
                        newItem.IsDrawn = parentNode.IsExpanded;

                        parentNode.AddNode(newNode);
                    }
                }

                // Draw the branch and update layout
                DrawBranch(newNode);
                if (parentNode != null)
                {
                    parentNode.RearrangeNode();
                }
                RearrangeTree();

                return newNode;
            }
            catch (Exception ex)
            {
                LogMessage($"Error adding node with branch: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Recursively draws the branch starting from the given node, including all children.
        /// </summary>
        /// <param name="node">The starting node of the branch to draw.</param>
        private void DrawBranch(BeepTreeNode node)
        {
            if (node == null || node.Nodes == null || node.Nodes.Count == 0)
                return;

            // Ensure the node's children are rendered if expanded
            if (node.IsExpanded)
            {
                foreach (var childItem in node.Nodes)
                {
                    if (node.NodesControls.Any(n => n.NodeInfo == childItem))
                        continue; // Skip if already created

                    var childNode = CreateTreeNodeFromMenuItem(childItem, node);
                    if (childNode != null)
                    {
                        childNode.GuidID = childItem.GuidId;
                        childItem.ContainerGuidID = childNode.GuidID;
                        childItem.RootContainerGuidID = node.GuidID;

                        node.AddNode(childNode); // Add to NodesControls and UI
                        DrawBranch(childNode); // Recursively draw children
                    }
                }
                node.RearrangeNode(); // Update layout after adding children
            }
        }
        /// <summary>
        /// Adds a new SimpleItem to an existing BeepTreeNode branch and updates the UI.
        /// </summary>
        /// <param name="newItem">The SimpleItem to add.</param>
        /// <param name="parentNode">The existing BeepTreeNode to add the item under.</param>
        /// <returns>The created BeepTreeNode, or null if the operation fails.</returns>
        public BeepTreeNode AddNodeToBranch(SimpleItem newItem, BeepTreeNode parentNode)
        {
            if (newItem == null)
            {
                LogMessage("Cannot add null SimpleItem.");
                return null;
            }
            if (parentNode == null)
            {
                LogMessage("Parent BeepTreeNode cannot be null. Use AddNodeWithBranch for root nodes.");
                return null;
            }

            try
            {
                var parentItem = parentNode.NodeInfo;
                if (parentItem == null)
                {
                    LogMessage($"Parent node {parentNode.GuidID} has no associated SimpleItem.");
                    return null;
                }

                // Ensure newItem has a GuidId
                if (string.IsNullOrEmpty(newItem.GuidId))
                {
                    newItem.GuidId = Guid.NewGuid().ToString();
                }

                // Add to parent's Children
                parentItem.Children.Add(newItem);
                newItem.ParentItem = parentItem;

                // Create the new node
                var newNode = CreateTreeNodeFromMenuItem(newItem, parentNode);
                if (newNode == null)
                {
                    LogMessage("Failed to create BeepTreeNode for the new item.");
                    return null;
                }

                // Set GUIDs and hierarchy
                newNode.GuidID = newItem.GuidId;
                newItem.ContainerGuidID = newNode.GuidID;
                newItem.RootContainerGuidID = parentNode.GuidID;

                // Add to parent and update UI
                parentNode.AddNode(newNode);
                parentNode.RearrangeNode(); // Update parent layout
                RearrangeTree(); // Update entire tree

                return newNode;
            }
            catch (Exception ex)
            {
                LogMessage($"Error adding node to branch: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Adds a new SimpleItem to an existing branch identified by the parent's GuidId and updates Nodes.
        /// </summary>
        /// <param name="newItem">The SimpleItem to add.</param>
        /// <param name="parentGuidId">The GuidId of the parent SimpleItem in the branch.</param>
        /// <returns>The SimpleItem added, or null if the operation fails.</returns>
        public SimpleItem AddNodeToBranchByGuid(SimpleItem newItem, string parentGuidId)
        {
            if (newItem == null)
            {
                LogMessage("Cannot add null SimpleItem.");
                return null;
            }
            if (string.IsNullOrEmpty(parentGuidId))
            {
                LogMessage("Parent GuidId cannot be null or empty. Use Nodes.Add for root nodes.");
                return null;
            }

            try
            {
                // Find the parent SimpleItem
                var parentItem = GetNodeByGuidID(parentGuidId);
                if (parentItem == null)
                {
                    LogMessage($"Parent SimpleItem with GuidId {parentGuidId} not found.");
                    return null;
                }

                // Ensure newItem has a GuidId
                if (string.IsNullOrEmpty(newItem.GuidId))
                {
                    newItem.GuidId = Guid.NewGuid().ToString();
                }

                // Add to parent's Children
                parentItem.Children.Add(newItem);
                newItem.ParentItem = parentItem;
                newItem.RootContainerGuidID = parentGuidId;

                // Find the corresponding BeepTreeNode (if exists)
                var parentNode = GetBeepTreeNodeByGuid(parentGuidId);
                if (parentNode != null)
                {
                    var newNode = CreateTreeNodeFromMenuItem(newItem, parentNode);
                    if (newNode != null)
                    {
                        newNode.GuidID = newItem.GuidId;
                        newItem.ContainerGuidID = newNode.GuidID;

                        parentNode.AddNode(newNode);
                        parentNode.RearrangeNode();
                        RearrangeTree();
                    }
                    else
                    {
                        LogMessage("Failed to create BeepTreeNode for the new item.");
                    }
                }
                else
                {
                    LogMessage($"Parent node for GuidId {parentGuidId} not yet created. Added to data only.");
                }

                NodeAdded?.Invoke(this, new BeepMouseEventArgs { EventName = "NodeAdded", Data = newItem.GuidId });
                return newItem;
            }
            catch (Exception ex)
            {
                LogMessage($"Error adding node to branch: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Adds a new SimpleItem to an existing SimpleItem parent's Children and updates the tree.
        /// </summary>
        /// <param name="newItem">The SimpleItem to add.</param>
        /// <param name="parentItem">The SimpleItem parent to add the item under.</param>
        /// <returns>The BeepTreeNode of the parent, or null if the parent isn’t drawn or the operation fails.</returns>
        public BeepTreeNode AddNodeToBranch(SimpleItem newItem, SimpleItem parentItem)
        {
            if (newItem == null)
            {
                LogMessage("Cannot add null SimpleItem.");
                return null;
            }
            if (parentItem == null)
            {
                LogMessage("Parent SimpleItem cannot be null. Use Nodes.Add for root nodes.");
                return null;
            }

            try
            {
                // Ensure newItem has a GuidId
                if (string.IsNullOrEmpty(newItem.GuidId))
                {
                    newItem.GuidId = Guid.NewGuid().ToString();
                }

                // Add to parent's Children
                parentItem.Children.Add(newItem);
                newItem.ParentItem = parentItem;

                // Find the corresponding parent BeepTreeNode (if it exists)
                var parentNode = GetBeepTreeNodeByGuid(parentItem.GuidId);
                if (parentNode != null)
                {
                    // Create a new BeepTreeNode for the new item
                    var newNode = CreateTreeNodeFromMenuItem(newItem, parentNode);
                    if (newNode != null)
                    {
                        newNode.GuidID = newItem.GuidId;
                        newItem.ContainerGuidID = newNode.GuidID;
                        newItem.RootContainerGuidID = parentNode.GuidID;
                        newItem.IsDrawn = parentNode.IsExpanded;

                        parentNode.AddNode(newNode);
                        parentNode.RearrangeNode();
                        RearrangeTree();
                    }
                    else
                    {
                        LogMessage($"Failed to create BeepTreeNode for item with GuidId {newItem.GuidId}.");
                    }

                    // Return the parent node
                    return parentNode;
                }
                else
                {
                    // If parent node isn’t drawn yet, update data only
                    newItem.RootContainerGuidID = parentItem.GuidId;
                    newItem.IsDrawn = false;
                    LogMessage($"Parent node for GuidId {parentItem.GuidId} not yet created. Added to data only.");
                }

                // Trigger NodeAdded event
                NodeAdded?.Invoke(this, new BeepMouseEventArgs { EventName = "NodeAdded", Data = newItem.GuidId });

                return null; // Return null if parent isn’t drawn
            }
            catch (Exception ex)
            {
                LogMessage($"Error adding node to branch: {ex.Message}");
                return null;
            }
        }
        #endregion "Nodes Creation"
        #region "Remove Nodes"
        /// <summary>
        /// Removes a SimpleItem from the tree and updates Nodes and the UI.
        /// </summary>
        /// <param name="simpleItem">The SimpleItem to remove.</param>
        public void RemoveNode(SimpleItem simpleItem)
        {
            if (simpleItem == null)
            {
                LogMessage("Cannot remove null SimpleItem.");
                return;
            }

            try
            {
                // Find and remove the SimpleItem from the hierarchy
                var parentItem = FindParentNode(rootnodeitems, simpleItem);
                if (parentItem != null)
                {
                    parentItem.Children.Remove(simpleItem);
                }
                else
                {
                    // If it’s a root node, remove it directly from rootnodeitems
                    rootnodeitems.Remove(simpleItem);
                }

                // Clean up UI controls if the node exists
                var node = GetBeepTreeNodeByGuid(simpleItem.GuidId);
                if (node != null)
                {
                    if (node.ParentNode != null)
                    {
                        node.ParentNode.RemoveNode(node);
                        node.ParentNode.RearrangeNode();
                    }
                    else
                    {
                        _beeptreeRootnodes.Remove(node);
                    }
                    RemoveNodeControlsRecursive(_beeptreeRootnodes, simpleItem);
                    RearrangeTree();
                }

                // Trigger NodeDeleted event
                NodeDeleted?.Invoke(this, new BeepMouseEventArgs { EventName = "NodeDeleted", Data = simpleItem.GuidId });
            }
            catch (Exception ex)
            {
                LogMessage($"Error removing node: {ex.Message}");
            }
        }
        private SimpleItem FindParentNode(IEnumerable<SimpleItem> items, SimpleItem target)
        {
            foreach (var item in items)
            {
                if (item.Children.Contains(target))
                {
                    return item;
                }

                var found = FindParentNode(item.Children, target);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }
        private void RemoveNodeControlsRecursive(IEnumerable<BeepTreeNode> nodes, SimpleItem simpleItem)
        {
            foreach (var node in nodes.ToList())
            {
                if (node.NodeInfo == simpleItem)
                {
                    // Remove the node and its panel
                    if (_nodePanels.TryGetValue(node.NodeSeq, out var panel))
                    {
                        Controls.Remove(panel);
                        _nodePanels.Remove(node.NodeSeq);
                    }

                    _beeptreeRootnodes.Remove(node);
                }
                else
                {
                    // Recursively check child nodes
                    RemoveNodeControlsRecursive(node.NodesControls, simpleItem);
                }
            }
        }

        public void ClearNodes()
        {
            rootnodeitems.Clear(); // Clear the data
            foreach (var panel in _nodePanels)
            {
                Controls.Remove(panel.Value); // Remove associated UI controls
            }
            _nodePanels.Clear();
            _beeptreeRootnodes.Clear(); // Clear root nodes
            RearrangeTree(); // Update the layout
        }

        public void RemoveNode(string NodeName)
        {
            // Find the SimpleItem by name
            var simpleItem = FindNodeByName(rootnodeitems, NodeName);
            if (simpleItem != null)
            {
                RemoveNode(simpleItem);
            }
        }

        // Helper to find a node by name in the hierarchy
        public SimpleItem FindNodeByName(IEnumerable<SimpleItem> items, string name)
        {
            foreach (var item in items)
            {
                if (item.Text == name)
                {
                    return item;
                }

                var found = FindNodeByName(item.Children, name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public void RemoveNode(int NodeIndex)
        {
            if (NodeIndex >= 0 && NodeIndex < rootnodeitems.Count)
            {
                var simpleItem = rootnodeitems[NodeIndex];
                RemoveNode(simpleItem);
            }
        }

        public void RemoveNode(BeepTreeNode node)
        {
            // Recursively remove all child nodes
            foreach (var child in node.NodesControls.ToList())
            {
                RemoveNode(child);
            }

            // Remove the node's panel and itself from the root nodes
            if (_nodePanels.TryGetValue(node.NodeSeq, out var panel))
            {
                Controls.Remove(panel);
                _nodePanels.Remove(node.NodeSeq);
            }

            _beeptreeRootnodes.Remove(node);
            RearrangeTree(); // Update the layout
        }

        #endregion "Remove Nodes"
        #region "Travsersal"
        /// <summary>
        /// Recursively adds the given node and all its visible (and expanded) descendants.
        /// </summary>
        private void AddVisibleNodes(BeepTreeNode node, List<BeepTreeNode> list)
        {
            if (!node.Visible)
                return;

            list.Add(node);
            if (node.IsExpanded)
            {
                foreach (var child in node.NodesControls)
                {
                    AddVisibleNodes(child, list);
                }
            }
        }
        /// <summary>
        /// Retrieves all visible nodes in the tree, including expanded children.
        /// </summary>
        public List<BeepTreeNode> GetAllVisibleNodes()
        {
            List<BeepTreeNode> visibleNodes = new List<BeepTreeNode>();

            foreach (var rootNode in _beeptreeRootnodes)
            {
                TraverseVisibleNodes(rootNode, visibleNodes);
            }

            return visibleNodes;
        }

        /// <summary>
        /// Recursively traverses only visible nodes.
        /// </summary>
        private void TraverseVisibleNodes(BeepTreeNode node, List<BeepTreeNode> nodeList)
        {
            if (!node.IsVisible) return;

            nodeList.Add(node);

            if (node.IsExpanded)
            {
                foreach (var child in node.NodesControls)
                {
                    TraverseVisibleNodes(child, nodeList);
                }
            }
        }

        public IEnumerable<BeepTreeNode> GetDisplayNodes()
        {
            foreach (var rootNode in _beeptreeRootnodes.Where(n => n.ParentNode == null))
            {
                yield return rootNode;

                if (rootNode.IsExpanded)
                {
                    foreach (var descendant in rootNode.GetDescendants())
                    {
                        yield return descendant;
                    }
                }
            }
        }
        public IEnumerable<BeepTreeNode> GetDescendants()
        {
            foreach (var child in _beeptreeRootnodes)
            {
                yield return child;

                if (child.IsExpanded)
                {
                    foreach (var descendant in child.GetDescendants())
                    {
                        yield return descendant;
                    }
                }
            }
        }
        /// <summary>
        /// Recursively traverses all nodes starting from <c>_beeptreeRootnodes</c>.
        /// </summary>
        public IEnumerable<BeepTreeNode> Traverse()
        {
            // Flatten all root and descendant nodes in a single list
            var visited = new List<BeepTreeNode>();
            foreach (var rootNode in _beeptreeRootnodes)
            {
                TraverseInternal(rootNode, visited);
            }
            return visited;
        }

        /// <summary>
        /// Internal recursive traversal helper.
        /// </summary>
        private void TraverseInternal(BeepTreeNode node, List<BeepTreeNode> visited)
        {
            visited.Add(node);

            // If node is expanded and has children, recurse down
            if (node.IsExpanded && node.NodesControls.Count > 0)
            {
                foreach (var child in node.NodesControls)
                {
                    TraverseInternal(child, visited);
                }
            }
        }


        #endregion "Travsersal"
        #region "Find and Filter"
        #region "Find Tree Node"

        // Finds a node by its text (case-sensitive) in the hierarchy
        public BeepTreeNode FindNode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            return FindNode(n => string.Equals(n.Text, text, StringComparison.Ordinal));
        }

        // Finds a node by its GUID in the hierarchy
        public BeepTreeNode GetBeepTreeNodeByGuid(string guidid)
        {
            if (string.IsNullOrWhiteSpace(guidid))
                return null;

            return FindNode(n => string.Equals(n.GuidID, guidid, StringComparison.Ordinal));
        }

        // Finds a node by its name in the hierarchy
        public BeepTreeNode GetBeepTreeNode(string nodeName)
        {
            if (string.IsNullOrWhiteSpace(nodeName))
                return null;

            return FindNode(n => string.Equals(n.Name, nodeName, StringComparison.OrdinalIgnoreCase));
        }

        // Finds a node by a predicate and highlights it
        public BeepTreeNode FindNode(Func<BeepTreeNode, bool> predicate)
        {
            foreach (var rootNode in _beeptreeRootnodes)
            {
                var matchingNode = FindNodeInHierarchy(rootNode, predicate);
                if (matchingNode != null)
                {
                    HighlightNode(matchingNode);
                    return matchingNode;
                }
            }

            return null;
        }

        // Core recursive method to traverse the hierarchy and find a node
        private BeepTreeNode FindNodeInHierarchy(BeepTreeNode currentNode, Func<BeepTreeNode, bool> predicate)
        {
            if (predicate(currentNode))
                return currentNode;

            // Traverse child nodes
            foreach (var childNode in currentNode.NodesControls)
            {
                var foundNode = FindNodeInHierarchy(childNode, predicate);
                if (foundNode != null)
                    return foundNode;
            }

            return null;
        }

        // Traverse all SimpleItems recursively and find a node by GUID
        public SimpleItem GetNodeByGuidID(string guidID)
        {
            if (string.IsNullOrWhiteSpace(guidID))
                return null;

            return TraverseAllItems(Nodes).FirstOrDefault(n => n.GuidId == guidID);
        }

        // Traverse all SimpleItems recursively and find a node by name
        public SimpleItem GetNode(string nodeName)
        {
            if (string.IsNullOrWhiteSpace(nodeName))
                return null;

            return TraverseAllItems(Nodes).FirstOrDefault(n => n.Text == nodeName);
        }

        // Traverse all SimpleItems recursively and find a node by index
        public SimpleItem GetNode(int nodeIndex)
        {
            if (nodeIndex < 0)
                return null;

            int currentIndex = 0;
            foreach (var item in TraverseAllItems(Nodes))
            {
                if (currentIndex == nodeIndex)
                    return item;
                currentIndex++;
            }

            return null;
        }


        #endregion
        #region "Filtering"
        // <summary>
        /// Filters nodes based on <paramref name="predicate"/>. 
        /// A node is visible if it or any descendant matches the predicate.
        /// </summary>
        /// <param name="predicate">Condition to determine if a node should remain visible.</param>
        public void FilterNodes(Func<BeepTreeNode, bool> predicate)
        {
            // Mark each node based on the predicate
            foreach (var node in Traverse())
            {
                bool isMatch = predicate(node);
                node.IsVisible = isMatch;
            }

            // Keep ancestors visible if a child is visible
            foreach (var node in Traverse())
            {
                if (node.IsVisible)
                {
                    var parent = node.ParentNode;
                    while (parent != null)
                    {
                        parent.IsVisible = true;
                        parent = parent.ParentNode;
                    }
                }
            }

            // Optionally expand visible nodes, or load children if needed
            // For instance:
            // foreach (var node in Traverse())
            // {
            //     if (node.IsVisible && node.IsExpanded && node.NodesControls.Count == 0 && node.Tag is SimpleItem menuItem)
            //     {
            //         CreateChildNodes(node, menuItem);
            //     }
            // }

            // Finally redraw or rearrange
            RearrangeTree();
        }

        /// <summary>
        /// Clears the current filter and resets all nodes to be visible.
        /// </summary>
        public void ClearFilter()
        {
            foreach (var node in Traverse())
            {
                node.IsVisible = true;
            }

            // Rearrange the tree to reflect the changes
            RearrangeTree();
        }

        #endregion "Filtering"
        #region "Find SimpleItem"
        /// <summary>
        /// Recursively traverses the entire hierarchy of SimpleItems.
        /// </summary>
        /// <param name="items">A collection of SimpleItem objects (e.g., your root-level Nodes).</param>
        /// <returns>An enumerable containing all nodes, including nested children.</returns>
        public IEnumerable<SimpleItem> TraverseAllItems(IEnumerable<SimpleItem> items)
        {
            foreach (var item in items)
            {
                // Return the current item
                yield return item;

                // Recursively return all of its children
                if (item.Children != null && item.Children.Count > 0)
                {
                    foreach (var child in TraverseAllItems(item.Children))
                    {
                        yield return child;
                    }
                }
            }
        }
    

        #endregion "Find SimpleItem"
        public List<BeepTreeNode> GetNodes()
        {
            return _beeptreeRootnodes;
        }
        #endregion "Find and Filter"
        #region "Theme"
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textFont = Font;
          //  Console.WriteLine("Font Changed");
            if (AutoSize)
            {
                Size textSize = TextRenderer.MeasureText(Text, _textFont);
                this.Size = new Size(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
            }
        }
        /// <summary>
        /// Highlights the given <paramref name="node"/>, expanding ancestors, and scrolling it into view.
        /// </summary>
        /// <param name="node">The node to highlight.</param>
        private void HighlightNode(BeepTreeNode node)
        {
            if (node == null) return;

            // Expand all parents
            var current = node.ParentNode;
            while (current != null)
            {
                current.IsExpanded = true;
                current = current.ParentNode;
            }

            // Mark the node as highlighted
            //node.HilightNode();

            // Scroll into view if you have a panel or control logic
            var panel = GetBeepTreeNodeFromPanel(node.NodeSeq);
            if (panel != null)
            {
                panel.Focus();
                VerticalScroll.Value = Math.Min(VerticalScroll.Maximum, panel.Top);
            }
        }
        /// <summary>
        /// Highlights nodes that match the specified predicate.
        /// </summary>
        /// <param name="predicate">A function to test each node for a condition.</param>
        //public void HighlightNodes(Func<BeepTreeNode, bool> predicate)
        //{
        //    foreach (var node in Traverse())
        //    {
        //        if (predicate(node))
        //        {
        //            node.HilightNode(); // Use existing method to highlight
        //        }
        //        else
        //        {
        //            node.UnHilightNode(); // Remove highlight
        //        }
        //    }
        //}
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor = _currentTheme.ButtonBackColor;
            _nodePanels.Values.ToList().ForEach(p => p.BackColor = _currentTheme.ButtonBackColor);
            foreach (BeepTreeNode node in _beeptreeRootnodes)
            {
                node.UseThemeFont = UseThemeFont;
                node.UseScaledFont = UseScaledFont;
                node.Theme = Theme;
                if(UseThemeFont)
                {
                    node.UseThemeFont = true;
                    node.Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                }
                else
                {
                    node.UseThemeFont = false;
                    node.Font = TextFont;
                }
                //     node.ApplyTheme();
            }
        }
        /// <summary>
        /// Selects all nodes in the tree.
        /// </summary>
        public void SelectAllNodes()
        {
            foreach (var node in Traverse())
            {
                node.IsSelected = true; // Will raise NodeSelected event
            }
        }

        /// <summary>
        /// Deselects all nodes in the tree.
        /// </summary>
        public void DeselectAllNodes()
        {
            foreach (var node in Traverse())
            {
                node.IsSelected = false; // Will raise NodeDeselected event
            }
        }
        public void DeselectSelectdNodes()
        {
            // Create a copy of currently selected nodes, as modifying IsSelected will trigger NodeDeselected
            // events that modify SelectedNodes
             if (SelectedNodes == null) return;
            if (SelectedNodes.Count == 0) return;
            var currentlySelected = SelectedNodes.ToList();

            foreach (var node in currentlySelected)
            {
                node.IsSelected = false; // Will raise NodeDeselected, removing it from SelectedNodes
            }
        }
        #endregion "Theme"
        #region "Flyout Menu"
      
        public void ShowFlyoutMenu(object sender , EventArgs e)
        {
            var clickedNode = sender as BeepTreeNode;
            if (clickedNode == null) return;
            List<SimpleItem> menuList = DynamicMenuManager.GetMenuItemsList(clickedNode.NodeInfo as SimpleItem);

            CurrentMenutems = new BindingList<SimpleItem>(menuList);
            if (BeepFlyoutMenu == null)
            {
                BeepFlyoutMenu = new BeepFlyoutMenu();
            }
            // 
            BeepFlyoutMenu.ListItems = CurrentMenutems;
            BeepFlyoutMenu.MenuClicked += MenuItemClicked;
            BeepFlyoutMenu.Show();
        }
        private void MenuItemClicked(object sender, BeepEventDataArgs e)
        {
            
        }
        #endregion "Flyout Menu"
        #region "Popup List Methods"
        protected void TogglePopup()
        {
            CloseOpenPopupMenu();
            ClickedNode.MenuItemSelected += LastNodeMenuShown_MenuItemSelected;
            ClickedNode.ShowContextMenu(CurrentMenutems);
            LastNodeMenuShown = ClickedNode;
        }
        protected void CloseOpenPopupMenu()
        {
            if (LastNodeMenuShown != null)
            {
                LastNodeMenuShown.ClosePopup();
                LastNodeMenuShown.MenuItemSelected -= LastNodeMenuShown_MenuItemSelected;
            }
        }

        private void LastNodeMenuShown_MenuItemSelected(object? sender, SelectedItemChangedEventArgs e)
        {
            
            MenuItemSelected?.Invoke(sender, e);
        }

        private void ClosePopup()
        {
           LastNodeMenuShown.ClosePopup();
            //_isPopupOpen = false;
            //if (_popupForm != null)
            //{
            //    _popupForm.Hide();
            //  //  LastNodeMenuShown = null;

            //}


        }

       
        #endregion "Popup List Methods"
        #region "Expand and Collapse"
        private void ExpandWithAnimation(BeepTreeNode node)
        {
            Timer timer = new Timer { Interval = 10 };
            int step = 5;
            int targetHeight = node.Height + 100; // Example height increase

            timer.Tick += (sender, args) =>
            {
                if (node.Height >= targetHeight)
                {
                    timer.Stop();
                    node.IsExpanded = true;
                }
                else
                {
                    node.Height += step;
                }
            };

            timer.Start();
        }

        #endregion "Expand and Collapse"
    }

}
