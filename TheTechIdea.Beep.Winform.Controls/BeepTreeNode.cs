using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;


namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepTreeNode : BeepControl
    {
        #region "Events"

        public EventHandler<BeepEventDataArgs> LeftButtonClicked;
        public EventHandler<BeepEventDataArgs> RighButtonClicked;
        public EventHandler<BeepEventDataArgs> MiddleButtonClicked;
        public EventHandler<BeepEventDataArgs> NodeClicked;
        public EventHandler<BeepEventDataArgs> NodeDoubleClicked;
        public EventHandler<BeepEventDataArgs> NodeSelected;
        public EventHandler<BeepEventDataArgs> NodeDeselected;
        public EventHandler<BeepEventDataArgs> NodeExpanded;
        public EventHandler<BeepEventDataArgs> NodeCollapsed;
        public EventHandler<BeepEventDataArgs> NodeChecked;
        public EventHandler<BeepEventDataArgs> NodeUnchecked;
        public EventHandler<BeepEventDataArgs> NodeVisible;
        public EventHandler<BeepEventDataArgs> NodeInvisible;
        public EventHandler<BeepEventDataArgs> NodeReadOnly;
        public EventHandler<BeepEventDataArgs> NodeEditable;
        public EventHandler<BeepEventDataArgs> NodeDeletable;
        public EventHandler<BeepEventDataArgs> NodeDirty;
        public EventHandler<BeepEventDataArgs> NodeModified;
        public EventHandler<BeepEventDataArgs> NodeDeleted;
        public EventHandler<BeepEventDataArgs> NodeAdded;
        public EventHandler<BeepEventDataArgs> NodeExpandedAll;
        public EventHandler<BeepEventDataArgs> NodeCollapsedAll;
        public EventHandler<BeepEventDataArgs> NodeCheckedAll;
        public EventHandler<BeepEventDataArgs> NodeUncheckedAll;
        public EventHandler<BeepEventDataArgs> NodeVisibleAll;
        public EventHandler<BeepEventDataArgs> NodeInvisibleAll;
        public EventHandler<BeepEventDataArgs> NodeRightClicked;
        public EventHandler<BeepEventDataArgs> NodeLeftClicked;
        public EventHandler<BeepEventDataArgs> NodeMiddleClicked;
        public EventHandler<BeepEventDataArgs> NodeMouseEnter;
        public EventHandler<BeepEventDataArgs> NodeMouseLeave;
        public EventHandler<BeepEventDataArgs> NodeMouseHover;
        public EventHandler<BeepEventDataArgs> NodeMouseWheel;
        public EventHandler<BeepEventDataArgs> NodeMouseUp;
        public EventHandler<BeepEventDataArgs> NodeMouseDown;
        public EventHandler<BeepEventDataArgs> NodeMouseMove;

        public EventHandler<BeepEventDataArgs> ShowMenu;
        #endregion "Events"
        #region "Properties"
        private BeepTree _tree;
        private BeepTreeNode _parent;
        //Controls for the Node Drawing
        private BeepButton NodeMainMiddlebutton;
        private BeepButton Nodeleftbutton;
        private BeepButton Noderightbutton;
        private BeepButton _toggleButton; // used to show the dropdown for the node with childern nodes
        private BeepImage lefttreebranchimage; // used to show the left branch of the tree
      //  private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        public List<BeepTreeNode> BeepTreeNodes { get; private set; }
        private int cachedHeight;
        private int cachedWidth;
        private bool isSizeCached = false;
        private int _nodeseq;
        private string _key;
        private string _text;
        private string _imageKey;
        private string _selectedImageKey;
        private bool _isExpanded;
        private bool _isSelected;
        private bool _isChecked;
        private bool _isVisible;
        private bool _isReadOnly;
        private bool _isEnabled;
        private bool _isEditable;
        private bool _isDeletable;
        private bool _isDirty;
        private bool _isModified;
        private bool _isDeleted;
        private bool _isAdded;
        private bool _isleftbuttonVisible = false;
        private bool _isrightbuttonVisible = false;
        private bool _ismiddlebuttonVisible = true;

        private string _leftbuttonimagepath;
        private string _rightbuttonimagepath;

        private int _level;

        private string _guidid=Guid.NewGuid().ToString();
        private Panel _nodePanel; // Panel to encapsulate the node
        private Panel _childrenPanel; // Panel for child nodes

        private string _nodeLevelID = string.Empty;
        private int childnodesSeq = 0;


        public  string GuidID => _guidid;
        public int NodeHeight { get; set; } = 30;
        public int NodeWidth { get; set; } = 100;
        public int SmallNodeHeight { get; set; } = 15;
        public int MinimumTextWidth { get; set; } = 100;
        public string NodeDataType { get; set; } = "SimpleItem";

        /// <summary>
        /// this id is maintained by the treeview to identify the node
        /// its unique and is used to identify the node
        /// its split by levels and the node id
        /// so if the node is at level 1 and its id is 2 then the node id will be 1_2
        /// and if its at level 2 and its id 3 then the node id will be 1_2_3
        /// and to find it we go to the level 1 and find the node with id 2 and then find the node with id 3
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string NodeLevelID
        {
            get => _nodeLevelID;
            set
            {
                _nodeLevelID = value;
                //if (BeepTreeNodes.Count > 0)
                //{
                //    foreach (var item in BeepTreeNodes)
                //    {
                //        item.NodeLevelID = $"{_nodeLevelID}_{item.NodeSeq}";
                //    }
                //}
            }

        }
        /// <summary>
        /// Gets the cumulative height of all visible child nodes.
        /// </summary>
        public int ChildNodesHeight
        {
            get
            {
                // Sum the heights of all visible child nodes, including padding
                int totalHeight = 0;
                int padding = 0; // Adjust padding as needed

                foreach (var child in BeepTreeNodes)
                {
                    if (child.Visible)
                    {
                        totalHeight += child.Height + padding;
                    }
                }

                return totalHeight;
            }
        }
        /// <summary>
        /// this is the sequence of the child nodes
        /// this will help us set Nodeleveid for the child nodes
        /// </summary>

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ChildNodesSeq
        {
            get => childnodesSeq++;
            
        }

        private int nodeimagesize = 16;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxImageSize
        {
            get => nodeimagesize;
            set
            {
                nodeimagesize = value;
                ChangeNodeImageSettings();
            }
        }

        private bool _showCheckBox = false;
        private BeepCheckBox<bool> _checkBox; // Instance of BeepCheckBox

        public bool ShowCheckBox
        {
            get => _showCheckBox;
            set
            {
                _showCheckBox = value;
                if (_checkBox != null)
                {
                    _checkBox.Visible = _showCheckBox;
                    RearrangeNode(); // Adjust layout when the checkbox visibility changes
                }
            }
        }


        private bool _shownodeimage = true;

        public bool ShowNodeImage
        {
            get { return _shownodeimage; }
            set { _shownodeimage = value; ChangeNodeImageSettings(); }
        }

     

        private SimpleItem _menuitem;

        public SimpleItem NodeInfo
        {
            get { return _menuitem; }
            set { _menuitem = value; }
        }
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        //[Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> Nodes
        {
            get => Tree.Nodes;
           
        }
        private bool _isinitialized = false;
        public bool IsInitialized
        {
            get { return _isinitialized; }
            set { _isinitialized = value; }
        }
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }



        public int NodeSeq
        {
            get { return _nodeseq; }
            set { _nodeseq = value; }
        }

        public string LeftButtonImagePath
        {
            get { return _leftbuttonimagepath; }
            set { _leftbuttonimagepath = value; }
        }
        public string RightButtonImagePath
        {
            get { return _rightbuttonimagepath; }
            set { _rightbuttonimagepath = value; }
        }


        public bool IsleftbuttonVisible
        {
            get { return _isleftbuttonVisible; }
            set { _isleftbuttonVisible = value; }
        }
        public bool IsrightbuttonVisible
        {
            get { return _isrightbuttonVisible; }
            set { _isrightbuttonVisible = value; }
        }
        public bool IsmiddlebuttonVisible
        {
            get { return _ismiddlebuttonVisible; }
            set { _ismiddlebuttonVisible = value; }
        }
        public BeepTree Tree
        {
            get { return _tree; }
            set { _tree = value; }
        }
        public BeepTreeNode ParentNode
        {
            get { return _parent; }
            set { _parent = value; }
        }
     
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
        public new string Text
        {
            get { return _text; }
            set { _text = value;if(NodeMainMiddlebutton!=null) NodeMainMiddlebutton.Text = value; }
        }
        public string ImagePath
        {
            get { return _imageKey; }
            set { _imageKey = value; if (NodeMainMiddlebutton != null) NodeMainMiddlebutton.ImagePath = value; }
        }
        public string SelectedImageKey
        {
            get { return _selectedImageKey; }
            set { _selectedImageKey = value; }
        }
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                ToggleExpansion();
            }
        }
       
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                if (_checkBox != null)
                {
                    _checkBox.CurrentValue = _isSelected;
                }
                Invalidate(); // Redraw the node if necessary
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { _isReadOnly = value; }
        }
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }
        public bool IsEditable
        {
            get { return _isEditable; }
            set { _isEditable = value; }
        }
        public bool IsDeletable
        {
            get { return _isDeletable; }
            set { _isDeletable = value; }
        }
        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }
        public bool IsModified
        {
            get { return _isModified; }
            set { _isModified = value; }
        }
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; }
        }
        public bool IsAdded
        {
            get { return _isAdded; }
            set { _isAdded = value; }
        }
        #endregion "Properties"

        #region "Constructors"
        public BeepTreeNode()
        {
          //  LogMessage($"Construct 1");
            UpdateDrawingRect();
            BoundProperty= "Text";
            Height = NodeHeight;
            Width = NodeWidth;
           
            InitNode();
        }
        protected override Size DefaultSize => new Size(NodeWidth,NodeHeight);
        public BeepTreeNode(string key, string text)
        {
            _key = key;
            _text = text;
            BeepTreeNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            BeepTreeNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            BeepTreeNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey, bool isExpanded)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            _isExpanded = isExpanded;
            BeepTreeNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey, bool isExpanded, bool isSelected)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            _isExpanded = isExpanded;
            _isSelected = isSelected;
            BeepTreeNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey, bool isExpanded, bool isSelected, bool isChecked)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            _isExpanded = isExpanded;
            _isSelected = isSelected;
            _isChecked = isChecked;
            BeepTreeNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey, bool isExpanded, bool isSelected, bool isChecked, bool isVisible)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            _isExpanded = isExpanded;
            _isSelected = isSelected;
            _isChecked = isChecked;
            _isVisible = isVisible;
            BeepTreeNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey, bool isExpanded, bool isSelected, bool isChecked, bool isVisible, bool isReadOnly)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            _isExpanded = isExpanded;
            _isSelected = isSelected;
            _isChecked = isChecked;
            _isVisible = isVisible;
            _isReadOnly = isReadOnly;
            BeepTreeNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey, bool isExpanded, bool isSelected, bool isChecked, bool isVisible, bool isReadOnly, bool isEnabled)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            _isExpanded = isExpanded;
            _isSelected = isSelected;
            _isChecked = isChecked;
            _isVisible = isVisible;
            _isReadOnly = isReadOnly;
            _isEnabled = isEnabled;
            BeepTreeNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey, bool isExpanded, bool isSelected, bool isChecked, bool isVisible, bool isReadOnly, bool isEnabled, bool isEditable)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            _isExpanded = isExpanded;
            _isSelected = isSelected;
            _isChecked = isChecked;
            _isVisible = isVisible;
            _isReadOnly = isReadOnly;
            _isEnabled = isEnabled;
            _isEditable = isEditable;
            BeepTreeNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey, bool isExpanded, bool isSelected, bool isChecked, bool isVisible, bool isReadOnly, bool isEnabled, bool isEditable, bool isDeletable)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            _isExpanded = isExpanded;
            _isSelected = isSelected;
            _isChecked = isChecked;
            _isVisible = isVisible;
            _isReadOnly = isReadOnly;
            _isEnabled = isEnabled;
            _isEditable = isEditable;
            _isDeletable = isDeletable;
            BeepTreeNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey, bool isExpanded, bool isSelected, bool isChecked, bool isVisible, bool isReadOnly, bool isEnabled, bool isEditable, bool isDeletable, bool isDirty)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            _isExpanded = isExpanded;
            _isSelected = isSelected;
            _isChecked = isChecked;
            _isVisible = isVisible;
            _isReadOnly = isReadOnly;
            _isEnabled = isEnabled;
            _isEditable = isEditable;
            _isDeletable = isDeletable;
            _isDirty = isDirty;
            BeepTreeNodes = new List<BeepTreeNode>();
            InitNode();
        }
        #endregion "Constructors"
        #region "Events Methods"
        private void NodeMainMiddlebutton_Click(object sender, EventArgs e)
        {
            e.Equals(MouseButtons.Right);
            BeepEventDataArgs args = new BeepEventDataArgs("NodeMainButtonClicked", this);
          
           

        }
        private void Nodeleftbutton_Click(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeleftbuttonClicked", this);
            NodeLeftClicked?.Invoke(this, args);
        }
        private void Noderightbutton_Click(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NoderightbuttonClicked", this);
            NodeRightClicked?.Invoke(this, args);
           
        }
        private void NodeMainMiddlebutton_DoubleClick(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeMainButtonDoubleClicked", this);
            NodeDoubleClicked?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_MouseEnter(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeMainButtonMouseEnter", this);
            NodeMouseEnter?.Invoke(this, args);
            HilightNode();

        }
        private void NodeMainMiddlebutton_MouseLeave(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeMainButtonMouseLeave", this);
            NodeMouseLeave?.Invoke(this, args);
            UnHilightNode();
        }
        private void NodeMainMiddlebutton_MouseHover(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeMainButtonMouseHover", this);
            NodeMouseHover?.Invoke(this, args);
            HilightNode();

        }
        private void NodeMainMiddlebutton_MouseWheel(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeMainButtonMouseWheel", this);
            NodeMouseWheel?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_MouseUp(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeMainButtonMouseUp", this);
            NodeMouseUp?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_MouseDown(object sender, MouseEventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeMainButtonMouseDown", this);
            NodeMouseDown?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_MouseMove(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeMainButtonMouseMove", this);
            NodeMouseMove?.Invoke(this, args);
            if (e.Equals(MouseButtons.Right))
            {
                BeepEventDataArgs args1 = new BeepEventDataArgs("NodeMainButtonRightClicked", this);
                NodeRightClicked?.Invoke(this, args1);
                if(ShowMenu != null)
                {
                    ShowMenu?.Invoke(this, args1);
                }
            }
            if (e.Equals(MouseButtons.Left))
            {
                BeepEventDataArgs args1 = new BeepEventDataArgs("NodeMainButtonLeftClicked", this);
                NodeLeftClicked?.Invoke(this, args1);
            }
        }
        private void Nodeleftbutton_MouseEnter(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeleftbuttonMouseEnter", this);
            NodeMouseEnter?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseLeave(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeleftbuttonMouseLeave", this);
            NodeMouseLeave?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseHover(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeleftbuttonMouseHover", this);
            NodeMouseHover?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseWheel(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeleftbuttonMouseWheel", this);
            NodeMouseWheel?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseUp(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeleftbuttonMouseUp", this);
            NodeMouseUp?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseDown(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeleftbuttonMouseDown", this);
            NodeMouseDown?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseMove(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NodeleftbuttonMouseMove", this);
            NodeMouseMove?.Invoke(this, args);
        }
        private void Noderightbutton_MouseEnter(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NoderightbuttonMouseEnter", this);
            NodeMouseEnter?.Invoke(this, args);
        }
        private void Noderightbutton_MouseLeave(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NoderightbuttonMouseLeave", this);
            NodeMouseLeave?.Invoke(this, args);
        }
        private void Noderightbutton_MouseHover(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NoderightbuttonMouseHover", this);
            NodeMouseHover?.Invoke(this, args);
        }
        private void Noderightbutton_MouseWheel(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NoderightbuttonMouseWheel", this);
            NodeMouseWheel?.Invoke(this, args);
        }
        private void Noderightbutton_MouseUp(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NoderightbuttonMouseUp", this);
            NodeMouseUp?.Invoke(this, args);
        }
        private void Noderightbutton_MouseDown(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NoderightbuttonMouseDown", this);
            NodeMouseDown?.Invoke(this, args);
        }
        private void Noderightbutton_MouseMove(object sender, EventArgs e)
        {
            BeepEventDataArgs args = new BeepEventDataArgs("NoderightbuttonMouseMove", this);
            NodeMouseMove?.Invoke(this, args);
        }

        #endregion "Events Methods"
        #region "Painting Methods"
      
        int startx = 0;
        int starty = 0;
        public void InitNode()
        {
          //  LogMessage($"init");
            UpdateDrawingRect();
            _nodePanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = parentbackcolor
            };

            _childrenPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = parentbackcolor
            };
        //    LogMessage($"Construct 2");
            Controls.Add(_childrenPanel);
            Controls.Add(_nodePanel);
        //    LogMessage($"Construct 3");
            BeepTreeNodes = new List<BeepTreeNode>();
            _nodePanel.Controls.Clear();
        //     LogMessage($"init1");
            IsShadowAffectedByTheme =false;
            IsBorderAffectedByTheme = false;
            IsFramless = true;
            IsChild = true;
            startx = DrawingRect.Left;
            starty = DrawingRect.Top;
            NodeWidth = DrawingRect.Width;
            NodeHeight = DrawingRect.Height;
           // LogMessage($"init2");
            _checkBox = new BeepCheckBox
            {
                CheckedValue = true,
                UncheckedValue = false,
                CurrentValue = IsSelected,
                Visible = _showCheckBox,
                IsChild = true,
                IsFramless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                Theme = Theme,
                IsRounded = false,
                Size = new Size(20, 20), // Adjust size as needed
            };
         //   LogMessage($"init3");
            _checkBox.StateChanged += CheckBox_StateChanged;
        //    LogMessage($"init4");
            _nodePanel.Controls.Add(_checkBox);
         //   LogMessage($"init6");
            //   DrawLeftBranch();
            DrawLeftButton();
            DrawMiddleButton();
            DrawRightButton();
            DrawToggleButton();
            starty = NodeHeight;
         //   LogMessage($"init3");
            //foreach (var child in BeepTreeNodes)
            //{
            //    if(child.IsInitialized == false) child.InitNode();
            //    child.Visible = IsExpanded;
            //    if (IsExpanded)
            //    {
            //        child.Location = new Point(20, starty);
            //        child.Width = Width -20; // Indent child nodes
            //        starty += child.Height + 5;
            //        child.Theme= Theme;
            //        child.ApplyTheme();
            //        child.RearrangeNode();
            //    }
            //}
            if (NodeMainMiddlebutton != null) NodeMainMiddlebutton.Text = Text;
            if (NodeMainMiddlebutton != null) NodeMainMiddlebutton.ImagePath = ImagePath;
            RearrangeNode();
            //LogMessage($"init4");
            //   Invalidate();
            _isinitialized = true;
        }
        public void RearrangeNode()
        {
            SuspendLayout(); // Temporarily stop layout updates
            try
            {
                if (!isSizeCached)
                {
                    CalculateSize();
                }

                // Apply cached sizes
                UpdateDrawingRect();
                // this.Height = DrawingRect.Height; ;
                //  this.Width = DrawingRect.Width;
                //  _childrenPanel.Width = cachedWidth;
               int  padding = 5; // Padding around elements
                int startx = padding; // Horizontal start point
                int centerY = (NodeHeight - SmallNodeHeight) / 2; // Center alignment for small buttons

                // Adjust the size of the main node panel
               // _nodePanel.Width = Parent?.Width ?? DrawingRect.Width; // Match parent's width
             //   LogMessage($"Rearranging inside Nodes 1 : {_nodePanel.Width}");
                _nodePanel.Height = NodeHeight;

                // Position the toggle button
                if (_toggleButton != null)
                {
                    _toggleButton.Location = new Point(startx, centerY); // Center vertically
                    _toggleButton.Size = new Size(SmallNodeHeight, SmallNodeHeight);
                    startx += _toggleButton.Width + padding;
                }

                // Position the CheckBox
                if (_checkBox != null && _showCheckBox)
                {
                    _checkBox.Location = new Point(startx, centerY);
                    startx += _checkBox.Width + padding;
                }
                //// Position the left button
                //if (Nodeleftbutton != null)
                //{
                //    Nodeleftbutton.Location = new Point(startx, centerY); // Center vertically
                //    Nodeleftbutton.Size = new Size(SmallNodeHeight, SmallNodeHeight);
                //    startx += Nodeleftbutton.Width + padding;
                //}

                // Position the main middle button
                if (NodeMainMiddlebutton != null)
                {
                    NodeMainMiddlebutton.Location = new Point(startx, 0); // Top aligned
                    NodeMainMiddlebutton.MinimumSize = new Size(MinimumTextWidth, NodeHeight); // Ensure a minimum size
                    NodeMainMiddlebutton.Size = new Size(Math.Max(MinimumTextWidth, _nodePanel.Width - startx - SmallNodeHeight - 2 * padding), NodeHeight);
                    startx += NodeMainMiddlebutton.Width + padding;
                }

                // Position the right button
                if (Noderightbutton != null)
                {
                    Noderightbutton.Location = new Point(startx, centerY); // Center vertically
                    Noderightbutton.Size = new Size(SmallNodeHeight, SmallNodeHeight);
                    startx += Noderightbutton.Width + padding;
                }

                // Adjust the size of the node panel
                _nodePanel.Height = NodeHeight;// + padding * 2;
              
                int xlevel = 1;
                // Adjust the size of `_childrenPanel` based on expansion
                if (BeepTreeNodes.Count > 0)
                {
                    if (IsExpanded)
                    {
                        int childStartY = padding;
                        foreach (var child in BeepTreeNodes)
                        {
                           
                            child.Location = new Point((xlevel * padding), childStartY); // Indent child nodes
                            child.Width = Width - (xlevel * padding);
                            child.Theme = Theme;
                            child.RearrangeNode();
                            childStartY += child.Height ;
                        }

                        _childrenPanel.Height = childStartY;
                        _childrenPanel.Visible = true;

                    }
                    else
                    {
                        _childrenPanel.Height = 0;
                        _childrenPanel.Visible = false;

                    }
                }

                checktoggle();
                // Adjust the node's height
                Height = _nodePanel.Height + _childrenPanel.Height;

                // If the parent is a panel, adjust its height
                if (Parent is Panel parentPanel)
                {
                    parentPanel.Height = Height;
                }
            
            }
            finally
            {
                ResumeLayout(); // Resume layout updates
            }
           
        }
        public void ToggleCheckBoxVisibility(bool show)
        {
            ShowCheckBox = show;
            foreach (var child in BeepTreeNodes)
            {
                child.ToggleCheckBoxVisibility(show);
            }
        }

        // Synchronize IsSelected with the CheckBox state
        private void CheckBox_StateChanged(object sender, EventArgs e)
        {
            if (_checkBox != null)
            {
                IsSelected = _checkBox.CurrentValue;
            }
        }
        private void checktoggle()
        {
            if (BeepTreeNodes.Count > 0)
            {
                if (IsExpanded)
                {
                    _toggleButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.square-minus.svg";
                }
                else
                {
                    _toggleButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.square-plus.svg";
                }
            }else
            {
                _toggleButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.square-minus.svg";
            }
        }
        public void DrawLeftBranch()
        {
            // Draw Left Branch use startx and starty

            lefttreebranchimage = new BeepImage()
            {
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                IsFramless = true,
                IsChild = true,
                IsRounded=false,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.L.svg"
                
            };
           // lefttreebranchimage.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-down.svg";
            lefttreebranchimage.Location = new System.Drawing.Point(startx, starty);
            lefttreebranchimage.Size = new System.Drawing.Size(10, 10);
            _nodePanel.Controls.Add(lefttreebranchimage);
            startx = startx + NodeHeight;
        }
        public void DrawLeftButton()
        {
            if (_isleftbuttonVisible)
            {
                Nodeleftbutton = new BeepButton();
                Nodeleftbutton.Text = "<";
                Nodeleftbutton.Width = NodeHeight;
                if (_leftbuttonimagepath != null)
                {
                    Nodeleftbutton.ImagePath = _leftbuttonimagepath;
                }
                Nodeleftbutton.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                Nodeleftbutton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                Nodeleftbutton.IsRounded = false;
                Nodeleftbutton.IsChild = true;
                Nodeleftbutton.IsFramless = true;
                Nodeleftbutton.IsShadowAffectedByTheme = true;
                Nodeleftbutton.IsBorderAffectedByTheme = true;
                Nodeleftbutton.MaxImageSize = new System.Drawing.Size(MaxImageSize , MaxImageSize);
                Nodeleftbutton.Size = new System.Drawing.Size(NodeHeight, NodeHeight);
                Nodeleftbutton.Height = NodeHeight;
          
                Nodeleftbutton.Click += Nodeleftbutton_Click;
                Nodeleftbutton.MouseEnter += Nodeleftbutton_MouseEnter;
                Nodeleftbutton.MouseLeave += Nodeleftbutton_MouseLeave;
                Nodeleftbutton.MouseHover += Nodeleftbutton_MouseHover;
                Nodeleftbutton.MouseWheel += Nodeleftbutton_MouseWheel;
                Nodeleftbutton.MouseUp += Nodeleftbutton_MouseUp;
                Nodeleftbutton.MouseDown += Nodeleftbutton_MouseDown;
                Nodeleftbutton.MouseMove += Nodeleftbutton_MouseMove;
                _nodePanel.Controls.Add(Nodeleftbutton);
                startx = startx + NodeHeight;
            }
        }
        public void DrawRightButton()
        {
            if (_isrightbuttonVisible)
            {
                Noderightbutton = new BeepButton();
                Noderightbutton.Text = ">";
                Noderightbutton.Width = NodeHeight;
                if (_rightbuttonimagepath != null)
                {
                    Noderightbutton.ImagePath = _rightbuttonimagepath;
                }
                Noderightbutton.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                Noderightbutton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
             //   Noderightbutton.HideText = true;
                Noderightbutton.IsChild = true;
                Noderightbutton.IsRounded = false;
                Noderightbutton.IsFramless = true;
                Noderightbutton.IsShadowAffectedByTheme = true;
                Noderightbutton.IsBorderAffectedByTheme = true;
                Noderightbutton.MaxImageSize = new System.Drawing.Size(MaxImageSize , MaxImageSize);
                Noderightbutton.Size = new System.Drawing.Size(NodeHeight, NodeHeight);
                
              
                Noderightbutton.Click += Noderightbutton_Click;
                Noderightbutton.MouseEnter += Noderightbutton_MouseEnter;
                Noderightbutton.MouseLeave += Noderightbutton_MouseLeave;
                Noderightbutton.MouseHover += Noderightbutton_MouseHover;
                Noderightbutton.MouseWheel += Noderightbutton_MouseWheel;
                Noderightbutton.MouseUp += Noderightbutton_MouseUp;
                Noderightbutton.MouseDown += Noderightbutton_MouseDown;
                Noderightbutton.MouseMove += Noderightbutton_MouseMove;
                _nodePanel.Controls.Add(Noderightbutton);
                startx = startx + NodeHeight;
            }
        }
        public void DrawMiddleButton()
        {
            if (_ismiddlebuttonVisible)
            {
                NodeMainMiddlebutton = new BeepButton();
                NodeMainMiddlebutton.Text = _text;
            
                if (_imageKey != null)
                {
                    NodeMainMiddlebutton.ImagePath = _imageKey;
                }
                NodeMainMiddlebutton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
                NodeMainMiddlebutton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                
                NodeMainMiddlebutton.IsChild = true;
                NodeMainMiddlebutton.IsFramless = true;
                NodeMainMiddlebutton.IsRounded = false;
                NodeMainMiddlebutton.IsShadowAffectedByTheme = false;
                NodeMainMiddlebutton.IsBorderAffectedByTheme = false;
                NodeMainMiddlebutton.MaxImageSize = new System.Drawing.Size(MaxImageSize, MaxImageSize);
               // NodeMainMiddlebutton.Size = new System.Drawing.Size(NodeWidth - 2 * NodeHeight, NodeHeight);
                //   NodeMainMiddlebutton.Font=BeepThemesManager.ToFont(_currentTheme.LabelSmall);


                NodeMainMiddlebutton.Click += NodeMainMiddlebutton_Click;
                NodeMainMiddlebutton.DoubleClick += NodeMainMiddlebutton_DoubleClick;
                NodeMainMiddlebutton.MouseEnter += NodeMainMiddlebutton_MouseEnter;
                NodeMainMiddlebutton.MouseLeave += NodeMainMiddlebutton_MouseLeave;
                NodeMainMiddlebutton.MouseHover += NodeMainMiddlebutton_MouseHover;
                NodeMainMiddlebutton.MouseWheel += NodeMainMiddlebutton_MouseWheel;
                NodeMainMiddlebutton.MouseUp += NodeMainMiddlebutton_MouseUp;
                NodeMainMiddlebutton.MouseDown += NodeMainMiddlebutton_MouseDown;
                NodeMainMiddlebutton.MouseMove += NodeMainMiddlebutton_MouseMove;
                _nodePanel.Controls.Add(NodeMainMiddlebutton);
            }
        }
        private void DrawToggleButton()
        {
            _toggleButton = new BeepButton
            {
                Text = "-",
                Size = new Size(SmallNodeHeight, SmallNodeHeight),
                ImageAlign = System.Drawing.ContentAlignment.MiddleCenter,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText,
                IsChild = true,
                ShowAllBorders = false,
                HideText = true,
                IsFramless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ApplyThemeOnImage = true,
                IsRounded = false,
                MaxImageSize = new System.Drawing.Size(SmallNodeHeight - 2, SmallNodeHeight - 2),
                Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall),
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.square-minus.svg"
            };

            _toggleButton.Click += (s, e) =>
            {
                IsExpanded = !IsExpanded;
            };

            _nodePanel.Controls.Add(_toggleButton);
        }
        private void UpdatePanelSize()
        {
            try
            {
                // Calculate the new height of the node panel
                int newPanelHeight = NodeHeight;
                int newHeight = newPanelHeight + (IsExpanded ? ChildNodesHeight : 0);

                // Only update sizes if there is a change
                if (_nodePanel.Height != newPanelHeight)
                {
                    _nodePanel.Height = newPanelHeight;
                }

                if (Height != newHeight)
                {
                    Height = newHeight;

                    // Notify parent tree or layout if size changes
                    if (Parent is Panel parentPanel)
                    {
                        parentPanel.Height = Height;
                    }
                }

                // Rearrange child nodes if the expanded state has changed
                if (IsExpanded)
                {
                    foreach (var child in Traverse())
                    {
                        child.RearrangeNode();
                    }
                }

                // Call RearrangeTree only if necessary
                //Tree?.RearrangeTree();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdatePanelSize: {ex.Message}");
            }
        }
        private void ToggleButton_Click(object? sender, EventArgs e)
        {
            IsExpanded = !IsExpanded;
           
        }
        private void ToggleExpansion()
        {
            if (_toggleButton != null)
            {
               // _toggleButton.Text = IsExpanded ? "-" : "+";
               _toggleButton.ImagePath = IsExpanded ? "TheTechIdea.Beep.Winform.Controls.GFX.SVG.square-minus.svg" : "TheTechIdea.Beep.Winform.Controls.GFX.SVG.square-plus.svg";
            }

            UpdatePanelSize();
            RearrangeNode();
            Tree?.RearrangeTree();
        }
        #endregion "Painting Methods"
        public override void ApplyTheme()
        {
           // base.ApplyTheme();
            if (NodeMainMiddlebutton != null)
            {
                NodeMainMiddlebutton.Theme = Theme;
                NodeMainMiddlebutton.Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                
            }
            if (Nodeleftbutton != null)
            {
                Nodeleftbutton.Theme = Theme;
                if (Noderightbutton != null)
                {
                    Noderightbutton.Theme = Theme;
                }
                foreach (var item in BeepTreeNodes)
                {
                    item.Theme = Theme;
                   // item.ApplyTheme();
                }

            }
            if (_checkBox != null)
            {
                _checkBox.Theme = Theme;
                _checkBox.ApplyTheme();
            }
            NodeMainMiddlebutton.ForeColor = _currentTheme.AccentColor;
            NodeMainMiddlebutton.BackColor = _currentTheme.PanelBackColor;
            _nodePanel.BackColor = _currentTheme.PanelBackColor;
            _childrenPanel.BackColor = _currentTheme.PanelBackColor;
            this.BackColor = _currentTheme.PanelBackColor;
            _toggleButton.BackColor = _currentTheme.PanelBackColor;
            _toggleButton.ForeColor = _currentTheme.AccentColor;
       //     Noderightbutton.BackColor = _currentTheme.PanelBackColor;
        }
        public void HilightNode()
        {
            NodeMainMiddlebutton.ForeColor = _currentTheme.ButtonHoverForeColor;
            NodeMainMiddlebutton.BackColor = _currentTheme.ButtonHoverBackColor;
         //   LogMessage("Hilight");

        }
        public void UnHilightNode()
        {
            NodeMainMiddlebutton.ForeColor = _currentTheme.ButtonForeColor;
            NodeMainMiddlebutton.BackColor = _currentTheme.ButtonBackColor;
         //   LogMessage("UnHilight");

        }
        public void ChangeNodeImageSettings()
        {
            if (NodeMainMiddlebutton != null)
            {
                
                NodeMainMiddlebutton.TextImageRelation = ShowNodeImage ? System.Windows.Forms.TextImageRelation.ImageBeforeText : System.Windows.Forms.TextImageRelation.TextBeforeImage;
                NodeMainMiddlebutton.ImageAlign = ShowNodeImage ? System.Drawing.ContentAlignment.MiddleLeft : System.Drawing.ContentAlignment.MiddleCenter;
                NodeMainMiddlebutton.TextAlign = ShowNodeImage ? System.Drawing.ContentAlignment.MiddleCenter : System.Drawing.ContentAlignment.MiddleLeft;
                NodeMainMiddlebutton.MaxImageSize = new System.Drawing.Size(MaxImageSize, MaxImageSize);

            }
            if (ShowNodeImage)
            {
                if (NodeMainMiddlebutton != null)
                {
                    NodeMainMiddlebutton.ImagePath = _imageKey;
                }
            }
            else
            {
                if (NodeMainMiddlebutton != null)
                {
                    NodeMainMiddlebutton.ImagePath = null;
                }
            }
            foreach (var item in BeepTreeNodes)
            {
                item.ShowNodeImage = _shownodeimage;
                item.ChangeNodeImageSettings();
            }
            
            Invalidate();

        }
        #region "Node Methods"
        public void AddNode(BeepTreeNode node)
        {
            BeepTreeNodes.Add(node);
        }
        public void RemoveNode(BeepTreeNode node)
        {
            BeepTreeNodes.Remove(node);
        }
        public void RemoveNode(string NodeName)
        {
            BeepTreeNode node = BeepTreeNodes.Where(c => c.Name == NodeName).FirstOrDefault();
            if (node != null)
            {
                BeepTreeNodes.Remove(node);
            }
        }
        public void RemoveNode(int NodeIndex)
        {
            if (NodeIndex >= 0 && NodeIndex < BeepTreeNodes.Count)
            {
                BeepTreeNodes.RemoveAt(NodeIndex);
            }
        }
        public void ClearNodes()
        {
            BeepTreeNodes.Clear();
        }
        public BeepTreeNode GetNode(string NodeName)
        {
            return BeepTreeNodes.Where(c => c.Name == NodeName).FirstOrDefault();
        }
        public BeepTreeNode GetNode(int NodeIndex)
        {
            if (NodeIndex >= 0 && NodeIndex < BeepTreeNodes.Count)
            {
                return BeepTreeNodes[NodeIndex];
            }
            else
            {
                return null;
            }
        }
        public List<BeepTreeNode> GetNodes()
        {
            return BeepTreeNodes;
        }
       
        public void ExpandAll()
        {
            IsExpanded = true;
            foreach (var child in BeepTreeNodes)
            {
                child.ExpandAll();

            }
            _toggleButton.Text = "-";
            RearrangeNode();
        }
        public void MiniAll()
        {
            IsExpanded = true;
            foreach (var child in BeepTreeNodes)
            {
                child.MiniAll();
            }
            _toggleButton.Text = "-";
            RearrangeNode();
        }


        #endregion "Node Methods"
        #region Child Node Management
        public void BeginUpdate()
        {
            SuspendLayout();
        }

        public void EndUpdate()
        {
            ResumeLayout();
            RearrangeNode();
        }
      
        public void HighlightSelectedNode()
        {
            if (IsSelected)
            {
                BackColor = _currentTheme.HighlightBackColor;
            }
            else
            {
                BackColor = _currentTheme.PanelBackColor;
            }
        }

        public void SyncCheckedState()
        {
            foreach (var child in BeepTreeNodes)
            {
                child.IsChecked = this.IsChecked;
                child.SyncCheckedState();
            }
        }
        /// <summary>
        /// Calculates and caches the size of the node based on its current state.
        /// </summary>
        private void CalculateSize()
        {
            if (isSizeCached && Tree != null && cachedWidth == Tree.Width - 20)
            {
                // Size is already cached and fits the current Tree width
                return;
            }

            using (Graphics g = this.CreateGraphics())
            {
                SizeF textSize = g.MeasureString(_text, this.Font);
                int textHeight = (int)textSize.Height + 10; // Add padding

                // Set width based on the parent BeepTree's width
                if (Tree != null)
                {
                    // Assuming the panel in BeepTree subtracts 20 pixels for padding
                    cachedWidth = Tree.Width - 20; // Adjust as needed
                }
                else
                {
                    cachedWidth = NodeWidth; // Fallback to default if Tree is not set
                }

                cachedHeight = textHeight + (_isExpanded ? ChildNodesHeight : 0);
            }

            isSizeCached = true;
        }


        private bool IsSizeCached { get; set; } = false;

        public void InvalidateSizeCache()
        {
            IsSizeCached = false;
        }
        public IEnumerable<BeepTreeNode> Traverse()
        {
            yield return this;
            foreach (var child in BeepTreeNodes)
            {
                foreach (var descendant in child.Traverse())
                {
                    yield return descendant;
                }
            }
        }

        public void AddChild(BeepTreeNode child)
        {
            BeepTreeNodes.Add(child);
            LogMessage($"AddChild {child.Text}");
            //items.Add(child.Tag as SimpleItem);
            _childrenPanel.Controls.Add(child);
            UpdatePanelSize();
        }

        public void RemoveChild(BeepTreeNode child)
        {
            BeepTreeNodes.Remove(child);
           // items.Remove(child.Tag as SimpleItem);
            _childrenPanel.Controls.Remove(child);
            UpdatePanelSize();
        }

        public void ClearChildren()
        {
            BeepTreeNodes.Clear();
            //items.Clear();
            _childrenPanel.Controls.Clear();
            UpdatePanelSize();
        }
        public IEnumerable<BeepTreeNode> GetDescendants()
        {
            if (!IsExpanded) yield break;
            foreach (var child in BeepTreeNodes)
            {
                yield return child;
                foreach (var descendant in child.GetDescendants())
                {
                    yield return descendant;
                }
            }
        }

        #endregion
        private void LogMessage(string message)
        {
            try
            {
                File.AppendAllText(@"C:\Logs\debug_log.txt", $"{DateTime.Now}: {message}{Environment.NewLine}");

            }
            catch { /* Ignore logging errors */ }
        }
    }
}
