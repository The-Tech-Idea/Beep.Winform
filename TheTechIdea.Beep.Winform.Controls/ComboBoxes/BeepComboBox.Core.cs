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
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup;

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
        internal bool IsButtonHovered       => _isButtonHovered;
        internal bool IsControlHovered      => _isHovered;
        internal bool IsDropdownOpen        => _isDropdownOpen;
        internal bool ClearButtonHovered    => _clearButtonHovered;
        internal Rectangle ClearButtonRect  => _clearButtonRect;
        internal float ChevronAngle         => _chevronAngle;
        internal float LoadingRotationAngle => _loadingRotationAngle;
        internal float SkeletonOffset       => _skeletonOffset;
        internal bool  IsEditingInline        => _isEditing;

        // maps SimpleItem identity → chip-close button rect (populated by design-system chip painters)
        internal readonly System.Collections.Generic.Dictionary<string, Rectangle> ChipCloseRects
            = new System.Collections.Generic.Dictionary<string, Rectangle>();

        // ENH-15: tooltip for truncated display text
        private System.Windows.Forms.ToolTip _overflowTooltip;
        internal System.Windows.Forms.ToolTip OverflowTooltip => _overflowTooltip;

        #endregion
        
        #region Core Fields
        
        // Visual Style
        private ComboBoxType _comboBoxType = ComboBoxType.OutlineDefault;
        private bool _comboBoxTypeWasExplicitlySet = false;
        private bool _suppressComboBoxTypeExplicitTracking = false;
        
        // List management
        private BindingList<SimpleItem> _listItems = new BindingList<SimpleItem>();
        private SimpleItem _selectedItem;
        private int _selectedItemIndex = -1;
        
        // Dropdown state
        private bool _isDropdownOpen = false;
        private IComboBoxPopupHost _popupHost;
        private List<SimpleItem> _popupSelectionSnapshot;
        private string _popupSearchText = string.Empty;
        
        // Text and editing
        private string _inputText = string.Empty;
        private bool _isEditing = false;

        // Inline text editor — a BeepTextBox child shown over _textAreaRect on click
        private BeepTextBox _inlineEditor;
        
        // Layout rectangles — recomputed fresh every frame (BeepButton pattern)
        private Rectangle _textAreaRect;
        private Rectangle _dropdownButtonRect;
        private Rectangle _imageRect;
        
        // Layout flags
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

        // Clear button
        private bool _clearButtonHovered = false;
        private Rectangle _clearButtonRect;
        private const int ClearButtonWidthLogical = 22;

        // Chevron animation
        private float _chevronAngle = 0f;           // 0 = closed / 180 = open
        private Timer _chevronTimer;
        private float _chevronAnimTarget = 0f;
        private const float ChevronAnimStep = 180f / (150f / 16f); // ~19.2 deg per 16 ms tick
        private string _dropdownIconPath = "";
        
        // Font — resolved from theme in ApplyTheme(); never use this.Font / Control.Font (SKILL Rule 2)
        private Font _textFont = SystemFonts.DefaultFont;
        
        // Performance optimizations
        private Timer _delayedInvalidateTimer;
        private bool _needsLayoutUpdate = false;
        
        // Loading state
        private bool _isLoading = false;
        private Timer _loadingAnimationTimer;
        private float _loadingRotationAngle = 0f;

        // ENH-23: Skeleton shimmer
        private Timer _skeletonTimer;
        private float _skeletonOffset = 0f;   // 0..1 sweep position

        // Auto-complete debouncing
        private Timer _autoCompleteDelayTimer;

        // Select-only typeahead buffer
        private string _typeAheadBuffer = string.Empty;
        private Timer _typeAheadTimer;
        
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
            MinimumSize = new Size(80, 28);  // Enforce minimum so rects are always usable
            BorderRadius = 4;
            ShowAllBorders = true;
            ApplyLayoutDefaultsFromPainter(applyHeight: true);
            
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

            // ENH-15: tooltip for truncated display text
            _overflowTooltip = new System.Windows.Forms.ToolTip
            {
                InitialDelay = 500,
                AutoPopDelay = 4000,
                ReshowDelay  = 200,
                ShowAlways   = true
            };

            // Initialize context menu (using inherited BeepContextMenu from BaseControl)
            InitializeContextMenu();
            
            // Set dropdown icon
            SetDropdownIcon();
            
            // Apply theme
            ApplyTheme();
            ApplyComboBoxTypeFromControlStyleIfNeeded();
            
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

            _typeAheadTimer = new Timer { Interval = 700 };
            _typeAheadTimer.Tick += (s, e) =>
            {
                _typeAheadTimer.Stop();
                _typeAheadBuffer = string.Empty;
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
            BeepContextMenu.ShowSearchBox = ComboBoxVisualTokenCatalog.SupportsSearch(ComboBoxType) || ShowSearchInDropdown;
            BeepContextMenu.ShowSeparators = false;
            BeepContextMenu.ContextMenuType = FormStyle.Modern;
            // Ensure theme and lifecycle are aligned with the control
            BeepContextMenu.Theme = this.Theme;
            BeepContextMenu.DestroyOnClose = false; // reuse for dropdown
            SyncDropdownMetrics();
            
            // Wire up events
            BeepContextMenu.ItemClicked += OnContextMenuItemClicked;
            BeepContextMenu.MenuClosed += (s, e) =>
            {
                // Sync state when menu closes itself (e.g. Deactivate/focus-loss)
                // without going through our CloseDropdown() method
                if (_isDropdownOpen)
                {
                    _isDropdownOpen = false;
                    PopupClosed?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
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
            if (DesignMode) return; // Prevent design-time redraws on focus selection
            // Focus gain may shift DrawingRect (different border for focused state).
            // UpdateLayout always recomputes fresh + syncs inline editor.
            UpdateLayout();
            Invalidate();
        }
        
        private void OnControlLostFocus(object sender, EventArgs e)
        {
            if (DesignMode) return; // Prevent design-time redraws on focus loss
            _isEditing = false;
            // Sync dropdown state: if the context menu closed itself (via deactivation)
            // without going through CloseDropdown(), the flag may still be true.
            if (_isDropdownOpen && (BeepContextMenu == null || !BeepContextMenu.Visible))
            {
                _isDropdownOpen = false;
                PopupClosed?.Invoke(this, EventArgs.Empty);
            }
            // Focus loss may shift DrawingRect. Always recompute.
            UpdateLayout();
            Invalidate();
        }
        
        private void OnControlClick(object sender, EventArgs e)
        {
            // If clicked on text area and editable, start editing
            // Otherwise handled by hit areas
        }
        
        private void OnContextMenuItemClicked(object sender, MenuItemEventArgs e)
        {
            // ENH-18: handle the virtual "Select all / Clear all" row
            if (e.Item?.Name == "_selectall")
            {
                bool allSelected = _listItems.Count > 0 &&
                                   (_selectedItems?.Count ?? 0) >= _listItems.Count;
                SelectedItems = allSelected
                    ? new System.Collections.Generic.List<SimpleItem>()
                    : new System.Collections.Generic.List<SimpleItem>(_listItems);
                SelectedItemsChanged?.Invoke(this, EventArgs.Empty);
                return; // keep dropdown open so user can refine
            }

            // ENH-06: skip non-selectable group headers
            if (e.Item?.Name?.StartsWith("_grp_") == true || e.Item?.IsEnabled == false)
                return;

            if (AllowMultipleSelection || (BeepContextMenu != null && BeepContextMenu.MultiSelect))
            {
                // Toggle selection in selected items
                var selectedList = SelectedItems ?? new System.Collections.Generic.List<SimpleItem>();
                int existingIndex = selectedList.FindIndex(item => IsSameSimpleItem(item, e.Item));
                if (existingIndex >= 0)
                {
                    selectedList.RemoveAt(existingIndex);
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

        private void ApplyLayoutDefaultsFromPainter(bool force = false, bool applyHeight = false)
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

                // ENH-05: apply SizeVariant height only when the caller explicitly opts in.
                // Theme / FormStyle / ComboBoxType / property-reset callers use applyHeight=false
                // so they can never accidentally shrink or grow the control.
                if (applyHeight)
                {
                    int targetH = SizeVariant switch
                    {
                        BeepComboBoxSize.Small  => ScaleLogicalY(24),
                        BeepComboBoxSize.Large  => ScaleLogicalY(40),
                        _                       => ScaleLogicalY(32)
                    };
                    if (Height != targetH)
                    {
                        Size = new System.Drawing.Size(Width, targetH);
                    }
                }

                _layoutDefaultsInitialized = true;
            }
        }
        
        /// <summary>
        /// Recalculates all layout rectangles from the current DrawingRect.
        /// Following BeepButton's approach: always compute fresh, no caching.
        /// </summary>
        private void UpdateLayout()
        {
            if (Width <= 0 || Height <= 0) return;

            if (!_layoutDefaultsInitialized)
            {
                ApplyLayoutDefaultsFromPainter(force: false, applyHeight: false);
            }

            // Refresh DrawingRect so we work with the latest value
            UpdateDrawingRect();

            // Always compute fresh — identical to DrawContent pipeline
            _helper.CalculateLayout(DrawingRect, out _textAreaRect, out _dropdownButtonRect, out _imageRect);

            // Clear-button carve-out
            if (ShowClearButton && (_selectedItem != null || !string.IsNullOrEmpty(_inputText)))
            {
                int cbw = Math.Max(16, Math.Min(ScaleLogicalX(ClearButtonWidthLogical), _textAreaRect.Width / 4));
                _clearButtonRect = new Rectangle(
                    _dropdownButtonRect.Left - cbw,
                    DrawingRect.Y, cbw, DrawingRect.Height);
                _textAreaRect = new Rectangle(
                    _textAreaRect.X, _textAreaRect.Y,
                    Math.Max(1, _textAreaRect.Width - cbw), _textAreaRect.Height);
            }
            else
            {
                _clearButtonRect = Rectangle.Empty;
            }

            // RTL mirror
            if (IsRtl && !DrawingRect.IsEmpty)
            {
                _dropdownButtonRect = MirrorRect(_dropdownButtonRect, DrawingRect);
                _textAreaRect       = MirrorRect(_textAreaRect,       DrawingRect);
                _clearButtonRect    = _clearButtonRect.IsEmpty ? Rectangle.Empty
                                     : MirrorRect(_clearButtonRect, DrawingRect);
                if (!_imageRect.IsEmpty)
                    _imageRect = MirrorRect(_imageRect, DrawingRect);
            }

            // Sync inline editor
            if (_inlineEditor != null && _inlineEditor.Visible)
            {
                if (_inlineEditor.Bounds != _textAreaRect)
                    _inlineEditor.Bounds = _textAreaRect;
            }
        }

        /// <summary>ENH-24: Mirrors a rectangle horizontally inside a container.</summary>
        private static Rectangle MirrorRect(Rectangle r, Rectangle container)
        {
            int mirrored = container.Right - (r.Right - container.Left);
            return new Rectangle(mirrored, r.Y, r.Width, r.Height);
        }
        
        private void InvalidateLayout()
        {
            if (DesignMode) return; // Never schedule timer-driven repaints at design time
            _needsLayoutUpdate = true;
            _delayedInvalidateTimer?.Stop();
            _delayedInvalidateTimer?.Start();
        }

        protected override void OnDpiScaleChanged(float oldScaleX, float oldScaleY, float newScaleX, float newScaleY)
        {
            base.OnDpiScaleChanged(oldScaleX, oldScaleY, newScaleX, newScaleY);

            if (newScaleX <= 0f || newScaleY <= 0f)
            {
                return;
            }

            float ratioX = oldScaleX > 0f ? (newScaleX / oldScaleX) : 1f;
            float ratioY = oldScaleY > 0f ? (newScaleY / oldScaleY) : 1f;

            if (_dropdownButtonWidthSetExplicitly)
            {
                int scaledWidth = (int)Math.Round(_dropdownButtonWidth * ratioX);
                _dropdownButtonWidth = Math.Max(ScaleLogicalX(18), Math.Max(1, scaledWidth));
            }
            else
            {
                _layoutDefaultsInitialized = false;
            }

            if (_innerPaddingSetExplicitly)
            {
                _innerPadding = new Padding(
                    Math.Max(0, (int)Math.Round(_innerPadding.Left * ratioX)),
                    Math.Max(0, (int)Math.Round(_innerPadding.Top * ratioY)),
                    Math.Max(0, (int)Math.Round(_innerPadding.Right * ratioX)),
                    Math.Max(0, (int)Math.Round(_innerPadding.Bottom * ratioY)));
            }
            else
            {
                _layoutDefaultsInitialized = false;
            }

            ApplyLayoutDefaultsFromPainter(force: true, applyHeight: false);
            UpdateLayout();
            Invalidate();
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
                
                _chevronTimer?.Stop();
                _chevronTimer?.Dispose();
                _chevronTimer = null;

                _loadingAnimationTimer?.Stop();
                _loadingAnimationTimer?.Dispose();
                _loadingAnimationTimer = null;

                _overflowTooltip?.Dispose();
                _overflowTooltip = null;

                _skeletonTimer?.Stop();
                _skeletonTimer?.Dispose();
                _skeletonTimer = null;

                _autoCompleteDelayTimer?.Stop();
                _autoCompleteDelayTimer?.Dispose();
                _autoCompleteDelayTimer = null;

                // Inline editor is a child control — Dispose releases it
                if (_inlineEditor != null)
                {
                    _inlineEditor.LostFocus     -= InlineEditor_LostFocus;
                    _inlineEditor.KeyDown       -= InlineEditor_KeyDown;
                    _inlineEditor.TextChanged   -= InlineEditor_TextChanged;
                    _inlineEditor.Dispose();
                    _inlineEditor = null;
                }
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

        // ── ENH-23: Skeleton shimmer ────────────────────────────────────────
        internal void StartSkeletonAnimation()
        {
            if (_skeletonTimer == null)
            {
                _skeletonTimer = new Timer { Interval = 24 }; // ~42 FPS
                _skeletonTimer.Tick += SkeletonTimer_Tick;
            }
            _skeletonOffset = 0f;
            _skeletonTimer.Start();
        }

        internal void StopSkeletonAnimation()
        {
            _skeletonTimer?.Stop();
            _skeletonOffset = 0f;
        }

        private void SkeletonTimer_Tick(object sender, EventArgs e)
        {
            if (!ShowSkeleton)
            {
                StopSkeletonAnimation();
                Invalidate();
                return;
            }
            _skeletonOffset += 0.015f;
            if (_skeletonOffset > 1f) _skeletonOffset = 0f;
            Invalidate();
        }
        
        #endregion
        
        #endregion
    }
#pragma warning restore IL2026
}
