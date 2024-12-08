using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepTree : BeepControl
    {
        private int nodeseq = 0;
        public int NodeWidth { get; set; } = 100;
        private SimpleItemCollection items = new SimpleItemCollection();
        private List<BeepTreeNode> _childnodes = new List<BeepTreeNode>();
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
        private int _nodeHeight = 30;
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
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleItemCollection Nodes
        {
            get => items;
            set
            {
                items = value;
               // InitializeTreeFromMenuItems();
            }
        }

        public BeepTree()
        {
            this.Name = "BeepTree";
            _childnodes= new List<BeepTreeNode>();
            items.ListChanged += Items_ListChanged;
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
          //  InitializeTreeFromMenuItems();
            RearrangeTree();
        }
        private void Items_ListChanged(object? sender, ListChangedEventArgs e)
        {
            SuspendLayout(); // Temporarily pause layout updates
            try
            {
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
                        // Reinitialize the entire tree for bulk changes
                        InitializeTreeFromMenuItems();
                        break;

                    default:
                        Console.WriteLine($"Unhandled ListChangedType: {e.ListChangedType}");
                        break;
                }
            }
            finally
            {
                ResumeLayout(); // Resume layout updates
            }
        }

       

        private void HandleItemAdded(int index)
        {
            if (index < 0 || index >= items.Count) return;

            Console.WriteLine($"Adding node at index {index}");

            var menuItem = items[index];
            var node = CreateTreeNodeFromMenuItem(menuItem, null); // Parent is null for root nodes
            if (node != null)
            {
                AddNode(node); // Add the new node
                RearrangeTree(); // Adjust the layout
            }
        }
        private void HandleItemDeleted(int index)
        {
            if (index < 0 || index >= _childnodes.Count) return;

            Console.WriteLine($"Deleting node at index {index}");

            var nodeToRemove = _childnodes[index];
            RemoveNode(nodeToRemove); // Remove the node
            RearrangeTree(); // Adjust the layout
        }
        private void HandleItemChanged(int index)
        {
            if (index < 0 || index >= items.Count) return;

            Console.WriteLine($"Updating node at index {index}");

            var menuItem = items[index];
            var existingNode = GetNodeByIndex(index); // Get the corresponding node
            if (existingNode != null)
            {
                // Update the node properties
                existingNode.Text = menuItem.Text;
                existingNode.ImagePath = menuItem.Image;

                existingNode.RearrangeNode(); // Update child nodes if necessary
            }
        }
        public void ToggleNode(BeepTreeNode node)
        {
            node.IsExpanded = !node.IsExpanded;
            node.RearrangeNode();
            RearrangeTree(); // Adjust layout based on expanded/collapsed state
        }
        public IEnumerable<BeepTreeNode> TraverseTree()
        {
            foreach (var node in _childnodes)
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
            return TraverseTree().FirstOrDefault(node => node.Text == text);
        }
        

        private BeepTreeNode GetNodeByIndex(int index)
        {
            if (index < 0 || index >= _childnodes.Count) return null;
            return _childnodes[index];
        }

        private void InitializeTreeFromMenuItems()
        {
            Console.WriteLine("Initialize Tree");
            ClearNodes(); // Clear existing nodes
            Console.WriteLine("Items Count: " + items.Count);
            foreach (var menuItem in items)
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
                    node.NodeClicked += (sender, e) => NodeClicked?.Invoke(sender, e);
                    node.NodeRightClicked += (sender, e) => NodeRightClicked?.Invoke(sender, e);
                    node.NodeDoubleClicked += (sender, e) => NodeDoubleClicked?.Invoke(sender, e);
                    
                    node.NodeExpanded += (sender, e) => NodeExpanded?.Invoke(sender, e);
                    node.NodeCollapsed += (sender, e) => NodeCollapsed?.Invoke(sender, e);
                    node.NodeSelected += (sender, e) => NodeSelected?.Invoke(sender, e);
                    node.NodeDeselected += (sender, e) => NodeDeselected?.Invoke(sender, e);

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
            nodeseq++;
            try
            {
                Console.WriteLine("Creating Node: " + menuItem.Text);
                var node = new BeepTreeNode
                {
                    Name = menuItem.Text ?? $"Node{nodeseq}",
                    Text = menuItem.Text ,
                    ImagePath = menuItem.Image,
                    ParentNode = parent,
                    Tree = this,
                    NodeSeq = nodeseq,
                    Height = NodeHeight,
                    IsBorderAffectedByTheme =false,
                    IsShadowAffectedByTheme = false,
                    IsFramless = true,
                    IsChild=true,
                    Size = new Size(NodeWidth, NodeHeight),
                    Theme = this.Theme,
                   
                    Level = parent?.Level + 1 ?? 1 // Increment level for child nodes
                };
                Console.WriteLine("Node Created: " + node.Text);
                foreach (var childMenuItem in menuItem.Children)
                {
                    Console.WriteLine("Child Node: " + childMenuItem.Text);
                    var childNode = CreateTreeNodeFromMenuItem(childMenuItem, node);
                    node.AddChild(childNode);
                }
                return node;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
          
          
        }

        public void AddNode(BeepTreeNode node)
        {
            var panel = new Panel
            {
                Width = Width,
                Height = node.Height,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
               
            };
            _childnodes.Add(node);
            panel.Controls.Add(node); // Add the node to the panel
            Controls.Add(panel); // Add the panel to the tree
            _nodePanels.Add(node.NodeSeq,panel);

            RearrangeTree();
        }


        public void RemoveNode(BeepTreeNode node)
        {
            _childnodes.Remove(node);
            Controls.Remove(node);
            _nodePanels.Remove(node.NodeSeq);
            RearrangeTree();
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
            BeepTreeNode node = _childnodes.FirstOrDefault(c => c.Name == NodeName);
            if (node != null)
            {
                RemoveNode(node);
            }
        }

        public void RemoveNode(int NodeIndex)
        {
            if (NodeIndex >= 0 && NodeIndex < _childnodes.Count)
            {
                RemoveNode(GetNode(NodeIndex));
            }
        }

        public BeepTreeNode GetNode(string NodeName)
        {
            return _childnodes.FirstOrDefault(c => c.Name == NodeName);
        }

        public BeepTreeNode GetNode(int NodeIndex)
        {
            return NodeIndex >= 0 && NodeIndex < _childnodes.Count ? _childnodes[NodeIndex] : null;
        }

        public List<BeepTreeNode> GetNodes()
        {
            return _childnodes;
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor =_currentTheme.PanelBackColor ;
            _nodePanels.Values.ToList().ForEach(p => p.BackColor = _currentTheme.PanelBackColor);
            foreach (BeepTreeNode node in _childnodes)
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
           
            return _childnodes.FirstOrDefault(p=>p.NodeSeq==seq);
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
            foreach (var item in _childnodes)
            {
                item.ShowNodeImage = _shownodeimage;
                item.Refresh(); // Ensure the node redraws with the updated setting
            }
        }

    }

}
