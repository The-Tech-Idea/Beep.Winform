using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Tree")]
    [Category("Beep Controls")]
    [Description("An owner-drawn tree control using BeepButton, BeepImage, and BeepCheckBox without child controls.")]
    public class BeepTree : BeepControl
    {
        #region "Events"
        public event EventHandler<BeepMouseEventArgs> LeftButtonClicked;
        public event EventHandler<BeepMouseEventArgs> RightButtonClicked;
        public event EventHandler<BeepMouseEventArgs> MiddleButtonClicked;
        public event EventHandler<BeepMouseEventArgs> NodeDoubleClicked;
        public event EventHandler<BeepMouseEventArgs> NodeSelected;
        public event EventHandler<BeepMouseEventArgs> NodeDeselected;
        public event EventHandler<BeepMouseEventArgs> NodeExpanded;
        public event EventHandler<BeepMouseEventArgs> NodeCollapsed;
        public event EventHandler<BeepMouseEventArgs> NodeChecked;
        public event EventHandler<BeepMouseEventArgs> NodeUnchecked;
        public event EventHandler<BeepMouseEventArgs> NodeVisible;
        public event EventHandler<BeepMouseEventArgs> NodeInvisible;
        public event EventHandler<BeepMouseEventArgs> NodeReadOnly;
        public event EventHandler<BeepMouseEventArgs> NodeEditable;
        public event EventHandler<BeepMouseEventArgs> NodeDeletable;
        public event EventHandler<BeepMouseEventArgs> NodeDirty;
        public event EventHandler<BeepMouseEventArgs> NodeModified;
        public event EventHandler<BeepMouseEventArgs> NodeDeleted;
        public event EventHandler<BeepMouseEventArgs> NodeAdded;
        public event EventHandler<BeepMouseEventArgs> NodeExpandedAll;
        public event EventHandler<BeepMouseEventArgs> NodeCollapsedAll;
        public event EventHandler<BeepMouseEventArgs> NodeCheckedAll;
        public event EventHandler<BeepMouseEventArgs> NodeUncheckedAll;
        public event EventHandler<BeepMouseEventArgs> NodeVisibleAll;
        public event EventHandler<BeepMouseEventArgs> NodeInvisibleAll;
        public event EventHandler<BeepMouseEventArgs> NodeRightClicked;
        public event EventHandler<BeepMouseEventArgs> NodeLeftClicked;
        public event EventHandler<BeepMouseEventArgs> NodeMiddleClicked;
        public event EventHandler<BeepMouseEventArgs> NodeMouseEnter;
        public event EventHandler<BeepMouseEventArgs> NodeMouseLeave;
        public event EventHandler<BeepMouseEventArgs> NodeMouseHover;
        public event EventHandler<BeepMouseEventArgs> NodeMouseWheel;
        public event EventHandler<BeepMouseEventArgs> NodeMouseUp;
        public event EventHandler<BeepMouseEventArgs> NodeMouseDown;
        public event EventHandler<BeepMouseEventArgs> NodeMouseMove;
        public event EventHandler<SelectedItemChangedEventArgs> MenuItemSelected;
        public event EventHandler<BeepMouseEventArgs> ShowMenu;
        #endregion
        #region "Properties"
         int boxsize = 14;
        int imagesize = 20;
        private SimpleItem _lastHoveredItem = null;
        private Rectangle _lastHoveredRect = Rectangle.Empty;

        private SimpleItem _lastclicked = null;


        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));

        }
        #region "Scrollbars Properties"
     
        // Add these fields to your BeepTree class
        private BeepScrollBar _verticalScrollBar;
        private BeepScrollBar _horizontalScrollBar;
        private bool _showVerticalScrollBar = true;
        private bool _showHorizontalScrollBar = true;
        private int _yOffset = 0;
        private int _xOffset = 0;

        [Browsable(true)]
        [Category("Layout")]
        public bool ShowVerticalScrollBar
        {
            get => _showVerticalScrollBar;
            set
            {
                _showVerticalScrollBar = value;
                UpdateScrollBars();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        public bool ShowHorizontalScrollBar
        {
            get => _showHorizontalScrollBar;
            set
            {
                _showHorizontalScrollBar = value;
                UpdateScrollBars();
                Invalidate();
            }
        }
       
        #endregion "Scrollbars Properties"
        private BindingList<SimpleItem> _currentMenuItems = new BindingList<SimpleItem>();
        public BindingList<SimpleItem> CurrentMenutems
        {
            get => _currentMenuItems;
            set
            {
                _currentMenuItems = value;
                Invalidate();
            }
        }

        private bool _allowMultiSelect = false;
        public bool AllowMultiSelect
        {
            get => _allowMultiSelect;
            set
            {
                _allowMultiSelect = value;
                if (!_allowMultiSelect)
                {
                    if (SelectedNodes == null) return;
                    // Deselect all but the first node
                    for (int i = 1; i < SelectedNodes.Count; i++)
                    {
                        SelectedNodes[i].IsSelected = false;
                    }
                    SelectedNodes = SelectedNodes.Take(1).ToList();
                }
                Invalidate();
            }
        }

        private SimpleItem _lastSelectedNode;
        public List<SimpleItem> SelectedNodes { get; private set; } = new List<SimpleItem>();
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleItem SelectedNode
        {
            get
            {
                return _lastSelectedNode;
            }
            set
            {
                if (_lastSelectedNode != value)
                {
                    if (_lastSelectedNode != null)
                        _lastSelectedNode.IsSelected = false;  // Deselect previous node

                    _lastSelectedNode = value;

                    if (_lastSelectedNode != null)
                    {
                        _lastSelectedNode.IsSelected = true;
                        
                    }
                }
                else
                {
                    _lastSelectedNode = value;
                    _lastSelectedNode.IsSelected = true;
                }
                  
            }
        }

        public SimpleItem ClickedNode { get; private set; }

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
           
                Invalidate();
            }
        }
        private bool _useThemeFont = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("If true, the label's font is always set to the theme font.")]
        public new bool UseThemeFont
        {
            get => _useThemeFont;
            set
            {
                _useThemeFont = value;
                ApplyTheme();
                Invalidate();
            }
        }
        private TextAlignment textAlign = TextAlignment.Left;

        [Browsable(true)]
        [Category("Appearance")]
        public TextAlignment TextAlignment
        {
            get => textAlign;
            set
            {
                textAlign = value;
              
                Invalidate();
            }
        }
        private bool _showCheckBox = true;
        public bool ShowCheckBox
        {
            get => _showCheckBox;
            set
            {
                _showCheckBox = value;
                Invalidate();
            }
        }

        #endregion "Properties"
        // Data and layout
        // Add these fields to track content size
        private Size _virtualSize = Size.Empty;
        private int _totalContentHeight = 0;
        private List<SimpleItem> _nodes = new List<SimpleItem>();
        private List<NodeInfo> _visibleNodes = new List<NodeInfo>();
        private int _minRowHeight = 24;
        private int _indentWidth = 16;
        private int _verticalPadding = 4;

        // Renderers
        private BeepButton _toggleRenderer =new BeepButton();
        private BeepButton _button = new BeepButton();
        private BeepCheckBoxBool _checkRenderer=new BeepCheckBoxBool();
        private BeepImage _iconRenderer=new BeepImage();
        private SimpleItem lastmenuitem;
        private bool _isPopupOpen;
        private const string PlusIcon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.plus.svg";
        private const string MinusIcon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.minus.svg";

        // Node internal
        private class NodeInfo
        {
            public SimpleItem Item;
            public int Level;
            public Rectangle Bounds;
            public Rectangle ToggleRect;
            public Rectangle CheckRect;
            public Rectangle IconRect;
            public Rectangle TextRect;
            public int RowHeight;
        }

        [Browsable(false)] public IList<SimpleItem> Nodes { get => _nodes; set { _nodes = new List<SimpleItem>(value); RebuildVisible(); Invalidate(); } }

        public BeepTree() : base()
        { // but the SetStyle gives you full control:
            //this.DoubleBuffered = true;

            //// these styles ensure all drawing happens offscreen
            //this.SetStyle(
            //    ControlStyles.UserPaint |
            //    ControlStyles.AllPaintingInWmPaint |
            //    ControlStyles.OptimizedDoubleBuffer |
            //    ControlStyles.ResizeRedraw,
            //    true);
            //this.UpdateStyles();
            BeepButton _toggleRenderer = new BeepButton { IsChild=true, MaxImageSize=new Size(boxsize-2,boxsize-2), Size = new Size(boxsize, boxsize) ,ImageAlign= ContentAlignment.MiddleCenter,HideText=true};
            BeepCheckBoxBool _checkRenderer = new BeepCheckBoxBool { IsChild = true, CheckBoxSize = boxsize };
            BeepImage _iconRenderer = new BeepImage { IsChild = true, ScaleMode = ImageScaleMode.KeepAspectRatio };
            BeepButton _button = new BeepButton {IsSelectedOptionOn=true, IsChild = true, MaxImageSize = new Size(boxsize, boxsize) };
           MouseDown += OnMouseDownHandler;
            MouseUp += OnMouseUpHandler;
            MouseMove += OnMouseMoveHandler;
            MouseDoubleClick += OnMouseDoubleClickHandler;
            MouseHover += OnMouseHoverHandler;
            //this.AutoScroll = true;
            //this.VerticalScroll.Visible = true;
            //this.HorizontalScroll.Visible = true;
            //_currentTheme = BeepThemesManager.AutumnTheme;
            MouseEnter += (s, e) => NodeMouseEnter?.Invoke(this, new BeepMouseEventArgs("MouseEnter", null));
            MouseLeave += (s, e) => NodeMouseLeave?.Invoke(this, new BeepMouseEventArgs("MouseLeave", null));
            MouseWheel += (s, e) => NodeMouseWheel?.Invoke(this, new BeepMouseEventArgs("MouseWheel", null));
            _button.SelectedItemChanged += button_SelectedItemChanged;
            InitializeScrollbars();
        }

     

        protected override void InitLayout()
        {
            base.InitLayout();
        

        }
        private void RebuildVisible()
        {
            _visibleNodes.Clear();
            void Recurse(SimpleItem item, int level)
            {
                _visibleNodes.Add(new NodeInfo { Item = item, Level = level });
                if (item.IsExpanded && item.Children?.Count > 0)
                    foreach (var c in item.Children) Recurse(c, level + 1);

            }
            foreach (var root in _nodes) Recurse(root, 0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
         
           
        }
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            HitList.Clear();
            RebuildVisible();

            // Update scrollbars based on current content
            UpdateScrollBars();

            int y = 0;
            foreach (var root in _nodes)
            {
                DrawNodeRecursive(g, root, 0, ref y);
            }

            // Ensure scrollbars are drawn on top
            if (_verticalScrollBar.Visible)
                _verticalScrollBar.Invalidate();
            if (_horizontalScrollBar.Visible)
                _horizontalScrollBar.Invalidate();
        }

        private void DrawNodeRecursive(Graphics g, SimpleItem item, int level, ref int y)
        {
            // Adjust for scrolling
            int adjustedY = y - _yOffset;
            int adjustedX = level * _indentWidth - _xOffset;

            // Skip drawing if the node is offscreen
            int rowHeight = Math.Max(_minRowHeight, _button.GetPreferredSize(Size.Empty).Height + 2 * _verticalPadding);
            if (adjustedY + rowHeight < 0 || adjustedY > ClientRectangle.Height)
            {
                // Still increment y for future nodes
                y += rowHeight;

                // Process children even if parent is not visible
                if (item.IsExpanded && item.Children?.Count > 0)
                {
                    foreach (var child in item.Children)
                    {
                        DrawNodeRecursive(g, child, level + 1, ref y);
                    }
                }
                return;
            }

            // Measure text
            Font drawFont = UseThemeFont ? BeepThemesManager.ToFont(_currentTheme.LabelSmall) : (_useScaledfont ? Font : _textFont);
            string text = item.Text;
            _button.Text = text;
            _button.Size = _button.GetPreferredSize(Size.Empty);
            Size textSize = _button.Size;
            rowHeight = Math.Max(_minRowHeight, Math.Max(textSize.Height, Math.Max(boxsize, imagesize)) + 2 * _verticalPadding);

            // Use adjusted coordinates for drawing
            Rectangle rowRect = new Rectangle(0, DrawingRect.Top + adjustedY, DrawingRect.Width, rowHeight);
            int checkboxWidth = 0;

            // Toggle
            if (item.Children?.Count > 0)
            {
                _toggleRenderer.ImagePath = item.IsExpanded ? MinusIcon : PlusIcon;
            }
            else
            {
                _toggleRenderer.ImagePath = MinusIcon;
            }

            Rectangle toggleRect = new Rectangle(adjustedX, adjustedY + (rowHeight - boxsize) / 2, boxsize, boxsize);
            _toggleRenderer.Size = new Size(imagesize, imagesize);
            _toggleRenderer.MaxImageSize = new Size(imagesize - 2, imagesize - 2);
            _toggleRenderer.Draw(g, toggleRect);
            AddHitArea($"toggle_{item.GuidId}", toggleRect);

            // Checkbox
            if (_showCheckBox)
            {
                Rectangle checkRect = new Rectangle(adjustedX + boxsize + 4, adjustedY + (rowHeight - boxsize) / 2, boxsize, boxsize);
                _checkRenderer.CurrentValue = item.IsChecked;
                _checkRenderer.Draw(g, checkRect);
                AddHitArea($"check_{item.GuidId}", checkRect);
                checkboxWidth = boxsize + 4; // Width plus spacing
            }

            // Icon
            int iconX = adjustedX + boxsize + checkboxWidth + 4;
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                BeepImage iconRenderer = new BeepImage
                {
                    ImagePath = item.ImagePath,
                    Size = new Size(imagesize, imagesize),
                    ScaleMode = ImageScaleMode.KeepAspectRatio
                };

                int iconY = adjustedY + (rowHeight - imagesize) / 2;
                Rectangle iconRect = new Rectangle(iconX, iconY, imagesize, imagesize);

                GraphicsState state = g.Save();
                try
                {
                    iconRenderer.DrawImage(g, iconRect);
                    AddHitArea($"icon_{item.GuidId}", iconRect);
                }
                finally
                {
                    g.Restore(state);
                }
            }

            // Text
            int textX = iconX + imagesize + 8;
            Rectangle textRect = new Rectangle(textX, adjustedY + _verticalPadding, ClientSize.Width - textX, textSize.Height);
            _button.Text = text;
            _button.Size = textRect.Size;
            _button.Location = textRect.Location;
            _button.TextAlign = ContentAlignment.MiddleLeft;
            _button.Size = _button.GetPreferredSize(Size.Empty);
            textRect = new Rectangle(textX, adjustedY + _verticalPadding, _button.Size.Width, _button.Size.Height);

            if (item == _lastHoveredItem && !item.IsSelected)
            {
                _button.IsHovered = true;
            }
            else
            {
                _button.IsHovered = false;
            }

            if (item.IsSelected)
            {
                _button.IsSelected = true;
            }
            else
            {
                _button.IsSelected = false;
            }

            _button.Draw(g, textRect);
            AddHitArea($"row_{item.GuidId}", rowRect);

            // Store actual Y position (with scrolling adjustment)
            item.X = adjustedX;
            item.Y = adjustedY;


            // Advance y position for next node
            y += rowHeight;

            // Draw children
            if (item.IsExpanded && item.Children?.Count > 0)
            {
                foreach (var child in item.Children)
                {
                    DrawNodeRecursive(g, child, level + 1, ref y);
                }
            }
        }


        private void OnMouseDownHandler(object sender, MouseEventArgs e)
        {
            NodeMouseDown?.Invoke(this, new BeepMouseEventArgs("MouseDown", null));
            var point = e.Location;

            // Right-click context menu
            if (e.Button == MouseButtons.Right && HitTest(point, out var ht) && ht.Name.StartsWith("row_"))
            {
                var guid = ht.Name.Substring(4); // everything after "row_"
                var item = FindItemByGuid(guid);
                if (item != null)
                {
                    ClickedNode = item;
                    var args = new BeepMouseEventArgs("RightClick", null);
                 //   RightButtonClicked?.Invoke(this, args);
                    NodeRightClicked?.Invoke(this, args);
                  //  ShowMenu?.Invoke(this, args);
                   // SelectedNode = item;
           
                    var a = HandlersFactory.GlobalMenuItemsProvider(item);
                    if (a == null) return;
                    CurrentMenutems = new BindingList<SimpleItem>(a);
                    if (CurrentMenutems!=null && CurrentMenutems.Count > 0)
                    {
                        TogglePopup();
                    }
                   
                }
                return;
            }

            // Left or middle click on toggle/check/row
            if (HitTest(point, out var hit))
            {
                var parts = hit.Name.Split('_');
                if (parts.Length == 2)
                {
                    string type = parts[0], guid = parts[1];
                    var item = FindItemByGuid(guid);
                    if (item == null) return;

                    var args = new BeepMouseEventArgs(type, null);
                    switch (type)
                    {
                        case "toggle":
                            item.IsExpanded = !item.IsExpanded;
                            (item.IsExpanded ? NodeExpanded : NodeCollapsed)?.Invoke(this, args);
                            // Update scrollbars based on current content
                            UpdateScrollBars();
                            break;
                        case "check":
                            item.IsChecked = !item.IsChecked;
                          
                           
                            (item.IsChecked ? NodeChecked : NodeUnchecked)?.Invoke(this, args);
                            break;
                        case "row":
                            if (e.Button == MouseButtons.Left)
                            {
                                ClickedNode = item;
                                _lastclicked = item;
                                SelectedNode = item;
                                if (AllowMultiSelect)
                                {
                                    if (SelectedNodes.Contains(item))
                                    {
                                        SelectedNodes.Remove(item);
                                        item.IsSelected = false;
                                        NodeDeselected?.Invoke(this, args);
                                    }
                                    else
                                    {
                                        SelectedNodes.Add(item);
                                        item.IsSelected = true;
                                        NodeSelected?.Invoke(this, args);
                                    }
                                }
                                else
                                {
                                    item.IsSelected = true;
                                    SelectedNode = item;
                                }
                                LeftButtonClicked?.Invoke(this, args);
                              
                                NodeSelected?.Invoke(this, args);
                                OnSelectedItemChanged(item);
                            }
                            else if (e.Button == MouseButtons.Middle)
                            {
                               
                                NodeMiddleClicked?.Invoke(this, args);
                            }
                            break;
                    }

                    Invalidate();
                }
            }
        }
        private SimpleItem FindItemByGuid(string guid)
        {
            return TraverseAll(_nodes).FirstOrDefault(i => i.GuidId == guid);
        }
        private IEnumerable<SimpleItem> TraverseAll(IEnumerable<SimpleItem> list)
        {
            foreach (var i in list)
            {
                yield return i;
                if (i.Children != null)
                {
                    foreach (var c in TraverseAll(i.Children))
                        yield return c;
                }
            }
        }
        #region "Mouse Handling"
        private void OnMouseHoverHandler(object? sender, EventArgs e)
        {
            GetHover();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            // Clear any hover state
            if (_lastHoveredItem != null)
            {
                if (!_lastHoveredRect.IsEmpty)
                    Invalidate(_lastHoveredRect);

                _lastHoveredItem = null;
                _lastHoveredRect = Rectangle.Empty;
            }

            // Raise the standard mouse leave event
            NodeMouseLeave?.Invoke(this, new BeepMouseEventArgs("MouseLeave", null));
        }
        private void GetHover()
        {
            Point mousePosition = PointToClient(MousePosition);

            // Find the node at the current mouse position
            if (HitTest(mousePosition, out var hitTest))
            {
                string[] parts = hitTest.Name.Split('_');
                if (parts.Length == 2)
                {
                    string type = parts[0], guid = parts[1];
                    SimpleItem hoveredItem = FindItemByGuid(guid);

                    if (hoveredItem != null)
                    {
                        // If we're hovering over a different item than before
                        if (_lastHoveredItem != hoveredItem)
                        {
                            // Reset last hovered item
                            if (_lastHoveredItem != null)
                            {
                                // Clear previous hover state
                                _lastHoveredItem = null;

                                // Force redraw of previous area
                                if (!_lastHoveredRect.IsEmpty)
                                    Invalidate(_lastHoveredRect);
                            }

                            // Set hover state for current item
                            _lastHoveredItem = hoveredItem;
                            _lastHoveredRect = hitTest.TargetRect;

                            // Raise event with the hovered item
                            BeepMouseEventArgs args = new BeepMouseEventArgs("MouseHover", hoveredItem);
                            NodeMouseHover?.Invoke(this, args);

                            // Redraw only the area of the hovered item
                            Invalidate(_lastHoveredRect);
                        }

                        // Show tooltip if item has tooltip text
                        //if (!string.IsNullOrEmpty(hoveredItem.ToolTipText))
                        //{
                        //    ShowToolTip(hoveredItem.ToolTipText);
                        //}
                    }
                }
            }
            else
            {
                // Mouse isn't over any node
                if (_lastHoveredItem != null)
                {
                    // Clear hover state
                    _lastHoveredItem = null;

                    // Force redraw of previous hover area
                    if (!_lastHoveredRect.IsEmpty)
                        Invalidate(_lastHoveredRect);

                    _lastHoveredRect = Rectangle.Empty;
                    _lastclicked = null;
                    // Hide tooltip
                    HideToolTip();
                }
            }
        }
        private void OnMouseUpHandler(object s, MouseEventArgs e)
        {
            NodeMouseUp?.Invoke(this, new BeepMouseEventArgs("MouseUp", null));
        }
        private void OnMouseMoveHandler(object s, MouseEventArgs e)
        {
            GetHover();
            NodeMouseMove?.Invoke(this, new BeepMouseEventArgs("MouseMove", null));
        }
        private void OnMouseDoubleClickHandler(object s, MouseEventArgs e)
        {
            NodeDoubleClicked?.Invoke(this, new BeepMouseEventArgs("NodeDoubleClick", null));
        }
        #endregion "Mouse Handling"

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textFont = Font;
            // // Console.WriteLine("Font Changed");
            if (AutoSize)
            {
                Size textSize = TextRenderer.MeasureText(Text, _textFont);
                this.Size = new Size(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
            }
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (IsChild)
            {
                ParentBackColor = Parent.BackColor;
                BackColor = ParentBackColor;

            }
            else
            {
                BackColor = _currentTheme.TreeBackColor;
            }
          
           
            if (UseThemeFont)
            {
                Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                _textFont = Font;
                _button.Font = Font;
                _button.UseThemeFont = UseThemeFont;
            }
            else
            {
                Font = _textFont;
                _button.TextFont = Font;
            }

            if (UseScaledFont)
            {
                _button.UseScaledFont = UseScaledFont;
            }
            _toggleRenderer.Theme = Theme;
            _toggleRenderer.BackColor= BackColor;
            _checkRenderer.Theme = Theme;
            _checkRenderer.BackColor = BackColor;
            _iconRenderer.Theme = Theme;
            _iconRenderer.BackColor = BackColor;
             ForeColor = _currentTheme.TreeForeColor;
            _button.Theme = Theme;
            _button.IsColorFromTheme=false;
            _button.BackColor = BackColor;
            _button.ForeColor = _currentTheme.TreeForeColor;
            _button.SelectedForeColor = _currentTheme.TreeNodeSelectedForeColor;
            _button.SelectedBackColor = _currentTheme.TreeNodeSelectedBackColor;
            _button.BorderColor = _currentTheme.TreeNodeSelectedBackColor;
            _button.HoverBackColor = _currentTheme.TreeNodeHoverBackColor;
            _button.HoverForeColor = _currentTheme.TreeNodeHoverForeColor;
            _verticalScrollBar.Theme = Theme;
            _horizontalScrollBar.Theme = Theme;
            Invalidate();

        }
        #region "Find and Filter"
        #region "Find Tree Node"

        // Finds a node by its text (case-sensitive) in the hierarchy
        public SimpleItem FindNode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            return FindNode(n => string.Equals(n.Text, text, StringComparison.Ordinal));
        }

        // Finds a node by its GUID in the hierarchy
        public SimpleItem GetNodeByGuid(string guidid)
        {
            if (string.IsNullOrWhiteSpace(guidid))
                return null;

            return FindNode(n => string.Equals(n.GuidId, guidid, StringComparison.Ordinal));
        }

        // Finds a node by its name in the hierarchy
        public SimpleItem GetNode(string nodeName)
        {
            if (string.IsNullOrWhiteSpace(nodeName))
                return null;

            return FindNode(n => string.Equals(n.Text, nodeName, StringComparison.OrdinalIgnoreCase));
        }

        // Finds a node by a predicate and highlights it
        public SimpleItem FindNode(Func<SimpleItem, bool> predicate)
        {
            foreach (var item in TraverseAll(_nodes))
            {
                if (predicate(item))
                {
                    HighlightNode(item);
                    return item;
                }
            }

            return null;
        }

        // Core recursive method to traverse the hierarchy and find a node
        private SimpleItem FindNodeInHierarchy(SimpleItem currentNode, Func<SimpleItem, bool> predicate)
        {
            if (predicate(currentNode))
                return currentNode;

            // Traverse child nodes
            if (currentNode.Children != null)
            {
                foreach (var childNode in currentNode.Children)
                {
                    var foundNode = FindNodeInHierarchy(childNode, predicate);
                    if (foundNode != null)
                        return foundNode;
                }
            }

            return null;
        }

        // Traverse all SimpleItems recursively and find a node by GUID
        public SimpleItem GetNodeByGuidID(string guidID)
        {
            if (string.IsNullOrWhiteSpace(guidID))
                return null;

            return TraverseAllItems(_nodes).FirstOrDefault(n => n.GuidId == guidID);
        }

        // Traverse all SimpleItems recursively and find a node by index
        public SimpleItem GetNode(int nodeIndex)
        {
            if (nodeIndex < 0 || _nodes == null)
                return null;

            int currentIndex = 0;
            foreach (var item in TraverseAllItems(_nodes))
            {
                if (currentIndex == nodeIndex)
                    return item;
                currentIndex++;
            }

            return null;
        }
        #endregion

        #region "Filtering"
        /// <summary>
        /// Filters nodes based on predicate. 
        /// A node is visible if it or any descendant matches the predicate.
        /// </summary>
        /// <param name="predicate">Condition to determine if a node should remain visible.</param>
        public void FilterNodes(Func<SimpleItem, bool> predicate)
        {
            // Mark each node based on the predicate
            foreach (var node in TraverseAll(_nodes))
            {
                bool isMatch = predicate(node);
                node.IsVisible = isMatch;
            }

            // Keep ancestors visible if a child is visible
            foreach (var node in TraverseAll(_nodes))
            {
                if (node.IsVisible && node.ParentItem != null)
                {
                    var parent = node.ParentItem;
                    while (parent != null)
                    {
                        parent.IsVisible = true;
                        parent = parent.ParentItem;
                    }
                }
            }

            // Rebuild visible nodes list and redraw
            RebuildVisible();
            Invalidate();
        }

        /// <summary>
        /// Clears the current filter and resets all nodes to be visible.
        /// </summary>
        public void ClearFilter()
        {
            foreach (var node in TraverseAll(_nodes))
            {
                node.IsVisible = true;
            }

            // Rebuild visible nodes list and redraw
            RebuildVisible();
            Invalidate();
        }
        #endregion

        #region "Find SimpleItem"
        /// <summary>
        /// Recursively traverses the entire hierarchy of SimpleItems.
        /// </summary>
        /// <param name="items">A collection of SimpleItem objects (e.g., your root-level Nodes).</param>
        /// <returns>An enumerable containing all nodes, including nested children.</returns>
        public IEnumerable<SimpleItem> TraverseAllItems(IEnumerable<SimpleItem> items)
        {
            if (items == null)
                yield break;

            foreach (var item in items)
            {
                // Return the current item
                yield return item;

                // Recursively return all of its children
                if (item.Children != null && item.Children.Count > 0)
                {
                    foreach (var child in TraverseAllItems(item.Children))
                    {
                        yield return child;
                    }
                }
            }
        }
        #endregion

        #region "Highlight and Navigation"
        /// <summary>
        /// Highlights the given node, expanding ancestors, and scrolling it into view.
        /// </summary>
        /// <param name="node">The node to highlight.</param>
        private void HighlightNode(SimpleItem node)
        {
            if (node == null) return;

            // Expand all parents
            var current = node.ParentItem;
            while (current != null)
            {
                current.IsExpanded = true;
                current = current.ParentItem;
            }

            // Set as selected node
            SelectedNode = node;

            // Rebuild visible nodes with expanded ancestors
            RebuildVisible();

            // Find the node's position to scroll to it
            int yPos = 0;
            bool found = false;

            // Calculate node position
            foreach (var vNode in _visibleNodes)
            {
                if (vNode.Item == node)
                {
                    found = true;
                    break;
                }
                yPos += vNode.RowHeight > 0 ? vNode.RowHeight : _minRowHeight;
            }

            if (found)
            {
                // Scroll to the node
                AutoScrollPosition = new Point(AutoScrollPosition.X, yPos);
            }

            // Redraw to reflect changes
            Invalidate();
        }

        /// <summary>
        /// Selects the previous visible node (if any) in the tree.
        /// </summary>
        public void SelectPreviousNode()
        {
            if (SelectedNode == null || _visibleNodes.Count == 0)
                return;

            int currentIndex = -1;
            for (int i = 0; i < _visibleNodes.Count; i++)
            {
                if (_visibleNodes[i].Item == SelectedNode)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex > 0)
            {
                SelectedNode = _visibleNodes[currentIndex - 1].Item;
                HighlightNode(SelectedNode);
            }
        }

        /// <summary>
        /// Selects the next visible node (if any) in the tree.
        /// </summary>
        public void SelectNextNode()
        {
            if (SelectedNode == null || _visibleNodes.Count == 0)
                return;

            int currentIndex = -1;
            for (int i = 0; i < _visibleNodes.Count; i++)
            {
                if (_visibleNodes[i].Item == SelectedNode)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex >= 0 && currentIndex < _visibleNodes.Count - 1)
            {
                SelectedNode = _visibleNodes[currentIndex + 1].Item;
                HighlightNode(SelectedNode);
            }
        }

        /// <summary>
        /// Selects all nodes in the tree.
        /// </summary>
        public void SelectAllNodes()
        {
            if (!AllowMultiSelect)
                return;

            SelectedNodes.Clear();

            foreach (var node in TraverseAll(_nodes))
            {
                node.IsSelected = true;
                SelectedNodes.Add(node);
            }

            Invalidate();
        }

        /// <summary>
        /// Deselects all nodes in the tree.
        /// </summary>
        public void DeselectAllNodes()
        {
            foreach (var node in TraverseAll(_nodes))
            {
                node.IsSelected = false;
            }

            SelectedNodes.Clear();
            _lastSelectedNode = null;

            Invalidate();
        }
        #endregion

        #region "Expansion"
        /// <summary>
        /// Expands all nodes in the tree.
        /// </summary>
        public void ExpandAll()
        {
            foreach (var item in TraverseAll(_nodes))
            {
                if (item.Children != null && item.Children.Count > 0)
                {
                    item.IsExpanded = true;
                }
            }

            RebuildVisible();
            Invalidate();

            NodeExpandedAll?.Invoke(this, new BeepMouseEventArgs("ExpandAll", null));
        }

        /// <summary>
        /// Collapses all nodes in the tree.
        /// </summary>
        public void CollapseAll()
        {
            foreach (var item in TraverseAll(_nodes))
            {
                item.IsExpanded = false;
            }

            RebuildVisible();
            Invalidate();

            NodeCollapsedAll?.Invoke(this, new BeepMouseEventArgs("CollapseAll", null));
        }

        /// <summary>
        /// Ensures all ancestors of the specified node are expanded.
        /// </summary>
        /// <param name="item">The item whose ancestors should be expanded.</param>
        public void EnsureVisible(SimpleItem item)
        {
            if (item == null)
                return;

            // Expand all parent items
            var parent = item.ParentItem;
            while (parent != null)
            {
                parent.IsExpanded = true;
                parent = parent.ParentItem;
            }

            RebuildVisible();

            // Find and scroll to the node's position
            int y = 0;
            foreach (var node in _visibleNodes)
            {
                if (node.Item == item)
                {
                    AutoScrollPosition = new Point(0, y);
                    break;
                }
                y += node.RowHeight > 0 ? node.RowHeight : _minRowHeight;
            }

            Invalidate();
        }
        #endregion

        /// <summary>
        /// Helper method to hide tooltips if they are currently visible.
        /// </summary>
        private void HideToolTip()
        {
            // Call the base class implementation from BeepControl
            base.HideToolTip();

            // Also reset any tooltip-related state specific to BeepTree
            if (_lastHoveredItem != null && !string.IsNullOrEmpty(_lastHoveredItem.Text))
            {
                // Reset the tooltip flag if we were tracking it per-item
                tooltipShown = false;
            }
        }

        #endregion "Find and Filter"
        #region "Popup List Methods"
        BeepPopupListForm menuDialog;
        // popup list items form
        [Browsable(false)]
        public BeepPopupListForm PopupListForm
        {
            get => menuDialog;
            set => menuDialog = value;
        }
        private void TogglePopup()
        {
            if (_isPopupOpen)
                ClosePopup();
            else
                ShowPopup();
        }
        public void ShowPopup()
        {
            if (_isPopupOpen) return;
            if (CurrentMenutems.Count == 0)
            {
                return;
            }

            // Close any existing popup before showing a new one
            ClosePopup();

            menuDialog = new BeepPopupListForm(CurrentMenutems.ToList());

            menuDialog.Theme = Theme;
            menuDialog.SelectedItemChanged += button_SelectedItemChanged;

            // Use the synchronous ShowPopup method
            SimpleItem x = menuDialog.ShowPopup(Text, this,new Point(10,ClickedNode.Y), BeepPopupFormPosition.Right);
            _isPopupOpen = true;
            Invalidate();
        }
    
        protected void CloseOpenPopupMenu()
        {
            _button.ClosePopup();
        }
        private void LastNodeMenuShown_MenuItemSelected(object? sender, SelectedItemChangedEventArgs e)
        {
            MenuItemSelected?.Invoke(sender, e);
        }
        #region "Menu "
        public void ShowContextMenu(BindingList<SimpleItem> menuList)
        {
            CurrentMenutems = menuList;
          
            TogglePopup();
        }

        private void button_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            MenuItemSelected?.Invoke(this, e);
        }

        public void ClosePopup()
        {

            if (!_isPopupOpen) return;

            if (menuDialog != null)
            {
                menuDialog.SelectedItemChanged -= button_SelectedItemChanged;
                menuDialog.CloseCascade();
                //  menuDialog.Close();
                menuDialog.Dispose();
                menuDialog = null;
            }
            _isPopupOpen = false;
            Invalidate();
        }
        #endregion "Menu"
        #endregion
        #region "Add Nodes"
        /// <summary>
        /// Adds a new SimpleItem to the specified parent's Children and draws the entire branch.
        /// If no valid parent is specified or found, creates a root node.
        /// </summary>
        /// <param name="newItem">The SimpleItem to add.</param>
        /// <param name="parentGuidId">The GuidID of the parent node (null or invalid for root).</param>
        /// <returns>The created BeepTreeNode, or null if the operation fails.</returns>
        public SimpleItem AddNodeWithBranch(SimpleItem newItem, string parentGuidId = null)
        {
            if (newItem == null)
            {
                MiscFunctions.SendLog("Cannot add null SimpleItem.");
                return null;
            }

            try
            {
              
                SimpleItem parentItem = null;
                

                // Ensure newItem has a GuidId
                if (string.IsNullOrEmpty(newItem.GuidId))
                {
                    newItem.GuidId = Guid.NewGuid().ToString();
                }

                // Handle root node case (null or invalid parentGuidId)
                if (string.IsNullOrEmpty(parentGuidId))
                {
                    Nodes.Add(newItem); // Add to rootnodeitems for data consistency
                }
                else
                {
                    // Find the parent node by GuidID
                    parentItem = GetNodeByGuidID(parentGuidId);
                    if (parentItem == null)
                    {
                       

                        Nodes.Add(newItem);
                    }
                    else
                    {
                        // Parent exists: add as child
                        
                        if (parentItem == null)
                        {
                            MiscFunctions.SendLog($"Parent node {parentGuidId} has no associated SimpleItem.");
                            return null;
                        }

                        parentItem.Children.Add(newItem);
                        newItem.ParentItem = parentItem;

                     
                    }
                }

               
                return newItem;
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"Error adding node with branch: {ex.Message}");
                return null;
            }
        }
        #endregion  "Add Nodes"
        #region "Remove Nodes"
        /// <summary>
        /// Removes a SimpleItem from the tree and updates Nodes and the UI.
        /// </summary>
        /// <param name="simpleItem">The SimpleItem to remove.</param>
        public void RemoveNode(SimpleItem simpleItem)
        {
            if (simpleItem == null)
            {
                MiscFunctions.SendLog("Cannot remove null SimpleItem.");
                return;
            }

            try
            {
                var item = FindNodeByName(Nodes, simpleItem.Name);
                if (item != null)
                {
                    RemoveNode(item);
                }
               

                // Trigger NodeDeleted event
                NodeDeleted?.Invoke(this, new BeepMouseEventArgs { EventName = "NodeDeleted", Data = simpleItem.GuidId });
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"Error removing node: {ex.Message}");
            }
        }
        private SimpleItem FindParentNode(IEnumerable<SimpleItem> items, SimpleItem target)
        {
            foreach (var item in items)
            {
                if (item.Children.Contains(target))
                {
                    return item;
                }

                var found = FindParentNode(item.Children, target);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }
       

        public void ClearNodes()
        {
            Nodes.Clear();
            Invalidate();
        }

        public void RemoveNode(string NodeName)
        {
            // Find the SimpleItem by name
            var simpleItem = FindNodeByName(Nodes, NodeName);
            if (simpleItem != null)
            {
                RemoveNode(simpleItem);
            }
        }

        // Helper to find a node by name in the hierarchy
        public SimpleItem FindNodeByName(IEnumerable<SimpleItem> items, string name)
        {
            foreach (var item in items)
            {
                if (item.Text == name)
                {
                    return item;
                }

                var found = FindNodeByName(item.Children, name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public void RemoveNode(int NodeIndex)
        {
            if (NodeIndex >= 0 && NodeIndex < Nodes.Count)
            {
                var simpleItem = Nodes[NodeIndex];
                RemoveNode(simpleItem);
            }
        }


        #endregion "Remove Nodes"
        #region "Scroll Management"
        // Call this from your constructor
        private void InitializeScrollbars()
        {
            // Disable built-in scrollbars
            this.AutoScroll = false;
            this.VerticalScroll.Visible = false;
            this.HorizontalScroll.Visible = false;

            // Create vertical scrollbar
            _verticalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Vertical,
                Visible = false,
                Width = 10
            };
            _verticalScrollBar.Scroll += VerticalScrollBar_Scroll;
            Controls.Add(_verticalScrollBar);

            // Create horizontal scrollbar
            _horizontalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Horizontal,
                Visible = false,
                Height = 10
            };
            _horizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
            Controls.Add(_horizontalScrollBar);
        }

        private void VerticalScrollBar_Scroll(object sender, EventArgs e)
        {
            _yOffset = _verticalScrollBar.Value;
            Invalidate();
        }

        private void HorizontalScrollBar_Scroll(object sender, EventArgs e)
        {
            _xOffset = _horizontalScrollBar.Value;
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (_verticalScrollBar.Visible)
            {
                // Calculate new scroll position
                int newValue = _verticalScrollBar.Value;
                if (e.Delta > 0)
                {
                    // Scroll up
                    newValue = Math.Max(0, newValue - _verticalScrollBar.SmallChange);
                }
                else
                {
                    // Scroll down
                    newValue = Math.Min(_verticalScrollBar.Maximum - _verticalScrollBar.LargeChange,
                                newValue + _verticalScrollBar.SmallChange);
                }

                _verticalScrollBar.Value = newValue;
                _yOffset = newValue;
                Invalidate();
            }
        }

        private void UpdateScrollBars()
        {
            if (DesignMode)
                return;

            // Calculate total content height based on visible nodes
            int contentHeight = CalculateTotalContentHeight();
            int contentWidth = CalculateTotalContentWidth();

            // Get visible area dimensions
            int visibleHeight = ClientRectangle.Height;
            int visibleWidth = ClientRectangle.Width;

            // Debug info
            System.Diagnostics.Debug.WriteLine($"Content: {contentWidth}x{contentHeight}, Visible: {visibleWidth}x{visibleHeight}");

            // Determine if scrollbars are needed
            bool needsVertical = _showVerticalScrollBar && contentHeight > visibleHeight;
            bool needsHorizontal = _showHorizontalScrollBar && contentWidth > visibleWidth;

            // Update scrollbar for vertical scrolling
            if (needsVertical)
            {
                // Configure scrollbar
                _verticalScrollBar.Minimum = 0;
                _verticalScrollBar.Maximum = contentHeight;
                _verticalScrollBar.SmallChange = 20;
                _verticalScrollBar.LargeChange = visibleHeight;
                _verticalScrollBar.Value = Math.Min(_yOffset, Math.Max(0, contentHeight - visibleHeight));

                // Position scrollbar
                _verticalScrollBar.Location = new Point(
                    ClientRectangle.Right - _verticalScrollBar.Width,
                    ClientRectangle.Top);
                _verticalScrollBar.Height = needsHorizontal ?
                    visibleHeight - _horizontalScrollBar.Height :
                    visibleHeight;

                // Make visible
                _verticalScrollBar.Visible = true;
                _verticalScrollBar.BringToFront();

                System.Diagnostics.Debug.WriteLine($"Vertical scrollbar visible: H={_verticalScrollBar.Height}, Max={_verticalScrollBar.Maximum}");
            }
            else
            {
                _verticalScrollBar.Visible = false;
                _yOffset = 0;
            }

            // Update scrollbar for horizontal scrolling
            if (needsHorizontal)
            {
                // Configure scrollbar
                _horizontalScrollBar.Minimum = 0;
                _horizontalScrollBar.Maximum = contentWidth;
                _horizontalScrollBar.SmallChange = 20;
                _horizontalScrollBar.LargeChange = visibleWidth;
                _horizontalScrollBar.Value = Math.Min(_xOffset, Math.Max(0, contentWidth - visibleWidth));

                // Position scrollbar
                _horizontalScrollBar.Location = new Point(
                    ClientRectangle.Left,
                    ClientRectangle.Bottom - _horizontalScrollBar.Height);
                _horizontalScrollBar.Width = needsVertical ?
                    visibleWidth - _verticalScrollBar.Width :
                    visibleWidth;

                // Make visible
                _horizontalScrollBar.Visible = true;
                _horizontalScrollBar.BringToFront();
            }
            else
            {
                _horizontalScrollBar.Visible = false;
                _xOffset = 0;
            }
        }

        private int CalculateTotalContentHeight()
        {
            int totalHeight = 0;

            // Sum the heights of all visible rows
            foreach (var node in _visibleNodes)
            {
                // Calculate row height (same logic as in DrawNodeRecursive)
                Font drawFont = UseThemeFont ? BeepThemesManager.ToFont(_currentTheme.LabelSmall) : (_useScaledfont ? Font : _textFont);
                _button.Text = node.Item.Text ?? string.Empty;
                _button.Size = _button.GetPreferredSize(Size.Empty);
                Size textSize = _button.Size;

                int rowHeight = Math.Max(_minRowHeight, Math.Max(textSize.Height, Math.Max(boxsize, imagesize)) + 2 * _verticalPadding);
                totalHeight += rowHeight;
            }

            // Add some padding
            totalHeight += 20;

            return Math.Max(totalHeight, 50); // Ensure minimum height
        }

        private int CalculateTotalContentWidth()
        {
            int maxWidth = 0;

            // Find the widest row
            foreach (var node in _visibleNodes)
            {
                // Calculate content width for this row (similar to DrawNodeRecursive)
                int level = node.Level;
                int baseIndent = level * _indentWidth;

                _button.Text = node.Item.Text ?? string.Empty;
                _button.Size = _button.GetPreferredSize(Size.Empty);
                Size textSize = _button.Size;

                int rowWidth = baseIndent + boxsize + (_showCheckBox ? boxsize + 4 : 0) + imagesize + 8 + textSize.Width + 40;

                maxWidth = Math.Max(maxWidth, rowWidth);
            }

            return Math.Max(maxWidth, 100); // Ensure minimum width
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateScrollBars();
        }

        #endregion 

    }
}
