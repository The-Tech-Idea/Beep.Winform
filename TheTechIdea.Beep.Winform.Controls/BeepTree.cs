using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Template;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepTree : BeepControl
    {
        private int nodeseq = 0;
        public int NodeHeight { get; set; } = 30;
        public int NodeWidth { get; set; } = 100;
        private SimpleItemCollection items = new SimpleItemCollection();
        private List<BeepTreeNode> _childnodes = new List<BeepTreeNode>();

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
                InitializeTreeFromMenuItems();
            }
        }

        public BeepTree()
        {
            this.Name = "BeepTree";
            _childnodes= new List<BeepTreeNode>();
            items.ListChanged += Items_ListChanged;
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
                    Parent = parent,
                    Tree = this,
                    NodeSeq = nodeseq,
                    Width=NodeWidth,
                    Height = NodeHeight,
                    IsBorderAffectedByTheme =false,
                    IsShadowAffectedByTheme = false,
                    //IsFramless = true,
                    //IsChild=true,
                    Size = new Size(NodeWidth, NodeHeight),
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
            Console.WriteLine("Adding Node: " + node.Text);
            node.Tree = this;

            if (node.NodeSeq == 0)
            {
                nodeseq++;
                node.NodeSeq = nodeseq;
            }
            Console.WriteLine("Node Seq: " + node.NodeSeq);
            if (string.IsNullOrEmpty(node.Name))
            {
                node.Name = "Node" + nodeseq.ToString();
            }

            if (node.Level == 0)
            {
                node.Level = 1; // Represent the root node
            }
            Console.WriteLine("Node Level: " + node.Level);
            _childnodes.Add(node);
            Console.WriteLine("Node Added: " + node.Text);
            Controls.Add(node);
            Console.WriteLine("Node Added to Control: " + node.Text);
            RearrangeTree();
        }

        public void RemoveNode(BeepTreeNode node)
        {
            _childnodes.Remove(node);
            Controls.Remove(node);
            RearrangeTree();
        }

        public void ClearNodes()
        {

                Controls.Clear();
            _childnodes.Clear();
            RearrangeTree();
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
            foreach (BeepTreeNode node in _childnodes)
            {
                node.Theme = Theme;
            }
        }

        private void RearrangeTree()
        {
            Console.WriteLine("Rearrange Tree");
            UpdateDrawingRect();
            DrawingRect.Inflate(-3, -3); // Allow some padding
            int starty = DrawingRect.Top+3;
            int startx = DrawingRect.Left+3;
            int NodeWidth = DrawingRect.Width-6;
            Console.WriteLine("Node Width: " + NodeWidth);
            foreach (var node in _childnodes.OrderBy(p=>p.NodeSeq))
            {
                try
                {
                   
                    node.Location = new Point(startx, starty); // Adjust X and Y for alignment
                    node.RearrangeNode();
                  
                    Console.WriteLine($"Rearrange Node:{node.Text} - ({startx},{starty}) width {node.Width}");
                    starty += node.Height + 2; // Add spacing between nodes

                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

            }

            //Invalidate(); // Redraw tree
        }

        public void DrawTree()
        {
            foreach (var node in _childnodes)
            {
                if (node.Level == 1)
                {
                    if (!node.IsInitialized)
                    {
                        node.InitNode();
                    }
                }
            }

            RearrangeTree();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RearrangeTree();
        }
        public void ExpandAll()
        {
            foreach (var node in _childnodes)
            {
                node.ExpandAll();
            }
        }
    }
    
}
