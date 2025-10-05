using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.Models;


namespace TheTechIdea.Beep.Winform.Controls.NavBars
{
    /// <summary>
    /// BeepNavBar - Modern painter-based navigation bar control (horizontal)
    /// Base class for TopNavBar and BottomNavBar
    /// Follows the same architecture pattern as BeepSideBar
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepNavBar))]
    [Category("Beep Controls")]
    [Description("Modern navigation bar control with multiple visual styles using painters.")]
    [DisplayName("Beep Nav Bar")]
    public partial class BeepNavBar : BaseControl
    {
        #region Fields
        private BindingList<SimpleItem> _items = new BindingList<SimpleItem>();
        private SimpleItem _selectedItem;
        private int _itemWidth = 80;
        private int _itemHeight = 48;
        private NavBarOrientation _orientation = NavBarOrientation.Horizontal;
        #endregion

        #region Events
        public event Action<SimpleItem> ItemClicked;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructor
        public BeepNavBar()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
            
            Height = _itemHeight;
            Dock = DockStyle.Top;
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
        [Description("The list of navigation bar items.")]
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
        [Category("Layout")]
        [Description("Width of each navigation item (for horizontal layout).")]
        [DefaultValue(80)]
        public int ItemWidth
        {
            get => _itemWidth;
            set
            {
                _itemWidth = value;
                RefreshHitAreas();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Height of each navigation item.")]
        [DefaultValue(48)]
        public int ItemHeight
        {
            get => _itemHeight;
            set
            {
                _itemHeight = value;
                if (_orientation == NavBarOrientation.Horizontal)
                    Height = _itemHeight;
                RefreshHitAreas();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Orientation of the navigation bar.")]
        [DefaultValue(NavBarOrientation.Horizontal)]
        public NavBarOrientation Orientation
        {
            get => _orientation;
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    RefreshHitAreas();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Accent color for highlights and indicators.")]
        public Color AccentColor { get; set; } = Color.FromArgb(0, 120, 215);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use theme colors instead of custom accent color.")]
        [DefaultValue(true)]
        public bool UseThemeColors { get; set; } = true;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Enable shadow effects.")]
        [DefaultValue(true)]
        public bool EnableShadow { get; set; } = true;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Corner radius for rounded elements.")]
        [DefaultValue(8)]
        public int CornerRadius { get; set; } = 8;

        [Browsable(false)]
        public int NavItemHeight => _itemHeight;

        [Browsable(false)]
        public int NavItemWidth => _itemWidth;
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
        /// </summary>
        private void RecalculateItemSizes()
        {
            if (_items == null || _items.Count == 0)
                return;

            using (var g = CreateGraphics())
            {
                int maxWidth = 80; // Minimum width
                int maxHeight = 48; // Minimum height
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
                        using (var font = new Font("Segoe UI", 9f, FontStyle.Regular))
                        {
                            textSize = TextRenderer.MeasureText(g, item.Text, font,
                                new Size(int.MaxValue, int.MaxValue),
                                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
                        }
                    }

                    if (_orientation == NavBarOrientation.Horizontal)
                    {
                        // Horizontal: icon above text, stacked vertically
                        int requiredWidth;
                        int requiredHeight;

                        if (hasIcon && hasText)
                        {
                            // Width = max(icon, text) + padding
                            requiredWidth = Math.Max(iconSize, textSize.Width) + padding * 2;
                            // Height = icon + text + spacing between + padding
                            requiredHeight = iconSize + textSize.Height + padding * 3;
                        }
                        else if (hasIcon)
                        {
                            // Only icon
                            requiredWidth = iconSize + padding * 2;
                            requiredHeight = iconSize + padding * 2;
                        }
                        else
                        {
                            // Only text
                            requiredWidth = textSize.Width + padding * 2;
                            requiredHeight = textSize.Height + padding * 2;
                        }
                        
                        maxWidth = Math.Max(maxWidth, requiredWidth);
                        maxHeight = Math.Max(maxHeight, requiredHeight);
                    }
                    else
                    {
                        // Vertical: icon left, text right, side by side
                        int requiredWidth;
                        int requiredHeight;

                        if (hasIcon && hasText)
                        {
                            // Width = icon + spacing + text + padding
                            requiredWidth = iconSize + textSize.Width + padding * 3;
                            // Height = max(icon, text) + padding
                            requiredHeight = Math.Max(iconSize, textSize.Height) + padding * 2;
                        }
                        else if (hasIcon)
                        {
                            // Only icon
                            requiredWidth = iconSize + padding * 2;
                            requiredHeight = iconSize + padding * 2;
                        }
                        else
                        {
                            // Only text
                            requiredWidth = textSize.Width + padding * 2;
                            requiredHeight = textSize.Height + padding * 2;
                        }
                        
                        maxWidth = Math.Max(maxWidth, requiredWidth);
                        maxHeight = Math.Max(maxHeight, requiredHeight);
                    }
                }

                // Update sizes (add a bit of extra space for safety)
                _itemWidth = maxWidth + 8;
                _itemHeight = maxHeight + 4;

                // Update control height for horizontal navbar
                if (_orientation == NavBarOrientation.Horizontal)
                {
                    Height = _itemHeight;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Selects an item by index (used by painters)
        /// </summary>
        internal void SelectNavItemByIndex(int index)
        {
            if (_items != null && index >= 0 && index < _items.Count)
            {
                OnItemClicked(_items[index]);
            }
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Cleanup if needed
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    /// <summary>
    /// Orientation options for BeepNavBar
    /// </summary>
    public enum NavBarOrientation
    {
        Horizontal = 0,
        Vertical = 1
    }
}
