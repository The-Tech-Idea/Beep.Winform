using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    public enum NavigationWidgetStyle
    {
        Breadcrumb,       // Breadcrumb navigation
        StepIndicator,    // Multi-step process indicator
        TabContainer,     // Tab navigation
        Pagination,       // Page navigation
        MenuBar,          // Horizontal menu bar
        SidebarNav,       // Sidebar navigation
        WizardSteps,      // Wizard step navigation
        ProcessFlow,      // Process flow indicator
        TreeNavigation,   // Tree-Style navigation
        QuickActions      // Quick action buttons
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Navigation Widget")]
    [Category("Beep Widgets")]
    [Description("Navigation widget with multiple navigation styles.")]
    public class BeepNavigationWidget : BaseControl
    {
        #region Fields
        private NavigationWidgetStyle _style = NavigationWidgetStyle.Breadcrumb;
        private IWidgetPainter _painter;
        private List<NavigationItem> _items = new List<NavigationItem>();
        private int _currentIndex = 0;
        private Color _accentColor = Color.FromArgb(33, 150, 243);
        private Color _navItemBackColor = Color.White;
        private Color _navItemForeColor = Color.Black;
        private Color _navItemBorderColor = Color.FromArgb(200, 200, 200);
        private Color _navItemHoverBackColor = Color.FromArgb(245, 245, 245);
        private Color _navItemHoverForeColor = Color.Black;
        private Color _navItemSelectedBackColor = Color.FromArgb(33, 150, 243);
        private Color _navItemSelectedForeColor = Color.White;
        private Color _panelBackColor = Color.FromArgb(250, 250, 250);
        private Color _surfaceColor = Color.White;
        private Color _borderColor = Color.FromArgb(200, 200, 200);
        private Color _primaryColor = Color.FromArgb(33, 150, 243);
        private Color _highlightBackColor = Color.FromArgb(240, 240, 240);
        private bool _showIcons = true;
        private bool _isHorizontal = true;
        private string _title = "Navigation";

        // Events
        public event EventHandler<BeepEventDataArgs> ItemClicked;
        public event EventHandler<BeepEventDataArgs> NavigationChanged;
        #endregion

        #region Constructor
        public BeepNavigationWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            this.Size = new Size(400, 50);
            ApplyThemeToChilds = false;
            InitializeSampleItems();
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializeSampleItems()
        {
            _items.AddRange(new[]
            {
                new NavigationItem { Text = "Home", IsEnabled = true },
                new NavigationItem { Text = "Dashboard", IsEnabled = true },
                new NavigationItem { Text = "Reports", IsEnabled = true },
                new NavigationItem { Text = "Settings", IsEnabled = false }
            });
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case NavigationWidgetStyle.Breadcrumb:
                    _painter = new BreadcrumbPainter();
                    break;
                case NavigationWidgetStyle.StepIndicator:
                    _painter = new StepIndicatorPainter();
                    break;
                case NavigationWidgetStyle.TabContainer:
                    _painter = new TabContainerPainter();
                    break;
                case NavigationWidgetStyle.Pagination:
                    _painter = new PaginationPainter();
                    break;
                case NavigationWidgetStyle.MenuBar:
                    _painter = new MenuBarPainter();
                    break;
                case NavigationWidgetStyle.SidebarNav:
                    _painter = new SidebarNavPainter();
                    break;
                case NavigationWidgetStyle.WizardSteps:
                    _painter = new WizardStepsPainter();
                    break;
                case NavigationWidgetStyle.ProcessFlow:
                    _painter = new ProcessFlowPainter();
                    break;
                case NavigationWidgetStyle.TreeNavigation:
                    _painter = new TreeNavigationPainter();
                    break;
                case NavigationWidgetStyle.QuickActions:
                    _painter = new QuickActionsPainter();
                    break;
                default:
                    _painter = new BreadcrumbPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }
        #endregion

        #region Properties
        [Category("Navigation")]
        [Description("Visual Style of the navigation widget.")]
        public NavigationWidgetStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Navigation")]
        [Description("Navigation items collection.")]
        public List<NavigationItem> Items
        {
            get => _items;
            set { _items = value ?? new List<NavigationItem>(); Invalidate(); }
        }

        [Category("Navigation")]
        [Description("Current selected/active item index.")]
        public int CurrentIndex
        {
            get => _currentIndex;
            set { _currentIndex = value; Invalidate(); OnNavigationChanged(); }
        }

        [Category("Appearance")]
        [Description("Primary accent color for the navigation.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Navigation")]
        [Description("Whether to show icons in navigation items.")]
        public bool ShowIcons
        {
            get => _showIcons;
            set { _showIcons = value; Invalidate(); }
        }

        [Category("Navigation")]
        [Description("Whether navigation is laid out horizontally.")]
        public bool IsHorizontal
        {
            get => _isHorizontal;
            set { _isHorizontal = value; Invalidate(); }
        }

        [Category("Navigation")]
        [Description("Title of the navigation widget.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            var ctx = new WidgetContext
            {
                DrawingRect = DrawingRect,
                Title = _title,
                AccentColor = _accentColor,
                ShowIcon = _showIcons,
                IsInteractive = true,
                CornerRadius = BorderRadius,
                CustomData = new Dictionary<string, object>
                {
                    ["Items"] = _items,
                    ["CurrentIndex"] = _currentIndex,
                    ["IsHorizontal"] = _isHorizontal
                }
            };

            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;

            _painter?.DrawBackground(g, ctx);
            _painter?.DrawContent(g, ctx);
            _painter?.DrawForegroundAccents(g, ctx);

            RefreshHitAreas(ctx);
            _painter?.UpdateHitAreas(this, ctx, (name, rect) => { });
        }

        private void RefreshHitAreas(WidgetContext ctx)
        {
            ClearHitList();

            // Add hit areas for each navigation item
            for (int i = 0; i < _items.Count; i++)
            {
                int itemIndex = i; // Capture for closure
                AddHitArea($"Item{i}", new Rectangle(), null, () =>
                {
                    if (_items[itemIndex].IsEnabled)
                    {
                        CurrentIndex = itemIndex;
                        ItemClicked?.Invoke(this, new BeepEventDataArgs("ItemClicked", this) { EventData = _items[itemIndex] });
                    }
                });
            }
        }

        private void OnNavigationChanged()
        {
            NavigationChanged?.Invoke(this, new BeepEventDataArgs("NavigationChanged", this) { EventData = _currentIndex });
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // Apply navigation-specific theme colors
            BackColor = _currentTheme.BackColor;
            ForeColor = _currentTheme.ForeColor;
            
            // Update navigation item colors
            _navItemBackColor = _currentTheme.ButtonBackColor;
            _navItemForeColor = _currentTheme.ButtonForeColor;
            _navItemBorderColor = _currentTheme.ButtonBorderColor;
            _navItemHoverBackColor = _currentTheme.ButtonHoverBackColor;
            _navItemHoverForeColor = _currentTheme.ButtonHoverForeColor;
            _navItemSelectedBackColor = _currentTheme.ButtonSelectedBackColor;
            _navItemSelectedForeColor = _currentTheme.ButtonSelectedForeColor;
            
            // Update panel and surface colors
            _panelBackColor = _currentTheme.PanelBackColor;
            _surfaceColor = _currentTheme.SurfaceColor;
            _borderColor = _currentTheme.BorderColor;
            
            // Update accent colors
            _accentColor = _currentTheme.AccentColor;
            _primaryColor = _currentTheme.PrimaryColor;
            _highlightBackColor = _currentTheme.HighlightBackColor;
            
            InitializePainter();
            Invalidate();
        }
    }

    /// <summary>
    /// Navigation item data structure
    /// </summary>
    public class NavigationItem
    {
        public string Text { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public object Tag { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}