
using System.ComponentModel;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Desktop.Common;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Tree")]
    [Category("Beep Controls")]
    [Description("A control that displays hierarchical data in a tree format.")]
    public class BeepTree : BeepControl
    {
        private int nodeseq = 0;
        private bool _isUpdatingTree = false;
        // Define the number of nodes to keep rendered above and below the viewport
        private const int BufferSize = 20;
     
        // Keep track of visible nodes
        private List<BeepTreeNode> visibleNodes = new List<BeepTreeNode>();
        public int NodeWidth { get; set; } = 100;
        private BindingList<SimpleItem> rootnodeitems = new BindingList<SimpleItem>();
        private List<BeepTreeNode> _beeptreenodes = new List<BeepTreeNode>();
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
  
        public List<BeepTreeNode> SelectedNodes { get; private set; }

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

        public bool ShowCheckBox { get; private set; }

        public BeepTree()
        {
            this.Name = "BeepTree";
            if (rootnodeitems == null)
            {
                rootnodeitems = new BindingList<SimpleItem>();
            }

            _beeptreenodes = new List<BeepTreeNode>();
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
                DeselectAllNodes();          // This will set IsSelected=false on previously selected nodes
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
                        DeselectAllNodes();

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
                    DeselectAllNodes();
                    clickedNode.IsSelected = true; // Raises NodeSelected event
                    _lastSelectedNode = clickedNode;
                }
            }

            // Refresh the UI if necessary
            Invalidate();
        }

        public void DeselectAllNodes()
        {
            // Create a copy of currently selected nodes, as modifying IsSelected will trigger NodeDeselected
            // events that modify SelectedNodes
            var currentlySelected = SelectedNodes.ToList();

            foreach (var node in currentlySelected)
            {
                node.IsSelected = false; // Will raise NodeDeselected, removing it from SelectedNodes
            }
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
                AddNode(node);
                LogMessage($"Node added for item at index {index}: {menuItem.Text}");
                RearrangeTree();
                _dontRearrange = false;
            }
        }
        private void HandleItemDeleted(int index)
        {
            if (index < 0 || index >= _beeptreenodes.Count)
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
        private void HandleItemChanged(int index)
        {
            if (index < 0 || index >= rootnodeitems.Count || index >= _beeptreenodes.Count)
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
               // node.Nodes = parentItem.Children;
                node.RearrangeNode();
                Console.WriteLine($"Node updated for item at index {index}: {menuItem.Text}");
                RearrangeTree();
            }
        }
        public BeepTreeNode SearchNode(string searchText)
        {
            return Traverse().FirstOrDefault(node => node.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }
        public void ToggleNode(BeepTreeNode node)
        {
            node.IsExpanded = !node.IsExpanded;
            node.RearrangeNode();
            RearrangeTree(); // Adjust layout based on expanded/collapsed state
        }
        public IEnumerable<BeepTreeNode> Traverse()
        {
            foreach (var node in _beeptreenodes)
            {
                yield return node;
                foreach (var child in node.Traverse())
                {
                    yield return child;
                }
            }
        }
        public BeepTreeNode FindNode(string text)
        {
            return Traverse().FirstOrDefault(node => node.Text == text);
        }
        private BeepTreeNode GetNodeByIndex(int index)
        {
            if (index < 0 || index >= _beeptreenodes.Count) return null;
            return _beeptreenodes[index];
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
                    if(node == null)
                    {
                        continue;
                    }
                  
                    AddNode(node);
                   

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
        public BeepTreeNode CreateTreeNode(string name,string imagepath,string BranchGuid,string typename, BeepTreeNode parent)
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
                    seq = nodeseq++; ;
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
                //    LogMessage($"Creating Node Childern: {menuItem.Text}, Depth: {depth}");

                //foreach (var childMenuItem in parentItem.Children)
                //{
                //    Console.WriteLine("Child Node: " + childMenuItem.Text);
                //    LogMessage($"adding Child Node:  {childMenuItem.Text} ");
                //    var childNode = CreateTreeNode(childMenuItem, node);
                //    node.AddChild(childNode);
                //}
                return node;

            }
            catch (Exception ex)
            {
                //   LogMessage($"Erro in creating node:  {ex.Message} ");
                Console.WriteLine("Error: " + ex.Message);
                return new BeepTreeNode();
            }
        }

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
                    seq = nodeseq++;
                }
                LogMessage($"Creating Node: {menuItem.Text}, Depth: {depth}");
                Console.WriteLine("Creating Node: " + menuItem.Text);
                var node = new BeepTreeNode
                {
                    Name = menuItem.Text ?? $"Node{seq}",
                    NodeLevelID =parent==null ? seq.ToString():parent.NodeLevelID + "_"+ parent.ChildNodesSeq,
                    Text = menuItem.Text ,
                    ImagePath = menuItem.ImagePath,
                    ParentNode = parent,
                    Tree = this,
                    NodeSeq = nodeseq,
                    Height = NodeHeight,
                    IsBorderAffectedByTheme =false,
                    IsShadowAffectedByTheme = false,
                    IsFramless = true,
                    IsChild=true,
                    Size = new Size(this.Width, NodeHeight),
                    Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                    Theme = this.Theme,
                    Tag = menuItem,
                    NodeDataType =menuItem.GetType().Name,
                    Level = parent?.Level + 1 ?? 1 // Increment level for child nodes
                };
                node.Text = menuItem.Text ?? $"Node{seq}";
                node.NodeClicked +=OnNodeClicked;
                node.NodeRightClicked += OnNodeRightClicked;
                node.NodeDoubleClicked += NodeDoubleClicked;
                node.ShowMenu += (sender, e) => ShowFlyoutMenu(node,new Point(node.Left,node.Top));
                node.NodeExpanded += OnNodeExpanded;
                node.NodeCollapsed += OnNodeCollapsed;
                node.NodeSelected += OnNodeSelected;
                node.NodeDeselected += OnNodeDeselected;
                node.MouseHover += (sender, e) => { node.HilightNode(); };
                node.MouseLeave += (sender, e) => { node.UnHilightNode(); };
                Console.WriteLine("Node Created: " + node.Text);
                LogMessage($"Creating Node Childern: {menuItem.Text}, Depth: {depth}");
               
                //foreach (var childMenuItem in parentItem.Children)
                //{
                //    Console.WriteLine("Child Node: " + childMenuItem.Text);
                //    LogMessage($"adding Child Node:  {childMenuItem.Text} ");
                //    var childNode = CreateTreeNode(childMenuItem, node);
                //    node.AddChild(childNode);
                //}
                return node;

            }
            catch (Exception ex)
            {
                LogMessage($"Erro in creating node:  {ex.Message} ");
                Console.WriteLine("Error: " + ex.Message);
                return new BeepTreeNode();
            }
        }
      
        public void CreateChildNodes(BeepTreeNode parent,SimpleItem parentItem)
        {
            if (parent == null)
            {
                return;
            }
            if (parentItem.Children.Count == 0)
            {
                return;
            }
            foreach (var item in parentItem.Children)
            {

                var node = CreateTreeNodeFromMenuItem(item, parent);
                if (node != null)
                {
                    parent.AddChild(node);
                }
            }
        }
        public void AddNode(BeepTreeNode node)
        {
            var panel = new Panel
            {
                Width = Width,
                Height = node.Height,
                AutoSize = true,
                Dock = DockStyle.Top,
                
                AutoSizeMode = AutoSizeMode.GrowAndShrink
               
            };

            _beeptreenodes.Add(node);
            panel.Controls.Add(node); // Add the node to the panel
            Controls.Add(panel); // Add the panel to the tree
            _nodePanels.Add(node.NodeSeq,panel);
            // Subscribe to node's expansion and collapse events
            node.NodeExpanded += (s, e) => RearrangeTree();
            node.NodeCollapsed += (s, e) => RearrangeTree();
          //  node.RearrangeNode();
            if (!_dontRearrange)
                RearrangeTree();
        }
       
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
        //public void AddNode(SimpleItem item, BeepTreeNode parent=null)
        //{
        //    if(parent != null)
        //    {
        //        int index = Nodes.IndexOf(parent.Tag as SimpleItem);
        //        rootnodeitems[index].Children.Add(item);
        //        int parentreenodeindex = _beeptreenodes.IndexOf(parent);
        //        _beeptreenodes[parentreenodeindex].AddChild(CreateTreeNode(item, parent));
        //        _beeptreenodes[parentreenodeindex].RearrangeNode();
        //    }
        //    else
        //    {
        //        rootnodeitems.Add(item);
        //        AddNode(CreateTreeNode(item, null));
        //    }
          
        //}
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
            if (NodeIndex >= 0 && NodeIndex < _beeptreenodes.Count)
            {
                RemoveNode(GetNode(NodeIndex));
            }

        }
        private void RemoveNode(BeepTreeNode node)
        {
            _beeptreenodes.Remove(node);
            Controls.Remove(node);
            _nodePanels.Remove(node.NodeSeq);
            RearrangeTree();
        }
        public BeepTreeNode GetNode(string NodeName)
        {
            return _beeptreenodes.FirstOrDefault(c => c.Name == NodeName);
        }
        public BeepTreeNode GetNode(int NodeIndex)
        {
            return NodeIndex >= 0 && NodeIndex < _beeptreenodes.Count ? _beeptreenodes[NodeIndex] : null;
        }
        public List<BeepTreeNode> GetNodes()
        {
            return _beeptreenodes;
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor =_currentTheme.PanelBackColor ;
            _nodePanels.Values.ToList().ForEach(p => p.BackColor = _currentTheme.PanelBackColor);
            foreach (BeepTreeNode node in _beeptreenodes)
            {
                node.Theme = Theme;
           //     node.ApplyTheme();
            }
        }
        //public void RearrangeTree()
        //{
        //    SuspendLayout(); // Suspend layout updates
        //    int startY = 5; // Initial offset

        //    foreach (var panel in _nodePanels)
        //    {
        //        try
        //        {
        //            BeepTreeNode node = GetBeepTreeNodeFromPanel(panel.Key);
        //            Console.WriteLine("Node: " + node.Text);
        //            Panel panel1 = panel.Value;
        //            panel1.Location = new Point(5, startY);
        //            panel1.Width = Width - 10; // Adjust panel width
        //            LogMessage($"Rearranging Node: {node.Text} width {panel1.Width}");
        //            node.RearrangeNode();
        //            startY += panel1.Height + 5; // Add spacing
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error: " + ex.Message);
        //        }

        //    }
        //    ResumeLayout(); // Resume layout updates
        //    Invalidate();
        //}
      
        public async Task LoadTreeAsync(IEnumerable<SimpleItem> data)
        {
          
            await Task.Run(() =>
            {
                SuspendLayout(); // Suspend layout updates
                // Perform data processing here
                var nodes = CreateTreeNodes(data);
              
                // Return the processed nodes
                return nodes;
            }).ContinueWith(nodes =>
            {
                _dontRearrange = true;
                // Update the UI with the loaded nodes
                foreach (var node in nodes.Result)
                {
                    
                    AddNode(node);
                }
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
                    AddNode(node);
                }
            }
            return nodes;
        }
        private BeepTreeNode GetBeepTreeNodeFromPanel(int seq)
        {
           
            return _beeptreenodes.FirstOrDefault(p=>p.NodeSeq==seq);
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
        private void ChangeNodeImageSettings()
        {
            foreach (var item in _beeptreenodes)
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
            foreach (var rootNode in _beeptreenodes.Where(n => n.ParentNode == null))
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
            foreach (var child in _beeptreenodes)
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

            foreach (var node in _beeptreenodes)
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
            // Iterate over all nodes that are directly within the _beeptreenodes collection
            // These are top-level nodes or root nodes in the tree
            foreach (var node in _beeptreenodes)
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
