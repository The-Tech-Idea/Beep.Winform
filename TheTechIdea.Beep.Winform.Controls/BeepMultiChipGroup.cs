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

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepMultiChipGroup : BeepControl
    {
        #region Fields
        private BindingList<SimpleItem> _chipItems = new BindingList<SimpleItem>(); // List to store the chips
        private List<ChipItem> _chips = new List<ChipItem>(); // Internal list for rendering
        private readonly int _chipPadding = 5; // Padding between chips
        private readonly int _chipHeight = 30; // Fixed height for each chip
        private readonly int _chipCornerRadius = 15; // Radius for rounded chips
        private SimpleItem _selectedItem; // Currently selected item
        private int _selectedIndex = -1; // Selected index
        private ChipItem _hoveredChip = null; // Track the currently hovered chip

        // Title-related fields
        private string _titleText = "Multi-Select Chip Group"; // Default title
        private Font _titleFont = new Font("Segoe UI", 12, FontStyle.Bold);
        private Color _titleColor = Color.Black;
        private ContentAlignment _titleAlignment = ContentAlignment.TopLeft;
        private int _titleHeight = 30; // Height reserved for the title
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

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
            //OnValueChanged?.Invoke(this, new BeepComponentEventArgs(this, "SelectedItem", null, selectedItem));
        }

        // Disable AutoScroll and ensure no scrollbars
        public new bool AutoScroll
        {
            get => false;
            set { } // Do nothing, force disable
        }

      
        #endregion

        #region Constructor
        public BeepMultiChipGroup()
        {
            DoubleBuffered = true;
            AutoSize = true; // Enable auto-sizing to fit all chips
            Padding = new Padding(5);
            IsRounded = true;
            BorderThickness = 1; // Ensure a border is visible for modern look
            ShowShadow = true; // Add subtle shadow for depth
            // Set a default non-transparent color at design time

            BackColor = Color.White; // Default to white in designer
            AnimationType = DisplayAnimationType.Fade; // Fade animation on show
            CanBeHovered = true;
            CanBePressed = false; // Chips handle selection, not the group
            CanBeFocused = false;

            // Disable scrollbars (remove or hide them)
            vScrollBar = null;
            hScrollBar = null;

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
            public bool IsHovered { get; set; } // Track hover state
            public Color BackColor { get; set; } // Theme-based back color
            public Color ForeColor { get; set; } // Theme-based fore color
            public Color HoverBackColor { get; set; } // Theme-based hover back color
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
                    IsSelected = _selectedItem != null && _selectedItem.Equals(item),
                    IsHovered = false
                });
            }
        }

        private void UpdateSelectedChips()
        {
            foreach (var chip in _chips)
            {
                chip.IsSelected = _selectedItem != null && _selectedItem.Equals(chip.Item);
            }
        }
        #endregion

        #region Layout and Drawing
        private void UpdateChipBounds()
        {
            if (_chips == null || !_chips.Any()) return;

            UpdateDrawingRect(); // Ensure DrawingRect is up-to-date
            int x = DrawingRect.X;
            int y = DrawingRect.Y + _titleHeight; // Start chips below the title
            int maxHeightInRow = 0;
            int totalHeight = DrawingRect.Y + _titleHeight;

            foreach (var chip in _chips)
            {
                string displayText = chip.Item.Text ?? chip.Item.Name ?? chip.Item.Display ?? string.Empty;
                Size textSize = TextRenderer.MeasureText(displayText, Font);
                int chipWidth = textSize.Width + 20; // Add padding to text width (no minimum width)

                // Wrap to the next row if necessary
                if (x + chipWidth + _chipPadding > DrawingRect.Right && x > DrawingRect.X)
                {
                    x = DrawingRect.X;
                    y += maxHeightInRow + _chipPadding;
                    maxHeightInRow = 0;
                }

                chip.Bounds = new Rectangle(x, y, chipWidth, _chipHeight);
                x += chipWidth + _chipPadding;
                maxHeightInRow = Math.Max(maxHeightInRow, _chipHeight);
                totalHeight = y + maxHeightInRow;
            }

            // Update control size based on total height within DrawingRect
            if (AutoSize)
            {
                int totalWidth = DrawingRect.Width;
                int requiredHeight = totalHeight - DrawingRect.Y + Padding.Bottom;
                Size = new Size(totalWidth + Padding.Horizontal, Math.Max(Height, requiredHeight + Padding.Vertical));
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDrawingRect(); // Update DrawingRect on resize
            UpdateChipBounds();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Handle background, borders, shadows, etc., which draws outside DrawingRect
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Clip drawing to DrawingRect to ensure all chip drawing stays within it
            g.SetClip(DrawingRect);

            // Draw the title
            if (!string.IsNullOrEmpty(_titleText))
            {
                Rectangle titleRect = new Rectangle(DrawingRect.X, DrawingRect.Y, DrawingRect.Width, _titleHeight);
                TextFormatFlags titleFlags = GetTextFormatFlags(_titleAlignment);
                TextRenderer.DrawText(g, _titleText, _titleFont, titleRect, _titleColor, titleFlags);
            }

            // Draw the chips
            foreach (var chip in _chips)
            {
                string displayText = chip.Item.Text ?? chip.Item.Name ?? chip.Item.Display ?? string.Empty;

                // Determine the chip's appearance based on its state
                Color chipBackColor = chip.IsSelected
                    ? _currentTheme.ButtonSelectedForeColor
                    : (chip.IsHovered
                        ? _currentTheme.ButtonHoverBackColor
                        : chip.BackColor);
                Color chipForeColor = chip.IsSelected
                    ? _currentTheme.ButtonSelectedForeColor
                    : (chip.IsHovered
                        ? _currentTheme.ButtonHoverForeColor
                        : chip.ForeColor);

                // Slightly scale the chip when hovered, ensuring it stays within DrawingRect
                Rectangle drawBounds = chip.Bounds;
                if (chip.IsHovered)
                {
                    float scaleFactor = 1.05f; // 5% larger when hovered
                    int newWidth = (int)(chip.Bounds.Width * scaleFactor);
                    int newHeight = (int)(chip.Bounds.Height * scaleFactor);
                    int deltaX = (newWidth - chip.Bounds.Width) / 2;
                    int deltaY = (newHeight - chip.Bounds.Height) / 2;
                    drawBounds = new Rectangle(
                        Math.Max(chip.Bounds.X - deltaX, DrawingRect.X),
                        Math.Max(chip.Bounds.Y - deltaY, DrawingRect.Y),
                        Math.Min(newWidth, DrawingRect.Right - drawBounds.X),
                        Math.Min(newHeight, DrawingRect.Bottom - drawBounds.Y));
                }

                // Ensure drawBounds is within DrawingRect
                drawBounds.Intersect(DrawingRect);

                // Draw the chip as a rounded rectangle
                using (GraphicsPath path = GetRoundedRectPath(drawBounds, _chipCornerRadius))
                {
                    using (SolidBrush brush = new SolidBrush(chipBackColor))
                    {
                        g.FillPath(brush, path);
                    }

                    if (BorderThickness > 0 || chip.IsHovered)
                    {
                        using (Pen pen = new Pen(chip.IsHovered ? _currentTheme.HoverLinkColor : (chip.IsSelected ? _currentTheme.ActiveBorderColor : BorderColor), chip.IsHovered ? 2 : BorderThickness))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }

                // Draw the chip text (centered within the original bounds, clipped to DrawingRect)
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                Rectangle textBounds = chip.Bounds;
                textBounds.Intersect(DrawingRect);
                TextRenderer.DrawText(g, displayText, Font, textBounds, chipForeColor, flags);
            }

            // Reset the clip region
            g.ResetClip();
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
        #endregion

        #region Mouse Interaction
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && DrawingRect.Contains(e.Location))
            {
                foreach (var chip in _chips)
                {
                    if (chip.Bounds.Contains(e.Location))
                    {
                        SelectedItem = chip.Item; // Update selected item
                        break;
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool wasHovered = _hoveredChip != null;
            ChipItem previouslyHoveredChip = _hoveredChip;
            _hoveredChip = null;

            if (DrawingRect.Contains(e.Location))
            {
                foreach (var chip in _chips)
                {
                    bool isHovered = chip.Bounds.Contains(e.Location);
                    chip.IsHovered = isHovered;
                    if (isHovered)
                    {
                        _hoveredChip = chip;
                        ShowToolTipIfExists(); // Show tooltip if the chip has one
                    }
                }
            }
            else
            {
                foreach (var chip in _chips)
                {
                    chip.IsHovered = false;
                }
            }

            if (wasHovered != (_hoveredChip != null) || previouslyHoveredChip != _hoveredChip)
            {
                Invalidate(); // Redraw to reflect hover state changes
            }

            IsHovered = _hoveredChip != null;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            bool wasHovered = _hoveredChip != null;
            _hoveredChip = null;
            foreach (var chip in _chips)
            {
                chip.IsHovered = false;
            }
            IsHovered = false;
            if (wasHovered)
            {
                Invalidate(); // Redraw to remove hover effects
            }
            HideToolTip();
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
            base.ApplyTheme();
            // Apply runtime transparency if supported
           
            // Update title color based on theme
            _titleColor = _currentTheme.LabelForeColor ;
            BackColor = _currentTheme.LabelBackColor;
            ApplyThemeToChips();
            Invalidate();
        }

        private void ApplyThemeToChips()
        {
            foreach (var chip in _chips)
            {
                chip.BackColor = _currentTheme.ButtonBackColor;
                chip.ForeColor = _currentTheme.ButtonForeColor;
                chip.HoverBackColor = _currentTheme.ButtonHoverBackColor;
            }
        }
        #endregion
    }
}