using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Core fields, properties, and initialization for BeepListBox
    /// Modern implementation using painter methodology
    /// </summary>
    public partial class BeepListBox : BeepPanel
    {
#pragma warning disable IL2026 // Suppress trimmer warnings for BindingList<T> used in WinForms data binding scenarios
        #region Helper and Painter
        
        /// <summary>
        /// The main helper that manages list box logic
        /// </summary>
    private BeepListBoxHelper _helper;
    private BeepListBoxLayoutHelper _layoutHelper;
    private BeepListBoxHitTestHelper _hitHelper;
        
        /// <summary>
        /// Gets the helper instance for internal use
        /// </summary>
    internal BeepListBoxHelper Helper => _helper;
    internal BeepListBoxLayoutHelper LayoutHelper => _layoutHelper;
        
        /// <summary>
        /// The painter instance (will be recreated when ListBoxType changes)
        /// </summary>
    private ListBoxs.Painters.IListBoxPainter _listBoxPainter;
    /// <summary>
    /// Returns the preferred item height from the active painter, or fallback to MenuItemHeight.
    /// Provides a safe public accessor to avoid touching private painter in helpers.
    /// </summary>
    public int PreferredItemHeight
    {
        get
        {
            if (_listBoxPainter != null)
            {
                try { return _listBoxPainter.GetPreferredItemHeight(); } catch { }
            }
            return _menuItemHeight;
        }
    }
        
        #endregion
        
        #region Core Fields
        
        // Visual style
        private ListBoxType _listBoxType = ListBoxType.Standard;
        
        // List management
    private BindingList<SimpleItem> _listItems = new BindingList<SimpleItem>();
        private SimpleItem _selectedItem;
        private List<SimpleItem> _selectedItems = new List<SimpleItem>();
        private int _selectedIndex = -1;
        
        // Search functionality
        private bool _showSearch = false;
        private string _searchText = string.Empty;
        private Rectangle _searchAreaRect;
        
        // Visual options
        private bool _showCheckBox = false;
        private bool _showImage = true;
        private bool _showHilightBox = true;
        
        // Layout caching
        private int _menuItemHeight = 32;
        private int _imageSize = 24;
        private Rectangle _contentAreaRect;
        
        // Visual state
        private bool _isHovered = false;
        private SimpleItem _hoveredItem = null;
        
        // Font
        private Font _textFont = new Font("Segoe UI", 9f);
        
        // DPI scaling
        private float _scaleFactor = 1.0f;
        
        // Checkbox tracking
        private Dictionary<SimpleItem, BeepCheckBoxBool> _itemCheckBoxes = new Dictionary<SimpleItem, BeepCheckBoxBool>();
        
        // Performance optimizations
        private Timer _delayedInvalidateTimer;
        private bool _needsLayoutUpdate = false;
        
        // Custom painter support
        private Action<Graphics, Rectangle, SimpleItem, bool, bool> _customItemRenderer;
        
        #endregion
        
        #region Events
        
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        public event EventHandler<SimpleItem> ItemClicked;
        public event EventHandler SearchTextChanged;
        
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }
        
        protected virtual void OnItemClicked(SimpleItem item)
        {
            ItemClicked?.Invoke(this, item);
        }
        
        protected virtual void OnSearchTextChanged()
        {
            SearchTextChanged?.Invoke(this, EventArgs.Empty);
        }
        
        #endregion
        
        #region Constructor
        
        public BeepListBox() : base()
        {
            // Initialize helper
            _helper = new BeepListBoxHelper(this);
            _layoutHelper = new BeepListBoxLayoutHelper(this);
            _hitHelper = new BeepListBoxHitTestHelper(this, _layoutHelper);
            
            // Initialize list
            if (_listItems == null)
            {
                _listItems = new BindingList<SimpleItem>();
            }
            _listItems.ListChanged += ListItems_ListChanged;
            
            // Set default size
            if (Width <= 0 || Height <= 0)
            {
                Width = 200;
                Height = 250;
            }
            
            // Panel settings
            ApplyThemeToChilds = false;
            CanBeSelected = false;
            CanBePressed = false;
            BorderRadius = 3;
            
            // Get DPI scaling
            using (var g = CreateGraphics())
            {
                _scaleFactor = g.DpiX / 96f;
            }
            
            // Initialize delayed invalidate timer
            _delayedInvalidateTimer = new Timer();
            _delayedInvalidateTimer.Interval = 50;
            _delayedInvalidateTimer.Tick += (s, e) =>
            {
                _delayedInvalidateTimer.Stop();
                if (_listBoxPainter != null)
                {
                    UpdateLayout();
                    _layoutHelper.CalculateLayout();
                    _hitHelper.RegisterHitAreas();
                }
                Invalidate();
            };
            
            // Enable double buffering for smooth rendering
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();
        }
#pragma warning restore IL2026
        
        private void ListItems_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            _needsLayoutUpdate = true;
            RequestDelayedInvalidate();
        }
        
        private void RequestDelayedInvalidate()
        {
            if (_delayedInvalidateTimer == null) return;
            _delayedInvalidateTimer.Stop();
            _delayedInvalidateTimer.Start();
        }
        
        #endregion
        
        #region DPI Scaling Helpers
        
        private int ScaleValue(int value)
        {
            return (int)(value * _scaleFactor);
        }
        
        private Size ScaleSize(Size size)
        {
            return new Size(ScaleValue(size.Width), ScaleValue(size.Height));
        }
        
        #endregion
        
        #region Layout Management
        
        private void UpdateLayout()
        {
            if (Width <= 0 || Height <= 0) return;
            
            var clientRect = DrawingRect;
            int currentY = DrawingRect.Top;
            
            // Search area
            if (_showSearch && _listBoxPainter != null && _listBoxPainter.SupportsSearch())
            {
                _searchAreaRect = new Rectangle(
                    clientRect.Left,
                    currentY,
                    clientRect.Width,
                    ScaleValue(36));
                currentY += _searchAreaRect.Height;
            }
            else
            {
                _searchAreaRect = Rectangle.Empty;
            }
            
            // Content area
            _contentAreaRect = new Rectangle(
                clientRect.Left,
                currentY,
                clientRect.Width,
                clientRect.Bottom - currentY);
        }
        
        #endregion
        
        #region Dispose
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_delayedInvalidateTimer != null)
                {
                    _delayedInvalidateTimer.Stop();
                    _delayedInvalidateTimer.Dispose();
                    _delayedInvalidateTimer = null;
                }
                
                if (_listItems != null)
                {
                    _listItems.ListChanged -= ListItems_ListChanged;
                }
                
                _itemCheckBoxes?.Clear();
            }
            
            base.Dispose(disposing);
        }
        
        #endregion
    }
}
