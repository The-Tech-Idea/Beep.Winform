using System.ComponentModel;

using System.Drawing.Design;



namespace TheTechIdea.Beep.Winform.Controls
{
    // A TabControl that hides the default tab headers
   // [Designer(typeof(TabControlWithoutTabsDesigner))]
    [DefaultProperty("TabPages")]
    [ToolboxItem(true)]
    [DisplayName("Beep Tabs")]
    [Category("Beep Controls")]
    [Designer(typeof(BeepTabsDesigner))]
    public class BeepTabs : BeepControl
    {
        private FlowLayoutPanel _headerPanel;
        private TabControlWithoutHeader _tabControl;
        private HeaderLocation _headerLocation = HeaderLocation.Top;
        public event EventHandler<TabEventArgs> TabSelected;
        public event EventHandler<TabEventArgs> TabButtonClicked;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public HeaderLocation HeaderLocation
        {
            get => _headerLocation;
            set
            {
                if (_headerLocation != value)
                {
                    _headerLocation = value;
                    PerformLayout(); // Trigger layout update when header location changes
                }
            }
        }

        private int _headerButtonHeight = 30;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int HeaderButtonHeight
        {
            get => _headerButtonHeight;
            set
            {
                _headerButtonHeight = value;
                RefreshHeaders();
            }
        }
        private int _headerButtonWidth = 60;
        private Rectangle rect;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int HeaderButtonWidth
        {
            get => _headerButtonWidth;
            set
            {
                _headerButtonWidth = value;
                RefreshHeaders();
            }
        }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabControl.TabPageCollection TabPages => _tabControl.TabPages;
        public TabPage SelectedTab
        {
            get => _tabControl.SelectedTab;
            set
            {
                if (value != null && _tabControl.TabPages.Contains(value))
                {
                    _tabControl.SelectedTab = value;
                    HighlightButtonAt(_tabControl.TabPages.IndexOf(value)); // Ensure UI reflects the selected tab
                    OnTabSelected(value, _tabControl.TabPages.IndexOf(value)); // Trigger the event
                }
            }
        }
        public int SelectedIndex
        {
            get => _tabControl.SelectedIndex;
            set
            {
                if (value >= 0 && value < _tabControl.TabPages.Count)
                {
                    _tabControl.SelectedIndex = value;
                    HighlightButtonAt(value); // Update header button highlights
                    OnTabSelected(_tabControl.TabPages[value], value); // Trigger the event
                }
            }
        }
        public BeepTabs()
        {
            _headerPanel = new FlowLayoutPanel
            {
                Top= DrawingRect.Top,
                Left = DrawingRect.Left,
                Width = DrawingRect.Width,
                Height = _headerButtonHeight,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = SystemColors.Control
            };

            _tabControl = new TabControlWithoutHeader
            {
                //Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                //Bounds = new Rectangle(DrawingRect.Left, _headerPanel.Height+DrawingRect.Top, DrawingRect.Width, DrawingRect.Height - _headerPanel.Height)

            };
            _tabControl.AllowDrop = true;
            // Hook into the TabPagesChanged event
            _tabControl.TabPagesChanged += (s, e) => RefreshHeaders();
            _tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            Controls.Add(_tabControl);
            Controls.Add(_headerPanel);
            this.AllowDrop = true;
            // Hook into drag-and-drop events
            //this.DragEnter += BeepTabs_DragEnter;
            //this.DragOver += BeepTabs_DragOver;
            //this.DragDrop += BeepTabs_DragDrop;
        }
        protected override Size DefaultSize => new Size(200, 200);
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Refresh the designer when the selected tab changes
            Console.WriteLine("Selected tab changed");
            //var host = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
            //if (host != null)
            //{
            //    host.Activate();
            //}
            Console.WriteLine("Designer activated");
            HighlightButtonAt(_tabControl.SelectedIndex);
            Console.WriteLine("Button highlighted");
            OnTabSelected(_tabControl.SelectedTab, _tabControl.SelectedIndex);
            Console.WriteLine("Tab selected event triggered");
            RefreshSelectedTab();
        }
        // Handle DragEnter event
        // Handle DragEnter event
        #region "Drag and Drop"
        //private void BeepTabs_DragEnter(object sender, DragEventArgs e)
        //{
        //    // Check if the dragged data is a toolbox item
        //    if (IsToolboxItem(e.Data))
        //    {
        //        e.Effect = DragDropEffects.Copy; // Allow copying the toolbox item
        //    }
        //    else
        //    {
        //        e.Effect = DragDropEffects.None; // Disallow other types of data
        //    }
        //}

        //// Handle DragOver event
        //private void BeepTabs_DragOver(object sender, DragEventArgs e)
        //{
        //    // Check if the dragged data is a toolbox item
        //    if (IsToolboxItem(e.Data))
        //    {
        //        e.Effect = DragDropEffects.Copy; // Allow copying the toolbox item
        //    }
        //    else
        //    {
        //        e.Effect = DragDropEffects.None; // Disallow other types of data
        //    }
        //}

        //// Handle DragDrop event
        //private void BeepTabs_DragDrop(object sender, DragEventArgs e)
        //{
        //    if (IsToolboxItem(e.Data))
        //    {
        //        // Get the IDesignerHost service
        //        var designerHost = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
        //        if (designerHost == null)
        //        {
        //            Console.WriteLine("Designer host is null. Cannot create component.");
        //            return;
        //        }

        //        // Get the ToolboxItem from the dragged data
        //        var toolboxItem = GetToolboxItem(e.Data);
        //        if (toolboxItem == null)
        //        {
        //            Console.WriteLine("ToolboxItem is null. Cannot create component.");
        //            return;
        //        }

        //        // Create the components from the ToolboxItem
        //        var components = toolboxItem.CreateComponents();
        //        if (components == null || components.Count() == 0)
        //        {
        //            Console.WriteLine("No components created from ToolboxItem.");
        //            return;
        //        }

        //        // Find the first Control in the created components
        //        var control = components.OfType<Control>().FirstOrDefault();
        //        if (control == null)
        //        {
        //            Console.WriteLine("No Control component created from ToolboxItem.");
        //            return;
        //        }

        //        Console.WriteLine("Control created successfully");

        //        // Add the control to the selected TabPage
        //        if (_tabControl.SelectedTab != null)
        //        {
        //            _tabControl.SelectedTab.Controls.Add(control);

        //            // Position the control at the drop point
        //            Point dropPoint = _tabControl.SelectedTab.PointToClient(new Point(e.X, e.Y));
        //            control.Location = dropPoint;

        //            Console.WriteLine("Control added to TabPage");
        //        }
        //    }
        //}

        //// Check if the dragged data is a toolbox item
        //private bool IsToolboxItem(IDataObject data)
        //{
        //    // Check for the ToolboxItem format
        //    return data.GetDataPresent(typeof(ToolboxItem)) || data.GetDataPresent("CF_TOOLBOXITEM");
        //}

        //// Get the toolbox item from the dragged data
        //private ToolboxItem GetToolboxItem(IDataObject data)
        //{
        //    if (data.GetDataPresent(typeof(ToolboxItem)))
        //    {
        //        return data.GetData(typeof(ToolboxItem)) as ToolboxItem;
        //    }
        //    else if (data.GetDataPresent("CF_TOOLBOXITEM"))
        //    {
        //        return data.GetData("CF_TOOLBOXITEM") as ToolboxItem;
        //    }
        //    return null;
        //}
        #endregion "Drag and Drop"
        // Triggered when a tab is selected
        protected virtual void OnTabSelected(TabPage selectedTab, int selectedIndex)
        {
            TabSelected?.Invoke(this, new TabEventArgs(selectedTab, selectedIndex));
        }

        // Triggered when a tab header button is clicked
        protected virtual void OnTabButtonClicked(TabPage clickedTab, int clickedIndex)
        {
            TabButtonClicked?.Invoke(this, new TabEventArgs(clickedTab, clickedIndex));
        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            Padding = new Padding(2);
            rect = DrawingRect;
            if (rect == Rectangle.Empty) return;

            switch (_headerLocation)
            {
                case HeaderLocation.Top:
                    _headerPanel.SetBounds(rect.X, rect.Y, rect.Width, _headerButtonHeight);
                    _tabControl.SetBounds(rect.X, rect.Y + _headerButtonHeight, rect.Width, rect.Height - _headerButtonHeight);
                    _headerPanel.FlowDirection = FlowDirection.LeftToRight;
                    break;

                case HeaderLocation.Bottom:
                    _tabControl.SetBounds(rect.X, rect.Y, rect.Width, rect.Height - _headerButtonHeight);
                    _headerPanel.SetBounds(rect.X, rect.Y + _tabControl.Height, rect.Width, _headerButtonHeight);
                    _headerPanel.FlowDirection = FlowDirection.LeftToRight;
                    break;

                case HeaderLocation.Left:
                    _headerPanel.SetBounds(rect.X, rect.Y, _headerButtonWidth, rect.Height);
                    _tabControl.SetBounds(rect.X + _headerButtonWidth, rect.Y, rect.Width - _headerButtonWidth, rect.Height);
                    _headerPanel.FlowDirection = FlowDirection.TopDown;
                    break;

                case HeaderLocation.Right:
                    _tabControl.SetBounds(rect.X, rect.Y, rect.Width - _headerButtonWidth, rect.Height);
                    _headerPanel.SetBounds(rect.Right - _headerButtonWidth, rect.Y, _headerButtonWidth, rect.Height);
                    _headerPanel.FlowDirection = FlowDirection.TopDown;
                    break;
            }
        }
        private void RefreshSelectedTab()
        {
            Console.WriteLine("Refreshing selected tab");
            HighlightButtonAt(_tabControl.SelectedIndex);
            Console.WriteLine("Button highlighted");
            _headerPanel.Invalidate(); // Redraw headers
            Console.WriteLine("Headers invalidated");
            _tabControl.Invalidate(); // Redraw tab content
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Console.WriteLine("Handle created");
            RefreshHeaders(); // Ensure headers are built at runtime
        }
        public void RefreshHeaders()
        {
            Console.WriteLine("Refreshing headers");
            _headerPanel.Controls.Clear();
            Console.WriteLine("Controls cleared");
            for (int i = 0; i < _tabControl.TabPages.Count; i++)
            {
                Console.WriteLine("Creating button for tab " + i);
                var page = _tabControl.TabPages[i];
                var btn = CreateTabButton(page, i);
                _headerPanel.Controls.Add(btn);
            }
            Console.WriteLine("Headers refreshed");
            // Highlight the selected tab button
            HighlightButtonAt(_tabControl.SelectedIndex);
            Console.WriteLine("Button highlighted");
        }
        private BeepButton CreateTabButton(TabPage page, int index)
        {
            int maxTextWidth = GetMaxTextWidth() + 20; // Include padding for horizontal alignment

            var btn = new BeepButton
            {
                Text = page.Text,
                Margin = new Padding(2),
                SavedGuidID = index.ToString(),
                HideText = false,
                IsChild = false,
                ShowAllBorders = true,
                ShowShadow = false,
                IsRounded = false,
                IsRoundedAffectedByTheme = false,
                IsBorderAffectedByTheme = true,
                IsShadowAffectedByTheme = false,
                UseScaledFont = true,
                Id = index,
                IsSelectedAuto = false,
                ImagePath = "", // Optionally set an image path
                Size = (_headerLocation == HeaderLocation.Left || _headerLocation == HeaderLocation.Right)
                    ? new Size(_headerButtonWidth, _headerButtonHeight) // Consistent size for vertical headers
                    : new Size(maxTextWidth, _headerButtonHeight), // Consistent size for horizontal headers
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageBeforeText // Text remains horizontal
            };

            btn.Click += (s, e) => SelectTab(index);

            // Apply theme if applicable
            btn.Theme = Theme;

            return btn;
        }
        public void SelectTab(int index)
        {
            if (index >= 0 && index < _tabControl.TabPages.Count)
            {
                _tabControl.SelectedIndex = index;
                HighlightButtonAt(index);
                OnTabSelected(_tabControl.TabPages[index], index);
            }
        }
        private void HighlightButtonAt(int index)
        {
            for (int i = 0; i < _headerPanel.Controls.Count; i++)
            {
                if (_headerPanel.Controls[i] is BeepButton btn)
                {
                    Console.WriteLine("Highlighting button " + i);
                    btn.IsSelected = btn.Id == index;
                    btn.Invalidate();
                }
            }
        }
        private int GetMaxTextWidth()
        {
            return _tabControl.TabPages.Cast<TabPage>()
                .Max(page => TextRenderer.MeasureText(page.Text, Font).Width);
        }
        public override void ApplyTheme()
        {
            if (Theme != null)
            {
                this.BackColor = _currentTheme.BackgroundColor;
                _headerPanel.BackColor = _currentTheme.ButtonBackColor; // Or another suitable property

                // Re-apply theme to all buttons
                foreach (Control ctrl in _headerPanel.Controls)
                {
                    if (ctrl is BeepButton btn)
                    {
                        btn.Theme = Theme;
                        btn.ApplyTheme();
                    }
                }
            }
            else
            {
                // If no theme, revert to defaults
                this.BackColor = SystemColors.Control;
                _headerPanel.BackColor = SystemColors.Control;
            }

            Invalidate();
        }
    }
    public enum HeaderLocation
    {
        Top,
        Bottom,
        Left,
        Right
    }
    // Custom event args for Tab Control
    public class TabEventArgs : EventArgs
    {
        public TabEventArgs(TabPage tabPage, int tabIndex)
        {
            TabPage = tabPage;
            TabIndex = tabIndex;
        }

        public TabPage TabPage { get; }
        public int TabIndex { get; }
    }

}