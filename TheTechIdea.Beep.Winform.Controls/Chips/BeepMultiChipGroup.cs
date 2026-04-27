using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Chips.Helpers;
using TheTechIdea.Beep.Winform.Controls.Chips.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Chips
{
    #region Enums
    /// <summary>
    /// Chip visual variants following Material Design principles
    /// </summary>
    public enum ChipVariant
    {
        Filled,
        Text,
        Outlined
    }

    public enum ChipColor
    {
        Default,
        Primary,
        Secondary,
        Info,
        Success,
        Warning,
        Error,
        Dark
    }

    public enum ChipSize
    {
        Small,
        Medium,
        Large
    }

    public enum ChipSelectionMode
    {
        Single,
        Multiple,
        Toggle
    }

    public enum ChipStyle
    {
        /// <summary>Default filled chip style</summary>
        Default,
        /// <summary>Modern Material Design style</summary>
        Modern,
        /// <summary>Classic outlined chip</summary>
        Classic,
        /// <summary>Text-only minimalist style</summary>
        Minimalist,
        /// <summary>Colorful gradient chips</summary>
        Colorful,
        /// <summary>Professional outlined style</summary>
        Professional,
        /// <summary>Soft pastel colors</summary>
        Soft,
        /// <summary>High contrast accessibility style</summary>
        HighContrast,
        /// <summary>Full pill-shaped rounded chips</summary>
        Pill,
        /// <summary>Likeable chips with heart icons (pink theme)</summary>
        Likeable,
        /// <summary>Ingredient/tag style chips with checkmarks</summary>
        Ingredient,
        /// <summary>Shaded chips with gradient backgrounds</summary>
        Shaded,
        /// <summary>Avatar chips with user profile images</summary>
        Avatar,
        /// <summary>Elevated chips with shadow effect</summary>
        Elevated,
        /// <summary>Smooth rounded corners with subtle shadows</summary>
        Smooth,
        /// <summary>Square/rare chips with minimal rounding</summary>
        Square,
        /// <summary>Dashed border style</summary>
        Dashed,
        /// <summary>Bold text with 30% opacity background</summary>
        Bold
    }

    /// <summary>
    /// Chip shape variants
    /// </summary>
    public enum ChipShape
    {
        /// <summary>Standard rounded rectangle</summary>
        Rounded,
        /// <summary>Full pill shape (height/2 radius)</summary>
        Pill,
        /// <summary>Square with minimal rounding</summary>
        Square,
        /// <summary>Stadium/capsule shape</summary>
        Stadium
    }
    #endregion

    #region Event Args
    /// <summary>
    /// Event arguments for chip selection changes
    /// </summary>
    public class ChipSelectionChangedEventArgs : EventArgs
    {
        public SimpleItem SelectedItem { get; }
        public IReadOnlyList<SimpleItem> SelectedItems { get; }
        public SimpleItem PreviousItem { get; }
        public ChipSelectionMode SelectionMode { get; }
        public bool IsSelected { get; }

        public ChipSelectionChangedEventArgs(
            SimpleItem selectedItem,
            IReadOnlyList<SimpleItem> selectedItems,
            SimpleItem previousItem,
            ChipSelectionMode mode,
            bool isSelected)
        {
            SelectedItem = selectedItem;
            SelectedItems = selectedItems;
            PreviousItem = previousItem;
            SelectionMode = mode;
            IsSelected = isSelected;
        }
    }

    /// <summary>
    /// Event arguments for chip click
    /// </summary>
    public class ChipClickedEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public int Index { get; }
        public MouseButtons Button { get; }
        public bool IsCloseButton { get; }

        public ChipClickedEventArgs(SimpleItem item, int index, MouseButtons button, bool isCloseButton = false)
        {
            Item = item;
            Index = index;
            Button = button;
            IsCloseButton = isCloseButton;
        }
    }

    /// <summary>
    /// Event arguments for chip reorder via drag-and-drop
    /// </summary>
    public class ChipReorderedEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public int OldIndex { get; }
        public int NewIndex { get; }

        public ChipReorderedEventArgs(SimpleItem item, int oldIndex, int newIndex)
        {
            Item = item;
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }
    }

    /// <summary>
    /// Event arguments for chip text edited inline
    /// </summary>
    public class ChipEditedEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public int Index { get; }
        public string OldText { get; }
        public string NewText { get; }
        public bool Cancelled { get; }

        public ChipEditedEventArgs(SimpleItem item, int index, string oldText, string newText, bool cancelled = false)
        {
            Item = item;
            Index = index;
            OldText = oldText;
            NewText = newText;
            Cancelled = cancelled;
        }
    }

    /// <summary>
    /// Event arguments for chip added via input
    /// </summary>
    public class ChipAddedEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public int Index { get; }
        public bool Cancelled { get; set; }

        public ChipAddedEventArgs(SimpleItem item, int index)
        {
            Item = item;
            Index = index;
        }
    }
    #endregion

    [ToolboxItem(true)]
    [DisplayName("Beep MultiChip")]
    [Description("A MultiChip.")]
    public partial class BeepMultiChipGroup : BaseControl
    {
        #region Fields
        private BindingList<SimpleItem> _chipItems = new BindingList<SimpleItem>();
        private List<ChipItem> _chips = new List<ChipItem>();
        private readonly int _chipPadding = 5;
        private readonly int _chipCornerRadius = 15;
        private SimpleItem _selectedItem;
        private int _selectedIndex = -1;

        // Title
        private string _titleText = "Multi-Select Chip Group";
        private Font _titleFont = BeepThemesManager.ToFont("Segoe UI", 12f, FontWeight.Bold, FontStyle.Bold);
        private Color _titleColor = Color.Black;
        private ContentAlignment _titleAlignment = ContentAlignment.TopLeft;
        private int _titleHeight = 30;

        // Chip styling
        private ChipVariant _chipVariant = ChipVariant.Filled;
        private ChipColor _chipColor = ChipColor.Default;
        private ChipSize _chipSize = ChipSize.Medium;
        private ChipSelectionMode _selectionMode = ChipSelectionMode.Single;
        private BindingList<SimpleItem> _selectedItems = new BindingList<SimpleItem>();

        private ChipStyle _chipStyle = ChipStyle.Default;
        private int _chipBorderWidth = 1;
        private bool _showChipBorders = true;
        private ChipShape _chipShape = ChipShape.Rounded;
        private bool _showAvatar = false;
        private bool _showLeadingIcon = true;
        private bool _showTrailingIcon = true;
        private bool _showCloseButton = true;
        private bool _showCheckmark = true;
        private bool _animateSelection = true;

        // Painter infra
        private IChipGroupPainter _painter;
        private readonly Dictionary<int, Rectangle> _closeRects = new();
        private ChipRenderOptions _renderOptions = new ChipRenderOptions();

        // Previous selection for event tracking
        private SimpleItem _previousSelectedItem;

        // ── Drag-and-drop reorder ─────────────────────────────────────
        private bool _allowDragReorder = true;
        private int _dragChipIndex = -1;
        private Point _dragStartPoint = Point.Empty;
        private bool _isDragging = false;
        private int _dropInsertIndex = -1;
        private Point _dragGhostLocation = Point.Empty;
        private const int DragThreshold = 5;

        // ── Inline chip editing ───────────────────────────────────────
        private TextBox? _editTextBox;
        private int _editingChipIndex = -1;
        private string _editOriginalText = string.Empty;

        // ── Chip input / creation ─────────────────────────────────────
        private bool _allowChipCreation = false;
        private TextBox? _inputTextBox;
        private Rectangle _inputRect = Rectangle.Empty;
        private string _inputPlaceholderText = "Type to add...";
        private char[] _inputDelimiters = new[] { ',', ';', '\t', '\r' };

        // ── Ripple animation ──────────────────────────────────────────
        private bool _enableRipple = true;
        private readonly List<ChipRipple> _activeRipples = new();
        private System.Windows.Forms.Timer? _rippleTimer;

        // ── Tooltip ───────────────────────────────────────────────────
        private bool _showTooltip = true;
        private System.Windows.Forms.Timer? _tooltipTimer;
        private int _tooltipChipIndex = -1;
        private bool _tooltipShowing = false;

        // ── Add/remove animation ──────────────────────────────────────
        private bool _enableAddRemoveAnimation = true;
        private readonly List<ChipAnimation> _chipAnimations = new();
        private System.Windows.Forms.Timer? _animationTimer;

        // ── Utility row ───────────────────────────────────────────────
        private bool _showUtilityRow = true;
        #endregion

        private IBeepTheme GetEffectiveTheme()
        {
            return _currentTheme ?? BeepThemesManager.CurrentTheme ?? BeepThemesManager.GetDefaultTheme();
        }

        #region Properties
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data")]
        [Description("The list of chip items to display in the group.")]
        public BindingList<SimpleItem> ListItems
        {
            get => _chipItems;
            set
            {
                _chipItems = value;
                UpdateChipsFromItems();
                ApplyThemeToChips();
                UpdateChipBounds();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The currently selected chip item.")]
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                if (_selectedItem != null)
                {
                    _selectedIndex = _chipItems.IndexOf(_selectedItem);
                    UpdateSelectedChips();
                }
                else
                {
                    _selectedIndex = -1;
                    _chips.ForEach(c => c.IsSelected = false);
                }
                OnSelectedItemChanged(_selectedItem);
                Invalidate();
            }
        }

        [Browsable(false)]
        [Category("Data")]
        [Description("The index of the currently selected chip item.")]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0 && value < _chipItems.Count)
                {
                    _selectedIndex = value;
                    _selectedItem = _chipItems[_selectedIndex];
                    UpdateSelectedChips();
                    OnSelectedItemChanged(_selectedItem);
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The title text displayed above the chip group.")]
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value; UpdateChipBounds(); Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The font used for the title text.")]
        public Font TitleFont
        {
            get => _titleFont;
            set { _titleFont = value; UpdateChipBounds(); Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The color of the title text.")]
        public Color TitleColor { get => _titleColor; set { _titleColor = value; Invalidate(); } }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The alignment of the title text.")]
        public ContentAlignment TitleAlignment { get => _titleAlignment; set { _titleAlignment = value; Invalidate(); } }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual variant of the chips (Filled, Text, Outlined).")]
        [DefaultValue(ChipVariant.Filled)]
        public ChipVariant ChipVariant { get => _chipVariant; set { _chipVariant = value; Invalidate(); } }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The color of the chip backgrounds.")]
        public ChipColor ChipColor { get => _chipColor; set { _chipColor = value; ApplyThemeToChips(); Invalidate(); } }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The size of the chips.")]
        public ChipSize ChipSize
        {
            get => _chipSize;
            set
            {
                if (_chipSize != value)
                {
                    _chipSize = value;
                    SyncRenderOptions();
                    UpdateChipBounds();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("The selection mode of the chip group (Single, Multiple, Toggle).")]
        public ChipSelectionMode SelectionMode { get => _selectionMode; set { _selectionMode = value; Invalidate(); } }

        [Browsable(true)]
        [Category("Data")]
        [Description("The collection of currently selected chip items.")]
        public BindingList<SimpleItem> SelectedItems { get => _selectedItems; set { _selectedItems = value; UpdateSelectedChips(); Invalidate(); } }

        [Browsable(true)]
        [Category("Chip ProgressBarStyle")]
        [Description("Predefined styling preset for chips.")]
        [DefaultValue(ChipStyle.Default)]
        public ChipStyle ChipStyle { get => _chipStyle; set { _chipStyle = value; ApplyChipStyle(value); Invalidate(); } }

        [Browsable(true)]
        [Category("Chip ProgressBarStyle")]
        [Description("Width of chip borders.")]
        [DefaultValue(1)]
        public int ChipBorderWidth
        {
            get => _chipBorderWidth;
            set
            {
                int safeValue = Math.Max(0, value);
                if (_chipBorderWidth != safeValue)
                {
                    _chipBorderWidth = safeValue;
                    SyncRenderOptions();
                    UpdateChipBounds();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Chip ProgressBarStyle")]
        [Description("Whether to show borders around chips.")]
        [DefaultValue(true)]
        public bool ShowChipBorders
        {
            get => _showChipBorders;
            set
            {
                if (_showChipBorders != value)
                {
                    _showChipBorders = value;
                    SyncRenderOptions();
                    UpdateChipBounds();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Chip ProgressBarStyle")]
        [Description("Show a leading icon in chips when an image/icon source exists.")]
        [DefaultValue(true)]
        public bool ShowLeadingIcon
        {
            get => _showLeadingIcon;
            set
            {
                if (_showLeadingIcon != value)
                {
                    _showLeadingIcon = value;
                    SyncRenderOptions();
                    UpdateChipBounds();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Chip ProgressBarStyle")]
        [Description("Show trailing icon area when supported by the active painter.")]
        [DefaultValue(true)]
        public bool ShowTrailingIcon
        {
            get => _showTrailingIcon;
            set
            {
                if (_showTrailingIcon != value)
                {
                    _showTrailingIcon = value;
                    SyncRenderOptions();
                    UpdateChipBounds();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Chip ProgressBarStyle")]
        [Description("Show close button for selected chips.")]
        [DefaultValue(true)]
        public bool ShowCloseButton
        {
            get => _showCloseButton;
            set
            {
                if (_showCloseButton != value)
                {
                    _showCloseButton = value;
                    SyncRenderOptions();
                    UpdateChipBounds();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Chip ProgressBarStyle")]
        [Description("Show check mark for selected chips.")]
        [DefaultValue(true)]
        public bool ShowCheckmark
        {
            get => _showCheckmark;
            set
            {
                if (_showCheckmark != value)
                {
                    _showCheckmark = value;
                    SyncRenderOptions();
                    UpdateChipBounds();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Chip ProgressBarStyle")]
        [Description("Show avatar slot when supported by the active painter.")]
        [DefaultValue(false)]
        public bool ShowAvatar
        {
            get => _showAvatar;
            set
            {
                if (_showAvatar != value)
                {
                    _showAvatar = value;
                    SyncRenderOptions();
                    UpdateChipBounds();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Fired when the selected item changes (legacy event for backward compatibility)
        /// </summary>
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        
        /// <summary>
        /// Fired when chip selection changes (primary event for synchronization)
        /// This event provides comprehensive information for syncing with BeepListBox
        /// </summary>
        public event EventHandler<ChipSelectionChangedEventArgs> SelectionChanged;
        
        /// <summary>
        /// Fired when a chip is clicked (includes close button clicks)
        /// </summary>
        public event EventHandler<ChipClickedEventArgs> ChipClicked;
        
        /// <summary>
        /// Fired when a chip is removed via close button
        /// </summary>
        public event EventHandler<ChipClickedEventArgs> ChipRemoved;

        /// <summary>
        /// Fired when a chip is reordered via drag-and-drop
        /// </summary>
        public event EventHandler<ChipReorderedEventArgs> ChipReordered;

        /// <summary>
        /// Fired when a chip text is edited inline
        /// </summary>
        public event EventHandler<ChipEditedEventArgs> ChipEdited;

        /// <summary>
        /// Fired when a new chip is added via the input field
        /// </summary>
        public event EventHandler<ChipAddedEventArgs> ChipAdded;
        
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem) 
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
            
            // Also fire the unified SelectionChanged event
            OnSelectionChanged(selectedItem, true);
        }
        
        /// <summary>
        /// Raises the SelectionChanged event with full details
        /// </summary>
        protected virtual void OnSelectionChanged(SimpleItem item, bool isSelected)
        {
            SelectionChanged?.Invoke(this, new ChipSelectionChangedEventArgs(
                item,
                _selectedItems.ToList().AsReadOnly(),
                _previousSelectedItem,
                _selectionMode,
                isSelected
            ));
            
            _previousSelectedItem = item;
        }
        
        /// <summary>
        /// Raises the ChipClicked event
        /// </summary>
        protected virtual void OnChipClicked(SimpleItem item, int index, MouseButtons button, bool isCloseButton)
        {
            ChipClicked?.Invoke(this, new ChipClickedEventArgs(item, index, button, isCloseButton));
        }
        
        /// <summary>
        /// Raises the ChipRemoved event
        /// </summary>
        protected virtual void OnChipRemoved(SimpleItem item, int index)
        {
            ChipRemoved?.Invoke(this, new ChipClickedEventArgs(item, index, MouseButtons.Left, true));
        }

        /// <summary>
        /// Raises the ChipReordered event
        /// </summary>
        protected virtual void OnChipReordered(SimpleItem item, int oldIndex, int newIndex)
        {
            ChipReordered?.Invoke(this, new ChipReorderedEventArgs(item, oldIndex, newIndex));
        }

        /// <summary>
        /// Raises the ChipEdited event
        /// </summary>
        protected virtual void OnChipEdited(SimpleItem item, int index, string oldText, string newText, bool cancelled = false)
        {
            ChipEdited?.Invoke(this, new ChipEditedEventArgs(item, index, oldText, newText, cancelled));
        }

        /// <summary>
        /// Raises the ChipAdded event
        /// </summary>
        protected virtual void OnChipAdded(SimpleItem item, int index)
        {
            ChipAdded?.Invoke(this, new ChipAddedEventArgs(item, index));
        }

        public new bool AutoScroll { get => false; set { } }
        #endregion

        #region Constructor
        public BeepMultiChipGroup() : base()
        {
            DoubleBuffered = true;
            AutoSize = true;
            IsRounded = true;
            BorderThickness = 1;
            ShowShadow = true;
            BackColor = Color.White;
            AnimationType = DisplayAnimationType.Fade;
            CanBeHovered = true;
            CanBePressed = false;
            CanBeFocused = true;

            // Painter setup
            _painter = new DefaultChipGroupPainter();
            _painter.Initialize(this, GetEffectiveTheme());
            _renderOptions = new ChipRenderOptions
            {
                Font = this.Font,
                CornerRadius = _chipCornerRadius,
                Gap = _chipPadding,
                Theme = GetEffectiveTheme(),
                Size = _chipSize
            };
            SyncRenderOptions();

            ApplyChipStyle(_chipStyle);

            _chipItems.ListChanged += (s, e) =>
            {
                UpdateChipsFromItems();
                ApplyThemeToChips();
                UpdateChipBounds();
                Invalidate();
            };

            // Ripple animation timer
            _rippleTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _rippleTimer.Tick += RippleTimer_Tick;

            // Tooltip timer
            _tooltipTimer = new System.Windows.Forms.Timer { Interval = 500 };
            _tooltipTimer.Tick += TooltipTimer_Tick;

            // Add/remove animation timer
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _animationTimer.Tick += AnimationTimer_Tick;
        }
        #endregion

        #region Chip Item Class
        private class ChipItem
        {
            public SimpleItem Item { get; set; }
            public Rectangle Bounds { get; set; }
            public bool IsSelected { get; set; }
            public bool IsHovered { get; set; }
            public ChipVariant Variant { get; set; }
            public ChipColor Color { get; set; }
            public ChipSize Size { get; set; }
            public bool IsDisabled { get; set; }
            public float AnimationProgress { get; set; } = 1f;
            public bool IsNew { get; set; }
            public bool IsRemoving { get; set; }
        }
        #endregion

        #region Animation Support Classes
        private class ChipRipple
        {
            public Rectangle Bounds;
            public Point Center;
            public float Progress; // 0..1
            public Color Color;
        }

        private class ChipAnimation
        {
            public int ChipIndex;
            public float Progress; // 0..1
            public bool IsAdd; // true = add, false = remove
            public Rectangle Bounds;
        }
        #endregion

        #region Chip Management
        private void UpdateChipsFromItems()
        {
            _chips.Clear();
            foreach (var item in _chipItems)
            {
                _chips.Add(new ChipItem
                {
                    Item = item,
                    IsSelected = _selectedItems.Contains(item),
                    IsHovered = false,
                    Variant = _chipVariant,
                    Color = _chipColor,
                    Size = _chipSize
                });
            }
            UpdateChipBounds();
        }

        private void UpdateSelectedChips()
        {
            foreach (var chip in _chips)
            {
                chip.IsSelected = _selectedItems.Contains(chip.Item);
            }
            _selectedItem = _selectedItems.FirstOrDefault();
            _selectedIndex = _selectedItem != null ? _chipItems.IndexOf(_selectedItem) : -1;
        }
        #endregion

        #region Data Binding
        public override void SetValue(object value)
        {
            if (value is SimpleItem selectedItem)
            {
                SelectedItem = selectedItem;
            }
            else if (value is string guidId)
            {
                SelectedItem = _chipItems.FirstOrDefault(item => item.GuidId == guidId);
            }
        }

        public override object GetValue() => SelectedItem;

        public override void ClearValue()
        {
            SelectedItem = null;
            _selectedIndex = -1;
            _chips.ForEach(c => c.IsSelected = false);
            Invalidate();
        }
        #endregion
        
        #region Selection Synchronization Methods
        
        /// <summary>
        /// Sets the selection from a list of items (for sync with BeepListBox)
        /// This method does NOT fire SelectionChanged to avoid circular updates
        /// </summary>
        public void SetSelectionSilent(IEnumerable<SimpleItem> items)
        {
            _selectedItems.Clear();
            foreach (var item in items)
            {
                var match = _chipItems.FirstOrDefault(c => c.GuidId == item.GuidId);
                if (match != null && !_selectedItems.Contains(match))
                {
                    _selectedItems.Add(match);
                }
            }
            UpdateSelectedChips();
            Invalidate();
        }
        
        /// <summary>
        /// Sets the selection from a list of items and fires SelectionChanged event
        /// </summary>
        public void SetSelection(IEnumerable<SimpleItem> items)
        {
            var previousSelected = _selectedItem;
            _selectedItems.Clear();
            
            foreach (var item in items)
            {
                var match = _chipItems.FirstOrDefault(c => c.GuidId == item.GuidId);
                if (match != null && !_selectedItems.Contains(match))
                {
                    _selectedItems.Add(match);
                }
            }
            
            UpdateSelectedChips();
            
            var newSelected = _selectedItems.FirstOrDefault();
            if (newSelected != previousSelected)
            {
                _selectedItem = newSelected;
                _selectedIndex = newSelected != null ? _chipItems.IndexOf(newSelected) : -1;
                OnSelectionChanged(newSelected, newSelected != null);
            }
            
            Invalidate();
        }
        
        /// <summary>
        /// Adds an item to the selection (for multi-select mode)
        /// </summary>
        public void AddToSelection(SimpleItem item, bool fireEvent = true)
        {
            if (item == null) return;
            
            var match = _chipItems.FirstOrDefault(c => c.GuidId == item.GuidId);
            if (match == null) return;
            
            if (!_selectedItems.Contains(match))
            {
                _selectedItems.Add(match);
                UpdateSelectedChips();
                
                if (fireEvent)
                {
                    OnSelectionChanged(match, true);
                }
                
                Invalidate();
            }
        }
        
        /// <summary>
        /// Removes an item from the selection
        /// </summary>
        public void RemoveFromSelection(SimpleItem item, bool fireEvent = true)
        {
            if (item == null) return;
            
            var match = _chipItems.FirstOrDefault(c => c.GuidId == item.GuidId);
            if (match == null) return;
            
            if (_selectedItems.Contains(match))
            {
                _selectedItems.Remove(match);
                UpdateSelectedChips();
                
                if (fireEvent)
                {
                    OnSelectionChanged(match, false);
                }
                
                Invalidate();
            }
        }
        
        /// <summary>
        /// Clears all selections
        /// </summary>
        public void ClearSelection(bool fireEvent = true)
        {
            var hadSelection = _selectedItems.Count > 0;
            _selectedItems.Clear();
            _selectedItem = null;
            _selectedIndex = -1;
            _chips.ForEach(c => c.IsSelected = false);
            
            if (fireEvent && hadSelection)
            {
                OnSelectionChanged(null, false);
            }
            
            Invalidate();
        }
        
        /// <summary>
        /// Toggles the selection state of an item
        /// </summary>
        public void ToggleSelection(SimpleItem item, bool fireEvent = true)
        {
            if (item == null) return;
            
            var match = _chipItems.FirstOrDefault(c => c.GuidId == item.GuidId);
            if (match == null) return;
            
            if (_selectedItems.Contains(match))
            {
                RemoveFromSelection(match, fireEvent);
            }
            else
            {
                if (_selectionMode == ChipSelectionMode.Single)
                {
                    // Clear other selections first
                    _selectedItems.Clear();
                    _chips.ForEach(c => c.IsSelected = false);
                }
                AddToSelection(match, fireEvent);
            }
        }
        
        #endregion

        protected override void OnDpiChangedBeforeParent(EventArgs e)
        {
            base.OnDpiChangedBeforeParent(e);
            
            // Re-apply theme to update fonts with new DPI
            ApplyTheme();
            
            // Recalculate layout
            UpdateChipBounds();
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (IsDisposed || Width <= 0 || Height <= 0) return;
            UpdateChipBounds();
            Invalidate();
        }

        #region Theming
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            float dpiScale = DpiScalingHelper.GetDpiScaleFactor(this);
            _renderOptions.DpiScale = dpiScale;
            SyncRenderOptions();

            // Apply font theme based on ControlStyle
            Helpers.ChipFontHelpers.ApplyFontTheme(this, ControlStyle, _chipSize, dpiScale);

            if (_currentTheme != null)
            {
                // Use theme helpers for consistent color retrieval
                _titleColor = Helpers.ChipThemeHelpers.GetTitleColor(_currentTheme, UseThemeColors);
                BackColor = Helpers.ChipThemeHelpers.GetGroupBackgroundColor(_currentTheme, UseThemeColors);
                
                // Update title font (uses BeepFontManager internally)
                _titleFont = Helpers.ChipFontHelpers.GetTitleFont(ControlStyle, dpiScale);

                // Use ChipFontHelpers (which uses BeepFontManager) for chip text font
                _textFont = Helpers.ChipFontHelpers.GetChipFont(ControlStyle, _chipSize, dpiScale);
                
                _renderOptions.Theme = GetEffectiveTheme();
                _painter?.UpdateTheme(GetEffectiveTheme());
                ApplyThemeToChips();
            }
            else
            {
                _renderOptions.Theme = GetEffectiveTheme();
                _painter?.UpdateTheme(GetEffectiveTheme());
            }

            UpdateChipBounds();
            Invalidate();
        }

        private void ApplyThemeToChips()
        {
            Invalidate();
        }

        private void SyncRenderOptions()
        {
            if (_renderOptions == null)
            {
                _renderOptions = new ChipRenderOptions();
            }

            float dpiScale = _renderOptions.DpiScale > 0
                ? _renderOptions.DpiScale
                : DpiScalingHelper.GetDpiScaleFactor(this);

            _renderOptions.DpiScale = dpiScale;
            _renderOptions.CornerRadius = _chipCornerRadius;
            _renderOptions.Gap = _chipPadding;
            _renderOptions.VerticalGap = _chipPadding;
            _renderOptions.ShowBorders = _showChipBorders;
            _renderOptions.BorderWidth = _chipBorderWidth;
            _renderOptions.Size = _chipSize;
            _renderOptions.Style = _chipStyle;
            _renderOptions.Theme = GetEffectiveTheme();
            _renderOptions.ShowLeadingIcon = _showLeadingIcon;
            _renderOptions.ShowTrailingIcon = _showTrailingIcon;
            _renderOptions.ShowAvatar = _showAvatar;
            _renderOptions.ShowIcon = _showLeadingIcon || _showTrailingIcon || _showAvatar;
            _renderOptions.ShowCloseOnSelected = _showCloseButton;
            _renderOptions.ShowSelectionCheck = _showCheckmark;
            _renderOptions.IconMaxSize = ChipIconHelpers.GetChipIconSize(_chipSize);
            _renderOptions.IconTextGap = DpiScalingHelper.ScaleValue(6, dpiScale);
        }

        private void ApplyChipStyle(ChipStyle style)
        {
            // Keep borders config lightweight; actual visuals are painter-driven
            switch (style)
            {
                case ChipStyle.Minimalist:
                    _showChipBorders = false; _chipBorderWidth = 0; break;
                case ChipStyle.Classic:
                case ChipStyle.HighContrast:
                    _showChipBorders = true; _chipBorderWidth = 2; break;
                default:
                    _showChipBorders = true; _chipBorderWidth = 1; break;
            }

            SwitchPainter(style);
        }
        #endregion
    }
}