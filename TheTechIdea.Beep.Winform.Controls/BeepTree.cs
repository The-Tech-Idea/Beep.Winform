﻿
using System.ComponentModel;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Desktop.Common;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Tree")]
    [Category("Beep Controls")]
    [Description("A control that displays hierarchical data in a tree format.")]
    public class BeepTree : BeepControl
    {
        public int Nodeseq { get; set; } = 0;
        private bool _isUpdatingTree = false;
        // Define the number of nodes to keep rendered above and below the viewport
        private const int BufferSize = 20;
     
        // Keep track of visible nodes
        private List<BeepTreeNode> visibleNodes = new List<BeepTreeNode>();
        public int NodeWidth { get; set; } = 100;
        private BindingList<SimpleItem> rootnodeitems = new BindingList<SimpleItem>();
        private List<BeepTreeNode> _beeptreeRootnodes = new List<BeepTreeNode>();
        private Dictionary<int, Panel> _nodePanels = new Dictionary<int, Panel>();
        private bool _shownodeimage = true;
        private int defaultHeight = 100;
        public event EventHandler<BeepEventDataArgs> NodeClicked;
        public event EventHandler<BeepEventDataArgs> NodeRightClicked;
        public event EventHandler<BeepEventDataArgs> NodeDoubleClicked;
        public event EventHandler<BeepEventDataArgs> NodeExpanded;
        public event EventHandler<BeepEventDataArgs> NodeCollapsed;
        public event EventHandler<BeepEventDataArgs> NodeSelected;
        public event EventHandler<BeepEventDataArgs> NodeDeselected;
        private int _nodeHeight = 40;
        private BeepFlyoutMenu BeepFlyoutMenu;

        private Dictionary<string,SimpleItem> _menus = new Dictionary<string,SimpleItem>();
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Dictionary<string, SimpleItem> ContextMenu
        {
            get => _menus;
            set
            {
                _menus = value;
            }
        }
        private int nodeimagesize = 16;
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
        private int depth;
        private bool _dontRearrange;

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

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public List<BeepTreeNode> SelectedNodes { get; private set; }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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
        public BindingList<SimpleItem> Nodes
        {
            get => rootnodeitems;
            set
            {
                rootnodeitems = value;
               // InitializeTreeFromMenuItems();
            }
        }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public List<BeepTreeNode> NodesControls
        {
            get => _beeptreeRootnodes;
            set
            {
                _beeptreeRootnodes = value;
            }
        }
        public bool ShowCheckBox { get; private set; }
        public BeepTree()
        {
            SetStyle(ControlStyles.ResizeRedraw| ControlStyles.SupportsTransparentBackColor| ControlStyles.AllPaintingInWmPaint| ControlStyles.EnableNotifyMessage| ControlStyles.OptimizedDoubleBuffer, true);
            this.Name = "BeepTree";
            if (rootnodeitems == null)
            {
                rootnodeitems = new BindingList<SimpleItem>();
            }

            _beeptreeRootnodes = new List<BeepTreeNode>();
            rootnodeitems.ListChanged -= Items_ListChanged;
            rootnodeitems.ListChanged += Items_ListChanged;

            ApplyThemeToChilds = false;
            InitLayout();
            // Enable double buffering to reduce flickering
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            Padding = new Padding(5);
            // Initialize scroll event handling for virtualization
            this.AutoScroll = true;
            this.VerticalScroll.Visible = true;
            this.NodeClicked += OnNodeClicked;
            this.NodeSelected += OnNodeSelected;
            this.NodeDeselected += OnNodeDeselected;
        }
        #region "Node Clicked"
        private void OnNodeRightClicked(object sender, BeepEventDataArgs e)
        {
            NodeRightClicked?.Invoke(sender, e);
        }

        private void OnNodeDoubleClicked(object sender, BeepEventDataArgs e)
        {
            NodeDoubleClicked?.Invoke(sender, e);
        }
        private void OnNodeExpanded(object sender, BeepEventDataArgs e)
        {
            NodeExpanded?.Invoke(sender, e);
        }

        private void OnNodeCollapsed(object sender, BeepEventDataArgs e)
        {
            NodeCollapsed?.Invoke(sender, e);
        }

        private void OnNodeDeselected(object? sender, BeepEventDataArgs e)
        {
            var node = sender as BeepTreeNode;
            if (node == null) return;

            // Remove from SelectedNodes if present
            if (SelectedNodes.Contains(node))
                SelectedNodes.Remove(node);
        }

        private void OnNodeSelected(object? sender, BeepEventDataArgs e)
        {
            var node = sender as BeepTreeNode;
            if (node == null) return;

            // Add to SelectedNodes if not already present
            if (!SelectedNodes.Contains(node))
                SelectedNodes.Add(node);
        }

        private BeepTreeNode _lastSelectedNode = null;

        private void OnNodeClicked(object sender, BeepEventDataArgs e)
        {
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

            // Refresh the UI if necessary
            Invalidate();
        }

     
        #endregion "Node Clicked"
        #region Event Handlers
        #endregion Event Handlers
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
        public void ToggleNode(BeepTreeNode node)
        {
            node.IsExpanded = !node.IsExpanded;
            node.RearrangeNode();
            RearrangeTree(); // Adjust layout based on expanded/collapsed state
        }

        #region "Root Nodes Creation"
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

        private void Items_ListChanged(object? sender, ListChangedEventArgs e)
        {
            if (_isUpdatingTree)
            {
                Console.WriteLine("Skipping ListChanged event due to ongoing update.");
                return;
            }

            Console.WriteLine($"ListChanged: Type={e.ListChangedType}, Index={e.NewIndex}");
            try
            {
                _isUpdatingTree = true;

                switch (e.ListChangedType)
                {
                    case ListChangedType.ItemAdded:
                        HandleItemAdded(e.NewIndex);
                        break;

                    case ListChangedType.ItemDeleted:
                        HandleItemDeleted(e.NewIndex);
                        break;

                    case ListChangedType.ItemChanged:
                        HandleItemChanged(e.NewIndex);
                        break;

                    case ListChangedType.Reset:
                        if (rootnodeitems.Count == 0)
                        {
                            ClearNodes();
                        }
                        else
                        {
                            InitializeTreeFromMenuItems();
                        }
                        break;

                    default:
                        Console.WriteLine($"Unhandled ListChangedType: {e.ListChangedType}");
                        break;
                }
            }
            finally
            {
                _isUpdatingTree = false;
            }
        }
        private void HandleItemAdded(int index)
        {

            if (index < 0 || index >= rootnodeitems.Count)
            {
                LogMessage($"Invalid index for addition: {index}");
                return;
            }

            var menuItem = rootnodeitems[index];
            LogMessage($"Handling item addition for index {index}: {menuItem.Text}");

            // Commented out for debugging:
            var node = CreateTreeNodeFromMenuItem(menuItem, null);
            if (node != null)
            {
                _dontRearrange = true;
                Panel panel = AddRootNode(node);
                panel.Tag = node.GuidID;
                menuItem.ContainerGuidID = node.GuidID;
                menuItem.RootContainerGuidID = node.GuidID;
                node.Nodes = menuItem.Children;
                LogMessage($"Node added for item at index {index}: {menuItem.Text}");
                RearrangeTree();
                _dontRearrange = false;
            }
        }
        private void HandleItemDeleted(int index)
        {
            if (index < 0 || index >= _beeptreeRootnodes.Count)
            {
                LogMessage($"Invalid index for deletion: {index}");
                return;
            }

            var node = GetNode(index);
            if (node != null)
            {
                RemoveNode(node);
                LogMessage($"Node removed for item at index {index}");
                RearrangeTree();
            }
        }
      
        public void RemoveNode(SimpleItem simpleItem)
        {
            if (simpleItem == null)
            {
                return;
            }
            var node = _beeptreeRootnodes.FirstOrDefault(n => n.Tag == simpleItem);
            if (node != null)
            {
                RemoveNode(node);
            }
        }
        private void HandleItemChanged(int index)
        {
            if (index < 0 || index >= rootnodeitems.Count || index >= _beeptreeRootnodes.Count)
            {
                Console.WriteLine($"Invalid index for update: {index}");
                return;
            }

            var menuItem = rootnodeitems[index];
            var node = GetNode(index);
            if (node != null && menuItem != null)
            {
                node.Text = menuItem.Text;
                node.ImagePath = menuItem.ImagePath;
                node.Children = menuItem.Children; // Sync children
                //node.RearrangeNode();
                Console.WriteLine($"Node updated for item at index {index}: {menuItem.Text}");
                RearrangeTree();
            }
        }
        private void InitializeTreeFromMenuItems()
        {
            LogMessage("Initialize Tree");
            ClearNodes(); // Clear existing nodes
            LogMessage("Items Count: " + rootnodeitems.Count);
            _dontRearrange = true;
            foreach (var menuItem in rootnodeitems)
            {
                try
                {
                    LogMessage("MenuItem: " + menuItem.Text);
                    var node = CreateTreeNodeFromMenuItem(menuItem, null);
                    LogMessage("Created Node: " + node.Text);
                    if (node == null)
                    {
                        continue;
                    }

                    Panel panel= AddRootNode(node);
                    menuItem.ContainerGuidID = node.GuidID;
                    menuItem.RootContainerGuidID = node.GuidID;
                    panel.Tag = node.GuidID;
                    LogMessage("Node Added");

                }
                catch (Exception ex)
                {
                    LogMessage("Error: " + ex.Message);
                }
            }
            RearrangeTree();
            _dontRearrange = false;
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
                    Level = parent?.Level + 1 ?? 1 // Increment level for child nodes
                };
                node.Text = name ?? $"Node{seq}";
                node.NodeClicked += (sender, e) => NodeClicked?.Invoke(sender, e);
                node.NodeRightClicked += (sender, e) => NodeRightClicked?.Invoke(sender, e);
                node.NodeDoubleClicked += (sender, e) => NodeDoubleClicked?.Invoke(sender, e);
                node.ShowMenu += (sender, e) => ShowFlyoutMenu(node, new Point(node.Left, node.Top));
                node.NodeExpanded += (sender, e) => NodeExpanded?.Invoke(sender, e);
                node.NodeCollapsed += (sender, e) => NodeCollapsed?.Invoke(sender, e);
                node.NodeSelected += (sender, e) => NodeSelected?.Invoke(sender, e);
                node.NodeDeselected += (sender, e) => NodeDeselected?.Invoke(sender, e);
                node.MouseHover += (sender, e) => { node.HilightNode(); };
                node.MouseLeave += (sender, e) => { node.UnHilightNode(); };
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
                    ImagePath = menuItem.ImagePath,
                    ParentNode = parent,
                    Tree = this,
                    NodeSeq = Nodeseq,
                    Height = NodeHeight,
                    IsBorderAffectedByTheme = false,
                    IsShadowAffectedByTheme = false,
                    MaxImageSize= NodeImageSize,
                    IsFramless = true,
                    IsChild = true,
                    Size = new Size(this.Width, NodeHeight),
                    Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                    Theme = this.Theme,
                    Tag = menuItem,
                   
                    NodeDataType = menuItem.GetType().Name,
                    Level = parent?.Level + 1 ?? 1
                };
                node.Text = menuItem.Text ?? $"Node{seq}";
                node.NodeClicked += OnNodeClicked;
                node.NodeRightClicked += OnNodeRightClicked;
                node.NodeDoubleClicked += NodeDoubleClicked;
                node.ShowMenu += (sender, e) => ShowFlyoutMenu(node, new Point(node.Left, node.Top));
                node.NodeExpanded += OnNodeExpanded;
                node.NodeCollapsed += OnNodeCollapsed;
                node.NodeSelected += OnNodeSelected;
                node.NodeDeselected += OnNodeDeselected;
                node.MouseHover += (sender, e) => { node.HilightNode(); };
                node.MouseLeave += (sender, e) => { node.UnHilightNode(); };
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

        #endregion "Root Nodes Creation"

        /// <summary>
        /// Populates the tree from a hierarchical data structure.
        /// </summary>
        /// <param name="data">The data to populate the tree with.</param>
        public void PopulateTree(IEnumerable<SimpleItem> data)
        {
            Nodes.Clear();
            foreach (var item in data.Where(p=>p.ParentItem==null))
            {
                Nodes.Add(item);
            }
        }
        public void ClearNodes()
        {
            foreach (var panel in _nodePanels)
            {
                Controls.Remove(panel.Value);
            }
            _nodePanels.Clear();
        }
        public void RemoveNode(string NodeName)
        {
            // find the node by travers the list and child nodes
            foreach (var panel in _nodePanels)
            {
                BeepTreeNode node = GetBeepTreeNodeFromPanel(panel.Key);
                if (node.Text == NodeName)
                {
                    RemoveNode(node);
                }
            }
          
        }
        public void RemoveNode(int NodeIndex)
        {
            if (NodeIndex >= 0 && NodeIndex < _beeptreeRootnodes.Count)
            {
                RemoveNode(GetNode(NodeIndex));
            }

        }
        private void RemoveNode(BeepTreeNode node)
        {
            _beeptreeRootnodes.Remove(node);
            Controls.Remove(node);
            _nodePanels.Remove(node.NodeSeq);
            RearrangeTree();
        }
        #region "Find and Filter"
        public IEnumerable<BeepTreeNode> Traverse()
        {
            foreach (var node in _beeptreeRootnodes)
            {
                yield return node;
                foreach (var child in node.Traverse())
                {
                    yield return child;
                }
            }
        }
        /// <summary>
        /// Searches for the first node that contains the specified text (case-insensitive).
        /// </summary>
        /// <param name="searchText">The text to search for.</param>
        /// <returns>The first matching node, or null if no match is found.</returns>
        /// <summary>
        /// Searches for the first node containing the specified text in its `NodesControls` representation
        /// and dynamically expands the necessary nodes to display the result.
        /// </summary>
        /// <param name="searchText">The text to search for.</param>
        /// <returns>The first matching node, or null if no match is found.</returns>
        public BeepTreeNode SearchNode(string searchText)
        {
            // Search for the matching item in the NodesControls (backing data)
            var menuItem = rootnodeitems.FirstOrDefault(item =>
                !string.IsNullOrEmpty(item.Text) &&
                item.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase));

            if (menuItem != null)
            {
                // Find the root node containing this item
                var rootNode = _beeptreeRootnodes.FirstOrDefault(node => node.Tag == menuItem);
                if (rootNode == null)
                {
                    // If the root node is not yet created, create it
                    //rootNode = CreateTreeNodeFromMenuItem(menuItem, null);
                    //AddRootNode(rootNode);
                    Nodes.Add(menuItem);
                }

                // Expand and render child nodes for the root
                ExpandRootToFindNode(rootNode, menuItem);

                // Return the matching node
                var matchingNode = rootNode.Traverse().FirstOrDefault(n => n.Tag == menuItem);
                if (matchingNode != null)
                {
                    HighlightNode(matchingNode);
                    return matchingNode;
                }
            }

            return null;
        }
        /// <summary>
        /// Highlights a node and ensures it is scrolled into view.
        /// </summary>
        /// <param name="node">The node to highlight.</param>
        private void HighlightNode(BeepTreeNode node)
        {
            if (node == null) return;

            BeepTreeNode current = node.ParentNode;
            while (current != null)
            {
                current.IsExpanded = true;
                current = current.ParentNode;
            }

            node.HilightNode();

            var panel = GetBeepTreeNodeFromPanel(node.NodeSeq);
            if (panel != null)
            {
                panel.Focus();
                VerticalScroll.Value = Math.Min(VerticalScroll.Maximum, panel.Top);
            }
        }

        /// <summary>
        /// Finds the first node whose text exactly matches the specified text.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <returns>The first matching node, or null if no match is found.</returns>
        /// <summary>
        /// Finds the first node whose text exactly matches the specified text and ensures it is fully expanded and rendered.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <returns>The first matching node, or null if no match is found.</returns>
        public BeepTreeNode FindNode(string text)
        {
            // Search in the backing data (NodesControls) to locate the target menu item
            var menuItem = rootnodeitems.FirstOrDefault(item =>
                !string.IsNullOrEmpty(item.Text) &&
                item.Text.Equals(text, StringComparison.Ordinal));

            if (menuItem != null)
            {
                // Locate or create the root node corresponding to the menu item
                var rootNode = _beeptreeRootnodes.FirstOrDefault(node => node.Tag == menuItem);
                if (rootNode == null)
                {
                    //rootNode = CreateTreeNodeFromMenuItem(menuItem, null);
                    //AddRootNode(rootNode);
                    Nodes.Add(menuItem);
                }

                // Expand and render the required child nodes
                ExpandRootToFindNode(rootNode, menuItem);

                // Return the fully rendered node
                var matchingNode = rootNode.Traverse().FirstOrDefault(n => n.Tag == menuItem);
                if (matchingNode != null)
                {
                    HighlightNode(matchingNode);
                    return matchingNode;
                }
            }

            return null;
        }


        /// <summary>
        /// Gets a node by its index in the internal node list.
        /// </summary>
        /// <param name="index">The index of the node.</param>
        /// <returns>The node at the specified index, or null if the index is out of range.</returns>
        private BeepTreeNode GetNodeByIndex(int index)
        {
            return index >= 0 && index < _beeptreeRootnodes.Count ? _beeptreeRootnodes[index] : null;
        }

        /// <summary>
        /// Filters nodes based on the provided predicate.
        /// NodesControls that match the predicate or have descendants matching the predicate remain visible.
        /// </summary>
        /// <param name="predicate">A function to determine if a node should be visible.</param>
        public void FilterNodes(Func<BeepTreeNode, bool> predicate)
        {
            // Traverse all nodes to apply the filter
            foreach (var node in Traverse())
            {
                // Determine visibility based on the predicate
                bool isVisible = predicate(node);

                // Ensure parent nodes of visible nodes remain visible
                if (isVisible && node.ParentNode != null)
                {
                    var parent = node.ParentNode;
                    while (parent != null)
                    {
                        parent.IsVisible = true;
                        parent = parent.ParentNode;
                    }
                }

                // Update the node's visibility
                node.IsVisible = isVisible;

                // Dynamically load child nodes if the node is visible and expanded
                if (node.IsVisible && node.IsExpanded && node.NodesControls.Count == 0 && node.Tag is SimpleItem menuItem)
                {
                  //  CreateChildNodes(node, menuItem);
                }
            }

            // Rearrange the tree after filtering
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

        /// <summary>
        /// Finds the first node matching the specified predicate, dynamically expanding nodes as needed.
        /// </summary>
        /// <param name="predicate">A function to determine the node to find.</param>
        /// <returns>The first matching node, or null if no match is found.</returns>
        public BeepTreeNode FindNode(Func<BeepTreeNode, bool> predicate)
        {
            foreach (var rootNode in _beeptreeRootnodes)
            {
                // Traverse the root node and its children
                var matchingNode = FindNodeInHierarchy(rootNode, predicate);
                if (matchingNode != null)
                {
                    // Highlight and focus the found node
                    HighlightNode(matchingNode);
                    return matchingNode;
                }
            }

            return null;
        }

        /// <summary>
        /// Recursively searches for a node matching the predicate within a node hierarchy.
        /// Dynamically expands nodes if needed.
        /// </summary>
        /// <param name="currentNode">The node to start searching from.</param>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The matching node, or null if no match is found.</returns>
        private BeepTreeNode FindNodeInHierarchy(BeepTreeNode currentNode, Func<BeepTreeNode, bool> predicate)
        {
            // Check if the current node matches
            if (predicate(currentNode))
            {
                return currentNode;
            }

            // Expand the node to ensure children are available
            if (currentNode.NodesControls.Count == 0 && currentNode.Tag is SimpleItem menuItem)
            {
                //CreateChildNodes(currentNode, menuItem);
            }

            // Search the children recursively
            foreach (var childNode in currentNode.NodesControls)
            {
                var matchingNode = FindNodeInHierarchy(childNode, predicate);
                if (matchingNode != null)
                {
                    return matchingNode;
                }
            }

            return null;
        }


        /// <summary>
        /// Finds all nodes that match the specified predicate.
        /// </summary>
        /// <param name="predicate">A function to test each node for a condition.</param>
        /// <returns>An enumerable of matching nodes.</returns>
        public IEnumerable<BeepTreeNode> FindNodes(Func<BeepTreeNode, bool> predicate)
        {
            return Traverse().Where(predicate);
        }


        public SimpleItem GetNode(string NodeName)
        {
            foreach (SimpleItem item in Nodes)
            {
                if (item.Name == NodeName) return item;
                
            }
            return Nodes.FirstOrDefault(c => c.Name == NodeName);
        }
        public BeepTreeNode GetBeepTreeNode(SimpleItem node)
        {
            foreach (BeepTreeNode item in _beeptreeRootnodes)
            {
                if (item.Tag == node) return item;
                
            }
            return null;

        }
     
        public BeepTreeNode GetBeepTreeNode(string NodeName)
        {
            foreach (BeepTreeNode item in _beeptreeRootnodes)
            {
                if (item.Name == NodeName) return item;
              
            }
            return null;

        }
        public BeepTreeNode GetBeepTreeNodeByGuid(string guidid)
        {
            foreach (BeepTreeNode item in _beeptreeRootnodes)
            {
                if (item.GuidID == guidid) return item;

            }
            return null;

        }

        public SimpleItem GetNodeByGuidID(string guidID)
        {
            foreach (SimpleItem item in Nodes)
            {
                if (item.Name == guidID) return item;
               











































































































            }
            return null;
        }
       
        public SimpleItem GetNode(int NodeIndex)
        {
            return NodeIndex >= 0 && NodeIndex < Nodes.Count ? Nodes[NodeIndex] : null;
        }
        public List<BeepTreeNode> GetNodes()
        {
            return _beeptreeRootnodes;
        }
        #endregion "Find and Filter"
        #region "Theme"
        /// <summary>
        /// Highlights nodes that match the specified predicate.
        /// </summary>
        /// <param name="predicate">A function to test each node for a condition.</param>
        public void HighlightNodes(Func<BeepTreeNode, bool> predicate)
        {
            foreach (var node in Traverse())
            {
                if (predicate(node))
                {
                    node.HilightNode(); // Use existing method to highlight
                }
                else
                {
                    node.UnHilightNode(); // Remove highlight
                }
            }
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor = _currentTheme.PanelBackColor;
            _nodePanels.Values.ToList().ForEach(p => p.BackColor = _currentTheme.PanelBackColor);
            foreach (BeepTreeNode node in _beeptreeRootnodes)
            {
                node.Theme = Theme;
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
            var currentlySelected = SelectedNodes.ToList();

            foreach (var node in currentlySelected)
            {
                node.IsSelected = false; // Will raise NodeDeselected, removing it from SelectedNodes
            }
        }
        #endregion "Theme"
        public async Task LoadTreeAsync(IEnumerable<SimpleItem> data)
        {
          
            await Task.Run(() =>
            {_dontRearrange = true;
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
            foreach (var item in data.Where(p=>p.ParentItem==null))
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
           
            return _beeptreeRootnodes.FirstOrDefault(p=>p.NodeSeq==seq);
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
                    var childNode = currentNode.NodesControls.FirstOrDefault(n => n.Tag == childItem);
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
                item.ChangeNodeImageSettings(); // Ensure the node redraws with the updated setting
            }
        }
        private void LogMessage(string message)
        {
            try
            {
                File.AppendAllText(@"C:\Logs\debug_log.txt", $"{DateTime.Now}: {message}{Environment.NewLine}");

            }
            catch { /* Ignore logging errors */ }
        }
        #region "Flyout Menu"
        public void ShowMenu(BeepTreeNode node,BeepEventDataArgs e)
        {
            
            SimpleItem item;
            IBranch branch;
            string menuid = "";
            if (node.Tag is SimpleItem)
            {
                item = (SimpleItem)node.Tag;
                menuid = item.MenuID;
            }
            else
            {
                branch = (IBranch)node.Tag;
                menuid = branch.MenuID;
            }
            List<SimpleItem> menu = _menus.Where(c => c.Key == menuid).Select(c => c.Value).ToList();
            BeepFlyoutMenu.ListItems = new BindingList<SimpleItem>(menu);
            BeepFlyoutMenu.Show();
        }
        public void ShowFlyoutMenu(BeepTreeNode node, Point location)
        {
            SimpleItem item;
            IBranch branch;
            string menuid = "";
            if (BeepFlyoutMenu == null)
            {
                BeepFlyoutMenu = new BeepFlyoutMenu();
                
            }
            // 
            if(node.Tag is SimpleItem)
            {
                item = (SimpleItem)node.Tag;
                menuid = item.MenuID;

            }
            else
            {
                branch = (IBranch)node.Tag;
                menuid = branch.MenuID;
            }
            List<SimpleItem> menu = _menus.Where(c => c.Key == menuid).Select(c => c.Value).ToList();
            BeepFlyoutMenu.ListItems = new BindingList<SimpleItem>(menu);
            BeepFlyoutMenu.MenuClicked += MenuItemClicked;
            BeepFlyoutMenu.Show();
        }
        private void MenuItemClicked(object sender, BeepEventDataArgs e)
        {
            
        }
        #endregion "Flyout Menu"
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
        public void RearrangeTree()
        {
            SuspendLayout(); // Suspend layout updates for performance
            int startY = 5; // Initial Y offset

            foreach (var node in _beeptreeRootnodes)
            {
                if (!_nodePanels.TryGetValue(node.NodeSeq, out var panel))
                    continue;
                // Position the panel
           //     panel.Location = new Point(5, startY);
                panel.Width = this.Width - 10; // Adjust panel width based on tree width
                node.RearrangeNode(); // Adjust the node's internal layout
                startY += panel.Height; // Increment Y position based on panel's height
            }
            ResumeLayout(); // Resume layout updates
           // Invalidate(); // Redraw the tree
        }
        public void SetAllNodesCheckBoxVisibility(bool show)
        {
            // Iterate over all nodes that are directly within the _beeptreeRootnodes collection
            // These are top-level nodes or root nodes in the tree
            foreach (var node in _beeptreeRootnodes)
            {
                node.ToggleCheckBoxVisibility(show);
            }

            // Optionally, you can store this setting if needed for future reference
            // For instance: 
            this.ShowCheckBox = show; 
            // if you keep a similar property in the BeepTree itself.
        }
       

    }

}
