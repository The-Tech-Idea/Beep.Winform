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
        
        // Visual state
        private bool _isHovered = false;
        private bool _isButtonHovered = false;
        private string _dropdownIconPath = "";
        
        // Font
        private Font _textFont = new Font("Segoe UI", 9f);
        
        // Performance optimizations
        private Timer _delayedInvalidateTimer;
        private bool _needsLayoutUpdate = false;
        
        #endregion
        
        #region Events
        
        public event EventHandler<SelectedItemChangedEventArgs> SelectedIndexChanged;
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        public event EventHandler PopupOpened;
        public event EventHandler PopupClosed;
        
        #endregion
        
        #region Constructor
        
        public BeepComboBox()
        {
            // Set control styles
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);
            
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
            BeepContextMenu.ShowCheckBox = false;
            BeepContextMenu.ShowSeparators = false;
            BeepContextMenu.ContextMenuType = Forms.ModernForm.FormStyle.Modern;
            // Ensure theme and lifecycle are aligned with the control
            BeepContextMenu.Theme = this.Theme;
            BeepContextMenu.DestroyOnClose = false; // reuse for dropdown
            
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
            SelectedItem = e.Item;
            CloseDropdown();
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
        
        private void UpdateLayout()
        {
            if (Width <= 0 || Height <= 0) return;
            
            // Use helper to calculate layout
            _helper.CalculateLayout(DrawingRect, out _textAreaRect, out _dropdownButtonRect, out _imageRect);
        }
        
        private void InvalidateLayout()
        {
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
            }
            
            base.Dispose(disposing);
        }
        
        #endregion
    }
#pragma warning restore IL2026
}
