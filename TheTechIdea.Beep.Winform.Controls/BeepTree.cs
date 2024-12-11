using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepTree : BeepControl
    {
        private int nodeseq = 0;
        private bool _isUpdatingTree = false;
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
                AddNode(node);
                LogMessage($"Node added for item at index {index}: {menuItem.Text}");
                RearrangeTree();
            }
        }
        private void HandleItemDeleted(int index)
        {
            if (index < 0 || index >= _beeptreenodes.Count)
            {
                Console.WriteLine($"Invalid index for deletion: {index}");
                return;
            }

            var node = GetNode(index);
            if (node != null)
            {
                RemoveNode(node);
                Console.WriteLine($"Node removed for item at index {index}");
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
               // node.Nodes = menuItem.Children;
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
            Console.WriteLine("Initialize Tree");
            ClearNodes(); // Clear existing nodes
            Console.WriteLine("Items Count: " + rootnodeitems.Count);
            foreach (var menuItem in rootnodeitems)
            {
                try
                {
                    Console.WriteLine("MenuItem: " + menuItem.Text);
                    var node = CreateTreeNodeFromMenuItem(menuItem, null);
                    Console.WriteLine("Created Node: " + node.Text);
                    if(node == null)
                    {
                        continue;
                    }
                    AddNode(node);
               

                    Console.WriteLine("Node Added");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
               
            }

            RearrangeTree();
        }
        private BeepTreeNode CreateTreeNodeFromMenuItem(SimpleItem menuItem, BeepTreeNode parent)
        {
           
            try
            {
                if (depth > 100) // Arbitrary limit to prevent infinite loops
                {
                    LogMessage("Recursion depth exceeded");
                    return null;
                }
                LogMessage($"Creating Node: {menuItem.Text}, Depth: {depth}");
                Console.WriteLine("Creating Node: " + menuItem.Text);
                var node = new BeepTreeNode
                {
                    Name = menuItem.Text ?? $"Node{nodeseq}",
                    NodeLevelID =parent==null ? nodeseq++.ToString():parent.NodeLevelID + "_"+ parent.ChildNodesSeq,
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
                node.NodeClicked += (sender, e) => NodeClicked?.Invoke(sender, e);
                node.NodeRightClicked += (sender, e) => NodeRightClicked?.Invoke(sender, e);
                node.NodeDoubleClicked += (sender, e) => NodeDoubleClicked?.Invoke(sender, e);
                node.ShowMenu += (sender, e) => ShowFlyoutMenu(node,new Point(node.Left,node.Top));
                node.NodeExpanded += (sender, e) => NodeExpanded?.Invoke(sender, e);
                node.NodeCollapsed += (sender, e) => NodeCollapsed?.Invoke(sender, e);
                node.NodeSelected += (sender, e) => NodeSelected?.Invoke(sender, e);
                node.NodeDeselected += (sender, e) => NodeDeselected?.Invoke(sender, e);
                node.MouseHover += (sender, e) => { node.HilightNode(); };
                node.MouseLeave += (sender, e) => { node.UnHilightNode(); };
                Console.WriteLine("Node Created: " + node.Text);
                LogMessage($"Creating Node Childern: {menuItem.Text}, Depth: {depth}");
                foreach (var childMenuItem in menuItem.Children)
                {
                    Console.WriteLine("Child Node: " + childMenuItem.Text);
                    LogMessage($"Child Node:  {childMenuItem.Text} ");
                    var childNode = CreateTreeNodeFromMenuItem(childMenuItem, node);
                    node.AddChild(childNode);
                }
                if(parent!=null)
                {
                    int index = Nodes.IndexOf(menuItem);
                    Nodes[index].Children.Add(menuItem);
                }
                return node;

            }
            catch (Exception ex)
            {
                LogMessage($"Erro in creating node:  {ex.Message} ");
                Console.WriteLine("Error: " + ex.Message);
                return new BeepTreeNode();
            }
        }
        private void AddNode(BeepTreeNode node)
        {
            var panel = new Panel
            {
                Width = Width,
                Height = node.Height,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
               
            };
            _beeptreenodes.Add(node);
            panel.Controls.Add(node); // Add the node to the panel
            Controls.Add(panel); // Add the panel to the tree
            _nodePanels.Add(node.NodeSeq,panel);

            RearrangeTree();
        }

        private void RemoveNode(BeepTreeNode node)
        {
            _beeptreenodes.Remove(node);
            Controls.Remove(node);
            _nodePanels.Remove(node.NodeSeq);
            RearrangeTree();
        }
        /// <summary>
        /// Populates the tree from a hierarchical data structure.
        /// </summary>
        /// <param name="data">The data to populate the tree with.</param>
        public void PopulateTree(IEnumerable<SimpleItem> data)
        {
            Nodes.Clear();
            foreach (var item in data)
            {
                Nodes.Add(item);
            }
        }
        public void AddNode(SimpleItem item, BeepTreeNode parent=null)
        {
            // Commented out for debugging:
            //int parentIndex = Nodes.IndexOf(parent.Tag as SimpleItem);
            //int _childnodesindex= _beeptreenodes.IndexOf(parent);
            //AddSimpleitemtoNode(parent,item );
           
            if(parent != null)
            {
                int index = Nodes.IndexOf(parent.Tag as SimpleItem);
                rootnodeitems[index].Children.Add(item);
                int parentreenodeindex = _beeptreenodes.IndexOf(parent);
                _beeptreenodes[parentreenodeindex].AddChild(CreateTreeNodeFromMenuItem(item, parent));
            }
            else
            {
                rootnodeitems.Add(item);
                AddNode(CreateTreeNodeFromMenuItem(item, null));
            }
          
        }
       
        private void AddSimpleitemtoNode(BeepTreeNode node, SimpleItem item)
        {
            var childNode = CreateTreeNodeFromMenuItem(item, node);
            node.AddChild(childNode);
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
            if (NodeIndex >= 0 && NodeIndex < _beeptreenodes.Count)
            {
                RemoveNode(GetNode(NodeIndex));
            }
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
        public void RearrangeTree()
        {
            int startY = 5; // Initial offset

            foreach (var panel in _nodePanels)
            {
                try
                {
                    BeepTreeNode node = GetBeepTreeNodeFromPanel(panel.Key);
                    Console.WriteLine("Node: " + node.Text);
                    Panel panel1 = panel.Value;
                  //  panel1.BackColor = _currentTheme.PanelBackColor;
                    panel1.Location = new Point(5, startY);
                    panel1.Width = Width - 10; // Adjust panel width
                    LogMessage($"Rearranging Node: {node.Text} width {panel1.Width}");
                    node.RearrangeNode();
                    startY += panel1.Height + 5; // Add spacing
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
               
            }

            Invalidate();
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
              //  File.AppendAllText(@"C:\Logs\debug_log.txt", $"{DateTime.Now}: {message}{Environment.NewLine}");

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
    }

}
