using TheTechIdea.Beep.Winform.Controls.Accessibility;
using TheTechIdea.Beep.Winform.Controls.Backstage;
using TheTechIdea.Beep.Winform.Controls.Rendering;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeepRibbonControl"/> class.
        /// Sets up all controls, event handlers, accessibility helpers, and applies initial theme.
        /// </summary>
        public BeepRibbonControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Controls.Add(_tabs);
            Controls.Add(_contextHeader);
            Controls.Add(_quickAccess);
            Height = 120;

            _commandItems.ListChanged += CommandItems_ListChanged;
            _backstageItems.ListChanged += BackstageItems_ListChanged;
            _backstageRecentItems.ListChanged += BackstageItems_ListChanged;
            _backstagePinnedItems.ListChanged += BackstageItems_ListChanged;
            _backstageFooterItems.ListChanged += BackstageFooterItems_ListChanged;

            // Backstage setup
            _backstageButton = new ToolStripDropDownButton("File") { ShowDropDownArrow = true, AutoToolTip = false };
            _backstageDropDown = new ToolStripDropDown { Padding = Padding.Empty, AutoClose = true, AutoSize = false };
            InitializeBackstageLayout();
            _backstageHost = new ToolStripControlHost(_backstagePanelContent) { AutoSize = false, Size = _backstagePanelContent.Size, Margin = Padding.Empty, Padding = Padding.Empty };
            _backstageDropDown.Items.Add(_backstageHost);
            _backstageButton.DropDownOpening += BackstageButton_DropDownOpening;
            _backstageDropDown.Closed += BackstageDropDown_Closed;
            _backstageButton.DropDown = _backstageDropDown;
            _quickAccess.Items.Insert(0, _backstageButton);
            InitializeSearchControls();

            _quickAccess.Renderer = new BeepRibbonToolStripRenderer(this);
            _contextHeader.Paint += ContextHeader_Paint;
            _tabs.ControlAdded += (_, __) => _contextHeader.Invalidate();
            _tabs.ControlRemoved += (_, __) => _contextHeader.Invalidate();
            _tabs.SelectedIndexChanged += (_, __) =>
            {
                _contextHeader.Invalidate();
                RefreshKeyTipsVisibility();
            };
            _tabs.SelectedIndexChanged += Tabs_SelectedIndexChanged;
            _tabs.MouseUp += Tabs_MouseUp;
            _tabs.MouseDoubleClick += Tabs_MouseDoubleClick;

            _keyboardMap.Register(Keys.F6, () => FocusRibbonPane(1));
            _keyboardMap.Register(Keys.Shift | Keys.F6, () => FocusRibbonPane(-1));
            _contextTransitionTimer.Tick += ContextTransitionTimer_Tick;
            _backstageTransitionTimer.Tick += BackstageTransitionTimer_Tick;

            RibbonAccessibilityHelper.ApplyControlAccessibility(_quickAccess, "Quick Access Toolbar", "Primary ribbon quick access commands.", AccessibleRole.ToolBar);
            RibbonAccessibilityHelper.ApplyControlAccessibility(_tabs, "Ribbon Tabs", "Ribbon tabs and command groups.", AccessibleRole.PageTabList);
            RibbonAccessibilityHelper.ApplyControlAccessibility(_backstageNavList, "Backstage Navigation", "Backstage section list.", AccessibleRole.Outline);
            RibbonAccessibilityHelper.ApplyControlAccessibility(_backstageActions, "Backstage Actions", "Actions available for the selected backstage section.", AccessibleRole.Pane);
            RibbonAccessibilityHelper.ApplyControlAccessibility(_backstageFooter, "Backstage Footer Actions", "Footer commands such as options and account actions.", AccessibleRole.ToolBar);

            ApplyRightToLeftLayout();
            ApplyQuickAccessPlacement();
            ApplySearchAccessibility();
            ApplyPaneTabOrder();
            TrySubscribeThemeManager();
            if (_followGlobalFormStyle)
            {
                SyncWithGlobalThemeAndStyle();
            }
            else
            {
                ApplyTheme();
            }
        }

        /// <summary>
        /// Handles disposal of the control and unsubscribes from theme manager events.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _commandItems.ListChanged -= CommandItems_ListChanged;
                _backstageItems.ListChanged -= BackstageItems_ListChanged;
                _backstageRecentItems.ListChanged -= BackstageItems_ListChanged;
                _backstagePinnedItems.ListChanged -= BackstageItems_ListChanged;
                _backstageFooterItems.ListChanged -= BackstageFooterItems_ListChanged;
                _backstageNavList.SelectedIndexChanged -= BackstageNavList_SelectedIndexChanged;
                _backstageActions.SizeChanged -= BackstageActions_SizeChanged;
                _backstageButton.DropDownOpening -= BackstageButton_DropDownOpening;
                _backstageDropDown.Closed -= BackstageDropDown_Closed;
                _tabs.MouseDoubleClick -= Tabs_MouseDoubleClick;
                _tabs.MouseUp -= Tabs_MouseUp;
                _tabs.SelectedIndexChanged -= Tabs_SelectedIndexChanged;
                UnsubscribeThemeManager();
                _contextTransitionTimer.Stop();
                _contextTransitionTimer.Tick -= ContextTransitionTimer_Tick;
                _contextTransitionTimer.Dispose();
                _backstageTransitionTimer.Stop();
                _backstageTransitionTimer.Tick -= BackstageTransitionTimer_Tick;
                _backstageTransitionTimer.Dispose();
                _keyboardMap.Clear();
                HideKeyTips();
                HideMinimizedPopup();
                _minimizedTabPopup.Closed -= MinimizedTabPopup_Closed;
                _minimizedTabPopup.Dispose();
                _superTooltip.Dispose();
                ClearBackstageActions();
                ClearBackstageFooterActions();
                DisposeMinimizedImages();
                DisposeGeneratedImages();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Refreshes the command view and controls after configuration changes.
        /// Rebuilds UI elements affected by density, layout mode, or appearance changes.
        /// </summary>
        private void RefreshCommandView()
        {
            if (_suspendCommandRebuild)
            {
                return;
            }
            BuildFromSimpleItems();
            if (_isMinimized)
            {
                ApplyMinimizedState();
            }
        }

        /// <summary>
        /// Ensures search controls are visible and properly configured based on search mode.
        /// </summary>
        private void EnsureSearchControls()
        {
            _searchBox.Visible = _searchMode != RibbonSearchMode.Off;
            _searchResultsButton.Visible = _searchMode != RibbonSearchMode.Off;
            ApplySearchAccessibility();
        }

        /// <summary>
        /// Gets or sets the pane focus based on the direction index.
        /// Handles F6/Shift+F6 keyboard navigation between panes.
        /// </summary>
        private void FocusRibbonPane(int direction)
        {
            // Focus cycling between ribbon panes
            if (_tabs.TabCount > 0)
            {
                int nextIndex = (_tabs.SelectedIndex + direction) % _tabs.TabCount;
                if (nextIndex < 0) nextIndex += _tabs.TabCount;
                _tabs.SelectedIndex = nextIndex;
                _tabs.Focus();
            }
        }

        /// <summary>
        /// Applies pane tab order for keyboard navigation.
        /// </summary>
        private void ApplyPaneTabOrder()
        {
            _quickAccess.TabIndex = 0;
            _tabs.TabIndex = 1;
            _contextHeader.TabIndex = 2;
        }
    }
}
