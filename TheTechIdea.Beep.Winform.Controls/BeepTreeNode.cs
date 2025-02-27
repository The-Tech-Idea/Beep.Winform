using System.ComponentModel;
using System.Drawing.Design;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Winform.Controls.Properties;
using System.Diagnostics;
using TheTechIdea.Beep.Desktop.Common.Helpers;



namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(false)]
    public class BeepTreeNode : BeepControl
    {
        #region "Events"

        public EventHandler<BeepMouseEventArgs> LeftButtonClicked;
        public EventHandler<BeepMouseEventArgs> RighButtonClicked;
        public EventHandler<BeepMouseEventArgs> MiddleButtonClicked;
        public EventHandler<BeepMouseEventArgs> NodeClicked;
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
        private BindingList<BeepTreeNode> _beeptreenodes=new BindingList<BeepTreeNode>();
       
        private int cachedHeight;
        private int cachedWidth;
        private bool isSizeCached = false;
        private int _nodeseq;
        private string _key;

        private string _imageKey;
        private string _selectedImageKey;
        private bool _isExpanded=false;
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

       // private string _guidid=Guid.NewGuid().ToString();
        private Panel _nodePanel; // Panel to encapsulate the node
        private Panel _childrenPanel; // Panel for child nodes

        private string _nodeLevelID = string.Empty;
        private int childnodesSeq = 0;

        private bool _ischildDrawn = false;
        private SimpleItem _nodeinfo=new SimpleItem();
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public SimpleItem NodeInfo
        {
            get => _nodeinfo;
            set
            {
                _nodeinfo = value;
                if (NodeMainMiddlebutton != null)
                {
                    NodeMainMiddlebutton.Text = _nodeinfo.Text;
                    NodeMainMiddlebutton.ImagePath = _nodeinfo.ImagePath;
                }
            }
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
       
        private int _parentid;
        public int NodeHeight
        {
            get => Tree==null ? 20:Tree.NodeHeight;
        }
        public int NodeWidth { get; set; } = 100;
        public int SmallNodeHeight { get; set; } = 14;
        private int nodeimagesize = 14;
        public int MinimumTextWidth { get; set; } = 100;
        int padding = 5; // Padding around elements
        int spacing =1; // Padding around elements
        public string NodeDataType { get; set; } = "SimpleItem";
        public string LocalizedText => Resources.ResourceManager.GetString(Key) ?? Text;
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
                //if (NodesControls.Count > 0)
                //{
                //    foreach (var item in NodesControls)
                //    {
                //        item.NodeLevelID = $"{_nodeLevelID}_{item.NodeSeq}";
                //    }
                //}
            }

        }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BindingList<BeepTreeNode> NodesControls {
            get => _beeptreenodes;
            //set
            //{
            //    _beeptreenodes = value;
            //}
        }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ParentID
        {
            get => _parentid;
            set
            {
                _parentid = value;
                _nodeLevelID = _parentid.ToString();


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

                foreach (var child in NodesControls)
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

       
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]

        public BindingList<SimpleItem> Nodes
        {
            get => NodeInfo.Children;
            set
            {
                NodeInfo.Children = value;
            }

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

        public bool IsInvalidated { get; set; }= false;

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
            get { return NodeMainMiddlebutton.Text; }
            set { if(NodeMainMiddlebutton!=null) NodeMainMiddlebutton.Text = value; }
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
                if (NodeMainMiddlebutton != null)
                {
                    NodeMainMiddlebutton.IsSelected = _isSelected;
                }
                Invalidate(); // Redraw the node if necessary
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; _isSelected = value; }
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
                ApplyTheme();
                Invalidate();


            }
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
            _beeptreenodes = new BindingList<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _beeptreenodes= new BindingList<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            _beeptreenodes= new BindingList<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey, bool isExpanded)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            _isExpanded = isExpanded;
            _beeptreenodes= new BindingList<BeepTreeNode>();
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
            _beeptreenodes= new BindingList<BeepTreeNode>();
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
            _beeptreenodes= new BindingList<BeepTreeNode>();
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
            _beeptreenodes= new BindingList<BeepTreeNode>();
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
            _beeptreenodes= new BindingList<BeepTreeNode>();
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
            _beeptreenodes= new BindingList<BeepTreeNode>();
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
            _beeptreenodes= new BindingList<BeepTreeNode>();
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
            _beeptreenodes= new BindingList<BeepTreeNode>();
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
            _beeptreenodes= new BindingList<BeepTreeNode>();
            InitNode();
        }
        #endregion "Constructors"
        #region "Events Methods"
        #region "MiddleButton"
        private void NodeMainMiddlebutton_MouseMove(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseMove", this);
            NodeMouseMove?.Invoke(this, args);
        
        }
        private void NodeMainMiddlebutton_Click(object sender, EventArgs e)
        {
            ////Debug.WriteLine("Middel button Clicked");
            //// When a node is clicked, set this node as the selected node in the tree.
            //if (this.Tree != null)
            //{
            //    this.Tree.SelectedNode = this;
            //}
            //BeepMouseEventArgs args = new BeepMouseEventArgs("Click", this);
            //NodeClicked?.Invoke(this, args);
            //if (Tree.LastNodeSelected != null)
            //{
            //    Tree.LastNodeSelected.IsSelected = false;
            //}
            //Tree.LastNodeSelected = this;
            //NodeMainMiddlebutton.IsSelected = true;
        }
        private void NodeMainMiddlebutton_MouseDown(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine("MouseDown");
            base.OnMouseDown(e);
            //Debug.WriteLine("Middel button MouseDown");
            BeepMouseEventArgs args = MiscFunctions.GetMouseEventArgs("MouseDown", e);
            NodeMouseDown?.Invoke(this, args);
            if (e.Button == MouseButtons.Left)
            {
                NodeClicked?.Invoke(this, new BeepMouseEventArgs("Click", this));
                DoDragDrop(this, DragDropEffects.Move);
            }
            else
            {
                args = MiscFunctions.GetMouseEventArgs("RightClick", e);
                NodeRightClicked?.Invoke(this, args);
                ShowMenu?.Invoke(this, args);
            }
            if (Tree.LastNodeSelected != null)
            {
                Tree.LastNodeSelected.IsSelected = false;
            }

            Tree.LastNodeSelected = this;
            NodeMainMiddlebutton.IsSelected = true;

        }
        private void BeepTreeNode_MouseDown(object? sender, MouseEventArgs e)
        {
           
        }
        private void NodeMainMiddlebutton_DoubleClick(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("DoubleClick", this);
            NodeDoubleClicked?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_MouseEnter(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseEnter", this);
            NodeMouseEnter?.Invoke(this, args);
            //HilightNode();
            NodeMainMiddlebutton.IsHovered = true;

        }
        private void NodeMainMiddlebutton_MouseLeave(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseLeave", this);
            NodeMouseLeave?.Invoke(this, args);
            NodeMainMiddlebutton.IsHovered = false;
            // UnHilightNode();
        }
        private void NodeMainMiddlebutton_MouseHover(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseHover", this);
            NodeMouseHover?.Invoke(this, args);
           // HilightNode();
            NodeMainMiddlebutton.IsHovered = true;

        }
        private void NodeMainMiddlebutton_MouseWheel(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseWheel", this);
            NodeMouseWheel?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_MouseUp(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseUp", this);
            NodeMouseUp?.Invoke(this, args);
        }
      
        private void BeepTreeNode_LostFocus(object? sender, EventArgs e)
        {
            //Debug.WriteLine("LostFocus");
            NodeMainMiddlebutton.IsSelected = false;
        }

        private void BeepTreeNode_Click(object? sender, EventArgs e)
        {
            //Debug.WriteLine("Clicked");
        }
        private void BeepTreeNode_MouseHover(object? sender, EventArgs e)
        {
            //Debug.WriteLine("Hovered");
            NodeMainMiddlebutton.IsHovered = true;
        }

      
        private void BeepTreeNode_MouseLeave(object? sender, EventArgs e)
        {
            //Debug.WriteLine("MouseLeave");
            NodeMainMiddlebutton.IsHovered =false;
            
        }
        #endregion "MiddleButton"
        #region "LeftButton"
        private void Nodeleftbutton_Click(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("LeftClick", this);
            NodeLeftClicked?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseEnter(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseEnter", this);
            NodeMouseEnter?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseLeave(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseLeave", this);
            NodeMouseLeave?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseHover(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseHover", this);
            NodeMouseHover?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseWheel(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseWheel", this);
            NodeMouseWheel?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseUp(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseUp", this);
            NodeMouseUp?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseDown(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseDown", this);
            NodeMouseDown?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseMove(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseMove", this);
            NodeMouseMove?.Invoke(this, args);
        }
        #endregion  "LeftButton"
        #region "RightButton"
        private void Noderightbutton_Click(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("Right", this);
            NodeRightClicked?.Invoke(this, args);

        }
        private void Noderightbutton_MouseEnter(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseEnter", this);
            NodeMouseEnter?.Invoke(this, args);
        }
        private void Noderightbutton_MouseLeave(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseLeave", this);
            NodeMouseLeave?.Invoke(this, args);
        }
        private void Noderightbutton_MouseHover(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseHover", this);
            NodeMouseHover?.Invoke(this, args);
        }
        private void Noderightbutton_MouseWheel(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseWheel", this);
            NodeMouseWheel?.Invoke(this, args);
        }
        private void Noderightbutton_MouseUp(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseUp", this);
            NodeMouseUp?.Invoke(this, args);
        }
        private void Noderightbutton_MouseDown(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseDown", this);
            NodeMouseDown?.Invoke(this, args);
        }
        private void Noderightbutton_MouseMove(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseMove", this);
            NodeMouseMove?.Invoke(this, args);
        }
        #endregion "RightButton"
        #endregion "Events Methods"
        #region "Painting Methods"
        int startx = 0;
        int starty = 0;
        private int toggelbuttonsize=14;

        public void InitNode()
        {
          //  LogMessage($"init");
            UpdateDrawingRect();
            Margin = new Padding(1);
            Padding = new Padding(1);
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
            _beeptreenodes= new BindingList<BeepTreeNode>();
            _nodePanel.Controls.Clear();
        //     LogMessage($"init1");
            IsShadowAffectedByTheme =false;
            IsBorderAffectedByTheme = false;
            IsFrameless = true;
            IsChild = true;
            startx = DrawingRect.Left;
            starty = DrawingRect.Top;
            IsExpanded=false;
            NodeWidth = DrawingRect.Width;
           // LogMessage($"init2");
            _checkBox = new BeepCheckBoxBool
            {
                CheckedValue = true,
                UncheckedValue = false,
                CurrentValue = IsSelected,
                Visible = _showCheckBox,
                IsChild = true,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                Theme = Theme,
                IsRounded = false,
                HideText = true,
                
            };
            _checkBox.Size = new Size(toggelbuttonsize, toggelbuttonsize);
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
       
            if (NodeMainMiddlebutton != null) NodeMainMiddlebutton.Text = Text;
            if (NodeMainMiddlebutton != null) NodeMainMiddlebutton.ImagePath = ImagePath;
           // Nodes.ListChanged += Nodes_ListChanged;
         //   this.Click += BeepTreeNode_Click;
          //  this.MouseDown += BeepTreeNode_MouseDown;
            this.MouseHover += BeepTreeNode_MouseHover;
            this.LostFocus += BeepTreeNode_LostFocus;
            this.MouseLeave += BeepTreeNode_MouseLeave;
            //LogMessage($"init4");
            //   Invalidate();
            _isinitialized = true;
            _isExpanded=false;
           IsInvalidated=true;
            NodeMainMiddlebutton.PopPosition = BeepPopupFormPosition.Right;
            NodeMainMiddlebutton.SelectedItemChanged += NodeMainMiddlebutton_SelectedItemChanged;
            // RearrangeNode();
            ApplyTheme();
            RearrangeNode();
        }
        public void RearrangeNode()
        {
            SuspendLayout(); // Temporarily stop layout updates
            try
            {
                if (NodeInfo == null) return;

                CalculateSize();
                UpdateDrawingRect();
                int startx = 0; // Horizontal start point
                int centerY = (NodeHeight - MaxImageSize) / 2; // Center alignment for small buttons

                _nodePanel.Height = NodeHeight;

                // Position the toggle button
                if (_toggleButton != null)
                {
                    _toggleButton.Size = new Size(toggelbuttonsize, toggelbuttonsize);
                    int centerYfortoggle = (NodeHeight - _toggleButton.Size.Height) / 2;
                    _toggleButton.Location = new Point(startx, centerYfortoggle);
                    _toggleButton.MaxImageSize = new Size(toggelbuttonsize - 1, toggelbuttonsize - 1);
                    startx += _toggleButton.Width + spacing;
                }

                // Position the CheckBox
                if (_checkBox != null && _showCheckBox)
                {
                    _checkBox.Size = new Size(toggelbuttonsize, toggelbuttonsize);
                    int centerYforcheckbox = (NodeHeight - _checkBox.Size.Height) / 2;
                    _checkBox.Location = new Point(startx, centerYforcheckbox);
                    _checkBox.CheckBoxSize = toggelbuttonsize - 1;
                    startx += _checkBox.Width + spacing;
                }

                // Position the main middle button
                if (NodeMainMiddlebutton != null)
                {
                    NodeMainMiddlebutton.MinimumSize = new Size(MinimumTextWidth, NodeHeight);
                    NodeMainMiddlebutton.Size = new Size(Math.Max(MinimumTextWidth, _nodePanel.Width - startx - SmallNodeHeight - 2 * padding), NodeHeight);
                    int centerYformainbutton = (NodeHeight - NodeMainMiddlebutton.Size.Height) / 2;
                    NodeMainMiddlebutton.Location = new Point(startx, centerYformainbutton);
                    startx += NodeMainMiddlebutton.Width + spacing;
                }

                // Position the right button
                if (Noderightbutton != null)
                {
                    Noderightbutton.Location = new Point(startx, centerY);
                    Noderightbutton.Size = new Size(SmallNodeHeight, SmallNodeHeight);
                    startx += Noderightbutton.Width + spacing;
                }

                _nodePanel.Height = NodeHeight + spacing;

                int xlevel = 1;
                if (NodeInfo.Children != null)
                {
                    if (NodeInfo.Children.Count > 0)
                    {
                        if (IsExpanded)
                        {
                            int childStartY = padding;
                            foreach (var child in NodesControls)
                            {
                                if (!_childrenPanel.Controls.Contains(child))
                                {
                                    _childrenPanel.Controls.Add(child); // Ensure child is in UI
                                }
                                child.NodeInfo.IsDrawn = true; // Mark as drawn
                                child.Location = new Point(xlevel * padding, childStartY); // Indent child nodes
                                child.Width = Width - (xlevel * padding);
                                child.Theme = Theme;
                                child.RearrangeNode(); // Recursively arrange children
                                childStartY += child.Height + padding;
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
                    else
                    {
                        _childrenPanel.Height = 0;
                        _childrenPanel.Visible = false;
                    }

                }
                else
                {
                    _childrenPanel.Height = 0;
                    _childrenPanel.Visible = false;
                }

                checktoggle();
                Height = _nodePanel.Height + _childrenPanel.Height;

                if (Parent is Panel parentPanel)
                {
                    parentPanel.Height = Height;
                }
            }
            finally
            {
                checktoggle();
                IsInvalidated = true;
                ResumeLayout(); // Resume layout updates
            }
        }
        public void ToggleCheckBoxVisibility(bool show)
        {
            _showCheckBox = show;
            foreach (var child in NodesControls)
            {
                child.ShowCheckBox=show;
            }

        }
        // Synchronize IsSelected with the CheckBox state
        private void CheckBox_StateChanged(object sender, EventArgs e)
        {
            if (_checkBox != null)
            {
                IsSelected = _checkBox.CurrentValue;

                // Raise appropriate events without modifying SelectedNodes directly.
                if (IsSelected)
                {
                    NodeChecked?.Invoke(this, new BeepMouseEventArgs("NodeSelected", this));
                }
                else
                {
                    NodeChecked?.Invoke(this, new BeepMouseEventArgs("NodeDeselected", this));
                }
            }
        }
        private void checktoggle()
        {
            if (_toggleButton != null)
            {
               
                if (NodeInfo != null)
                {
                    if (NodeInfo.Children == null) return;
                    if (NodeInfo.Children.Count > 0)
                    {
                        if (IsExpanded)
                        {
                            _toggleButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.minus.svg";
                        }
                        else
                        {
                            _toggleButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.plus.svg";
                        }
                    }
                 else
                 {
                        _toggleButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.minus.svg";
                 }
                }
            }
    
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
                Nodeleftbutton.IsFrameless = true;
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
                Noderightbutton.IsFrameless = true;
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
              //  NodeMainMiddlebutton.PopupMode = true;
                NodeMainMiddlebutton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
                NodeMainMiddlebutton.TextAlign = TextAlignment;
                NodeMainMiddlebutton.IsChild = false;
                NodeMainMiddlebutton.IsFrameless = true;
                NodeMainMiddlebutton.IsRounded = false;
                NodeMainMiddlebutton.IsShadowAffectedByTheme = false;
                NodeMainMiddlebutton.IsBorderAffectedByTheme = false;
                NodeMainMiddlebutton.MaxImageSize = new System.Drawing.Size(MaxImageSize, MaxImageSize);
               // NodeMainMiddlebutton.Size = new System.Drawing.Size(NodeWidth - 2 * NodeHeight, NodeHeight);
                //   NodeMainMiddlebutton.TextFont=BeepThemesManager.ToFont(_currentTheme.LabelSmall);

              //  NodeMainMiddlebutton.
          
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
                Text = string.Empty,
                Size = new Size(SmallNodeHeight, SmallNodeHeight),
                ImageAlign = System.Drawing.ContentAlignment.MiddleCenter,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText,
                IsChild = true,
                ShowAllBorders = false,
                HideText = true,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ApplyThemeOnImage = true,
                IsRounded = false,
                MaxImageSize = new System.Drawing.Size(SmallNodeHeight - 2, SmallNodeHeight - 2),
                Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall),
                UseScaledFont = false,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.minus.svg"
            };
            _toggleButton.HideText = true;
            _toggleButton.Text= string.Empty;
            _toggleButton.Click += (s, e) =>
            {
                IsExpanded = !IsExpanded;
            };

            _nodePanel.Controls.Add(_toggleButton);
        }
        public void UpdatePanelSize()
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
                //StandardTree?.RearrangeTree();
            }
            catch (Exception ex)
            {
               // Console.WriteLine($"Error in UpdatePanelSize: {ex.Message}");
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
                _toggleButton.ImagePath = _isExpanded ? "TheTechIdea.Beep.Winform.Controls.GFX.SVG.minus.svg" : "TheTechIdea.Beep.Winform.Controls.GFX.SVG.plus.svg";
            }
            else return;
            if (Nodes.Count > 0)
            {
                IsInvalidated = true;
               // 
            }
         //  RearrangeNode();
          Tree?.RearrangeTree();

        }
        #endregion "Painting Methods"
        #region "Theme Methods"
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            
         //  // Console.WriteLine("Font Changed");
            if (AutoSize)
            {
                Size textSize = TextRenderer.MeasureText(Text, _textFont);
                this.Size = new Size(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
            }
        }
        public override void ApplyTheme()
        {
            // base.ApplyTheme();
            if (NodeMainMiddlebutton != null)
            {
                NodeMainMiddlebutton.Theme = Theme;
                if (UseThemeFont)
                {
                    _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                }
                NodeMainMiddlebutton.Font = _textFont; 
                Font = _textFont;
            }
            if (Noderightbutton != null)
            {
                Noderightbutton.Theme = Theme;
            }
            if (Nodeleftbutton != null)
            {
                Nodeleftbutton.Theme = Theme;
            }
            foreach (var item in NodesControls)
            {
                item.Theme = Theme;
                // item.ApplyTheme();
            }
            if (_checkBox != null)
            {
                _checkBox.Theme = Theme;
                _checkBox.ApplyTheme();
            }
            if (_toggleButton != null)
            {
                _toggleButton.ImageEmbededin = ImageEmbededin.Button;
                _toggleButton.ApplyThemeOnImage = true;
                _toggleButton.Theme = Theme;
            }
           // NodeMainMiddlebutton.ForeColor = _currentTheme.AccentColor;
           // NodeMainMiddlebutton.BackColor = _currentTheme.PanelBackColor;
            _nodePanel.BackColor = _currentTheme.ButtonBackColor;
            _childrenPanel.BackColor = _currentTheme.ButtonBackColor;
            this.BackColor = _currentTheme.ButtonBackColor;
     

            //     Noderightbutton.BackColor = _currentTheme.PanelBackColor;
        }
        //public void HilightNode()
        //{
        //    NodeMainMiddlebutton.ForeColor = _currentTheme.ButtonHoverForeColor;
        //    NodeMainMiddlebutton.BackColor = _currentTheme.ButtonHoverBackColor;
        //    //   LogMessage("Hilight");

        //}
        //public void UnHilightNode()
        //{
        //    NodeMainMiddlebutton.ForeColor = _currentTheme.ButtonForeColor;
        //    NodeMainMiddlebutton.BackColor = _currentTheme.ButtonBackColor;
        //    //   LogMessage("UnHilight");

        //}
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
            foreach (var item in NodesControls)
            {
                item.ShowNodeImage = _shownodeimage;
                item.ChangeNodeImageSettings();
            }

            Invalidate();

        }
        #endregion "Theme Methods"
        #region "Node Methods"
        private void ChangeTextAlignment()
        {
            NodeMainMiddlebutton.TextAlign = textAlign;
            // change all text alignment of all nodes
            foreach (var node in _childrenPanel.Controls)
            {
                if (node is BeepTreeNode)
                {
                    BeepTreeNode n = (BeepTreeNode)node;
                    n.TextAlignment = textAlign;
                }
            }
            IsInvalidated = true;
        }
        //private void Nodes_ListChanged(object? sender, ListChangedEventArgs e)
        //{
        //    try
        //    {
        //        switch (e.ListChangedType)
        //        {
        //            case ListChangedType.ItemAdded:
        //                // Handle node addition
        //                if (e.NewIndex >= 0 && e.NewIndex < _beeptreenodes.Count)
        //                {
        //                    var addedNode = NodeInfo.Children[e.NewIndex];
                          
        //                    RearrangeNode(); // Rearrange after addition
        //                }
        //                break;

        //            case ListChangedType.ItemDeleted:
        //                // Handle node removal
        //                HandleItemDeleted(e.OldIndex);
        //                break;

        //            case ListChangedType.Reset:
        //                // Handle a full reset (clear all nodes)
        //                _childrenPanel.Controls.Clear();
        //                RearrangeNode();
        //                break;

        //            default:
        //                // Other types (e.g., ItemChanged, PropertyDescriptorAdded)
        //                break;
        //        }
        //        IsInvalidated = true;
        //    }
        //    catch (Exception ex)
        //    {
        //       // Console.WriteLine($"Error in Nodes_ListChanged: {ex.Message}");
        //    }
        //}
        //private void HandleItemDeleted(int index)
        //{
        //    try
        //    {
        //        if (index < 0 || index >= NodeInfo.Children.Count)
        //        {
        //            LogMessage($"Invalid index for deletion: {index}");
        //            return;
        //        }

        //        // Get the deleted node from NodesControls
        //        var deletedNode = NodeInfo.Children[index];

        //        // Find the corresponding control in _childrenPanel
        //        var removedControl = _childrenPanel.Controls
        //            .OfType<BeepTreeNode>()
        //            .FirstOrDefault(node => node.SavedGuidID == deletedNode.GuidId);

        //        if (removedControl != null)
        //        {
        //            _childrenPanel.Controls.Remove(removedControl); // Remove from UI
        //            LogMessage($"Node with GuidID {deletedNode.GuidId} removed from UI.");
        //        }
        //        else
        //        {
        //            LogMessage($"Node with GuidID {deletedNode.GuidId} not found in _childrenPanel.");
        //        }

        //        RearrangeNode(); // Adjust layout after deletion
        //    }
        //    catch (Exception ex)
        //    {
        //       // Console.WriteLine($"Error in HandleItemDeleted: {ex.Message}");
        //    }
        //}
        protected void CreateChildNodes()
        {
          
            if (NodeInfo.Children.Count == 0)
            {
                return;
            }
            foreach (var item in NodeInfo.Children)
            {
                // check if 
                if (item.IsDrawn)
                {
                    continue;
                }
                var node = Tree.CreateTreeNodeFromMenuItem(item, this);
                if (node != null)
                {
                    node.GuidID = item.GuidId;
                    node.ParentID= this.Id;

                    addnode(node);
                }
            }
            IsInvalidated = true;
        }
       
        /// <summary>
        /// Adds a new node to the current node's collection and updates the UI.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void AddNode(SimpleItem node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            var treenode = Tree.CreateTreeNodeFromMenuItem(node, this);
            if (treenode != null)
            {
                treenode.GuidID = node.GuidId;
                treenode.ParentID = this.Id;
                node.IsDrawn = IsExpanded; // Draw if parent is expanded
                AddNode(treenode); // Use the unified AddNode method
            }
        }
        public void AddNodeNoDraw(SimpleItem node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
             node.IsDrawn = true;
            NodeInfo.Children.Add(node);

        }
        private void addnode(BeepTreeNode node)
        {
            if (node != null && !NodesControls.Contains(node))
            {
                NodesControls.Add(node);
                SimpleItem newnode = (SimpleItem)node.Tag;
                // if newnode not in nodes then add it
              
                _childrenPanel.Controls.Add(node); // Add to the UI panel
                node.NodeInfo.IsDrawn = true;
                node.ParentID = this.Id;
                
            }
            IsInvalidated = true;
        }
        public void AddNode(BeepTreeNode node)
        {
            if (node != null && !NodesControls.Contains(node))
            {
                NodesControls.Add(node);
                SimpleItem newItem = node.NodeInfo;
                newItem.GuidId = node.GuidID; // Ensure SimpleItem shares GuidId
                newItem.ContainerGuidID = node.GuidID;
                newItem.RootContainerGuidID = this.GuidID; // Set to parent's GuidID
                newItem.IsDrawn = true;

                if (!Nodes.Contains(newItem))
                {
                    Nodes.Add(newItem); // Updates NodeInfo.Children
                }

                _childrenPanel.Controls.Add(node);
                node.ParentNode = this;
                node.Tree = this.Tree;
                node.RearrangeNode();
                IsInvalidated = true;
                // Update toggle button visibility
                checktoggle();
            }
        }
        // remove child nodes that are draw when the node is collapsed
        protected void RemoveChildNodes()
        {
            SimpleItem simpleItem = (SimpleItem)Tag;
            if (simpleItem.Children.Count == 0)
            {
                return;
            }
            foreach (var item in simpleItem.Children)
            {
                // check if Drawn then remove it
                if (item.IsDrawn)
                {
                    BeepTreeNode node = GetNodeByGuid(item.GuidId);
                    RemoveNode(node);
                }

            }
            IsInvalidated = true;

        }
        /// <summary>
        /// Removes a specific node from the current node's collection and updates the UI.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        public void RemoveNode(BeepTreeNode node)
        {
            if (node != null && NodesControls.Contains(node))
            {
                NodesControls.Remove(node); // Remove from UI
                _childrenPanel.Controls.Remove(node);
                Nodes.Remove(node.NodeInfo); // Remove from NodeInfo.Children
                node.NodeInfo.IsDrawn = false;
                IsInvalidated = true;
                RearrangeNode();
            }
        }
        public void RemoveNode(SimpleItem node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            Nodes.Remove(node);
            IsInvalidated = true;

        }
        /// <summary>
        /// Removes a node by its name if it exists.
        /// </summary>
        /// <param name="NodeName">The name of the node to remove.</param>
        public void RemoveNode(string NodeName)
        {
            var node = NodesControls.FirstOrDefault(c => c.Name == NodeName);
            if (node != null)
            {
               // RemoveNode(node);
                SimpleItem simpleItem = (SimpleItem)node.Tag;
                Nodes.Remove(simpleItem);
                IsInvalidated = true;
            }
            
        }

        /// <summary>
        /// Removes a node by its index if it exists.
        /// </summary>
        /// <param name="NodeIndex">The index of the node to remove.</param>
        public void RemoveNode(int NodeIndex)
        {
            if (NodeIndex >= 0 && NodeIndex < NodesControls.Count)
            {
                var node = NodesControls[NodeIndex];
                SimpleItem simpleItem = (SimpleItem)node.Tag;
                Nodes.Remove(simpleItem);
                IsInvalidated = true;
            }
        }

        /// <summary>
        /// Clears all child nodes.
        /// </summary>
        public void ClearNodes()
        {
            NodesControls.Clear(); // Automatically triggers Nodes_ListChanged
            IsInvalidated = true;
        }

        /// <summary>
        /// Retrieves a node by its name.
        /// </summary>
        /// <param name="NodeName">The name of the node to retrieve.</param>
        /// <returns>The matching node or null if not found.</returns>
        public BeepTreeNode GetNode(string NodeName)
        {
            return NodesControls.FirstOrDefault(c => c.Name == NodeName);
        }
        public BeepTreeNode GetNodeByGuid(string guid)
        {
            return NodesControls.FirstOrDefault(c => c.GuidID == guid);
        }
        /// <summary>
        /// Retrieves a node by its index.
        /// </summary>
        /// <param name="NodeIndex">The index of the node to retrieve.</param>
        /// <returns>The matching node or null if the index is out of range.</returns>
        public BeepTreeNode GetNode(int NodeIndex)
        {
            return NodeIndex >= 0 && NodeIndex < NodesControls.Count ? NodesControls[NodeIndex] : null;
        }

        /// <summary>
        /// Retrieves all child nodes.
        /// </summary>
        /// <returns>A BindingList of child nodes.</returns>
        public BindingList<BeepTreeNode> GetNodes()
        {
            return NodesControls;
        }

        /// <summary>
        /// Expands all nodes recursively.
        /// </summary>
        public void ExpandAll()
        {
            IsExpanded = true;
            foreach (var child in NodesControls)
            {
                child.ExpandAll(); // Recursive expansion
            }
            RearrangeNode();
            IsInvalidated = true;
        }

        /// <summary>
        /// Collapses all nodes recursively.
        /// </summary>
        public void MiniAll()
        {
            IsExpanded = false;
            foreach (var child in NodesControls)
            {
                child.MiniAll(); // Recursive collapse
            }
            RearrangeNode();
            IsInvalidated = true;
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
            foreach (var child in NodesControls)
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
                // Size is already cached and fits the current StandardTree width
                return;
            }

            using (Graphics g = this.CreateGraphics())
            {
                SizeF textSize = g.MeasureString(Text,NodeMainMiddlebutton.Font);
                int textHeight = (int)textSize.Height ; // Add padding

                // Set width based on the parent BeepTree's width
                if (Tree != null)
                {
                    // Assuming the panel in BeepTree subtracts 20 pixels for padding
                    cachedWidth = Tree.Width ; // Adjust as needed
                }
                else
                {
                    cachedWidth = NodeWidth; // Fallback to default if StandardTree is not set
                }

                cachedHeight = textHeight + (_isExpanded ? ChildNodesHeight : 0);
            }

            isSizeCached = true;
        }
        public IEnumerable<BeepTreeNode> Traverse()
        {
            yield return this;
            foreach (var child in NodesControls)
            {
                foreach (var descendant in child.Traverse())
                {
                    yield return descendant;
                }
            }
        }

        public void ClearChildren()
        {
            Nodes.Clear();
            UpdatePanelSize();
        }
        public IEnumerable<BeepTreeNode> GetDescendants()
        {
            if (!IsExpanded) yield break;
            foreach (var child in NodesControls)
            {
                yield return child;
                foreach (var descendant in child.GetDescendants())
                {
                    yield return descendant;
                }
            }
        }

        #endregion
        #region "Drag and Drop"
        private bool IsDropAllowed(BeepTreeNode draggedNode)
        {
            // Prevent dropping onto itself or its descendants
            if (draggedNode == this || draggedNode.Traverse().Contains(this))
            {
                return false;
            }

            // Add additional rules if necessary
            return true;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Start drag-and-drop with the current node as the data
                DoDragDrop(this, DragDropEffects.Move);
            }
            base.OnMouseDown(e);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            // Check if the dragged data is a BeepTreeNode
            if (e.Data.GetData(typeof(BeepTreeNode)) is BeepTreeNode draggedNode)
            {
                // Validate that the node can be dropped here
                if (IsDropAllowed(draggedNode))
                {
                    e.Effect = DragDropEffects.Move;
                    HighlightAsDropTarget();
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            base.OnDragEnter(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            // Ensure we provide feedback during dragging
            if (e.Data.GetData(typeof(BeepTreeNode)) is BeepTreeNode draggedNode && IsDropAllowed(draggedNode))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
            base.OnDragOver(e);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            // Remove highlight when dragging leaves the node
            RemoveDropTargetHighlight();
            base.OnDragLeave(e);
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            if (e.Data.GetData(typeof(BeepTreeNode)) is BeepTreeNode draggedNode)
            {
                // Check if the drop is valid
                if (IsDropAllowed(draggedNode))
                {
                    // Remove the node from its current parent
                    draggedNode.ParentNode?.RemoveNode(draggedNode);

                    // Add the node to the current node's children
                    AddNode(draggedNode);

                    // Optionally rearrange the tree
                    RearrangeNode();
                }
            }

            // Remove any drag feedback
            RemoveDropTargetHighlight();

            base.OnDragDrop(e);
        }
        private void HighlightAsDropTarget()
        {
            BackColor = _currentTheme.HighlightBackColor; // Change to highlight color
        }
        private void RemoveDropTargetHighlight()
        {
            BackColor = _currentTheme.PanelBackColor; // Restore to default background color
        }


        #endregion "Drag and Drop"
        #region "Filter and Find"
        /// <summary>
        /// Ensures that this node is visible within its scrollable container.
        /// </summary>
        public void EnsureVisible()
        {
            // Expand parent nodes if necessary.
            BeepTreeNode current = this.ParentNode;
            while (current != null && !current.IsExpanded)
            {
                current.IsExpanded = true;
                current = current.ParentNode;
            }
            ScrollIntoView();
        }

        /// <summary>
        /// Scrolls this node into view by finding the first parent that is a ScrollableControl.
        /// </summary>
        private void ScrollIntoView()
        {
            Control parent = this.Parent;
            while (parent != null && !(parent is ScrollableControl))
            {
                parent = parent.Parent;
            }

            if (parent is ScrollableControl scrollable)
            {
                // Adjust the scroll position so that this node is visible.
                if (this.Bottom > scrollable.ClientRectangle.Bottom)
                {
                    scrollable.AutoScrollPosition = new Point(0, this.Bottom - scrollable.ClientRectangle.Height);
                }
                else if (this.Top < 0)
                {
                    scrollable.AutoScrollPosition = new Point(0, this.Top);
                }
            }
        }
        public BeepTreeNode FindNode(Func<BeepTreeNode, bool> predicate)
        {
            if (predicate(this)) return this;

            foreach (var child in NodesControls)
            {
                var result = child.FindNode(predicate);
                if (result != null) return result;
            }

            return null;
        }

        public void FilterNodes(Func<BeepTreeNode, bool> predicate)
        {
            Visible = predicate(this);

            foreach (var child in NodesControls)
            {
                child.FilterNodes(predicate);
            }
        }

        #endregion "Filter and Find"
        #region "Menu "
        public void ShowContextMenu(BindingList<SimpleItem> menuList)
        {
            NodeMainMiddlebutton.ListItems.Clear();

            foreach (var item1 in menuList)
            {

                NodeMainMiddlebutton.ListItems.Add(item1);

                //}
            }
            //BeepMouseEventArgs args = new BeepMouseEventArgs("ShowBeepMenu", this);
            //ShowMenu?.Invoke(this, args);
          
            NodeMainMiddlebutton.ShowPopup();
        }

        private void NodeMainMiddlebutton_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            MenuItemSelected?.Invoke(this, e);
        }

        public void ClosePopup()
        {
            NodeMainMiddlebutton.ClosePopup();
        }   
        #endregion "Menu"
        private void RaiseEvent(EventHandler<BeepMouseEventArgs> handler, string eventName)
        {
            handler?.Invoke(this, new BeepMouseEventArgs(eventName, this));
        }
        // Example usage:
       
        private void LogMessage(string message)
        {
            try
            {
               // File.AppendAllText(@"C:\Logs\debug_log.txt", $"{DateTime.Now}: {message}{Environment.NewLine}");
                //Debug.WriteLine(message);
            }
            catch { /* Ignore logging errors */ }
        }
    }
}
