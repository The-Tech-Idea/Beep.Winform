using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    /// <summary>
    /// BeepSideBar - Modern painter-based sidebar control
    /// Clean implementation using the painter architecture
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepSideBar))]
    [Category("Beep Controls")]
    [Description("Modern sidebar control with multiple visual styles using painters.")]
    [DisplayName("Beep Side Bar")]
    public partial class BeepSideBar : BaseControl
    {
        #region Fields
        // Internal fields (exposed to partial classes)
        internal BindingList<SimpleItem> _items = new BindingList<SimpleItem>();
        internal SimpleItem _selectedItem;
        internal bool _isCollapsed = false;
        internal int _expandedWidth = 200;
        internal int _collapsedWidth = 64;
        internal int _itemHeight = 44;
        #endregion

        #region Events
        public event Action<SimpleItem> ItemClicked;
        public event Action<bool> CollapseStateChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public BeepSideBar()
        {
            InitializeComponent();
            InitializePainter();
            InitializeAnimation();
            
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
            
            Width = _expandedWidth;
            Dock = DockStyle.Left;
            TabStop = true;
        }

        private void InitializeComponent()
        {
            // Subscribe to list changes
            _items.ListChanged += Items_ListChanged;
        }
        #endregion

        #region Public Properties

        [Browsable(true)]
        [Category("Data")]
        [Description("The list of sidebar menu items.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        public BindingList<SimpleItem> Items
        {
            get => _items;
            set
            {
                if (_items != null)
                    _items.ListChanged -= Items_ListChanged;
                
                _items = value ?? new BindingList<SimpleItem>();
                _items.ListChanged += Items_ListChanged;
                
                RefreshHitAreas();
                Invalidate();
            }
        }

        [Browsable(false)]
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether the sidebar is collapsed.")]
        [DefaultValue(false)]
        public bool IsCollapsed
        {
            get => _isCollapsed;
            set
            {
                if (_isCollapsed != value)
                {
                    _isCollapsed = value;
                    OnIsCollapsedChanging(value);
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Width when expanded.")]
        [DefaultValue(200)]
        public int ExpandedWidth
        {
            get => _expandedWidth;
            set
            {
                _expandedWidth = value;
                if (!_isCollapsed)
                    Width = _expandedWidth;
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Width when collapsed.")]
        [DefaultValue(64)]
        public int CollapsedWidth
        {
            get => _collapsedWidth;
            set
            {
                _collapsedWidth = value;
                if (_isCollapsed)
                    Width = _collapsedWidth;
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Height of each menu item.")]
        [DefaultValue(44)]
        public int ItemHeight
        {
            get => _itemHeight;
            set
            {
                _itemHeight = value;
                RefreshHitAreas();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Accent color for highlights and indicators.")]
        public Color AccentColor { get; set; } = Color.FromArgb(0, 120, 215);

        private bool _useThemeColors = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use theme colors instead of custom accent color.")]
        [DefaultValue(true)]
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set
            {
                _useThemeColors = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Enable shadow effects.")]
        [DefaultValue(true)]
        public bool EnableRailShadow { get; set; } = true;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Corner radius for rounded elements.")]
        [DefaultValue(10)]
        public int ChromeCornerRadius { get; set; } = 10;

        [Browsable(false)]
        public int MenuItemHeight => _itemHeight;
        #endregion

        #region Protected Methods
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RefreshHitAreas();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnItemClicked(SimpleItem item)
        {
            SelectedItem = item;
            ItemClicked?.Invoke(item);
        }
        #endregion

        #region Private Methods
        private void Items_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            RecalculateItemSizes();
            RefreshHitAreas();
            Invalidate();
        }

        /// <summary>
        /// Calculate required item sizes based on text and icon content to avoid clipping
        /// Sidebar is always vertical, so icon is on left, text on right
        /// </summary>
        private void RecalculateItemSizes()
        {
            if (_items == null || _items.Count == 0)
                return;

            using (var g = CreateGraphics())
            {
                int maxHeight = 44; // Minimum height
                const int iconSize = 24;
                const int padding = 12;

                foreach (var item in _items)
                {
                    bool hasIcon = !string.IsNullOrEmpty(item.ImagePath);
                    bool hasText = !string.IsNullOrEmpty(item.Text);

                    if (!hasIcon && !hasText)
                        continue;

                    // Measure text if present
                    Size textSize = Size.Empty;
                    if (hasText)
                    {
                        using (var font = new Font("Segoe UI", 10f, FontStyle.Regular))
                        {
                            textSize = TextRenderer.MeasureText(g, item.Text, font,
                                new Size(int.MaxValue, int.MaxValue),
                                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
                        }
                    }

                    // Sidebar is always vertical: icon left, text right
                    int requiredHeight;

                    if (hasIcon && hasText)
                    {
                        // Both icon and text - height = max(icon, text) + padding
                        requiredHeight = Math.Max(iconSize, textSize.Height) + padding * 2;
                    }
                    else if (hasIcon)
                    {
                        // Only icon
                        requiredHeight = iconSize + padding * 2;
                    }
                    else
                    {
                        // Only text
                        requiredHeight = textSize.Height + padding * 2;
                    }

                    maxHeight = Math.Max(maxHeight, requiredHeight);
                }

                // Update item height (add a bit of extra space for safety)
                _itemHeight = maxHeight + 4;
            }
        }

        /// <summary>
        /// Partial method for animation support - implemented in BeepSideBar.Animation.cs
        /// </summary>
        partial void OnIsCollapsedChanging(bool newValue);
        #endregion

        #region Public Methods
        /// <summary>
        /// Toggles the collapse state
        /// </summary>
        public void Toggle()
        {
            IsCollapsed = !IsCollapsed;
        }

        /// <summary>
        /// Selects an item by index (used by painters)
        /// </summary>
        internal void SelectMenuItemByIndex(int index)
        {
            if (_items != null && index >= 0 && index < _items.Count)
            {
                OnItemClicked(_items[index]);
            }
        }
        #endregion

        #region Dispose
        partial void DisposeAnimation();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeAnimation();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
