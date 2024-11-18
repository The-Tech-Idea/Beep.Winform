using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Template
{
    public partial class MenuItemEditorForm : Form
    {
        private TreeView menuTreeView;
        private PropertyGrid propertyGrid;
        private Button addButton;
        private Button editButton;
        private Button deleteButton;
        private Button saveButton;
        private Button cancelButton;

        public BindingList<SimpleMenuItem> MenuItems { get; private set; }

        public MenuItemEditorForm(BindingList<SimpleMenuItem> menuItems)
        {
            InitializeComponent();
            MenuItems = menuItems ?? new BindingList<SimpleMenuItem>();
            LoadMenuItemsToTree(MenuItems);
        }

        private void InitializeComponent()
        {
            menuTreeView = new TreeView();
            propertyGrid = new PropertyGrid();
            addButton = new Button();
            editButton = new Button();
            deleteButton = new Button();
            saveButton = new Button();
            cancelButton = new Button();

            SuspendLayout();

            // menuTreeView
            menuTreeView.Location = new Point(12, 12);
            menuTreeView.Name = "menuTreeView";
            menuTreeView.Size = new Size(300, 450);
            menuTreeView.TabIndex = 0;
            menuTreeView.AfterSelect += MenuTreeView_AfterSelect;

            // propertyGrid
            propertyGrid.Location = new Point(320, 12);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.Size = new Size(250, 450);
            propertyGrid.TabIndex = 1;

            // addButton
            addButton.Location = new Point(12, 470);
            addButton.Name = "addButton";
            addButton.Size = new Size(75, 23);
            addButton.Text = "Add";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddButton_Click;

            // editButton
            editButton.Location = new Point(100, 470);
            editButton.Name = "editButton";
            editButton.Size = new Size(75, 23);
            editButton.Text = "Edit";
            editButton.UseVisualStyleBackColor = true;
            editButton.Click += EditButton_Click;

            // deleteButton
            deleteButton.Location = new Point(190, 470);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(75, 23);
            deleteButton.Text = "Delete";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += DeleteButton_Click;

            // saveButton
            saveButton.Location = new Point(320, 470);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(75, 23);
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += SaveButton_Click;

            // cancelButton
            cancelButton.Location = new Point(400, 470);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButton_Click;

            // MenuItemEditorForm
            ClientSize = new Size(600, 510);
            Controls.Add(menuTreeView);
            Controls.Add(propertyGrid);
            Controls.Add(addButton);
            Controls.Add(editButton);
            Controls.Add(deleteButton);
            Controls.Add(saveButton);
            Controls.Add(cancelButton);
            Name = "MenuItemEditorForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Menu Item Editor";
            ResumeLayout(false);
        }


        private void LoadMenuItemsToTree(BindingList<SimpleMenuItem> menuItems, TreeNode parentNode = null)
        {
            foreach (var item in menuItems)
            {
                TreeNode node = new TreeNode(item.Name)
                {
                    Tag = item
                };
                if (parentNode == null)
                {
                    menuTreeView.Nodes.Add(node);
                }
                else
                {
                    parentNode.Nodes.Add(node);
                }

                // Recursively load children
                //if (item.Children.Count > 0)
                //{
                //    LoadMenuItemsToTree(item.Children, node);
                //}
            }
        }

        private void MenuTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (menuTreeView.SelectedNode?.Tag is MenuItem selectedItem)
            {
                propertyGrid.SelectedObject = selectedItem;
            }
        }


        private void AddButton_Click(object sender, EventArgs e)
        {
            SimpleMenuItem newItem = new SimpleMenuItem { Name = "New Item", Text = "New Text" };
            TreeNode newNode = new TreeNode(newItem.Name) { Tag = newItem };

            if (menuTreeView.SelectedNode != null)
            {
                // Add as a child node to the selected node
                menuTreeView.SelectedNode.Nodes.Add(newNode);
                menuTreeView.SelectedNode.Expand();
                //  ((SimpleMenuItem)menuTreeView.SelectedNode.Tag).Children.Add(newItem);
            }
            else
            {
                // Add as a root node
                menuTreeView.Nodes.Add(newNode);
                MenuItems.Add(newItem);
            }

            menuTreeView.SelectedNode = newNode;
            propertyGrid.SelectedObject = newItem;
        }


        private void EditButton_Click(object sender, EventArgs e)
        {
            if (menuTreeView.SelectedNode?.Tag is MenuItem selectedItem)
            {
                propertyGrid.SelectedObject = selectedItem;
                menuTreeView.SelectedNode.Text = selectedItem.Name; // Update the node's text in case it was changed
            }
        }


        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (menuTreeView.SelectedNode != null)
            {
                if (menuTreeView.SelectedNode.Parent != null)
                {
                    // Remove child node
                    var parentMenuItem = menuTreeView.SelectedNode.Parent.Tag as MenuItem;
                    parentMenuItem?.Children.Remove(menuTreeView.SelectedNode.Tag as MenuItem);
                    menuTreeView.SelectedNode.Parent.Nodes.Remove(menuTreeView.SelectedNode);
                }
                else
                {
                    // Remove root node
                    MenuItems.Remove(menuTreeView.SelectedNode.Tag as SimpleMenuItem);
                    menuTreeView.Nodes.Remove(menuTreeView.SelectedNode);
                }

                propertyGrid.SelectedObject = null;
            }
        }


        private void SaveButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }


        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }
    }
}
