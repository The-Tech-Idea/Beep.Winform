using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Design;


namespace TheTechIdea.Beep.Winform.Controls
{
    // A TabControl that hides the default tab headers
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.InheritanceDemand, Name = "FullTrust")]
    [Designer(typeof(TabControlWithoutTabsDesigner))]
    [ToolboxItem(false)]
    public class TabControlWithoutTabs : TabControl
    {
        // Constructor to initialize the control
        public TabControlWithoutTabs() : base()
        {
            AllowDrop = true; // Enable drag-and-drop by default

            // Set the size of the control to match the first TabPage
            if (TabPages.Count > 0)
            {
                Size = TabPages[0].Size;
            }
        }

        // Event triggered when TabPages are added or removed
        public event EventHandler TabPagesChanged;

        // Struct for P/Invoke to hide the tab headers
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

        /// <summary>
        /// Overrides the default window procedure to hide tab headers.
        /// </summary>
        /// <param name="m">The Windows Message being processed.</param>
        protected override void WndProc(ref Message m)
        {
            const int TCM_ADJUSTRECT = 0x1328;

            if (m.Msg == TCM_ADJUSTRECT && !DesignMode)
            {
                // Return zero rect to hide the tabs
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Called when a control (TabPage) is added to the TabControl.
        /// </summary>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (e.Control is TabPage tabPage)
            {
                // Resize TabControl to match the new TabPage size
                if (!DesignMode)
                {
                    Size = tabPage.Size;
                }
            }

            // Trigger the TabPagesChanged event
            TabPagesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when a control (TabPage) is removed from the TabControl.
        /// </summary>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            // Trigger the TabPagesChanged event
            TabPagesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Allows drag-and-drop operations on the selected TabPage.
        /// </summary>
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

            // Only allow drag-and-drop if a TabPage is selected
            if (SelectedTab != null)
            {
                drgevent.Effect = DragDropEffects.Move;
            }
            else
            {
                drgevent.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Handles dropping a control onto the selected TabPage.
        /// </summary>
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            if (SelectedTab != null && drgevent.Data.GetData(typeof(Control)) is Control droppedControl)
            {
                // Add the control to the selected TabPage
                SelectedTab.Controls.Add(droppedControl);

                // Position the control at the drop point
                Point dropPoint = SelectedTab.PointToClient(new Point(drgevent.X, drgevent.Y));
                droppedControl.Location = dropPoint;
            }
        }

       
        public event EventHandler<TabPage> TabPageLoading;

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (TabPages[SelectedIndex].Tag == null) // Check if content is not yet loaded
            {
                TabPageLoading?.Invoke(this, TabPages[SelectedIndex]);
                TabPages[SelectedIndex].Tag = "Loaded"; // Mark as loaded
            }
        }

    }
    // We assume BeepButton, BeepTheme, and related classes are in your namespace.
    // using TheTechIdea.Beep.Winform.Controls; // Adjust namespace as needed
    [DefaultProperty("TabPages")]
    [ToolboxItem(true)]
    [DisplayName("Beep Tabs")]
    [Category("Beep Controls")]
    public class BeepTabs : BeepControl
    {
        private FlowLayoutPanel _headerPanel;
        private TabControlWithoutTabs _tabControl;
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
            set => _tabControl.SelectedTab = value;
        }

        public int SelectedIndex
        {
            get => _tabControl.SelectedIndex;
            set => _tabControl.SelectedIndex = value;
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

            _tabControl = new TabControlWithoutTabs
            {
                //Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                //Bounds = new Rectangle(DrawingRect.Left, _headerPanel.Height+DrawingRect.Top, DrawingRect.Width, DrawingRect.Height - _headerPanel.Height)

            };
            _tabControl.AllowDrop = true;
            // Hook into the TabPagesChanged event
            _tabControl.TabPagesChanged += (s, e) => RefreshHeaders();
            _tabControl.SelectedIndexChanged += (s, e) =>
            {
                HighlightButtonAt(_tabControl.SelectedIndex);
                OnTabSelected(_tabControl.SelectedTab, _tabControl.SelectedIndex);
            };
            Controls.Add(_tabControl);
            Controls.Add(_headerPanel);

            this.MinimumSize = new Size(200, 100);
           
        }
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

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RefreshHeaders(); // Ensure headers are built at runtime
        }

        public void RefreshHeaders()
        {
            _headerPanel.Controls.Clear();

            for (int i = 0; i < _tabControl.TabPages.Count; i++)
            {
                var page = _tabControl.TabPages[i];
                var btn = CreateTabButton(page, i);
                _headerPanel.Controls.Add(btn);
            }

            // Highlight the selected tab button
            HighlightButtonAt(_tabControl.SelectedIndex);
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
                    bool isSelected = (i == index);
                    btn.IsSelected = isSelected;
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