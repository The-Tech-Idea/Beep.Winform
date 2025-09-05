using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
    #region Enums
    /// <summary>
    /// Chip visual variants following Material Design principles
    /// </summary>
    public enum ChipVariant
    {
        /// <summary>Filled background with text and border</summary>
        Filled,
        /// <summary>Transparent background with text only</summary>
        Text,
        /// <summary>Border with transparent background</summary>
        Outlined
    }

    /// <summary>
    /// Semantic color options for chips
    /// </summary>
    public enum ChipColor
    {
        /// <summary>Default theme color</summary>
        Default,
        /// <summary>Primary brand color</summary>
        Primary,
        /// <summary>Secondary brand color</summary>
        Secondary,
        /// <summary>Information color (typically blue)</summary>
        Info,
        /// <summary>Success color (typically green)</summary>
        Success,
        /// <summary>Warning color (typically orange/yellow)</summary>
        Warning,
        /// <summary>Error color (typically red)</summary>
        Error,
        /// <summary>Dark color</summary>
        Dark
    }

    /// <summary>
    /// Chip size variations
    /// </summary>
    public enum ChipSize
    {
        /// <summary>Small chip (20px height)</summary>
        Small,
        /// <summary>Medium chip (30px height)</summary>
        Medium,
        /// <summary>Large chip (40px height)</summary>
        Large
    }

    /// <summary>
    /// Selection behavior for chip group
    /// </summary>
    public enum ChipSelectionMode
    {
        /// <summary>Only one chip can be selected at a time</summary>
        Single,
        /// <summary>Multiple chips can be selected simultaneously</summary>
        Multiple,
        /// <summary>Toggle selection on/off for individual chips</summary>
        Toggle
    }

    /// <summary>
    /// Predefined chip styling presets for easy configuration
    /// </summary>
    public enum ChipStyle
    {
        /// <summary>Default theme-based styling</summary>
        Default,
        /// <summary>Modern flat design with subtle colors</summary>
        Modern,
        /// <summary>Classic design with defined borders</summary>
        Classic,
        /// <summary>Minimalist design with clean lines</summary>
        Minimalist,
        /// <summary>Colorful vibrant design</summary>
        Colorful,
        /// <summary>Professional business look</summary>
        Professional,
        /// <summary>Soft pastel colors</summary>
        Soft,
        /// <summary>High contrast for accessibility</summary>
        HighContrast
    }
    #endregion

    [ToolboxItem(true)]
    [DisplayName("Beep MultiChip")]
    [Description("A MultiChip.")]
    public class BeepMultiChipGroup : BaseControl
    {
        #region Fields
        private BindingList<SimpleItem> _chipItems = new BindingList<SimpleItem>(); // List to store the chips
        private List<ChipItem> _chips = new List<ChipItem>(); // Internal list for rendering
        private readonly int _chipPadding = 5; // Padding between chips
        private readonly int _chipCornerRadius = 15; // Radius for rounded chips
        private SimpleItem _selectedItem; // Currently selected item
        private int _selectedIndex = -1; // Selected index

        // Title-related fields
        private string _titleText = "Multi-Select Chip Group"; // Default title
        private Font _titleFont = new Font("Segoe UI", 12, FontStyle.Bold);
        private Color _titleColor = Color.Black;
        private ContentAlignment _titleAlignment = ContentAlignment.TopLeft;
        private int _titleHeight = 30; // Height reserved for the title

        // Chip styling properties
        private ChipVariant _chipVariant = ChipVariant.Filled;
        private ChipColor _chipColor = ChipColor.Default;
        private ChipSize _chipSize = ChipSize.Medium;
        private ChipSelectionMode _selectionMode = ChipSelectionMode.Single;
        private BindingList<SimpleItem> _selectedItems = new BindingList<SimpleItem>();
        
        // Single chip style property instead of individual styling properties
        private ChipStyle _chipStyle = ChipStyle.Default;
        private int _chipBorderWidth = 1;
        private bool _showChipBorders = true;
        
        // Internal color properties (set by ChipStyle)
        private Color _chipBackColor = Color.Empty;
        private Color _chipForeColor = Color.Empty;
        private Color _chipBorderColor = Color.Empty;
        private Color _chipHoverBackColor = Color.Empty;
        private Color _chipHoverForeColor = Color.Empty;
        private Color _chipSelectedBackColor = Color.Empty;
        private Color _chipSelectedForeColor = Color.Empty;
        #endregion

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
            set
            {
                _titleText = value;
                UpdateChipBounds(); // Recalculate layout to account for title space
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The font used for the title text.")]
        public Font TitleFont
        {
            get => _titleFont;
            set
            {
                _titleFont = value;
                UpdateChipBounds(); // Recalculate layout to account for title space
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The color of the title text.")]
        public Color TitleColor
        {
            get => _titleColor;
            set
            {
                _titleColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The alignment of the title text.")]
        public ContentAlignment TitleAlignment
        {
            get => _titleAlignment;
            set
            {
                _titleAlignment = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual variant of the chips (Filled, Text, Outlined).")]
        [DefaultValue(ChipVariant.Filled)]
        public ChipVariant ChipVariant
        {
            get => _chipVariant;
            set
            {
                _chipVariant = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The color of the chip backgrounds.")]
        public ChipColor ChipColor
        {
            get => _chipColor;
            set
            {
                _chipColor = value;
                ApplyThemeToChips();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The size of the chips.")]
        public ChipSize ChipSize
        {
            get => _chipSize;
            set
            {
                _chipSize = value;
                UpdateChipBounds();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("The selection mode of the chip group (Single, Multiple, Toggle).")]
        public ChipSelectionMode SelectionMode
        {
            get => _selectionMode;
            set
            {
                _selectionMode = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The collection of currently selected chip items.")]
        public BindingList<SimpleItem> SelectedItems
        {
            get => _selectedItems;
            set
            {
                _selectedItems = value;
                UpdateSelectedChips();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Chip Style")]
        [Description("Predefined styling preset for chips.")]
        [DefaultValue(ChipStyle.Default)]
        public ChipStyle ChipStyle
        {
            get => _chipStyle;
            set
            {
                _chipStyle = value;
                ApplyChipStyle(value);
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Chip Style")]
        [Description("Width of chip borders.")]
        [DefaultValue(1)]
        public int ChipBorderWidth
        {
            get => _chipBorderWidth;
            set
            {
                _chipBorderWidth = Math.Max(0, value);
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Chip Style")]
        [Description("Whether to show borders around chips.")]
        [DefaultValue(true)]
        public bool ShowChipBorders
        {
            get => _showChipBorders;
            set
            {
                _showChipBorders = value;
                Invalidate();
            }
        }

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }

        // Disable AutoScroll and ensure no scrollbars
        public new bool AutoScroll
        {
            get => false;
            set { } // Do nothing, force disable
        }
        #endregion

        #region Constructor
        public BeepMultiChipGroup():base()
        {
            DoubleBuffered = true;
            AutoSize = true; // Enable auto-sizing to fit all chips
           // Padding = new Padding(5);
            IsRounded = true;
            BorderThickness = 1; // Ensure a border is visible for modern look
            ShowShadow = true; // Add subtle shadow for depth
            // Set a default non-transparent color at design time

            BackColor = Color.White; // Default to white in designer
            AnimationType = DisplayAnimationType.Fade; // Fade animation on show
            CanBeHovered = true;
            CanBePressed = false; // Chips handle selection, not the group
            CanBeFocused = false;

            // Apply default chip style
            ApplyChipStyle(_chipStyle);

            // Initialize ListItems and handle changes
            _chipItems.ListChanged += (s, e) =>
            {
                UpdateChipsFromItems();
                ApplyThemeToChips();
                UpdateChipBounds();
                Invalidate();
            };
        }
        #endregion

        #region Chip Item Class
        private class ChipItem
        {
            public SimpleItem Item { get; set; }
            public Rectangle Bounds { get; set; }
            public bool IsSelected { get; set; }
            public bool IsHovered { get; set; } // Track hover state for HitArea system
            public ChipVariant Variant { get; set; }
            public ChipColor Color { get; set; }
            public ChipSize Size { get; set; }
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
            
            // Recalculate bounds and setup hit areas
            UpdateChipBounds();
        }

        private void UpdateSelectedChips()
        {
            foreach (var chip in _chips)
            {
                chip.IsSelected = _selectedItems.Contains(chip.Item);
            }
            
            // Update single selection for backward compatibility
            _selectedItem = _selectedItems.FirstOrDefault();
            _selectedIndex = _selectedItem != null ? _chipItems.IndexOf(_selectedItem) : -1;
        }
        #endregion

        #region Layout and Drawing
        private void UpdateChipBounds()
        {
            if (_chips == null || !_chips.Any()) return;

            UpdateDrawingRect(); // Ensure DrawingRect is up-to-date
            
            // Use DrawingRect for all calculations
            Rectangle availableRect = DrawingRect;
            
            int x = availableRect.X;
            int y = availableRect.Y + _titleHeight; // Start chips below the title
            int maxHeightInRow = 0;
            int totalHeight = y;
            
            // Get chip height based on size
            int chipHeight = GetChipHeight(_chipSize);

            foreach (var chip in _chips)
            {
                string displayText = chip.Item.Text ?? chip.Item.Name ?? chip.Item.DisplayField ?? string.Empty;
                Size textSize = TextRenderer.MeasureText(displayText, Font);
                int chipWidth = textSize.Width + GetChipPadding(_chipSize); // Size-aware padding

                // Wrap to the next row if necessary (within DrawingRect bounds)
                if (x + chipWidth + _chipPadding > availableRect.Right && x > availableRect.X)
                {
                    x = availableRect.X;
                    y += maxHeightInRow + _chipPadding;
                    maxHeightInRow = 0;
                }

                chip.Bounds = new Rectangle(x, y, chipWidth, chipHeight);
                x += chipWidth + _chipPadding;
                maxHeightInRow = Math.Max(maxHeightInRow, chipHeight);
                totalHeight = y + maxHeightInRow;
            }

            // Update control size based on total height within DrawingRect
            if (AutoSize)
            {
                int requiredHeight = totalHeight - availableRect.Y + Padding.Bottom + _titleHeight;
                Size = new Size(Width, Math.Max(Height, requiredHeight + Padding.Vertical));
            }
            
            // Setup hit areas after bounds calculation
            SetupChipHitAreas();
        }

        /// <summary>
        /// Get chip height based on size setting
        /// </summary>
        private int GetChipHeight(ChipSize size)
        {
            return size switch
            {
                ChipSize.Small => 24,
                ChipSize.Medium => 32,
                ChipSize.Large => 40,
                _ => 32
            };
        }

        /// <summary>
        /// Get chip padding based on size setting
        /// </summary>
        private int GetChipPadding(ChipSize size)
        {
            return size switch
            {
                ChipSize.Small => 16,  // 8px on each side
                ChipSize.Medium => 20, // 10px on each side  
                ChipSize.Large => 24,  // 12px on each side
                _ => 20
            };
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
           
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Work within DrawingRect bounds
            Rectangle availableRect = DrawingRect;

            // Draw the title within DrawingRect
            if (!string.IsNullOrEmpty(_titleText))
            {
                Rectangle titleRect = new Rectangle(availableRect.X, availableRect.Y, availableRect.Width, _titleHeight);
                TextFormatFlags titleFlags = GetTextFormatFlags(_titleAlignment);
                TextRenderer.DrawText(g, _titleText, _titleFont, titleRect, _titleColor, titleFlags);
            }

            // Draw the chips
            foreach (var chip in _chips)
            {
                DrawChip(g, chip);
            }
        }

        /// <summary>
        /// Draw individual chip with variant and color support
        /// </summary>
        private void DrawChip(Graphics g, ChipItem chip)
        {
            string displayText = chip.Item.Text ?? chip.Item.Name ?? chip.Item.DisplayField ?? string.Empty;
            
            // Get colors based on chip color, variant, and state
            var colors = GetChipColors(chip);

            // Draw based on variant
            switch (chip.Variant)
            {
                case ChipVariant.Filled:
                    DrawFilledChip(g, chip, displayText, colors);
                    break;
                case ChipVariant.Text:
                    DrawTextChip(g, chip, displayText, colors);
                    break;
                case ChipVariant.Outlined:
                    DrawOutlinedChip(g, chip, displayText, colors);
                    break;
            }
        }

        private TextFormatFlags GetTextFormatFlags(ContentAlignment alignment)
        {
            TextFormatFlags flags = TextFormatFlags.SingleLine | TextFormatFlags.PreserveGraphicsClipping;

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                    flags |= TextFormatFlags.Left | TextFormatFlags.Top;
                    break;
                case ContentAlignment.TopCenter:
                    flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.Top;
                    break;
                case ContentAlignment.TopRight:
                    flags |= TextFormatFlags.Right | TextFormatFlags.Top;
                    break;
                case ContentAlignment.MiddleLeft:
                    flags |= TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
                    break;
                case ContentAlignment.MiddleCenter:
                    flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                    break;
                case ContentAlignment.MiddleRight:
                    flags |= TextFormatFlags.Right | TextFormatFlags.VerticalCenter;
                    break;
                case ContentAlignment.BottomLeft:
                    flags |= TextFormatFlags.Left | TextFormatFlags.Bottom;
                    break;
                case ContentAlignment.BottomCenter:
                    flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom;
                    break;
                case ContentAlignment.BottomRight:
                    flags |= TextFormatFlags.Right | TextFormatFlags.Bottom;
                    break;
            }

            return flags;
        }

        /// <summary>
        /// Get a rounded rectangle GraphicsPath for chip rendering
        /// </summary>
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int r = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
            int d = r * 2;

            if (r > 0 && rect.Width > 0 && rect.Height > 0)
            {
                path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
            }
            else
            {
                path.AddRectangle(rect);
            }
            return path;
        }
        #endregion

        #region HitArea System Integration
    
        /// <summary>
        /// Setup HitAreas for each chip using BaseControl's HitArea system
        /// </summary>
        private void SetupChipHitAreas()
        {
            // Clear existing hit areas
            ClearHitList();

            // Add hit area for each chip
            for (int i = 0; i < _chips.Count; i++)
            {
                var chip = _chips[i];
                if (chip.Bounds.Width > 0 && chip.Bounds.Height > 0)
                {
                    int chipIndex = i; // Capture index for closure
                    AddHitArea($"Chip_{chipIndex}_{chip.Item.GuidId}", chip.Bounds, null, () => HandleChipClick(chip));
                }
            }
        }

        /// <summary>
        /// Handle chip click through HitArea system with multi-selection support
        /// </summary>
        private void HandleChipClick(ChipItem chip)
        {
            if (chip?.Item == null) return;

            switch (_selectionMode)
            {
                case ChipSelectionMode.Single:
                    // Clear all selections, select only this one
                    foreach (var c in _chips)
                        c.IsSelected = false;
                    chip.IsSelected = true;
                    _selectedItem = chip.Item;
                    _selectedItems.Clear();
                    _selectedItems.Add(chip.Item);
                    break;

                case ChipSelectionMode.Multiple:
                    // Toggle this chip, keep others
                    chip.IsSelected = !chip.IsSelected;
                    if (chip.IsSelected)
                    {
                        if (!_selectedItems.Contains(chip.Item))
                            _selectedItems.Add(chip.Item);
                    }
                    else
                    {
                        _selectedItems.Remove(chip.Item);
                    }
                    _selectedItem = _selectedItems.FirstOrDefault();
                    break;

                case ChipSelectionMode.Toggle:
                    // Just toggle this chip
                    chip.IsSelected = !chip.IsSelected;
                    if (chip.IsSelected)
                    {
                        _selectedItem = chip.Item;
                        _selectedItems.Clear();
                        _selectedItems.Add(chip.Item);
                    }
                    else
                    {
                        _selectedItem = null;
                        _selectedItems.Clear();
                    }
                    break;
            }

            OnSelectedItemChanged(_selectedItem);
            Invalidate();
        }

        /// <summary>
        /// Override OnMouseDown to prevent whole control click when chips are clicked
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Check if click is on any chip
            bool clickedOnChip = _chips.Any(chip => chip.Bounds.Contains(e.Location));
            
            if (!clickedOnChip)
            {
                // Only call base if not clicking on a chip
                base.OnMouseDown(e);
            }
        }

        /// <summary>
        /// Handle hover state changes through BaseControl
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Let BaseControl handle hit area detection first
            base.OnMouseMove(e);
            
            // Update hover states based on hit areas
            UpdateHoverStates(e.Location);
        }

        /// <summary>
        /// Update chip hover states based on current mouse position
        /// </summary>
        private void UpdateHoverStates(Point mouseLocation)
        {
            bool needsRedraw = false;
            
            foreach (var chip in _chips)
            {
                bool wasHovered = chip.IsHovered;
                chip.IsHovered = chip.Bounds.Contains(mouseLocation);
                
                if (wasHovered != chip.IsHovered)
                {
                    needsRedraw = true;
                }
            }
            
            if (needsRedraw)
            {
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            
            // Clear all hover states
            bool needsRedraw = false;
            foreach (var chip in _chips)
            {
                if (chip.IsHovered)
                {
                    chip.IsHovered = false;
                    needsRedraw = true;
                }
            }
            
            if (needsRedraw)
            {
                Invalidate();
            }
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

        public override object GetValue()
        {
            return SelectedItem;
        }

        public override void ClearValue()
        {
            SelectedItem = null;
            _selectedIndex = -1;
            _chips.ForEach(c => c.IsSelected = false);
            Invalidate();
        }
        #endregion

        #region Theming
        public override void ApplyTheme()
        {
            // Update title color based on theme
            _titleColor = _currentTheme.CardTitleForeColor;
            BackColor = _currentTheme.ButtonBackColor;
            ApplyThemeToChips();
            Invalidate();
        }

        private void ApplyThemeToChips()
        {
            // Theme colors are now applied directly in DrawContent method
            // This method kept for future chip-specific theme customizations
            Invalidate();
        }

        /// <summary>
        /// Apply predefined chip styling based on ChipStyle enum
        /// </summary>
        private void ApplyChipStyle(ChipStyle style)
        {
            switch (style)
            {
                case ChipStyle.Default:
                    // Use theme colors (Color.Empty means use theme)
                    _chipBackColor = Color.Empty;
                    _chipForeColor = Color.Empty;
                    _chipBorderColor = Color.Empty;
                    _chipHoverBackColor = Color.Empty;
                    _chipHoverForeColor = Color.Empty;
                    _chipSelectedBackColor = Color.Empty;
                    _chipSelectedForeColor = Color.Empty;
                    _showChipBorders = true;
                    _chipBorderWidth = 1;
                    break;

                case ChipStyle.Modern:
                    // Modern flat design with subtle colors
                    _chipBackColor = Color.FromArgb(248, 249, 250);
                    _chipForeColor = Color.FromArgb(52, 58, 64);
                    _chipBorderColor = Color.FromArgb(222, 226, 230);
                    _chipHoverBackColor = Color.FromArgb(233, 236, 239);
                    _chipHoverForeColor = Color.FromArgb(33, 37, 41);
                    _chipSelectedBackColor = Color.FromArgb(0, 123, 255);
                    _chipSelectedForeColor = Color.White;
                    _showChipBorders = true;
                    _chipBorderWidth = 1;
                    break;

                case ChipStyle.Classic:
                    // Classic design with defined borders
                    _chipBackColor = Color.White;
                    _chipForeColor = Color.FromArgb(73, 80, 87);
                    _chipBorderColor = Color.FromArgb(108, 117, 125);
                    _chipHoverBackColor = Color.FromArgb(248, 249, 250);
                    _chipHoverForeColor = Color.FromArgb(52, 58, 64);
                    _chipSelectedBackColor = Color.FromArgb(40, 167, 69);
                    _chipSelectedForeColor = Color.White;
                    _showChipBorders = true;
                    _chipBorderWidth = 2;
                    break;

                case ChipStyle.Minimalist:
                    // Clean minimal design
                    _chipBackColor = Color.Transparent;
                    _chipForeColor = Color.FromArgb(108, 117, 125);
                    _chipBorderColor = Color.FromArgb(222, 226, 230);
                    _chipHoverBackColor = Color.FromArgb(248, 249, 250);
                    _chipHoverForeColor = Color.FromArgb(73, 80, 87);
                    _chipSelectedBackColor = Color.FromArgb(73, 80, 87);
                    _chipSelectedForeColor = Color.White;
                    _showChipBorders = false;
                    _chipBorderWidth = 0;
                    break;

                case ChipStyle.Colorful:
                    // Vibrant colors
                    _chipBackColor = Color.FromArgb(255, 193, 7);
                    _chipForeColor = Color.FromArgb(52, 58, 64);
                    _chipBorderColor = Color.FromArgb(255, 193, 7);
                    _chipHoverBackColor = Color.FromArgb(255, 235, 59);
                    _chipHoverForeColor = Color.FromArgb(33, 37, 41);
                    _chipSelectedBackColor = Color.FromArgb(255, 87, 34);
                    _chipSelectedForeColor = Color.White;
                    _showChipBorders = true;
                    _chipBorderWidth = 1;
                    break;

                case ChipStyle.Professional:
                    // Professional business look
                    _chipBackColor = Color.FromArgb(242, 244, 246);
                    _chipForeColor = Color.FromArgb(55, 65, 81);
                    _chipBorderColor = Color.FromArgb(209, 213, 219);
                    _chipHoverBackColor = Color.FromArgb(229, 231, 235);
                    _chipHoverForeColor = Color.FromArgb(31, 41, 55);
                    _chipSelectedBackColor = Color.FromArgb(59, 130, 246);
                    _chipSelectedForeColor = Color.White;
                    _showChipBorders = true;
                    _chipBorderWidth = 1;
                    break;

                case ChipStyle.Soft:
                    // Soft pastel colors
                    _chipBackColor = Color.FromArgb(252, 241, 241);
                    _chipForeColor = Color.FromArgb(101, 116, 139);
                    _chipBorderColor = Color.FromArgb(255, 182, 193);
                    _chipHoverBackColor = Color.FromArgb(255, 228, 225);
                    _chipHoverForeColor = Color.FromArgb(71, 85, 105);
                    _chipSelectedBackColor = Color.FromArgb(244, 114, 182);
                    _chipSelectedForeColor = Color.White;
                    _showChipBorders = true;
                    _chipBorderWidth = 1;
                    break;

                case ChipStyle.HighContrast:
                    // High contrast for accessibility
                    _chipBackColor = Color.White;
                    _chipForeColor = Color.Black;
                    _chipBorderColor = Color.Black;
                    _chipHoverBackColor = Color.FromArgb(240, 240, 240);
                    _chipHoverForeColor = Color.Black;
                    _chipSelectedBackColor = Color.Black;
                    _chipSelectedForeColor = Color.White;
                    _showChipBorders = true;
                    _chipBorderWidth = 2;
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Color scheme for a chip in a specific state
        /// </summary>
        private struct ChipColorScheme
        {
            public Color BackColor;
            public Color ForeColor;
            public Color BorderColor;
        }

        /// <summary>
        /// Semantic color palette for chip styling
        /// </summary>
        private struct SemanticColors
        {
            public Color Primary;
            public Color OnPrimary;
            public Color Surface;
            public Color OnSurface;
            public Color Outline;
        }

        /// <summary>
        /// Get color scheme for chip based on color, state, and variant
        /// </summary>
        private ChipColorScheme GetChipColors(ChipItem chip)
        {
            var baseColors = GetSemanticColors(chip.Color);
            
            if (chip.IsSelected)
            {
                return new ChipColorScheme
                {
                    BackColor = _chipSelectedBackColor != Color.Empty ? _chipSelectedBackColor : baseColors.Primary,
                    ForeColor = _chipSelectedForeColor != Color.Empty ? _chipSelectedForeColor : baseColors.OnPrimary,
                    BorderColor = _chipBorderColor != Color.Empty ? _chipBorderColor : baseColors.Primary
                };
            }
            else if (chip.IsHovered)
            {
                return new ChipColorScheme
                {
                    BackColor = _chipHoverBackColor != Color.Empty ? _chipHoverBackColor : BlendColors(baseColors.Surface, baseColors.Primary, 0.1f),
                    ForeColor = _chipHoverForeColor != Color.Empty ? _chipHoverForeColor : baseColors.OnSurface,
                    BorderColor = _chipBorderColor != Color.Empty ? _chipBorderColor : baseColors.Primary
                };
            }
            else
            {
                return new ChipColorScheme
                {
                    BackColor = _chipBackColor != Color.Empty ? _chipBackColor : baseColors.Surface,
                    ForeColor = _chipForeColor != Color.Empty ? _chipForeColor : baseColors.OnSurface,
                    BorderColor = _chipBorderColor != Color.Empty ? _chipBorderColor : baseColors.Outline
                };
            }
        }

        /// <summary>
        /// Get semantic colors based on chip color and theme
        /// </summary>
        private SemanticColors GetSemanticColors(ChipColor chipColor)
        {
            return chipColor switch
            {
                ChipColor.Primary => new SemanticColors
                {
                    Primary = _currentTheme.ButtonSelectedBackColor,
                    OnPrimary = _currentTheme.ButtonSelectedForeColor,
                    Surface = Color.FromArgb(240, _currentTheme.ButtonSelectedBackColor),
                    OnSurface = _currentTheme.ButtonSelectedBackColor,
                    Outline = _currentTheme.ButtonSelectedBackColor
                },
                ChipColor.Secondary => new SemanticColors
                {
                    Primary = _currentTheme.AccentColor,
                    OnPrimary = Color.White,
                    Surface = Color.FromArgb(240, _currentTheme.AccentColor),
                    OnSurface = _currentTheme.AccentColor,
                    Outline = _currentTheme.AccentColor
                },
                ChipColor.Success => new SemanticColors
                {
                    Primary = Color.FromArgb(76, 175, 80),   // Green
                    OnPrimary = Color.White,
                    Surface = Color.FromArgb(240, 248, 240),
                    OnSurface = Color.FromArgb(46, 125, 50),
                    Outline = Color.FromArgb(76, 175, 80)
                },
                ChipColor.Warning => new SemanticColors
                {
                    Primary = Color.FromArgb(255, 152, 0),   // Orange
                    OnPrimary = Color.White,
                    Surface = Color.FromArgb(255, 248, 225),
                    OnSurface = Color.FromArgb(230, 81, 0),
                    Outline = Color.FromArgb(255, 152, 0)
                },
                ChipColor.Error => new SemanticColors
                {
                    Primary = Color.FromArgb(244, 67, 54),   // Red
                    OnPrimary = Color.White,
                    Surface = Color.FromArgb(255, 235, 238),
                    OnSurface = Color.FromArgb(198, 40, 40),
                    Outline = Color.FromArgb(244, 67, 54)
                },
                ChipColor.Info => new SemanticColors
                {
                    Primary = Color.FromArgb(33, 150, 243),  // Blue
                    OnPrimary = Color.White,
                    Surface = Color.FromArgb(227, 242, 253),
                    OnSurface = Color.FromArgb(25, 118, 210),
                    Outline = Color.FromArgb(33, 150, 243)
                },
                ChipColor.Dark => new SemanticColors
                {
                    Primary = Color.FromArgb(66, 66, 66),    // Dark gray
                    OnPrimary = Color.White,
                    Surface = Color.FromArgb(245, 245, 245),
                    OnSurface = Color.FromArgb(66, 66, 66),
                    Outline = Color.FromArgb(66, 66, 66)
                },
                _ => new SemanticColors // Default
                {
                    Primary = _currentTheme.ButtonBackColor,
                    OnPrimary = _currentTheme.ButtonForeColor,
                    Surface = _currentTheme.ButtonBackColor,
                    OnSurface = _currentTheme.ButtonForeColor,
                    Outline = _currentTheme.BorderColor
                }
            };
        }

        /// <summary>
        /// Blend two colors with specified opacity
        /// </summary>
        private Color BlendColors(Color baseColor, Color blendColor, float opacity)
        {
            opacity = Math.Max(0, Math.Min(1, opacity));
            
            int r = (int)(baseColor.R * (1 - opacity) + blendColor.R * opacity);
            int g = (int)(baseColor.G * (1 - opacity) + blendColor.G * opacity);
            int b = (int)(baseColor.B * (1 - opacity) + blendColor.B * opacity);
            
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Draw filled variant chip
        /// </summary>
        private void DrawFilledChip(Graphics g, ChipItem chip, string text, ChipColorScheme colors)
        {
            using (GraphicsPath path = GetRoundedRectPath(chip.Bounds, _chipCornerRadius))
            {
                // Fill background
                using (SolidBrush brush = new SolidBrush(colors.BackColor))
                {
                    g.FillPath(brush, path);
                }

                // Draw border if enabled and (hovered, selected, or custom border)
                if (_showChipBorders && (chip.IsHovered || chip.IsSelected || _chipBorderColor != Color.Empty))
                {
                    int borderWidth = chip.IsHovered ? Math.Max(_chipBorderWidth, 2) : _chipBorderWidth;
                    using (Pen pen = new Pen(colors.BorderColor, borderWidth))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Draw text
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
            TextRenderer.DrawText(g, text, Font, chip.Bounds, colors.ForeColor, flags);
        }

        /// <summary>
        /// Draw text variant chip (transparent background)
        /// </summary>
        private void DrawTextChip(Graphics g, ChipItem chip, string text, ChipColorScheme colors)
        {
            // Text variant has transparent background, only show background on hover/select
            if (chip.IsHovered || chip.IsSelected)
            {
                var bgColor = Color.FromArgb(32, colors.BackColor); // Semi-transparent
                using (GraphicsPath path = GetRoundedRectPath(chip.Bounds, _chipCornerRadius))
                {
                    using (SolidBrush brush = new SolidBrush(bgColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }

            // Draw text
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
            TextRenderer.DrawText(g, text, Font, chip.Bounds, colors.ForeColor, flags);
        }

        /// <summary>
        /// Draw outlined variant chip
        /// </summary>
        private void DrawOutlinedChip(Graphics g, ChipItem chip, string text, ChipColorScheme colors)
        {
            using (GraphicsPath path = GetRoundedRectPath(chip.Bounds, _chipCornerRadius))
            {
                // Fill background on hover/select
                if (chip.IsHovered || chip.IsSelected)
                {
                    var bgColor = Color.FromArgb(16, colors.BackColor); // Very light background
                    using (SolidBrush brush = new SolidBrush(bgColor))
                    {
                        g.FillPath(brush, path);
                    }
                }

                // Always draw border for outlined variant if borders are enabled
                if (_showChipBorders)
                {
                    int borderWidth = chip.IsSelected ? Math.Max(_chipBorderWidth, 2) : _chipBorderWidth;
                    using (Pen pen = new Pen(colors.BorderColor, borderWidth))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Draw text
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
            TextRenderer.DrawText(g, text, Font, chip.Bounds, colors.ForeColor, flags);
        }
    }
}