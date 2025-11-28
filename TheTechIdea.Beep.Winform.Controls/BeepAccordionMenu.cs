using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Editors;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;

 
 
 
 

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepAccordionMenu), "BeepAccordion.bmp")]
    [Description("A collapsible accordion control with expandable menu items.")]
    [DisplayName("Beep Accordion Menu")]
    public class BeepAccordionMenu : BeepControl
    {
        private bool isCollapsed = false;
        private const int DefaultExpandedWidth = 200;
        private const int DefaultCollapsedWidth = 64;
        private int animationStep = 20;
        private int animationDelay = 15;

        // used for Drawing only
        private BeepLabel _label;
        private BeepButton _button;
        private BeepImage _image;
        private List<SimpleItem> menuItems = new List<SimpleItem>();
        private int itemHeight = 40;
        private int childItemHeight = 30; // Child items are slightly smaller
        private bool isInitialized = false;
        private Timer animationTimer;
        private bool isAnimating = false;
        private SimpleItem _hoveredItem;
        private SimpleItem _selectedItem;
        private int headerHeight = 40;
        private int spacing = 1;
        private Rectangle _toggleButtonRect;
        private Dictionary<SimpleItem, bool> expandedState = new Dictionary<SimpleItem, bool>();
        private int indentationWidth = 20; // Space for child item indentation

        // Panel and logo/toggle references kept for compatibility
        private Panel itemsPanel;
        private BeepLabel logo;
        private BeepButton toggleButton;

        [Category("Behavior")]
        public int ExpandedWidth { get; set; } = DefaultExpandedWidth;

        [Category("Behavior")]
        public int CollapsedWidth { get; set; } = DefaultCollapsedWidth;

        [Category("Animation")]
        public int AnimationStep
        {
            get => animationStep;
            set => animationStep = Math.Max(1, value);
        }

        [Category("Animation")]
        public int AnimationDelay
        {
            get => animationDelay;
            set => animationDelay = Math.Max(1, value);
        }

        [Category("Appearance")]
        public int ItemHeight
        {
            get => itemHeight;
            set
            {
                itemHeight = Math.Max(20, value);
                childItemHeight = Math.Max(15, itemHeight - 10);
                if (isInitialized)
                {
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        public int ChildItemHeight
        {
            get => childItemHeight;
            set
            {
                childItemHeight = Math.Max(15, value);
                if (isInitialized)
                {
                    Invalidate();
                }
            }
        }

        [Category("Appearance")]
        public int IndentationWidth
        {
            get => indentationWidth;
            set
            {
                indentationWidth = Math.Max(10, value);
                Invalidate();
            }
        }

        public event EventHandler<BeepMouseEventArgs> ItemClick;
        public event EventHandler<BeepMouseEventArgs> ToggleClicked;
        public event EventHandler<BeepMouseEventArgs> HeaderExpandedChanged;

        private string _title = "Accordion";
        [Category("Appearance")]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                if (logo != null)
                    logo.Text = value;
                if (toggleButton != null)
                    toggleButton.Text = value;
                Invalidate();
            }
        }

        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                Invalidate();
            }
        }

        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => items;
            set
            {
                if (value != null && value != items)
                {
                    items = value;
                    if (isInitialized)
                    {
                        InitializeMenuItemState();
                        Invalidate();
                    }
                }
            }
        }

        protected override Size DefaultSize => new Size(DefaultExpandedWidth, 200);

        public BeepAccordionMenu()
        {
            items = new BindingList<SimpleItem>();
            DoubleBuffered = true;
            ApplyThemeToChilds = false;
            TabStop = true;
            Padding = new Padding(5);

            // Initialize GDI drawing components
            _label = new BeepLabel();
            _button = new BeepButton();
            _image = new BeepImage();

            animationTimer = new Timer { Interval = 10 };
            animationTimer.Tick += AnimationTimer_Tick;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try
            {
                if (!isInitialized)
                {
                    //InitializeAccordion();
                    InitializeMenu();
                    InitializeMenuItemState();
                    Invalidate();
                    isInitialized = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Crash in OnHandleCreated: {ex.Message}\n{ex.StackTrace}");
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (isInitialized)
            {
                Invalidate();
            }
        }

        private void InitializeAccordion()
        {
            // Set up the accordion properties
            BackColor = _currentTheme?.SideMenuBackColor ?? Color.Empty;

            // Subscribe to events
            items.ListChanged += Items_ListChanged;
        }

        private void Items_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            InitializeMenuItemState();
            Invalidate();
        }

        private void InitializeMenu()
        {
            // Clear existing hit areas
            ClearHitList();

            // This method now just invalidates the control to redraw with GDI
            Invalidate();
        }

        private void InitializeMenuItemState()
        {
            // Initialize expanded state for accordion headers
            foreach (var item in items)
            {
                // If it's not in the dictionary, add it (default to collapsed)
                if (!expandedState.ContainsKey(item))
                {
                    expandedState[item] = false;
                }

                // Also initialize any child items that have their own children
                foreach (var childItem in item.Children)
                {
                    if (childItem.Children.Count > 0 && !expandedState.ContainsKey((SimpleItem)childItem))
                    {
                        expandedState[(SimpleItem)childItem] = false;
                    }
                }
            }
        }

        private void StartAccordionAnimation()
        {
            if (!isInitialized) return;

            isAnimating = true;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!isAnimating) return;

            int targetWidth = isCollapsed ? CollapsedWidth : ExpandedWidth;
            int currentWidth = Width;

            if (isCollapsed)
            {
                currentWidth -= AnimationStep;
                if (currentWidth <= targetWidth)
                {
                    currentWidth = targetWidth;
                    animationTimer.Stop();
                    isAnimating = false;
                }
            }
            else
            {
                currentWidth += AnimationStep;
                if (currentWidth >= targetWidth)
                {
                    currentWidth = targetWidth;
                    animationTimer.Stop();
                    isAnimating = false;
                }
            }

            Width = currentWidth;
            Invalidate();
        }

        // Override DrawContent to implement custom drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            // Use the DrawingRect for our actual drawing
            Draw(g, DrawingRect);
        }

        // Implementation of Draw method using GDI and DrawingRect
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Enable anti-aliasing for smoother rendering
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Fill the background with menu background color
            using (SolidBrush backgroundBrush = new SolidBrush(_currentTheme?.SideMenuBackColor ?? Color.Empty))
            {
                graphics.FillRectangle(backgroundBrush, rectangle);
            }

            // Draw the title/header area
            Rectangle headerRect = new Rectangle(
                rectangle.X,
                rectangle.Y,
                rectangle.Width,
                headerHeight
            );

            using (SolidBrush headerBrush = new SolidBrush(_currentTheme?.SideMenuBackColor ?? Color.Empty))
            {
                graphics.FillRectangle(headerBrush, headerRect);
            }

            // Draw title text
            _label.Text = Title;
            _label.ForeColor = _currentTheme?.SideMenuForeColor ?? Color.White;
            _label.BackColor = _currentTheme?.SideMenuBackColor ?? Color.Empty;
            _label.TextAlign = ContentAlignment.MiddleLeft;

            // Add padding for title text
            Rectangle titleRect = new Rectangle(
                headerRect.Left + 10,
                headerRect.Top,
                isCollapsed ? headerRect.Width - 40 : headerRect.Width - 50,
                headerRect.Height);

            _label.Draw(graphics, titleRect);

            // Draw toggle button
            _toggleButtonRect = new Rectangle(
                titleRect.Right + 5,
                headerRect.Top + 5,
                30,
                30);

            DrawHamburgerIcon(graphics, _toggleButtonRect);

            // Clear hit list before adding new areas
            ClearHitList();

            // Add hit area for the toggle button
            AddHitArea(
                "ToggleButton",
                _toggleButtonRect,
                null,
                () => {
                    isCollapsed = !isCollapsed;
                    StartAccordionAnimation();
                    ToggleClicked?.Invoke(this, new BeepMouseEventArgs("ToggleClicked", isCollapsed));
                }
            );

            // Get mouse position for hover effects
            Point mousePoint = this.PointToClient(Control.MousePosition);

            // Start drawing menu items below the header
            int yOffset = headerRect.Bottom + 5;

            // Draw each root menu item (header) and its children if expanded
            foreach (var headerItem in items)
            {
                // If the header item was previously not in the expandedState, add it
                if (!expandedState.ContainsKey(headerItem))
                {
                    expandedState[headerItem] = false;
                }

                // Draw the header item
                yOffset = DrawHeaderItem(graphics, rectangle, headerItem, yOffset, mousePoint);

                // If this header is expanded, draw its children
                if (expandedState[headerItem] && !isCollapsed)
                {
                    yOffset = DrawChildItems(graphics, rectangle, headerItem, yOffset, mousePoint);
                }
            }
        }

        private int DrawHeaderItem(Graphics graphics, Rectangle rectangle, SimpleItem headerItem, int yOffset, Point mousePoint)
        {
            // Create the overall item rectangle
            Rectangle headerItemRect = new Rectangle(
                rectangle.X,
                yOffset,
                rectangle.Width,
                itemHeight
            );

            // Check if this item is hovered or selected
            bool isHovered = headerItemRect.Contains(mousePoint);
            bool isSelected = headerItem == SelectedItem;
            bool isExpanded = expandedState[headerItem];

            // Track the X position as we add components horizontally
            int currentX = headerItemRect.Left;

            // Draw highlight panel (left border)
            int highlightWidth = 5;
            Rectangle highlightRect = new Rectangle(
                currentX,
                yOffset,
                highlightWidth,
                itemHeight
            );

            // Choose highlight color based on item state
            Color highlightColor = BackColor;
            if (isHovered || isSelected)
            {
                highlightColor = _currentTheme?.MenuMainItemHoverForeColor ?? Color.DodgerBlue;
            }

            using (SolidBrush highlightBrush = new SolidBrush(highlightColor))
            {
                graphics.FillRectangle(highlightBrush, highlightRect);
            }

            currentX += highlightWidth;

            // Calculate content area
            Rectangle contentRect = new Rectangle(
                currentX,
                yOffset,
                headerItemRect.Width - highlightWidth,
                itemHeight
            );

            // Choose colors based on item state
            Color itemBackColor = BackColor;
            Color itemForeColor = _currentTheme?.SideMenuForeColor ?? Color.White;

            if (isSelected)
            {
                itemBackColor = _currentTheme?.MenuMainItemSelectedBackColor ?? Color.Empty;
            }
            else if (isHovered)
            {
                itemBackColor = _currentTheme?.MenuMainItemHoverBackColor ?? Color.Empty;
            }

            // Draw item background
            using (SolidBrush buttonBrush = new SolidBrush(itemBackColor))
            {
                graphics.FillRectangle(buttonBrush, contentRect);
            }

            // Draw image if available
            if (!string.IsNullOrEmpty(headerItem.ImagePath) && File.Exists(headerItem.ImagePath))
            {
                int imgSize = 24;
                int imgPadding = 8;

                Rectangle imgRect = new Rectangle(
                    currentX + imgPadding,
                    yOffset + ((itemHeight - imgSize) / 2),
                    imgSize,
                    imgSize
                );

                // Configure the BeepImage instance and draw it
                _image.ImagePath = headerItem.ImagePath;
                _image.Size = new Size(imgSize, imgSize);
                _image.ApplyThemeOnImage = true;
                _image.Draw(graphics, imgRect);

                // Advance the X position for text
                currentX += imgSize + (imgPadding * 2);
            }
            else
            {
                // If no image, add some padding
                currentX += 10;
            }

            // Don't draw text if collapsed, just show icons
            if (!isCollapsed)
            {
                // Calculate the text rectangle
                Rectangle textRect = new Rectangle(
                    currentX + 2, // Small text padding
                    yOffset,
                    headerItemRect.Width - currentX - 35, // Right padding for expand/collapse icon
                    itemHeight
                );

                // Use the appropriate font based on theme settings
                Font textFont = FontListHelper.CreateFontFromTypography(_currentTheme?.GetAnswerFont());

                // Draw text using the BeepLabel instance
                _label.Text = headerItem.Text;
                _label.TextFont = textFont;
                _label.ForeColor = itemForeColor;
                _label.BackColor = itemBackColor;
                _label.TextAlign = ContentAlignment.MiddleLeft;
                _label.Draw(graphics, textRect);

                // If the header has children, draw the expand/collapse indicator
                if (headerItem.Children.Count > 0)
                {
                    // Draw expand/collapse arrow
                    int arrowSize = 16;
                    Rectangle arrowRect = new Rectangle(
                        headerItemRect.Right - arrowSize - 10,
                        yOffset + (itemHeight - arrowSize) / 2,
                        arrowSize,
                        arrowSize
                    );

                    DrawExpandCollapseIcon(graphics, arrowRect, isExpanded, itemForeColor);

                    // Add hit area for expand/collapse
                    AddHitArea(
                        $"ExpandCollapse_{headerItem.Text}_{items.IndexOf(headerItem)}",
                        arrowRect,
                        null,
                        () => {
                            // Toggle expanded state
                            expandedState[headerItem] = !expandedState[headerItem];
                            HeaderExpandedChanged?.Invoke(this, new BeepMouseEventArgs(headerItem.Text, headerItem));
                            Invalidate();
                        }
                    );
                }
            }

            // Add hit test area for this header item
            AddHitArea(
                $"HeaderItem_{headerItem.Text}_{items.IndexOf(headerItem)}",
                headerItemRect,
                null,
                () => {
                    // If it has children, toggle expanded state on header click
                    if (headerItem.Children.Count > 0)
                    {
                        expandedState[headerItem] = !expandedState[headerItem];
                        HeaderExpandedChanged?.Invoke(this, new BeepMouseEventArgs(headerItem.Text, headerItem));
                    }

                    // Set as selected item
                    SelectedItem = headerItem;
                    ItemClick?.Invoke(this, new BeepMouseEventArgs(headerItem.Text, headerItem));
                    Invalidate();
                }
            );

            // Store the item location for hit testing
            headerItem.X = headerItemRect.X;
            headerItem.Y = headerItemRect.Y;
            headerItem.Width = headerItemRect.Width;
            headerItem.Height = headerItemRect.Height;

            // Update yOffset for the next item
            return yOffset + itemHeight + spacing;
        }

        private int DrawChildItems(Graphics graphics, Rectangle rectangle, SimpleItem headerItem, int yOffset, Point mousePoint)
        {
            foreach (var childItem in headerItem.Children)
            {
                // Create the child item rectangle with indentation
                Rectangle childItemRect = new Rectangle(
                    rectangle.X + indentationWidth,
                    yOffset,
                    rectangle.Width - indentationWidth,
                    childItemHeight
                );

                // Check if this child item is hovered or selected
                bool isHovered = childItemRect.Contains(mousePoint);
                bool isSelected = childItem == SelectedItem;

                // Choose colors based on item state
                Color itemBackColor = BackColor;
                Color itemForeColor = _currentTheme?.SideMenuForeColor ?? Color.White;

                if (isSelected)
                {
                    itemBackColor = _currentTheme?.MenuMainItemSelectedBackColor ?? Color.Empty;
                }
                else if (isHovered)
                {
                    itemBackColor = _currentTheme?.MenuMainItemHoverBackColor ?? Color.Empty;
                }

                // Draw child item background
                using (SolidBrush childBrush = new SolidBrush(itemBackColor))
                {
                    graphics.FillRectangle(childBrush, childItemRect);
                }

                // Draw connector line from parent to child (vertical line)
                using (Pen connectorPen = new Pen(Color.FromArgb(100, itemForeColor), 1))
                {
                    // Draw vertical line
                    int lineX = rectangle.X + (indentationWidth / 2);
                    int lineTop = headerItem.Y + itemHeight;
                    int lineBottom = yOffset + childItemHeight / 2;

                    graphics.DrawLine(connectorPen, lineX, lineTop, lineX, lineBottom);

                    // Draw horizontal line to child
                    graphics.DrawLine(connectorPen, lineX, lineBottom, childItemRect.X, lineBottom);
                }

                // Draw child image if available
                int currentX = childItemRect.X;
                if (!string.IsNullOrEmpty(childItem.ImagePath) && File.Exists(childItem.ImagePath))
                {
                    int imgSize = 18; // Smaller than parent items
                    int imgPadding = 4;

                    Rectangle imgRect = new Rectangle(
                        currentX + imgPadding,
                        yOffset + ((childItemHeight - imgSize) / 2),
                        imgSize,
                        imgSize
                    );

                    // Configure the BeepImage instance and draw it
                    _image.ImagePath = childItem.ImagePath;
                    _image.Size = new Size(imgSize, imgSize);
                    _image.ApplyThemeOnImage = true;
                    _image.Draw(graphics, imgRect);

                    // Advance the X position for text
                    currentX += imgSize + (imgPadding * 2);
                }
                else
                {
                    // If no image, add some padding
                    currentX += 8;
                }

                // Draw child text
                Rectangle textRect = new Rectangle(
                    currentX + 2, // Small text padding
                    yOffset,
                    childItemRect.Right - currentX - 4, // Right padding
                    childItemHeight
                );

                // Use slightly smaller font for child items
                Font childFont = BeepThemesManager.ToFont(_currentTheme.GetAnswerFont());
                if (childFont.Size > 8)
                {
                    childFont = new Font(childFont.FontFamily, childFont.Size - 1, childFont.Style);
                }

                // Draw text using the BeepLabel instance
                _label.Text = childItem.Text;
                _label.TextFont = childFont;
                _label.ForeColor = itemForeColor;
                _label.BackColor = itemBackColor;
                _label.TextAlign = ContentAlignment.MiddleLeft;
                _label.Draw(graphics, textRect);

                // Add hit test area for this child item
                AddHitArea(
                    $"ChildItem_{childItem.Text}_{headerItem.Children.IndexOf(childItem)}",
                    childItemRect,
                    null,
                    () => {
                        // Set as selected item
                        SelectedItem = (SimpleItem)childItem;
                        ItemClick?.Invoke(this, new BeepMouseEventArgs(childItem.Text, childItem));
                        Invalidate();
                    }
                );

                // Store the child item location for hit testing
                childItem.X = childItemRect.X;
                childItem.Y = childItemRect.Y;
                childItem.Width = childItemRect.Width;
                childItem.Height = childItemRect.Height;

                // Update yOffset for the next item
                yOffset += childItemHeight + spacing;
            }

            return yOffset;
        }

        // Helper method to draw expand/collapse icon
        private void DrawExpandCollapseIcon(Graphics graphics, Rectangle rect, bool isExpanded, Color color)
        {
            using (Pen pen = new Pen(color, 2))
            {
                // Draw the horizontal line
                graphics.DrawLine(pen,
                    rect.X + 2,
                    rect.Y + rect.Height / 2,
                    rect.X + rect.Width - 2,
                    rect.Y + rect.Height / 2);

                // If not expanded, draw vertical line to make a plus "+"
                if (!isExpanded)
                {
                    graphics.DrawLine(pen,
                        rect.X + rect.Width / 2,
                        rect.Y + 2,
                        rect.X + rect.Width / 2,
                        rect.Y + rect.Height - 2);
                }
            }
        }

        // Helper method to draw a hamburger icon
        private void DrawHamburgerIcon(Graphics g, Rectangle rect)
        {
            int barHeight = 2;
            int barWidth = 16;
            int spacing = 4;

            // Calculate the starting point to center the icon
            int startX = rect.X + (rect.Width - barWidth) / 2;
            int startY = rect.Y + (rect.Height - (3 * barHeight + 2 * spacing)) / 2;

            using (SolidBrush brush = new SolidBrush(_currentTheme?.SideMenuForeColor ?? Color.White))
            {
                // Draw 3 horizontal bars
                g.FillRectangle(brush, startX, startY, barWidth, barHeight);
                g.FillRectangle(brush, startX, startY + barHeight + spacing, barWidth, barHeight);
                g.FillRectangle(brush, startX, startY + 2 * (barHeight + spacing), barWidth, barHeight);
            }
        }

        // Override mouse events to handle hover and selection states
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Check if the mouse is over any of our hit areas
            HitTestWithMouse();

            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Let the hit test mechanism handle clicks
            if (HitTestWithMouse())
            {
                // A hit area was clicked. The action will be executed via HitTestWithMouse

                // Check if it was the toggle button that was clicked
                if (HitTestControl != null && HitTestControl.Name == "ToggleButton")
                {
                    // Handle toggle button click
                    isCollapsed = !isCollapsed;
                    StartAccordionAnimation();
                    ToggleClicked?.Invoke(this, new BeepMouseEventArgs("ToggleClicked", isCollapsed));
                }
                // Check if it was a header item or expand/collapse icon
                else if (HitTestControl != null && HitTestControl.Name.StartsWith("HeaderItem_"))
                {
                    // Get the item from the hit test
                    if (HitTestControl.HitAction != null)
                    {
                        HitTestControl.HitAction.Invoke();
                    }
                }
                // Check if it was a child item
                else if (HitTestControl != null && HitTestControl.Name.StartsWith("ChildItem_"))
                {
                    // Get the item from the hit test
                    if (HitTestControl.HitAction != null)
                    {
                        HitTestControl.HitAction.Invoke();
                    }
                }

                Invalidate();
            }
        }

        private void UpdateItemsPanelSize()
        {
            if (itemsPanel == null || toggleButton == null || logo == null) return;

            itemsPanel.Location = new Point(5, toggleButton.Bottom + 5);
            itemsPanel.Size = new Size(Width - 10, Height - toggleButton.Bottom - logo.Height - 15);
            itemsPanel.PerformLayout();
        }

        // Override ApplyTheme to apply theme colors to the control
        public override void ApplyTheme()
        {
            BackColor = _currentTheme?.SideMenuBackColor ?? Color.Empty;

            if (_label != null)
            {
                _label.Theme = Theme;
                _label.ForeColor = _currentTheme?.SideMenuForeColor ?? Color.White;
            }

            if (_button != null)
            {
                _button.Theme = Theme;
                _button.ApplyThemeOnImage = true;
                _button.ForeColor = _currentTheme?.SideMenuForeColor ?? Color.White;
            }

            if (_image != null)
            {
                _image.Theme = Theme;
            }

            if (logo != null)
            {
                logo.Theme = Theme;
                logo.ForeColor = _currentTheme?.SideMenuForeColor ?? Color.White;
            }

            if (toggleButton != null)
            {
                toggleButton.Theme = Theme;
                toggleButton.ApplyThemeOnImage = true;
                toggleButton.ForeColor = _currentTheme?.SideMenuForeColor ?? Color.White;
                toggleButton.ApplyThemeToSvg();
            }

            if (itemsPanel != null)
            {
                foreach (Control control in itemsPanel.Controls)
                {
                    if (control is BeepAccordionMenuItem menuItem)
                    {
                        menuItem.Theme = Theme;
                    }
                }
            }

            Invalidate();
        }

       
    }
}
