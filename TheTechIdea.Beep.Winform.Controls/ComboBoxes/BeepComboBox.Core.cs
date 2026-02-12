using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;
using TheTechIdea.Beep.Winform.Controls.ContextMenus;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Core fields, properties, and initialization for BeepComboBox
    /// Modern implementation using painter methodology and BaseControl features
    /// </summary>
#pragma warning disable IL2026 // Suppress trimmer warnings for BindingList<T> used in WinForms data binding scenarios
    public partial class BeepComboBox : BaseControl
    {
        #region Helper and Painter
        
        /// <summary>
        /// The main helper that manages combo box logic
        /// </summary>
        private BeepComboBoxHelper _helper;
        
        /// <summary>
        /// Gets the helper instance for internal use
        /// </summary>
        internal BeepComboBoxHelper Helper => _helper;
        
        /// <summary>
        /// The painter instance (will be recreated when ComboBoxType changes)
        /// </summary>
        private IComboBoxPainter _comboBoxPainter;
        
        /// <summary>
        /// Gets whether the dropdown button is currently hovered (for painters)
        /// </summary>
        internal bool IsButtonHovered => _isButtonHovered;
        internal bool IsControlHovered => _isHovered;
        
        #endregion
        
        #region Core Fields
        
        // Visual Style
        private ComboBoxType _comboBoxType = ComboBoxType.Standard;
        
        // List management
        private BindingList<SimpleItem> _listItems = new BindingList<SimpleItem>();
        private SimpleItem _selectedItem;
        private int _selectedItemIndex = -1;
        
        // Dropdown state (uses inherited BeepContextMenu from BaseControl)
        private bool _isDropdownOpen = false;
        
        // Text and editing
        private string _inputText = string.Empty;
        private bool _isEditing = false;
        
        // Layout caching
        private Rectangle _textAreaRect;
        private Rectangle _dropdownButtonRect;
        private Rectangle _imageRect;
        private float _cachedScaleX = -1f;
        private float _cachedScaleY = -1f;
        
        // Layout cache invalidation tracking
        private int _cachedWidth;
        private int _cachedHeight;
        private Padding _cachedInnerPadding;
        private string _cachedLeadingImagePath;
        private string _cachedLeadingIconPath;
        private int _cachedDropdownButtonWidth;
        private Font _cachedTextFont;
        private bool _layoutCacheValid = false;
        private bool _dropdownButtonWidthSetExplicitly = false;
        private bool _innerPaddingSetExplicitly = false;
        private bool _layoutDefaultsInitialized = false;
        private const int DefaultDropdownButtonWidthLogical = 32;
        private static readonly Padding DefaultInnerPaddingLogical = new Padding(8, 4, 8, 4);

        // Chip animations: track progress for chips when adding/removing
        private readonly System.Collections.Generic.Dictionary<SimpleItem, float> _chipProgress = new System.Collections.Generic.Dictionary<SimpleItem, float>();
        private readonly System.Collections.Generic.Dictionary<SimpleItem, bool> _chipAnimatingIn = new System.Collections.Generic.Dictionary<SimpleItem, bool>();
        private Timer _chipAnimationTimer;
        
        // Visual state
        private bool _isHovered = false;
        private bool _isButtonHovered = false;
        private string _dropdownIconPath = "";
        
        // Font
        private Font _textFont = new Font("Segoe UI", 9f);
        
        // Performance optimizations
        private Timer _delayedInvalidateTimer;
        private bool _needsLayoutUpdate = false;
        
        // Loading state
        private bool _isLoading = false;
        private Timer _loadingAnimationTimer;
        private float _loadingRotationAngle = 0f;
        
        // Auto-complete debouncing
        private Timer _autoCompleteDelayTimer;
        
        #endregion
        
        #region Events
        
        public event EventHandler<SelectedItemChangedEventArgs> SelectedIndexChanged;
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        public event EventHandler PopupOpened;
        public event EventHandler PopupClosed;
        
        #endregion
        
        #region Constructor
        
        public BeepComboBox():base()
        {
            // Set control styles
            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;
            DoubleBuffered = true;
            
            // Initialize helper
            _helper = new BeepComboBoxHelper(this);
            
            // Initialize delayed invalidate timer
            _delayedInvalidateTimer = new Timer { Interval = 50 };
            _delayedInvalidateTimer.Tick += (s, e) =>
            {
                _delayedInvalidateTimer.Stop();
                if (_needsLayoutUpdate)
                {
                    UpdateLayout();
                    _needsLayoutUpdate = false;
                }
                Invalidate();
            };
            
            // Set default properties
            Size = new Size(200, 40);
            BorderRadius = 4;
            ShowAllBorders = true;
            ApplyLayoutDefaultsFromPainter();
            
            // Set accessibility properties
            AccessibleRole = AccessibleRole.ComboBox;
            if (string.IsNullOrEmpty(AccessibleName))
            {
                AccessibleName = "Combo box";
            }
            if (string.IsNullOrEmpty(AccessibleDescription))
            {
                AccessibleDescription = "Select an item from the dropdown list";
            }
            
            // Initialize context menu (using inherited BeepContextMenu from BaseControl)
            InitializeContextMenu();
            
            // Set dropdown icon
            SetDropdownIcon();
            
            // Apply theme
            ApplyTheme();
            
            // Wire up events
            GotFocus += OnControlGotFocus;
            LostFocus += OnControlLostFocus;
            Click += OnControlClick;

            // Chip animations timer
            _chipAnimationTimer = new Timer { Interval = 16 }; // ~60 FPS
            _chipAnimationTimer.Tick += (s, e) =>
            {
                bool any = false;
                float step = 16f / Math.Max(1f, ChipAnimationDuration);
                var keys = new System.Collections.Generic.List<SimpleItem>(_chipProgress.Keys);
                foreach (var key in keys)
                {
                    bool animIn = _chipAnimatingIn.ContainsKey(key) && _chipAnimatingIn[key];
                    float p = _chipProgress[key];
                    p += animIn ? step : -step;
                    // Apply easing from theme, if available
                    var themeEasing = _currentTheme?.AnimationEasingFunction; // helper owner might not be present, fallback
                    // No concept of _helper.Owner as helper has the owner; fallback to use BeepControl easing if needed
                    // Evaluate easing on the resulting p later in rendering; here we keep linear progress
                    p = Math.Max(0f, Math.Min(1f, p));
                    _chipProgress[key] = p;
                    if (p <= 0f && !animIn)
                    {
                        _chipProgress.Remove(key);
                        _chipAnimatingIn.Remove(key);
                    }
                    else if (p >= 1f && animIn)
                    {
                        // animation finished
                        _chipAnimatingIn[key] = true; // keep it flagged as in
                    }
                    any = any || p > 0f && p < 1f || (_chipProgress.Count > 0);
                }
                if (any || _chipProgress.Count > 0) Invalidate();
            };
        }
        
        #endregion
        
        #region Initialization Methods
        
        private void InitializeContextMenu()
        {
            // Initialize the inherited BeepContextMenu from BaseControl
            if (BeepContextMenu == null)
            {
                BeepContextMenu = new BeepContextMenu();
            }
            

            BeepContextMenu.ShowImage = true;
            BeepContextMenu.ShowCheckBox = AllowMultipleSelection;
            // If this combo box type is searchable, show the search box in the dropdown
            BeepContextMenu.ShowSearchBox = (ComboBoxType == ComboBoxType.SearchableDropdown) || ShowSearchInDropdown;
            BeepContextMenu.ShowSeparators = false;
            BeepContextMenu.ContextMenuType = FormStyle.Modern;
            // Ensure theme and lifecycle are aligned with the control
            BeepContextMenu.Theme = this.Theme;
            BeepContextMenu.DestroyOnClose = false; // reuse for dropdown
            SyncDropdownMetrics();
            
            // Wire up events
            BeepContextMenu.ItemClicked += OnContextMenuItemClicked;
            BeepContextMenu.MenuClosing += (s, e) =>
            {
                _isDropdownOpen = false;
                PopupClosed?.Invoke(this, EventArgs.Empty);
                Invalidate();
            };
        }
        
        private void SetDropdownIcon()
        {
            // Use a standard dropdown arrow icon
            // This should be replaced with actual icon path from your icon system
            _dropdownIconPath = "dropdown_arrow"; // Placeholder
        }
        
        #endregion
        
        #region Event Handlers (Internal)
        
        private void OnControlGotFocus(object sender, EventArgs e)
        {
            Invalidate();
        }
        
        private void OnControlLostFocus(object sender, EventArgs e)
        {
            _isEditing = false;
            Invalidate();
        }
        
        private void OnControlClick(object sender, EventArgs e)
        {
            // If clicked on text area and editable, start editing
            // Otherwise handled by hit areas
        }
        
        private void OnContextMenuItemClicked(object sender, MenuItemEventArgs e)
        {
            if (AllowMultipleSelection || (BeepContextMenu != null && BeepContextMenu.MultiSelect))
            {
                // Toggle selection in selected items
                var selectedList = SelectedItems ?? new System.Collections.Generic.List<SimpleItem>();
                if (selectedList.Contains(e.Item))
                {
                    selectedList.Remove(e.Item);
                }
                else
                {
                    selectedList.Add(e.Item);
                }
                SelectedItems = selectedList;
                SelectedItemsChanged?.Invoke(this, EventArgs.Empty);
                // Do not close dropdown when MultiSelect is true unless context menu is configured to close
                if (BeepContextMenu != null && BeepContextMenu.CloseOnItemClick)
                {
                    CloseDropdown();
                }
            }
            else
            {
                SelectedItem = e.Item;
                CloseDropdown();
            }
        }
        
        #endregion
        
        #region Protected Event Raisers
        
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            var args = new SelectedItemChangedEventArgs(selectedItem);
            SelectedItemChanged?.Invoke(this, args);
            SelectedIndexChanged?.Invoke(this, args);
            RaiseSubmitChanges();
        }
        
        #endregion
        
        #region Layout Management

        internal int ScaleLogicalX(int logicalPixels)
        {
            var scaleX = _dpiScaleX > 0f ? _dpiScaleX : 1f;
            return DpiScalingHelper.ScaleValue(logicalPixels, scaleX);
        }

        internal int ScaleLogicalY(int logicalPixels)
        {
            var scaleY = _dpiScaleY > 0f ? _dpiScaleY : 1f;
            return DpiScalingHelper.ScaleValue(logicalPixels, scaleY);
        }

        internal Padding ScaleLogicalPadding(Padding logicalPadding)
        {
            return new Padding(
                ScaleLogicalX(logicalPadding.Left),
                ScaleLogicalY(logicalPadding.Top),
                ScaleLogicalX(logicalPadding.Right),
                ScaleLogicalY(logicalPadding.Bottom));
        }

        private void ApplyLayoutDefaultsFromPainter(bool force = false)
        {
            _comboBoxPainter ??= CreatePainter(_comboBoxType);
            _comboBoxPainter.Initialize(this, _currentTheme);

            // Only apply defaults if not initialized yet, or if forced (e.g., ComboBoxType change)
            if (!_layoutDefaultsInitialized || force)
            {
                if (!_dropdownButtonWidthSetExplicitly)
                {
                    int preferredButtonWidth = Math.Max(18, _comboBoxPainter.GetPreferredButtonWidth());
                    _dropdownButtonWidth = Math.Max(ScaleLogicalX(18), ScaleLogicalX(preferredButtonWidth));
                }

                if (!_innerPaddingSetExplicitly)
                {
                    var preferredPadding = _comboBoxPainter.GetPreferredPadding();
                    _innerPadding = ScaleLogicalPadding(preferredPadding);
                }
                
                _layoutDefaultsInitialized = true;
            }
        }
        
        private void UpdateLayout()
        {
            if (Width <= 0 || Height <= 0) return;
            
            // Only apply painter defaults if not yet initialized
            if (!_layoutDefaultsInitialized)
            {
                ApplyLayoutDefaultsFromPainter();
            }
            
            // Check if layout cache is still valid
            // Note: LeadingImagePath and LeadingIconPath may be inherited from BaseControl
            string currentLeadingImagePath = null;
            string currentLeadingIconPath = null;
            try { currentLeadingImagePath = LeadingImagePath; } catch { }
            try { currentLeadingIconPath = LeadingIconPath; } catch { }
            
            bool needsRecalc = !_layoutCacheValid ||
                _cachedWidth != Width ||
                _cachedHeight != Height ||
                _cachedInnerPadding != InnerPadding ||
                _cachedLeadingImagePath != currentLeadingImagePath ||
                _cachedLeadingIconPath != currentLeadingIconPath ||
                _cachedDropdownButtonWidth != DropdownButtonWidth ||
                _cachedTextFont != TextFont ||
                !DpiScalingHelper.AreScaleFactorsEqual(_cachedScaleX, _dpiScaleX) ||
                !DpiScalingHelper.AreScaleFactorsEqual(_cachedScaleY, _dpiScaleY);
            
            if (needsRecalc)
            {
                // Use helper to calculate layout
                _helper.CalculateLayout(DrawingRect, out _textAreaRect, out _dropdownButtonRect, out _imageRect);
                
                // Update cache tracking
                _cachedWidth = Width;
                _cachedHeight = Height;
                _cachedInnerPadding = InnerPadding;
                try { _cachedLeadingImagePath = LeadingImagePath; } catch { _cachedLeadingImagePath = null; }
                try { _cachedLeadingIconPath = LeadingIconPath; } catch { _cachedLeadingIconPath = null; }
                _cachedDropdownButtonWidth = DropdownButtonWidth;
                _cachedTextFont = TextFont;
                _cachedScaleX = _dpiScaleX;
                _cachedScaleY = _dpiScaleY;
                _layoutCacheValid = true;
            }
        }
        
        private void InvalidateLayout()
        {
            _layoutCacheValid = false;
            _needsLayoutUpdate = true;
            _delayedInvalidateTimer?.Stop();
            _delayedInvalidateTimer?.Start();
        }
        
        #endregion
        
        #region Dispose
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _delayedInvalidateTimer?.Dispose();
                _delayedInvalidateTimer = null;
                
                // Unwire BeepContextMenu events (but don't dispose - managed by BaseControl)
                if (BeepContextMenu != null)
                {
                    BeepContextMenu.ItemClicked -= OnContextMenuItemClicked;
                }
                
                _helper?.Dispose();
                _helper = null;
                _chipAnimationTimer?.Stop();
                _chipAnimationTimer?.Dispose();
                _chipAnimationTimer = null;
                
                _loadingAnimationTimer?.Stop();
                _loadingAnimationTimer?.Dispose();
                _loadingAnimationTimer = null;
                
                _autoCompleteDelayTimer?.Stop();
                _autoCompleteDelayTimer?.Dispose();
                _autoCompleteDelayTimer = null;
            }
            
            base.Dispose(disposing);
        }

        #region Chip Animation Helpers
        private void StartChipAnimation(SimpleItem item, bool animIn)
        {
            if (item == null) return;
            if (!_chipProgress.ContainsKey(item))
            {
                _chipProgress[item] = animIn ? 0f : 1f;
                _chipAnimatingIn[item] = animIn;
            }
            else
            {
                _chipAnimatingIn[item] = animIn;
                if (animIn && _chipProgress[item] < 1f) _chipProgress[item] = Math.Max(_chipProgress[item], 0f);
            }
            if (!_chipAnimationTimer.Enabled) _chipAnimationTimer.Start();
        }

        internal float GetChipAnimationProgress(SimpleItem item)
        {
            if (item == null) return 0f;
            if (_chipProgress.TryGetValue(item, out float p)) return p;
            // Default: if item is currently selected, return 1
            return _selectedItems != null && _selectedItems.Contains(item) ? 1f : 0f;
        }

        internal System.Collections.Generic.IEnumerable<SimpleItem> GetAnimatingChips()
        {
            return _chipProgress.Keys;
        }
        #endregion
        
        #region Loading State
        
        /// <summary>
        /// Starts the loading animation timer
        /// </summary>
        private void StartLoadingAnimation()
        {
            if (_loadingAnimationTimer == null)
            {
                _loadingAnimationTimer = new Timer();
                _loadingAnimationTimer.Interval = 50; // 20 FPS for smooth rotation
                _loadingAnimationTimer.Tick += LoadingAnimationTimer_Tick;
            }
            _loadingRotationAngle = 0f;
            _loadingAnimationTimer.Start();
        }

        /// <summary>
        /// Stops the loading animation timer
        /// </summary>
        private void StopLoadingAnimation()
        {
            if (_loadingAnimationTimer != null)
            {
                _loadingAnimationTimer.Stop();
            }
            _loadingRotationAngle = 0f;
        }

        /// <summary>
        /// Timer tick handler for loading animation
        /// </summary>
        private void LoadingAnimationTimer_Tick(object sender, EventArgs e)
        {
            if (!_isLoading)
            {
                StopLoadingAnimation();
                return;
            }

            _loadingRotationAngle += 15f; // Rotate 15 degrees per tick
            if (_loadingRotationAngle >= 360f)
            {
                _loadingRotationAngle = 0f;
            }
            Invalidate();
        }
        
        #endregion
        
        #endregion
    }
#pragma warning restore IL2026
}
