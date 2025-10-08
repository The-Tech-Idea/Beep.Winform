using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees
{
    /// <summary>
    /// Simple test form to verify BeepTree rendering
    /// </summary>
    public class TestBeepTreeForm : Form
    {
        private BeepTree _tree;
        private Button _btnTest;
        private Button _btnAddNode;
        private Button _btnRemoveNode;
        private Button _btnClear;
        private TextBox _txtDebug;
        private int _nodeCounter = 100;

        public TestBeepTreeForm()
        {
            InitializeComponents();
            LoadTestData();
        }

        private void InitializeComponents()
        {
            this.Text = "BeepTree Test Form - Dynamic Updates";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Debug output
            _txtDebug = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                Font = new Font("Consolas", 9)
            };
            this.Controls.Add(_txtDebug);

            // Button panel
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(5)
            };
            this.Controls.Add(buttonPanel);

            // Test button
            _btnTest = new Button
            {
                Text = "Reload Test Data",
                Location = new Point(5, 5),
                Size = new Size(150, 25)
            };
            _btnTest.Click += (s, e) => LoadTestData();
            buttonPanel.Controls.Add(_btnTest);
            
            // Add Node button
            _btnAddNode = new Button
            {
                Text = "Add Root Node",
                Location = new Point(160, 5),
                Size = new Size(150, 25)
            };
            _btnAddNode.Click += (s, e) => AddDynamicNode();
            buttonPanel.Controls.Add(_btnAddNode);
            
            // Remove Node button
            _btnRemoveNode = new Button
            {
                Text = "Remove First Node",
                Location = new Point(315, 5),
                Size = new Size(150, 25)
            };
            _btnRemoveNode.Click += (s, e) => RemoveFirstNode();
            buttonPanel.Controls.Add(_btnRemoveNode);
            
            // Clear button
            _btnClear = new Button
            {
                Text = "Clear All Nodes",
                Location = new Point(470, 5),
                Size = new Size(150, 25)
            };
            _btnClear.Click += (s, e) => ClearAllNodes();
            buttonPanel.Controls.Add(_btnClear);

            // Status label
            var lblStatus = new Label
            {
                Text = "Click buttons to test dynamic tree updates at runtime",
                Location = new Point(5, 35),
                Size = new Size(600, 25),
                ForeColor = Color.DarkBlue
            };
            buttonPanel.Controls.Add(lblStatus);

            // BeepTree
            _tree = new BeepTree
            {
                Dock = DockStyle.Fill,
                TreeStyle = TreeStyle.Standard,
                ShowCheckBox = true,
                BackColor = Color.White
            };
            this.Controls.Add(_tree);
        }

        private void LoadTestData()
        {
            Log("=== Loading Test Data ===");

            // Create test nodes
            var nodes = new List<SimpleItem>();

            // Root 1
            var root1 = new SimpleItem
            {
                ID = 1,
                Text = "Root Node 1",
                IsExpanded = true
            };
            root1.Children.Add(new SimpleItem { ID = 11, Text = "Child 1.1" });
            root1.Children.Add(new SimpleItem { ID = 12, Text = "Child 1.2" });
            nodes.Add(root1);

            // Root 2
            var root2 = new SimpleItem
            {
                ID = 2,
                Text = "Root Node 2",
                IsExpanded = false
            };
            root2.Children.Add(new SimpleItem { ID = 21, Text = "Child 2.1" });
            root2.Children.Add(new SimpleItem { ID = 22, Text = "Child 2.2" });
            nodes.Add(root2);

            Log($"Created {nodes.Count} root nodes");
            Log($"Root 1 has {root1.Children.Count} children");
            Log($"Root 2 has {root2.Children.Count} children");

            // Assign to tree
            _tree.Nodes = nodes;

            Log("=== Data loaded - check Output window for BeepTree debug messages ===");
            Log("");
        }
        
        private void AddDynamicNode()
        {
            Log($"=== Adding New Root Node (ID: {_nodeCounter}) ===");
            
            var newNode = new SimpleItem
            {
                ID = _nodeCounter++,
                Text = $"Dynamic Node {DateTime.Now:HH:mm:ss}",
                IsExpanded = true
            };
            
            // Add a child to demonstrate hierarchy
            newNode.Children.Add(new SimpleItem 
            { 
                ID = _nodeCounter++, 
                Text = "Dynamic Child" 
            });
            
            _tree.AddNode(newNode);
            
            Log($"Added node with ID {newNode.ID}");
            Log($"Total root nodes: {_tree.Nodes.Count}");
            Log("");
        }
        
        private void RemoveFirstNode()
        {
            if (_tree.Nodes.Count == 0)
            {
                Log("=== Cannot Remove - No nodes available ===");
                Log("");
                return;
            }
            
            Log("=== Removing First Root Node ===");
            var firstNode = _tree.Nodes[0];
            Log($"Removing: {firstNode.Text} (ID: {firstNode.ID})");
            
            _tree.RemoveNode(firstNode);
            
            Log($"Remaining root nodes: {_tree.Nodes.Count}");
            Log("");
        }
        
        private void ClearAllNodes()
        {
            Log("=== Clearing All Nodes ===");
            int count = _tree.Nodes.Count;
            
            _tree.ClearNodes();
            
            Log($"Cleared {count} root nodes");
            Log("Tree is now empty");
            Log("");
        }

        private void Log(string message)
        {
            _txtDebug.AppendText(message + Environment.NewLine);
            System.Diagnostics.Debug.WriteLine($"TestForm: {message}");
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestBeepTreeForm());
        }
    }
}
