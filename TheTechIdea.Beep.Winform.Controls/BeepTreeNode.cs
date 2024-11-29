using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepTreeNode : BeepControl
    {
        #region "Events"

        public EventHandler<BeepMouseEventArgs> LeftButtonClicked;
        public EventHandler<BeepMouseEventArgs> RighButtonClicked;
        public EventHandler<BeepMouseEventArgs> MiddleButtonClicked;
        public EventHandler<BeepMouseEventArgs> NodeClicked;
        public EventHandler<BeepMouseEventArgs> NodeDoubleClicked;
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
        private SimpleItemCollection items = new SimpleItemCollection();
        private List<BeepTreeNode> _childNodes;
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
        public  string GuidID => _guidid;
        public int NodeHeight { get; set; } = 30;
        public int NodeWidth { get; set; } = 100;
        public int SmallNodeHeight { get; set; } = 15;
        public int MinimumTextWidth { get; set; } = 100;
        public int MaxImageSize { get; set; } = 12;
        private int _minNodeHeight = 20;
        private int _minNodeWidth = 100;
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
        public SimpleItemCollection Nodes
        {
            get => items;
            set
            {
                items = value;
              
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
            get { return _isSelected; }
            set { _isSelected = value; }
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
            _childNodes = new List<BeepTreeNode>();
            UpdateDrawingRect();
            BoundProperty= "Text";
            Height = NodeHeight;
            Width = NodeWidth;
            _nodePanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor =parentbackcolor
            };

            _childrenPanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                BackColor = parentbackcolor
            };
            Controls.Add(_childrenPanel);
            Controls.Add(_nodePanel);
            InitNode();
        }
        protected override Size DefaultSize => new Size(NodeWidth,NodeHeight);
        public BeepTreeNode(string key, string text)
        {
            _key = key;
            _text = text;
            _childNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _childNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            _childNodes = new List<BeepTreeNode>();
            InitNode();
        }
        public BeepTreeNode(string key, string text, string imageKey, string selectedImageKey, bool isExpanded)
        {
            _key = key;
            _text = text;
            _imageKey = imageKey;
            _selectedImageKey = selectedImageKey;
            _isExpanded = isExpanded;
            _childNodes = new List<BeepTreeNode>();
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
            _childNodes = new List<BeepTreeNode>();
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
            _childNodes = new List<BeepTreeNode>();
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
            _childNodes = new List<BeepTreeNode>();
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
            _childNodes = new List<BeepTreeNode>();
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
            _childNodes = new List<BeepTreeNode>();
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
            _childNodes = new List<BeepTreeNode>();
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
            _childNodes = new List<BeepTreeNode>();
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
            _childNodes = new List<BeepTreeNode>();
            InitNode();
        }
        #endregion "Constructors"
        #region "Events Methods"
        private void NodeMainMiddlebutton_Click(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeMainButtonClicked", this);
            NodeClicked?.Invoke(this, args);
        }
        private void Nodeleftbutton_Click(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeleftbuttonClicked", this);
            NodeLeftClicked?.Invoke(this, args);
        }
        private void Noderightbutton_Click(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NoderightbuttonClicked", this);
            NodeRightClicked?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_DoubleClick(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeMainButtonDoubleClicked", this);
            NodeDoubleClicked?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_MouseEnter(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeMainButtonMouseEnter", this);
            NodeMouseEnter?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_MouseLeave(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeMainButtonMouseLeave", this);
            NodeMouseLeave?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_MouseHover(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeMainButtonMouseHover", this);
            NodeMouseHover?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_MouseWheel(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeMainButtonMouseWheel", this);
            NodeMouseWheel?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_MouseUp(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeMainButtonMouseUp", this);
            NodeMouseUp?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_MouseDown(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeMainButtonMouseDown", this);
            NodeMouseDown?.Invoke(this, args);
        }
        private void NodeMainMiddlebutton_MouseMove(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeMainButtonMouseMove", this);
            NodeMouseMove?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseEnter(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeleftbuttonMouseEnter", this);
            NodeMouseEnter?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseLeave(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeleftbuttonMouseLeave", this);
            NodeMouseLeave?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseHover(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeleftbuttonMouseHover", this);
            NodeMouseHover?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseWheel(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeleftbuttonMouseWheel", this);
            NodeMouseWheel?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseUp(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeleftbuttonMouseUp", this);
            NodeMouseUp?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseDown(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeleftbuttonMouseDown", this);
            NodeMouseDown?.Invoke(this, args);
        }
        private void Nodeleftbutton_MouseMove(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NodeleftbuttonMouseMove", this);
            NodeMouseMove?.Invoke(this, args);
        }
        private void Noderightbutton_MouseEnter(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NoderightbuttonMouseEnter", this);
            NodeMouseEnter?.Invoke(this, args);
        }
        private void Noderightbutton_MouseLeave(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NoderightbuttonMouseLeave", this);
            NodeMouseLeave?.Invoke(this, args);
        }
        private void Noderightbutton_MouseHover(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NoderightbuttonMouseHover", this);
            NodeMouseHover?.Invoke(this, args);
        }
        private void Noderightbutton_MouseWheel(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NoderightbuttonMouseWheel", this);
            NodeMouseWheel?.Invoke(this, args);
        }
        private void Noderightbutton_MouseUp(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NoderightbuttonMouseUp", this);
            NodeMouseUp?.Invoke(this, args);
        }
        private void Noderightbutton_MouseDown(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NoderightbuttonMouseDown", this);
            NodeMouseDown?.Invoke(this, args);
        }
        private void Noderightbutton_MouseMove(object sender, EventArgs e)
        {
            BeepMouseEventArgs args = new BeepMouseEventArgs("NoderightbuttonMouseMove", this);
            NodeMouseMove?.Invoke(this, args);
        }

        #endregion "Events Methods"
        #region "Painting Methods"
      
        int startx = 0;
        int starty = 0;
        public void InitNode()
        {
            UpdateDrawingRect();
           
            _nodePanel.Controls.Clear();
           
            IsShadowAffectedByTheme =false;
            IsBorderAffectedByTheme = false;
            IsFramless = true;
            IsChild = true;
            startx = DrawingRect.Left;
            starty = DrawingRect.Top;
            NodeWidth = DrawingRect.Width;
            NodeHeight = DrawingRect.Height;
           
         //   DrawLeftBranch();
            DrawLeftButton();
            DrawMiddleButton();
            DrawRightButton();
            DrawToggleButton();
            starty = NodeHeight;

            foreach (var child in _childNodes)
            {
                if(child.IsInitialized == false) child.InitNode();
                child.Visible = IsExpanded;
                if (IsExpanded)
                {
                    child.Location = new Point(20, starty);
                    child.Width = Width -20; // Indent child nodes
                    starty += child.Height + 5;
                    child.Theme= Theme;
                    child.ApplyTheme();
                    child.RearrangeNode();
                }
            }

            Invalidate();
            _isinitialized= true;
        }
        public void RearrangeNode()
        {
            int padding = 0; // Padding around elements
            int startx = padding; // Horizontal start point
            int centerY = (NodeHeight - SmallNodeHeight) / 2; // Center alignment for small buttons

            // Adjust the size of the main node panel
            _nodePanel.Width = DrawingRect.Width;
            _nodePanel.Height = NodeHeight;

            // Position the toggle button
            if (_toggleButton != null)
            {
                _toggleButton.Location = new Point(startx, centerY); // Center vertically
                _toggleButton.Size = new Size(SmallNodeHeight, SmallNodeHeight);
                startx += _toggleButton.Width + padding;
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
            _nodePanel.Height = NodeHeight + padding * 2;
            padding=5;
            // Adjust the size of `_childrenPanel` based on expansion
            if (_childNodes.Count > 0)
            {
                if (IsExpanded)
                {
                    int childStartY = padding;
                    foreach (var child in _childNodes)
                    {
                        child.Location = new Point(padding, childStartY); // Indent child nodes
                        child.Width = _childrenPanel.Width - 2 * padding;
                        child.Theme = Theme;
                        child.RearrangeNode();
                        childStartY += child.Height + 5;
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

        private void checktoggle()
        {
            if (_childNodes.Count > 0)
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

                Nodeleftbutton.IsChild = true;
                Nodeleftbutton.IsFramless = true;
                Nodeleftbutton.IsShadowAffectedByTheme = true;
                Nodeleftbutton.IsBorderAffectedByTheme = true;
                Nodeleftbutton.MaxImageSize = new System.Drawing.Size(MaxImageSize - 2, MaxImageSize - 2);
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
                Noderightbutton.IsFramless = true;
                Noderightbutton.IsShadowAffectedByTheme = true;
                Noderightbutton.IsBorderAffectedByTheme = true;
                Noderightbutton.MaxImageSize = new System.Drawing.Size(MaxImageSize -2, MaxImageSize-2);
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
                NodeMainMiddlebutton.IsShadowAffectedByTheme = false;
                NodeMainMiddlebutton.IsBorderAffectedByTheme = false;
                NodeMainMiddlebutton.MaxImageSize = new System.Drawing.Size(MaxImageSize-2, MaxImageSize-2);
                NodeMainMiddlebutton.Font=BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                
                
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
            _nodePanel.Height = NodeHeight;

            // Adjust visibility of child nodes based on expansion state
            
          //  _childrenPanel.BackColor = _currentTheme.PanelBackColor;
            // Ensure the total height reflects the node and its children
            Height = _nodePanel.Height + (IsExpanded ? _childrenPanel.Height : 0);
            
            RearrangeNode();
            Tree.RearrangeTree();
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
                foreach (var item in _childNodes)
                {
                    item.Theme = Theme;
                   // item.ApplyTheme();
                }

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
        private void ChangeNodeImageSettings()
        {
            if (NodeMainMiddlebutton != null)
            {
                
                NodeMainMiddlebutton.TextImageRelation = ShowNodeImage ? System.Windows.Forms.TextImageRelation.ImageBeforeText : System.Windows.Forms.TextImageRelation.TextBeforeImage;
                NodeMainMiddlebutton.ImageAlign = ShowNodeImage ? System.Drawing.ContentAlignment.MiddleLeft : System.Drawing.ContentAlignment.MiddleCenter;
                NodeMainMiddlebutton.TextAlign = ShowNodeImage ? System.Drawing.ContentAlignment.MiddleCenter : System.Drawing.ContentAlignment.MiddleLeft;
             
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
            foreach (var item in _childNodes)
            {
                item.ShowNodeImage = _shownodeimage;
                item.ChangeNodeImageSettings();
            }
            
            Invalidate();

        }
        #region "Node Methods"
        public void AddNode(BeepTreeNode node)
        {
            _childNodes.Add(node);
        }
        public void RemoveNode(BeepTreeNode node)
        {
            _childNodes.Remove(node);
        }
        public void RemoveNode(string NodeName)
        {
            BeepTreeNode node = _childNodes.Where(c => c.Name == NodeName).FirstOrDefault();
            if (node != null)
            {
                _childNodes.Remove(node);
            }
        }
        public void RemoveNode(int NodeIndex)
        {
            if (NodeIndex >= 0 && NodeIndex < _childNodes.Count)
            {
                _childNodes.RemoveAt(NodeIndex);
            }
        }
        public void ClearNodes()
        {
            _childNodes.Clear();
        }
        public BeepTreeNode GetNode(string NodeName)
        {
            return _childNodes.Where(c => c.Name == NodeName).FirstOrDefault();
        }
        public BeepTreeNode GetNode(int NodeIndex)
        {
            if (NodeIndex >= 0 && NodeIndex < _childNodes.Count)
            {
                return _childNodes[NodeIndex];
            }
            else
            {
                return null;
            }
        }
        public List<BeepTreeNode> GetNodes()
        {
            return _childNodes;
        }
       
        public void ExpandAll()
        {
            IsExpanded = true;
            foreach (var child in _childNodes)
            {
                child.ExpandAll();

            }
            _toggleButton.Text = "-";
            RearrangeNode();
        }
        public void MiniAll()
        {
            IsExpanded = true;
            foreach (var child in _childNodes)
            {
                child.MiniAll();
            }
            _toggleButton.Text = "-";
            RearrangeNode();
        }


        #endregion "Node Methods"
        #region Child Node Management

        public void AddChild(BeepTreeNode child)
        {
            _childNodes.Add(child);
            _childrenPanel.Controls.Add(child);
            UpdatePanelSize();
        }

        public void RemoveChild(BeepTreeNode child)
        {
            _childNodes.Remove(child);
            _childrenPanel.Controls.Remove(child);
            UpdatePanelSize();
        }

        public void ClearChildren()
        {
            _childNodes.Clear();
            _childrenPanel.Controls.Clear();
            UpdatePanelSize();
        }

        #endregion
    }
}
