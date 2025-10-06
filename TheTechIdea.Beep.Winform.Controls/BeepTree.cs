using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
 
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Base;
 

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

        // With these:
        // With these:
        private int GetScaledBoxSize() => ScaleValue(14);
        private int GetScaledImageSize() => ScaleValue(20);
        private int GetScaledMinRowHeight() => ScaleValue(24);
        private int GetScaledIndentWidth() => ScaleValue(16);
        private int GetScaledVerticalPadding() => ScaleValue(4);
       // private int GetScaledMinRowHeight() = 24;
      //  private int GetScaledIndentWidth() = 16;
      //  private int GetScaledVerticalPadding() = 4;
      //  int GetScaledBoxSize() = 14;
       //int GetScaledImageSize() = 20;
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
              //SafeApplyFont(_textFont);

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
                RecalculateLayoutCache();
                UpdateScrollBars();
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
        // Virtualization settings
        private bool _virtualizeLayout = true;
        private int _virtualizationBufferRows = 100;
        [Browsable(true)]
        [Category("Performance")]
        [Description("If true, only measures nodes near the viewport to reduce work on massive trees.")]
        public bool VirtualizeLayout
        {
            get => _virtualizeLayout;
            set { _virtualizeLayout = value; Invalidate(); }
        }
        [Browsable(true)]
        [Category("Performance")]
        [Description("Extra rows to measure above/below the viewport when virtualization is enabled.")]
        public int VirtualizationBufferRows
        {
            get => _virtualizationBufferRows;
            set { _virtualizationBufferRows = Math.Max(0, value); Invalidate(); }
        }
      

        // Renderers
        private BeepButton _toggleRenderer =new BeepButton();
        private BeepButton _button = new BeepButton();
        private BeepCheckBoxBool _checkRenderer=new BeepCheckBoxBool();
        private BeepImage _iconRenderer=new BeepImage();
    private System.Windows.Forms.Timer _resizeTimer; // debounce resize work
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
            public Size TextSize;
            public int RowWidth;
            public int Y; // cached top position in content coordinates
            // cached content-space rectangles
            public Rectangle RowRectContent;
            public Rectangle ToggleRectContent;
            public Rectangle CheckRectContent;
            public Rectangle IconRectContent;
            public Rectangle TextRectContent;
        }

        [Browsable(false)] public IList<SimpleItem> Nodes { get => _nodes; set { _nodes = new List<SimpleItem>(value); RebuildVisible(); Invalidate(); } }

        public BeepTree() : base()
        { // but the SetStyle gives you full control:
            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.Opaque,
                true);
            this.UpdateStyles();
            this.DoubleBuffered = true;
            _toggleRenderer = new BeepButton { IsChild=true, MaxImageSize=new Size(GetScaledBoxSize()-2,GetScaledBoxSize()-2), Size = new Size(GetScaledBoxSize(), GetScaledBoxSize()) ,ImageAlign= ContentAlignment.MiddleCenter,HideText=true};
            _checkRenderer = new BeepCheckBoxBool { IsChild = true, CheckBoxSize = GetScaledBoxSize() };
            _iconRenderer = new BeepImage { IsChild = true, ScaleMode = ImageScaleMode.KeepAspectRatio };
            _button = new BeepButton {IsSelectedOptionOn=true, IsChild = true, MaxImageSize = new Size(GetScaledBoxSize(), GetScaledBoxSize()) };
            // Debounce timer for resize
            _resizeTimer = new System.Windows.Forms.Timer();
            _resizeTimer.Interval = 50; // ms
            _resizeTimer.Tick += (s, e) =>
            {
                _resizeTimer.Stop();
                UpdateScrollBars();
                Invalidate();
            };
           
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

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Suppress default background painting to avoid flicker.
            // We clear the background once in DrawContent.
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
            // Build cached sizes to avoid measuring on every paint/scroll calc
            RecalculateLayoutCache();
            
            // CRITICAL FIX: Update scrollbars whenever visible nodes are rebuilt
            if (!DesignMode && IsHandleCreated)
            {
                UpdateScrollBars();
            }
        }

        
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            UpdateDrawingRect();
            // Keep layout cache fresh around the current viewport when virtualization is enabled
            RecalculateLayoutCache();
            HitList.Clear();

            // Update scrollbars based on current content
            UpdateScrollBars();

            if (_visibleNodes.Count == 0)
                return;

            // Determine which rows intersect current viewport
            int viewportTop = _yOffset;
            int viewportBottom = _yOffset + DrawingRect.Height;

            // Find starting index (first node whose bottom >= viewportTop)
            int startIndex = 0;
            for (int i = 0; i < _visibleNodes.Count; i++)
            {
                var n = _visibleNodes[i];
                int bottom = n.Y + (n.RowHeight > 0 ? n.RowHeight : GetScaledMinRowHeight());
                if (bottom >= viewportTop)
                {
                    startIndex = i;
                    break;
                }
            }

            for (int i = startIndex; i < _visibleNodes.Count; i++)
            {
                var n = _visibleNodes[i];
                if (n.Y > viewportBottom)
                    break;

                DrawNodeFromCache(g, n);
            }

            // Ensure scrollbars are drawn on top
            if (_verticalScrollBar.Visible)
                _verticalScrollBar.Invalidate();
            if (_horizontalScrollBar.Visible)
                _horizontalScrollBar.Invalidate();
        }

        // Draw a node using cached layout and current scroll offsets
        private void DrawNodeFromCache(Graphics g, NodeInfo node)
        {
            var item = node.Item;
            int level = node.Level;
            int rowHeight = node.RowHeight > 0 ? node.RowHeight : GetScaledMinRowHeight();

            // Transform function from content-space to viewport-space
            Func<Rectangle, Rectangle> toViewport = (rc) => new Rectangle(
                DrawingRect.Left + rc.X - _xOffset,
                DrawingRect.Top + rc.Y - _yOffset,
                rc.Width,
                rc.Height);

            // Row background rect in viewport
            Rectangle rowRect = new Rectangle(DrawingRect.Left, DrawingRect.Top + (node.Y - _yOffset), DrawingRect.Width, rowHeight);

            // Toggle
            _toggleRenderer.ImagePath = (item.Children?.Count > 0) ? (item.IsExpanded ? MinusIcon : PlusIcon) : MinusIcon;
            _toggleRenderer.Size = new Size(GetScaledImageSize(), GetScaledImageSize());
            _toggleRenderer.MaxImageSize = new Size(GetScaledImageSize() - 2, GetScaledImageSize() - 2);
            if (!node.ToggleRectContent.IsEmpty)
            {
                _toggleRenderer.Draw(g, toViewport(node.ToggleRectContent));
            }

            // Checkbox
            if (_showCheckBox && !node.CheckRectContent.IsEmpty)
            {
                _checkRenderer.CurrentValue = item.IsChecked;
                _checkRenderer.Draw(g, toViewport(node.CheckRectContent));
            }

            // Icon
            if (!string.IsNullOrEmpty(item.ImagePath) && !node.IconRectContent.IsEmpty)
            {
                _iconRenderer.ImagePath = item.ImagePath;
                _iconRenderer.Size = new Size(GetScaledImageSize(), GetScaledImageSize());
                var state = g.Save();
                try
                {
                    _iconRenderer.DrawImage(g, toViewport(node.IconRectContent));
                }
                finally
                {
                    g.Restore(state);
                }
            }

            // Text
            var textRectContent = node.TextRectContent;
            Size textSize = node.TextSize;
            if (textSize.Width == 0 || textSize.Height == 0)
            {
                textSize = TextRenderer.MeasureText(item.Text ?? string.Empty, _textFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                textRectContent.Size = textSize;
            }
            var textRect = toViewport(textRectContent);
            _button.Text = item.Text ?? string.Empty;
            _button.Size = textRect.Size;
            _button.Location = textRect.Location;
            _button.TextAlign = ContentAlignment.MiddleLeft;
            _button.IsHovered = (item == _lastHoveredItem && !item.IsSelected);
            _button.IsSelected = item.IsSelected;
            _button.Draw(g, textRect);

            // Update SimpleItem coordinates (viewport coords)
            item.X = rowRect.X;
            item.Y = rowRect.Y;
        }

        private void DrawNodeRecursive(Graphics g, SimpleItem item, int level, ref int y)
        {
            // Adjust for scrolling
            int adjustedY = y - _yOffset;
            int adjustedX = level * GetScaledIndentWidth() - _xOffset;

            // Skip drawing if the node is offscreen
            int rowHeight = Math.Max(GetScaledMinRowHeight(), _button.GetPreferredSize(Size.Empty).Height + 2 * GetScaledVerticalPadding());
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
            Font drawFont = _textFont;
            string text = item.Text;
            _button.Text = text;
            _button.TextFont = drawFont;
            _button.Size = _button.GetPreferredSize(Size.Empty);
            Size textSize = _button.Size;
            rowHeight = Math.Max(GetScaledMinRowHeight(), Math.Max(textSize.Height, Math.Max(GetScaledBoxSize(), GetScaledImageSize())) + 2 * GetScaledVerticalPadding());

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

            Rectangle toggleRect = new Rectangle(adjustedX, adjustedY + (rowHeight - GetScaledBoxSize()) / 2, GetScaledBoxSize(), GetScaledBoxSize());
            _toggleRenderer.Size = new Size(GetScaledImageSize(), GetScaledImageSize());
            _toggleRenderer.MaxImageSize = new Size(GetScaledImageSize() - 2, GetScaledImageSize() - 2);
            _toggleRenderer.Draw(g, toggleRect);
            AddHitArea($"toggle_{item.GuidId}", toggleRect);

            // Checkbox
            if (_showCheckBox)
            {
                Rectangle checkRect = new Rectangle(adjustedX + GetScaledBoxSize() + 4, adjustedY + (rowHeight - GetScaledBoxSize()) / 2, GetScaledBoxSize(), GetScaledBoxSize());
                _checkRenderer.CurrentValue = item.IsChecked;
                _checkRenderer.Draw(g, checkRect);
                AddHitArea($"check_{item.GuidId}", checkRect);
                checkboxWidth = GetScaledBoxSize() + 4; // Width plus spacing
            }

            // Icon
            int iconX = adjustedX + GetScaledBoxSize() + checkboxWidth + 4;
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                _iconRenderer.ImagePath = item.ImagePath;
                _iconRenderer.Size = new Size(GetScaledImageSize(), GetScaledImageSize());

                int iconY = adjustedY + (rowHeight - GetScaledImageSize()) / 2;
                Rectangle iconRect = new Rectangle(iconX, iconY, GetScaledImageSize(), GetScaledImageSize());

                GraphicsState state = g.Save();
                try
                {
                    _iconRenderer.DrawImage(g, iconRect);
                    AddHitArea($"icon_{item.GuidId}", iconRect);
                }
                finally
                {
                    g.Restore(state);
                }
            }

            // Text
            int textX = iconX + GetScaledImageSize() + 8;
            Rectangle textRect = new Rectangle(textX, adjustedY + GetScaledVerticalPadding(), DrawingRect.Width - textX, textSize.Height);
            _button.Text = text;
            _button.Size = textRect.Size;
            _button.Location = textRect.Location;
            _button.TextAlign = ContentAlignment.MiddleLeft;
            _button.Size = _button.GetPreferredSize(Size.Empty);
            textRect = new Rectangle(textX, adjustedY + GetScaledVerticalPadding(), _button.Size.Width, _button.Size.Height);

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
            if (e.Button == MouseButtons.Right && LocalHitTest(point, out string htName, out var htItem, out var htRect) && htName.StartsWith("row_"))
            {
                var guid = htName.Substring(4); // everything after "row_"
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
            if (LocalHitTest(point, out var hitName, out var hitItem, out var hitRect))
            {
                var parts = hitName.Split('_');
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
                            
                            // CRITICAL FIX: Rebuild visible nodes and update scrollbars when expanding/collapsing
                            RebuildVisible();
                            UpdateScrollBars();
                            
                            (item.IsExpanded ? NodeExpanded : NodeCollapsed)?.Invoke(this, args);
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
            if (LocalHitTest(mousePosition, out var hitName, out var hitItem, out var targetRect))
            {
                string[] parts = hitName.Split('_');
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
                            _lastHoveredRect = targetRect;

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

        // Local hit-test using cached content rectangles (no per-paint HitList building)
        private bool LocalHitTest(Point p, out string name, out SimpleItem item, out Rectangle rect)
        {
            name = string.Empty; item = null; rect = Rectangle.Empty;
            if (_visibleNodes == null || _visibleNodes.Count == 0) return false;

            // Transform p to content-space by reversing viewport transform
            // We will compare against content-space rectangles translated to viewport
            int viewportTop = _yOffset;
            int viewportBottom = _yOffset + DrawingRect.Height;

            // Find first node potentially visible
            int startIndex = 0;
            for (int i = 0; i < _visibleNodes.Count; i++)
            {
                var n = _visibleNodes[i];
                int bottom = n.Y + (n.RowHeight > 0 ? n.RowHeight : GetScaledMinRowHeight());
                if (bottom >= viewportTop) { startIndex = i; break; }
            }

            Func<Rectangle, Rectangle> toViewport = (rc) => new Rectangle(
                DrawingRect.Left + rc.X - _xOffset,
                DrawingRect.Top + rc.Y - _yOffset,
                rc.Width,
                rc.Height);

            for (int i = startIndex; i < _visibleNodes.Count; i++)
            {
                var n = _visibleNodes[i];
                if (n.Y > viewportBottom) break;

                Rectangle rowVp = new Rectangle(DrawingRect.Left, DrawingRect.Top + (n.Y - _yOffset), DrawingRect.Width, n.RowHeight);
                if (!rowVp.Contains(p)) continue;

                // Check parts in priority order
                var toggleVp = toViewport(n.ToggleRectContent);
                if (!n.ToggleRectContent.IsEmpty && toggleVp.Contains(p)) { name = $"toggle_{n.Item.GuidId}"; item = n.Item; rect = toggleVp; return true; }

                if (_showCheckBox && !n.CheckRectContent.IsEmpty)
                {
                    var checkVp = toViewport(n.CheckRectContent);
                    if (checkVp.Contains(p)) { name = $"check_{n.Item.GuidId}"; item = n.Item; rect = checkVp; return true; }
                }

                if (!n.IconRectContent.IsEmpty)
                {
                    var iconVp = toViewport(n.IconRectContent);
                    if (iconVp.Contains(p)) { name = $"icon_{n.Item.GuidId}"; item = n.Item; rect = iconVp; return true; }
                }

                name = $"row_{n.Item.GuidId}"; item = n.Item; rect = rowVp; return true;
            }

            return false;
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

     
        public override void ApplyTheme()
        { // Store original size
            Size originalSize = this.Size;
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
                _textFont = FontListHelper.CreateFontFromTypography(_currentTheme.TreeNodeUnSelectedFont);

                _button.TextFont = _textFont;
                _button.UseThemeFont = UseThemeFont;
            }
            else
            {
                _button.TextFont = Font;
            }
          //  SafeApplyFont(TextFont ?? _textFont);
            if (UseScaledFont)
            {
                _button.UseScaledFont = UseScaledFont;
            }

         

            // Update DPI-scaled sizes for all rendering components
            if (_toggleRenderer != null)
            {
                _toggleRenderer.Theme = Theme;
                _toggleRenderer.BackColor = BackColor;
                _toggleRenderer.Size = new Size(GetScaledImageSize(), GetScaledImageSize());
                _toggleRenderer.MaxImageSize = new Size(GetScaledImageSize() - 2, GetScaledImageSize() - 2);
            }

            if (_checkRenderer != null)
            {
                _checkRenderer.Theme = Theme;
                _checkRenderer.BackColor = BackColor;
                _checkRenderer.CheckBoxSize = GetScaledBoxSize();
            }

            if (_iconRenderer != null)
            {
                _iconRenderer.Theme = Theme;
                _iconRenderer.BackColor = BackColor;
                _iconRenderer.Size = new Size(GetScaledImageSize(), GetScaledImageSize());
            }

            ForeColor = _currentTheme.TreeForeColor;

            _button.IsColorFromTheme = false;
            _button.BackColor = BackColor;
            _button.ForeColor = _currentTheme.TreeForeColor;
            _button.SelectedForeColor = _currentTheme.TreeNodeSelectedForeColor;
            _button.SelectedBackColor = _currentTheme.TreeNodeSelectedBackColor;
            _button.BorderColor = _currentTheme.TreeNodeSelectedBackColor;
            _button.HoverBackColor = _currentTheme.TreeNodeHoverBackColor;
            _button.HoverForeColor = _currentTheme.TreeNodeHoverForeColor;
           // _button.TextFont = _textFont;
            _button.MaxImageSize = new Size(GetScaledBoxSize(), GetScaledBoxSize());

            if (_verticalScrollBar != null)
                _verticalScrollBar.Theme = Theme;
            if (_horizontalScrollBar != null)
                _horizontalScrollBar.Theme = Theme;

         
       
            // Rebuild with new theme and DPI scaling
            if (this.Size != originalSize)
            {
                System.Diagnostics.Debug.WriteLine($"Theme changed size from {originalSize} to {Size} - restoring!");
                this.Size = originalSize;
            }
           
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
            UpdateScrollBars(); // CRITICAL FIX: Update scrollbars after highlighting

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
                yPos += vNode.RowHeight > 0 ? vNode.RowHeight : GetScaledMinRowHeight();
            }

            if (found)
            {
                // Scroll to the node using our BeepScrollBar instead of AutoScrollPosition
                if (_verticalScrollBar.Visible)
                {
                    _verticalScrollBar.Value = Math.Min(yPos, _verticalScrollBar.Maximum);
                    _yOffset = _verticalScrollBar.Value;
                }
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
            UpdateScrollBars(); // CRITICAL FIX: Update scrollbars after expanding all
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
            UpdateScrollBars(); // CRITICAL FIX: Update scrollbars after collapsing all
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
            UpdateScrollBars(); // CRITICAL FIX: Update scrollbars after ensuring visibility

            // Find and scroll to the node's position
            int y = 0;
            foreach (var node in _visibleNodes)
            {
                if (node.Item == item)
                {
                    // Scroll to the node using our BeepScrollBar instead of AutoScrollPosition
                    if (_verticalScrollBar.Visible)
                    {
                        _verticalScrollBar.Value = Math.Min(y, _verticalScrollBar.Maximum);
                        _yOffset = _verticalScrollBar.Value;
                    }
                    break;
                }
                y += node.RowHeight > 0 ? node.RowHeight : GetScaledMinRowHeight();
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
                //tooltipShown = false;
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
            SimpleItem x = menuDialog.ShowPopup(Text, this,new Point(10,ClickedNode.Y), BeepPopupFormPosition.Right,false,true);
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
            ClosePopup();
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
                //////MiscFunctions.SendLog("Cannot add null SimpleItem.");
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
                            //////MiscFunctions.SendLog($"Parent node {parentGuidId} has no associated SimpleItem.");
                            return null;
                        }

                        parentItem.Children.Add(newItem);
                        newItem.ParentItem = parentItem;

                     
                    }
                }

                // CRITICAL FIX: Update visible nodes and scrollbars after adding node
                RebuildVisible();
                UpdateScrollBars();
                Invalidate();
               
                return newItem;
            }
            catch (Exception ex)
            {
                //////MiscFunctions.SendLog($"Error adding node with branch: {ex.Message}");
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
                //////MiscFunctions.SendLog("Cannot remove null SimpleItem.");
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
                //////MiscFunctions.SendLog($"Error removing node: {ex.Message}");
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
            // CRITICAL FIX: Completely disable Windows Form scrollbars
            this.AutoScroll = false;
            this.VerticalScroll.Visible = false;
            this.HorizontalScroll.Visible = false;
            this.VerticalScroll.Enabled = false;
            this.HorizontalScroll.Enabled = false;

            // Create vertical scrollbar with DPI-scaled width
            _verticalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Vertical,
                Visible = false,
                Width = ScaleValue(15)  // Slightly wider for better usability
            };
            _verticalScrollBar.Scroll += VerticalScrollBar_Scroll;
            Controls.Add(_verticalScrollBar);

            // Create horizontal scrollbar with DPI-scaled height
            _horizontalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Horizontal,
                Visible = false,
                Height = ScaleValue(15)  // Slightly taller for better usability
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

            // Ensure drawing rect is up to date
            UpdateDrawingRect();

            // Calculate total content size based on visible nodes
            int contentHeight = CalculateTotalContentHeight();
            int contentWidth = CalculateTotalContentWidth();

            // Available client area for content (exclude padding/borders via DrawingRect)
            int availableHeight = Math.Max(0, DrawingRect.Height);
            int availableWidth = Math.Max(0, DrawingRect.Width);

            // Initial need assessment
            bool needsVertical = _showVerticalScrollBar && contentHeight > availableHeight;
            bool needsHorizontal = _showHorizontalScrollBar && contentWidth > availableWidth;

            // Re-evaluate to account for the space consumed by the other scrollbar
            bool changed;
            do
            {
                changed = false;

                if (needsVertical)
                {
                    int viewWidthWithV = Math.Max(0, availableWidth - _verticalScrollBar.Width);
                    bool shouldNeedHorizontal = _showHorizontalScrollBar && contentWidth > viewWidthWithV;
                    if (shouldNeedHorizontal && !needsHorizontal)
                    {
                        needsHorizontal = true;
                        changed = true;
                    }
                }

                if (needsHorizontal)
                {
                    int viewHeightWithH = Math.Max(0, availableHeight - _horizontalScrollBar.Height);
                    bool shouldNeedVertical = _showVerticalScrollBar && contentHeight > viewHeightWithH;
                    if (shouldNeedVertical && !needsVertical)
                    {
                        needsVertical = true;
                        changed = true;
                    }
                }
            } while (changed);

            // Final viewport size after accounting for scrollbars
            int viewportHeight = availableHeight - (needsHorizontal ? _horizontalScrollBar.Height : 0);
            int viewportWidth = availableWidth - (needsVertical ? _verticalScrollBar.Width : 0);
            viewportHeight = Math.Max(0, viewportHeight);
            viewportWidth = Math.Max(0, viewportWidth);

            // Vertical scrollbar setup
            if (needsVertical)
            {
                _verticalScrollBar.Minimum = 0;
                _verticalScrollBar.Maximum = Math.Max(contentHeight, 0);
                _verticalScrollBar.SmallChange = Math.Max(1, GetScaledMinRowHeight() / 2);
                _verticalScrollBar.LargeChange = Math.Max(1, viewportHeight);

                // Clamp value within valid range
                int maxValue = Math.Max(0, _verticalScrollBar.Maximum - _verticalScrollBar.LargeChange);
                _verticalScrollBar.Value = Math.Min(Math.Max(0, _yOffset), maxValue);

                // Position and size
                _verticalScrollBar.Location = new Point(DrawingRect.Right - _verticalScrollBar.Width, DrawingRect.Top);
                _verticalScrollBar.Height = viewportHeight;
                _verticalScrollBar.Visible = true;
                _verticalScrollBar.BringToFront();
            }
            else
            {
                _verticalScrollBar.Visible = false;
                _verticalScrollBar.Value = 0;
                _yOffset = 0;
            }

            // Horizontal scrollbar setup
            if (needsHorizontal)
            {
                _horizontalScrollBar.Minimum = 0;
                _horizontalScrollBar.Maximum = Math.Max(contentWidth, 0);
                _horizontalScrollBar.SmallChange = Math.Max(1, GetScaledIndentWidth());
                _horizontalScrollBar.LargeChange = Math.Max(1, viewportWidth);

                int maxValue = Math.Max(0, _horizontalScrollBar.Maximum - _horizontalScrollBar.LargeChange);
                _horizontalScrollBar.Value = Math.Min(Math.Max(0, _xOffset), maxValue);

                _horizontalScrollBar.Location = new Point(DrawingRect.Left, DrawingRect.Bottom - _horizontalScrollBar.Height);
                _horizontalScrollBar.Width = viewportWidth;
                _horizontalScrollBar.Visible = true;
                _horizontalScrollBar.BringToFront();
            }
            else
            {
                _horizontalScrollBar.Visible = false;
                _horizontalScrollBar.Value = 0;
                _xOffset = 0;
            }
        }

        private int CalculateTotalContentHeight()
        {
            int totalHeight = 0;

            // Sum cached heights of all visible rows
            foreach (var node in _visibleNodes)
            {
                int rowHeight = node.RowHeight > 0 ? node.RowHeight : GetScaledMinRowHeight();
                totalHeight += rowHeight;
            }

            // No artificial padding or minimums - return actual content height
            return totalHeight;
        }

        private int CalculateTotalContentWidth()
        {
            int maxWidth = 0;

            // Find the widest row using cached widths
            foreach (var node in _visibleNodes)
            {
                int rowWidth = node.RowWidth;
                if (rowWidth == 0)
                {
                    int baseIndent = node.Level * GetScaledIndentWidth();
                    int textWidth = node.TextSize.Width;
                    rowWidth = baseIndent + GetScaledBoxSize() + 4 + (_showCheckBox ? GetScaledBoxSize() + 4 : 0) + GetScaledImageSize() + 8 + textWidth;
                }
                maxWidth = Math.Max(maxWidth, rowWidth);
            }

            // No artificial minimum width - return actual content width
            return maxWidth;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Debounce heavy recalcs during interactive resize
            if (_resizeTimer != null)
            {
                _resizeTimer.Stop();
                _resizeTimer.Start();
            }
            else
            {
                UpdateScrollBars();
                Invalidate();
            }
        }

        #endregion
        #region DPI Methods
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);



            // Update button font and recalculate preferred size
            if (_button != null)
            {
                _button.TextFont = _textFont;
                _button.UseScaledFont = UseScaledFont;
            }

           
            // Font change affects measurements
            RecalculateLayoutCache();
            UpdateScrollBars();
            Invalidate();

            //if (AutoSize)
            //{
            //    Size textSize = TextRenderer.MeasureText(Text, _textFont);
            //    this.Size = new Size(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
            //}
        }
        // Override OnDpiChangedAfterParent to handle DPI changes
        //protected override void OnDpiChangedAfterParent(EventArgs e)
        //{
        //    base.OnDpiChangedAfterParent(e);

        //    // Update all DPI-dependent rendering components
        //    if (_toggleRenderer != null)
        //    {
        //        _toggleRenderer.Size = new Size(GetScaledImageSize(), GetScaledImageSize());
        //        _toggleRenderer.MaxImageSize = new Size(GetScaledImageSize() - 2, GetScaledImageSize() - 2);
        //    }

        //    if (_checkRenderer != null)
        //    {
        //        _checkRenderer.CheckBoxSize = GetScaledBoxSize();
        //    }

        //    if (_iconRenderer != null)
        //    {
        //        _iconRenderer.Size = new Size(GetScaledImageSize(), GetScaledImageSize());
        //    }

        //    if (_button != null)
        //    {
        //        _button.MaxImageSize = new Size(GetScaledBoxSize(), GetScaledBoxSize());
        //        _button.UseScaledFont = UseScaledFont;
        //    }

          
        //    // Force complete redraw
        //    Invalidate();
        //}
        #endregion

        // Compute cached sizes/positions for visible nodes (reduces per-frame work)
        private void RecalculateLayoutCache()
        {
            if (_visibleNodes == null || _visibleNodes.Count == 0)
                return;

            // Ensure measure uses current font
            _button.TextFont = _textFont;
            _button.UseScaledFont = UseScaledFont;

            // Determine virtualization indices
            int start = 0, end = _visibleNodes.Count - 1;
            if (_virtualizeLayout && DrawingRect.Height > 0)
            {
                // Find an approximate center index based on current vertical offset
                int yAccum = 0;
                for (int i = 0; i < _visibleNodes.Count; i++)
                {
                    int estH = _visibleNodes[i].RowHeight > 0 ? _visibleNodes[i].RowHeight : GetScaledMinRowHeight();
                    if (yAccum + estH >= _yOffset) { start = Math.Max(0, i - _virtualizationBufferRows); break; }
                    yAccum += estH;
                }
                end = Math.Min(_visibleNodes.Count - 1, start + (DrawingRect.Height / Math.Max(1, GetScaledMinRowHeight())) + 2 * _virtualizationBufferRows);
            }

            int y = 0;
            for (int i = 0; i < _visibleNodes.Count; i++)
            {
                var n = _visibleNodes[i];
                // Keep Y continuously increasing for fast viewport intersection
                int prevH = n.RowHeight > 0 ? n.RowHeight : GetScaledMinRowHeight();
                n.Y = y;

                if (i >= start && i <= end)
                {
                    string text = n.Item?.Text ?? string.Empty;
                    // Use TextRenderer to measure text once per node
                    Size textSize = TextRenderer.MeasureText(text, _textFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);

                    int rowHeight = Math.Max(GetScaledMinRowHeight(), Math.Max(textSize.Height, Math.Max(GetScaledBoxSize(), GetScaledImageSize())) + 2 * GetScaledVerticalPadding());
                    int baseIndent = n.Level * GetScaledIndentWidth();

                    // Content-space rectangles (no DrawingRect or offsets)
                    int toggleX = baseIndent;
                    int toggleY = n.Y + (rowHeight - GetScaledBoxSize()) / 2;
                    Rectangle toggleRect = new Rectangle(toggleX, toggleY, GetScaledBoxSize(), GetScaledBoxSize());

                    int xAfterToggle = toggleRect.Right + 4;
                    Rectangle checkRect = Rectangle.Empty;
                    if (_showCheckBox)
                    {
                        checkRect = new Rectangle(xAfterToggle, n.Y + (rowHeight - GetScaledBoxSize()) / 2, GetScaledBoxSize(), GetScaledBoxSize());
                        xAfterToggle = checkRect.Right + 4;
                    }

                    int iconX = xAfterToggle;
                    int iconY = n.Y + (rowHeight - GetScaledImageSize()) / 2;
                    Rectangle iconRect = new Rectangle(iconX, iconY, GetScaledImageSize(), GetScaledImageSize());

                    int textX = iconRect.Right + 8;
                    Rectangle textRect = new Rectangle(textX, n.Y + GetScaledVerticalPadding(), textSize.Width, textSize.Height);
                    Rectangle rowRect = new Rectangle(0, n.Y, baseIndent + (textRect.Right - baseIndent), rowHeight);

                    n.TextSize = textSize;
                    n.RowHeight = rowHeight;
                    // base indent + toggle + 4 + optional checkbox + 4 + icon + 8 + text
                    n.RowWidth = baseIndent + GetScaledBoxSize() + 4 + (_showCheckBox ? GetScaledBoxSize() + 4 : 0) + GetScaledImageSize() + 8 + textSize.Width;

                    n.ToggleRectContent = toggleRect;
                    n.CheckRectContent = checkRect;
                    n.IconRectContent = iconRect;
                    n.TextRectContent = textRect;
                    n.RowRectContent = rowRect;

                    prevH = rowHeight;
                }

                y += prevH;
            }
        }

    }
}
