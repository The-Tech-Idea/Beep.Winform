using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Editors;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepAccordionMenu), "BeepAccordion.bmp")]
    [Description("A collapsible accordion control with expandable menu items.")]
    [DisplayName("Beep Accordion Menu")]
    public class BeepAccordionMenu : BeepControl
    {
        #region Private Fields
        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        private List<(Rectangle Bounds, SimpleItem Item)> hitList = new List<(Rectangle, SimpleItem)>();
        private int itemHeight = 40;
        private int padding = 8;
        private int iconSize = 24;
        private int iconMargin = 10;
        private int indentSize = 20;
        private SimpleItem hoveredItem;
        private SimpleItem clickedItem;
        private int _selectedIndex = -1;
        private SimpleItem _selectedItem;
        private bool _showIcons = true;
        private bool _showBorder = true;
        private int _expandButtonSize = 16;
        private Color _expandButtonColor;
        private Color _collapsedButtonColor;
        private int _expandButtonMargin = 5;
        private bool _animateExpand = true;
        private Dictionary<SimpleItem, float> _animationProgress = new Dictionary<SimpleItem, float>();
        private Timer _animationTimer;
        private Font _headerFont;
        private bool _useCustomHeaderFont = false;
        private int _hoverBorderThickness = 1;
        private bool _autoExpandSelected = false;
        private bool _allowCollapse = true;

        // Temp Controls for Drawing
        private BeepLabel _label;
        private BeepImage _image;
        private BeepButton _button;
        #endregion

        #region Constructor
        public BeepAccordionMenu()
        {
            DoubleBuffered = true;

            // Initialize animation timer
            _animationTimer = new Timer();
            _animationTimer.Interval = 16; // ~60 FPS
            _animationTimer.Tick += AnimationTimer_Tick;

            // Default colors
            _expandButtonColor = Color.White;
            _collapsedButtonColor = Color.LightGray;

            // Set up header font
            _headerFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);

            // Configure event handlers
            MouseMove += OnMouseMove;
            MouseLeave += OnMouseLeave;
            MouseClick += OnMouseClick;

            // Default settings
            ShowAllBorders = true;
            BorderThickness = 1;
            AutoScroll = true;
            _label = new BeepLabel();
            _label.IsChild = true;
            _label.IsFrameless = true;
            _image = new BeepImage();
            _image.IsChild = true;
            _image.IsFrameless = true;
            _button = new BeepButton();
            _button.IsChild = true;
            _button.IsFrameless = true;

            // Apply theme
            ApplyTheme();
        }
        #endregion

        #region Theme Application
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            // Apply theme colors
            BackColor = _currentTheme?.SideMenuBackColor ?? Color.FromArgb(51, 51, 51);
            ForeColor = _currentTheme?.SideMenuForeColor ?? Color.White;
            BorderColor = _currentTheme?.SideMenuBorderColor ?? Color.Gray;
            _expandButtonColor = _currentTheme?.SideMenuSelectedForeColor ?? Color.White;
            _collapsedButtonColor = _currentTheme?.SideMenuForeColor ?? Color.LightGray;

            HoverBackColor = _currentTheme?.SideMenuHoverBackColor ?? Color.FromArgb(70, 70, 70);
            HoverForeColor = _currentTheme?.SideMenuHoverForeColor ?? Color.White;

            SelectedBackColor = _currentTheme?.SideMenuSelectedBackColor ?? Color.SteelBlue;
            SelectedForeColor = _currentTheme?.SideMenuSelectedForeColor ?? Color.White;

            Invalidate();
        }
        #endregion

        #region Painting
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Use proper anti-aliasing for smoother rendering
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            DrawContent(e.Graphics);
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            hitList.Clear();
            int y = padding;

            // Calculate total height needed
            int totalHeight = 0;
            foreach (var item in items)
            {
                CalculateHeightRecursive(item, 0, ref totalHeight);
            }

            // Set virtual size for AutoScroll
            if (AutoScroll)
            {
                AutoScrollMinSize = new Size(0, totalHeight + padding);
            }

            // Adjust for scroll position
            y -= AutoScrollPosition.Y;

            // Draw all items
            foreach (var item in items)
            {
                DrawItemRecursive(g, item, 0, ref y);
            }
        }

        private void CalculateHeightRecursive(SimpleItem item, int level, ref int totalHeight)
        {
            totalHeight += itemHeight + padding;

            if (item.IsExpanded && item.Children != null && item.Children.Any())
            {
                foreach (var child in item.Children)
                {
                    CalculateHeightRecursive(child, level + 1, ref totalHeight);
                }
            }
        }

        private void DrawItemRecursive(Graphics g, SimpleItem item, int level, ref int y)
        {
            // Skip if outside visible area
            if (y + itemHeight < 0 || y > Height)
            {
                // Estimate height of this tree branch
                if (item.IsExpanded && item.Children != null && item.Children.Any())
                {
                    int skipHeight = (itemHeight + padding) * item.Children.Count;
                    y += skipHeight + itemHeight + padding;
                }
                else
                {
                    y += itemHeight + padding;
                }
                return;
            }

            int indent = level * indentSize;
            Rectangle bounds = new Rectangle(padding + indent, y, Width - 2 * padding - indent - (AutoScroll ? SystemInformation.VerticalScrollBarWidth : 0), itemHeight);
            hitList.Add((bounds, item));

            bool isHovered = hoveredItem == item;
            bool isClicked = clickedItem == item;
            bool isSelected = SelectedItem == item;

            // Determine colors based on state
            Color bgColor = isSelected ? SelectedBackColor : (isHovered ? HoverBackColor : BackColor);
            Color fgColor = isSelected ? SelectedForeColor : (isHovered ? HoverForeColor : ForeColor);

            // Background
            using (Brush bgBrush = new SolidBrush(bgColor))
                g.FillRectangle(bgBrush, bounds);

            // Hover border if hovered
            if (isHovered && _hoverBorderThickness > 0)
            {
                using (Pen borderPen = new Pen(HoverBorderColor, _hoverBorderThickness))
                {
                    g.DrawRectangle(borderPen,
                        bounds.X, bounds.Y,
                        bounds.Width - 1,
                        bounds.Height - 1);
                }
            }

            // Selected border if selected
            if (isSelected && !isHovered)
            {
                using (Pen borderPen = new Pen(BorderColor, BorderThickness))
                {
                    g.DrawRectangle(borderPen,
                        bounds.X, bounds.Y,
                        bounds.Width - 1,
                        bounds.Height - 1);
                }
            }

            // Expand/collapse button (only if item has children)
            bool hasChildren = item.Children != null && item.Children.Any();
            if (hasChildren && _allowCollapse)
            {
                Rectangle expandBtnRect = new Rectangle(
                    bounds.X + _expandButtonMargin,
                    bounds.Y + (itemHeight - _expandButtonSize) / 2,
                    _expandButtonSize,
                    _expandButtonSize);

                Color expandColor = item.IsExpanded ? _expandButtonColor : _collapsedButtonColor;

                using (Pen expandPen = new Pen(expandColor, 2))
                {
                    // Draw the expand/collapse indicator
                    g.DrawLine(expandPen,
                        expandBtnRect.X + 2,
                        expandBtnRect.Y + expandBtnRect.Height / 2,
                        expandBtnRect.X + expandBtnRect.Width - 2,
                        expandBtnRect.Y + expandBtnRect.Height / 2);

                    // Only draw vertical line if not expanded
                    if (!item.IsExpanded)
                    {
                        g.DrawLine(expandPen,
                            expandBtnRect.X + expandBtnRect.Width / 2,
                            expandBtnRect.Y + 2,
                            expandBtnRect.X + expandBtnRect.Width / 2,
                            expandBtnRect.Y + expandBtnRect.Height - 2);
                    }
                }
            }

            int contentXPosition = bounds.X;
            if (hasChildren && _allowCollapse)
            {
                contentXPosition += _expandButtonSize + _expandButtonMargin * 2;
            }

            // Icon/Image if showing icons
            if (_showIcons && !string.IsNullOrEmpty(item.ImagePath))
            {
                Rectangle imgRect = new Rectangle(
                    contentXPosition + iconMargin,
                    bounds.Y + (itemHeight - iconSize) / 2,
                    iconSize,
                    iconSize);
                _image.Size = imgRect.Size;
                _image.ImagePath = item.ImagePath;
                _image.Draw(g,  imgRect);
                contentXPosition += iconSize + iconMargin;
            }

            // Text
            Rectangle textRect = new Rectangle(
                contentXPosition + iconMargin,
                bounds.Y,
                bounds.Width - (contentXPosition - bounds.X) - iconMargin * 2,
                itemHeight);

            // Choose font based on item level
            Font textFont = (level == 0 && _useCustomHeaderFont) ? _headerFont : Font;
            
            _label.Text = item.Text;
            _label.Font = textFont;
            _label.ForeColor = fgColor;
            _label.BackColor = BackColor;
            _label.TextAlign = ContentAlignment.MiddleLeft;
            _label.Draw(g, textRect);

            // Badge for additional information (like count)
            if (!string.IsNullOrEmpty(item.Description))
            {
                Size badgeSize = TextRenderer.MeasureText(item.Description, Font);
                badgeSize.Width += 10; // Add padding

                Rectangle badgeRect = new Rectangle(
                    bounds.Right - badgeSize.Width - iconMargin,
                    bounds.Y + (bounds.Height - badgeSize.Height) / 2,
                    badgeSize.Width,
                    badgeSize.Height);

                using (SolidBrush badgeBrush = new SolidBrush(Color.FromArgb(180, Color.Gray)))
                {
                    using (GraphicsPath path = GetRoundedRectPath(badgeRect, 4))
                    {
                        g.FillPath(badgeBrush, path);
                    }
                }

                TextRenderer.DrawText(g, item.Description, Font, badgeRect,
                    Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            y += itemHeight + padding;

            // Draw children if expanded
            if (item.IsExpanded && item.Children != null && item.Children.Any())
            {
                foreach (var child in item.Children)
                {
                    DrawItemRecursive(g, child, level + 1, ref y);
                }
            }
        }
        #endregion

        #region Event Handlers
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            SimpleItem oldHoveredItem = hoveredItem;
            hoveredItem = null;

            // Adjust for scrolling
            Point scrollAdjustedLocation = new Point(e.X, e.Y - AutoScrollPosition.Y);

            foreach (var (rect, item) in hitList)
            {
                if (rect.Contains(scrollAdjustedLocation))
                {
                    hoveredItem = item;
                    if (oldHoveredItem != hoveredItem)
                    {
                        Invalidate(); // Only redraw if the hovered item changed
                    }
                    return;
                }
            }

            // If no item is found, and the old hovered item was not null,
            // we need to redraw to remove the hover effect
            if (oldHoveredItem != null)
            {
                Invalidate();
            }
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (hoveredItem != null)
            {
                hoveredItem = null;
                Invalidate();
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            // Account for scrolling
            Point scrollAdjustedLocation = new Point(e.X, e.Y - AutoScrollPosition.Y);

            for (int i = 0; i < hitList.Count; i++)
            {
                var (rect, item) = hitList[i];
                if (rect.Contains(scrollAdjustedLocation))
                {
                    clickedItem = item;

                    // Check if click is on the expand/collapse button
                    bool hasChildren = item.Children != null && item.Children.Any();
                    if (hasChildren && _allowCollapse)
                    {
                        Rectangle expandBtnRect = new Rectangle(
                            rect.X + _expandButtonMargin,
                            rect.Y + (itemHeight - _expandButtonSize) / 2,
                            _expandButtonSize,
                            _expandButtonSize);

                        // Expand/collapse if clicking on the button
                        if (expandBtnRect.Contains(scrollAdjustedLocation))
                        {
                            ToggleExpand(item);
                            return;
                        }
                    }

                    // Handle selection
                    SelectedIndex = i;
                    SelectedItem = item;

                    // Auto-expand selected item's children if needed
                    if (_autoExpandSelected && !item.IsExpanded && item.Children?.Count > 0)
                    {
                        ToggleExpand(item);
                    }

                    // Trigger click event
                    OnItemClick(clickedItem);
                    Invalidate();
                    return;
                }
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            bool needsRedraw = false;

            // Update all animations
            foreach (var key in _animationProgress.Keys.ToList())
            {
                if (key.IsExpanded)
                {
                    _animationProgress[key] = Math.Min(1.0f, _animationProgress[key] + 0.1f);
                    needsRedraw = true;
                }
                else
                {
                    _animationProgress[key] = Math.Max(0.0f, _animationProgress[key] - 0.1f);
                    needsRedraw = true;
                }

                // Remove completed animations
                if (_animationProgress[key] == 0 || _animationProgress[key] == 1)
                {
                    // Keep animations at 1 if expanded
                    if (!key.IsExpanded)
                    {
                        _animationProgress.Remove(key);
                    }
                }
            }

            // Stop timer if no more animations
            if (_animationProgress.Count == 0 || !_animateExpand)
            {
                _animationTimer.Stop();
            }

            if (needsRedraw)
            {
                Invalidate();
            }
        }
        #endregion

        #region Helper Methods
        private void ToggleExpand(SimpleItem item)
        {
            item.IsExpanded = !item.IsExpanded;

            // Set up animation if enabled
            if (_animateExpand)
            {
                if (!_animationProgress.ContainsKey(item))
                {
                    _animationProgress[item] = item.IsExpanded ? 0.0f : 1.0f;
                }

                if (!_animationTimer.Enabled)
                {
                    _animationTimer.Start();
                }
            }

            // Trigger event
            OnItemExpandChanged(item);
            Invalidate();
        }
        #endregion

        #region Public Events
        [Category("Behavior")]
        public event EventHandler<SimpleItem> ItemClick;

        [Category("Behavior")]
        public event EventHandler<SimpleItem> ItemExpandChanged;

        [Category("Behavior")]
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        protected virtual void OnItemClick(SimpleItem item)
        {
            ItemClick?.Invoke(this, item);
        }

        protected virtual void OnItemExpandChanged(SimpleItem item)
        {
            ItemExpandChanged?.Invoke(this, item);
        }

        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }
        #endregion

        #region Public Properties
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("The menu items to display in the accordion.")]
        public BindingList<SimpleItem> ListItems
        {
            get => items;
            set { items = value; Invalidate(); }
        }

        [Browsable(false)]
        [Description("Gets or sets the index of the currently selected item.")]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= -1 && value < hitList.Count)
                {
                    _selectedIndex = value;

                    if (value >= 0)
                    {
                        _selectedItem = hitList[_selectedIndex].Item;
                        OnSelectedItemChanged(_selectedItem);
                    }
                    else
                    {
                        _selectedItem = null;
                        OnSelectedItemChanged(null);
                    }

                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        [Description("Gets or sets the currently selected item.")]
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;

                    // Update selectedIndex to match
                    _selectedIndex = -1;
                    for (int i = 0; i < hitList.Count; i++)
                    {
                        if (hitList[i].Item == value)
                        {
                            _selectedIndex = i;
                            break;
                        }
                    }

                    OnSelectedItemChanged(_selectedItem);
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        [DefaultValue(40)]
        [Description("The height of each menu item.")]
        public int ItemHeight
        {
            get => itemHeight;
            set { itemHeight = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(24)]
        [Description("The size of the icon shown for each menu item.")]
        public int IconSize
        {
            get => iconSize;
            set { iconSize = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Determines whether icons are shown for menu items.")]
        public bool ShowIcons
        {
            get => _showIcons;
            set { _showIcons = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Determines whether a border is shown around the control.")]
        public bool ShowBorder
        {
            get => _showBorder;
            set { _showBorder = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(16)]
        [Description("The size of the expand/collapse button shown for items with children.")]
        public int ExpandButtonSize
        {
            get => _expandButtonSize;
            set { _expandButtonSize = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("The color of the expand button when an item is expanded.")]
        public Color ExpandButtonColor
        {
            get => _expandButtonColor;
            set { _expandButtonColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("The color of the expand button when an item is collapsed.")]
        public Color CollapsedButtonColor
        {
            get => _collapsedButtonColor;
            set { _collapsedButtonColor = value; Invalidate(); }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Determines whether expanding/collapsing items is animated.")]
        public bool AnimateExpand
        {
            get => _animateExpand;
            set { _animateExpand = value; }
        }

        [Category("Appearance")]
        [Description("The font used for top-level menu items.")]
        public Font HeaderFont
        {
            get => _headerFont;
            set
            {
                _headerFont = value;
                _useCustomHeaderFont = true;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Determines whether to use a custom font for header items.")]
        public bool UseCustomHeaderFont
        {
            get => _useCustomHeaderFont;
            set { _useCustomHeaderFont = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(1)]
        [Description("The thickness of the border shown when hovering over an item.")]
        public int HoverBorderThickness
        {
            get => _hoverBorderThickness;
            set { _hoverBorderThickness = value; Invalidate(); }
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Determines whether selected items are automatically expanded.")]
        public bool AutoExpandSelected
        {
            get => _autoExpandSelected;
            set { _autoExpandSelected = value; }
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Determines whether items can be collapsed.")]
        public bool AllowCollapse
        {
            get => _allowCollapse;
            set { _allowCollapse = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("The color shown when hovering over an item.")]
        public Color HoverBackColor { get; set; } = Color.DimGray;

        [Category("Appearance")]
        [Description("The text color shown when hovering over an item.")]
        public Color HoverForeColor { get; set; } = Color.White;

        [Category("Appearance")]
        [Description("The border color shown when hovering over an item.")]
        public Color HoverBorderColor { get; set; } = Color.DodgerBlue;

        [Category("Appearance")]
        [Description("The background color of the selected item.")]
        public Color SelectedBackColor { get; set; } = Color.SteelBlue;

        [Category("Appearance")]
        [Description("The text color of the selected item.")]
        public Color SelectedForeColor { get; set; } = Color.White;

        [Category("Appearance")]
        [DefaultValue(20)]
        [Description("The amount of indentation for each level of hierarchy.")]
        public int IndentSize
        {
            get => indentSize;
            set { indentSize = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DefaultValue(8)]
        [Description("The padding between menu items.")]
        public int ItemPadding
        {
            get => padding;
            set { padding = value; Invalidate(); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Selects the item with the specified ID.
        /// </summary>
        public void SelectItemById(int id)
        {
            foreach (var (rect, item) in hitList)
            {
                if (item.Id == id)
                {
                    SelectedItem = item;
                    break;
                }
            }
        }

        /// <summary>
        /// Selects the item with the specified text.
        /// </summary>
        public void SelectItemByText(string text)
        {
            foreach (var (rect, item) in hitList)
            {
                if (string.Equals(item.Text, text, StringComparison.OrdinalIgnoreCase))
                {
                    SelectedItem = item;
                    break;
                }
            }
        }

        /// <summary>
        /// Expands all items in the menu.
        /// </summary>
        public void ExpandAll()
        {
            foreach (var item in items)
            {
                ExpandAllRecursive(item);
            }
            Invalidate();
        }

        /// <summary>
        /// Collapses all items in the menu.
        /// </summary>
        public void CollapseAll()
        {
            foreach (var item in items)
            {
                CollapseAllRecursive(item);
            }
            Invalidate();
        }

        private void ExpandAllRecursive(SimpleItem item)
        {
            if (item.Children?.Any() == true)
            {
                item.IsExpanded = true;
                foreach (var child in item.Children)
                {
                    ExpandAllRecursive(child);
                }
            }
        }

        private void CollapseAllRecursive(SimpleItem item)
        {
            if (item.Children?.Any() == true)
            {
                item.IsExpanded = false;
                foreach (var child in item.Children)
                {
                    CollapseAllRecursive(child);
                }
            }
        }

        /// <summary>
        /// Ensures the specified item is visible by expanding its parents and scrolling to it.
        /// </summary>
        public void EnsureVisible(SimpleItem item)
        {
            if (item == null)
                return;

            // First, expand all parent items
            ExpandParents(item);

            // Then, find the item's rectangle
            foreach (var (rect, listItem) in hitList)
            {
                if (listItem == item)
                {
                    // Scroll to the item
                    if (AutoScroll)
                    {
                        AutoScrollPosition = new Point(0, rect.Y - padding);
                    }
                    break;
                }
            }

            Invalidate();
        }

        private void ExpandParents(SimpleItem item)
        {
            // Find and expand all parent items
            foreach (var parentItem in items)
            {
                if (ExpandParentRecursive(parentItem, item))
                {
                    break;
                }
            }
        }

        private bool ExpandParentRecursive(SimpleItem parent, SimpleItem target)
        {
            if (parent == target)
                return true;

            if (parent.Children == null)
                return false;

            foreach (var child in parent.Children)
            {
                if (child == target || ExpandParentRecursive(child, target))
                {
                    parent.IsExpanded = true;
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
                _animationTimer = null;

                MouseMove -= OnMouseMove;
                MouseLeave -= OnMouseLeave;
                MouseClick -= OnMouseClick;

                _headerFont?.Dispose();
                _headerFont = null;
            }

            base.Dispose(disposing);
        }
        #endregion
    }

   
}
