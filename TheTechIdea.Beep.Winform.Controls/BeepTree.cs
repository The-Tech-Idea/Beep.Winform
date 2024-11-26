using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepTree : BeepControl
    {
        private int nodeseq = 0;
        public int NodeHeight { get; set; } = 30;
        public int NodeWidth { get; set; } = 100;
        private SimpleItemCollection items = new SimpleItemCollection();
        private List<BeepTreeNode> _childnodes = new List<BeepTreeNode>();
        private Dictionary<int,Panel> _nodePanels = new Dictionary<int,Panel>();

        private bool _shownodeimage = true;

        public bool ShowNodeImage
        {
            get { return _shownodeimage; }
            set { _shownodeimage = value; ChangeNodeImageSettings(); }
        }

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
       // [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleItemCollection Nodes
        {
            get => items;
            set
            {
                items = value;
                InitializeTreeFromMenuItems();
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
            InitializeTreeFromMenuItems();
            RearrangeTree();
        }
        private void Items_ListChanged(object? sender, ListChangedEventArgs e)
        {
            Console.WriteLine("List Changed");
            InitializeTreeFromMenuItems();
            Console.WriteLine("List Changed Done");
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
               
            }

        }
    }
    
}
