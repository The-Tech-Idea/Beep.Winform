using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Scroll List")]
    [Description("A carousel-style scrollable list control using BeepButtons for items, supporting vertical or horizontal orientation without a visible scrollbar.")]
    public class BeepScrollList : BeepControl
    {
        #region Fields
        private BindingList<SimpleItem> _listItems = new BindingList<SimpleItem>();
        private List<BeepButton> _buttonItems = new List<BeepButton>();
        private int _itemSize = 30; // Height for vertical, width for horizontal
        private int _visibleItemsCount;
        private float _scrollOffset = 0; // Float for smoother scrolling
        private int _selectedIndex = -1;
        private ScrollOrientation _orientation = ScrollOrientation.VerticalScroll;
        private bool _isDragging = false;
        private Point _dragStartPoint;
        private float _dragStartOffset;
        private int padding=3;
        private const float ScrollSpeed = 0.5f; // Adjust for scroll sensitivity
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The list of items to display.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => _listItems;
            set
            {
                _listItems = value ?? new BindingList<SimpleItem>();
                UpdateButtonItems();
                UpdateScrollBounds();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The size of each item (height for vertical, width for horizontal).")]
        [DefaultValue(30)]
        public int ItemSize
        {
            get => _itemSize;
            set
            {
                _itemSize = value;
                UpdateButtonItems();
                UpdateScrollBounds();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The orientation of the scroll list (vertical or horizontal).")]
        [DefaultValue(ScrollOrientation.VerticalScroll)]
        public ScrollOrientation Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                UpdateButtonItems();
                UpdateScrollBounds();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The index of the currently selected item.")]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= -1 && value < _listItems.Count)
                {
                    // Update the previous selected button
                    if (_selectedIndex >= 0 && _selectedIndex < _buttonItems.Count)
                    {
                        _buttonItems[_selectedIndex].IsSelected = false;
                    }

                    _selectedIndex = value;

                    // Update the new selected button
                    if (_selectedIndex >= 0 && _selectedIndex < _buttonItems.Count)
                    {
                        _buttonItems[_selectedIndex].IsSelected = true;
                    }

                    OnItemSelected();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The currently selected item.")]
        public SimpleItem SelectedItem
        {
            get => _selectedIndex >= 0 && _selectedIndex < _listItems.Count ? _listItems[_selectedIndex] : null;
            set
            {
                int index = _listItems.IndexOf(value);
                if (index >= 0)
                {
                    SelectedIndex = index;
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler<SimpleItem> ItemSelected;

        protected virtual void OnItemSelected()
        {
            ItemSelected?.Invoke(this, SelectedItem);
        }
        #endregion

        #region Constructor
        public BeepScrollList()
        {
            InitializeControl();
            ListItems.ListChanged += Items_ListChanged;
        }

        private void Items_ListChanged(object sender, ListChangedEventArgs e) => UpdateButtonItems();

        private void InitializeControl()
        {
            DoubleBuffered = true;
            UpdateScrollBounds();
        }
        #endregion

        #region Button Management
        private void UpdateButtonItems()
        {
            // Clear existing buttons
            foreach (var button in _buttonItems)
            {
                button.Click -= Button_Click;
                Controls.Remove(button);
                button.Dispose();
            }
            _buttonItems.Clear();

            // Create a BeepButton for each SimpleItem
            for (int i = 0; i < _listItems.Count; i++)
            {
                var item = _listItems[i];
                var button = new BeepButton
                {
                    Text = item.Text,
                    IsChild = false,
                    ShowAllBorders = false,
                    IsShadowAffectedByTheme = false,
                    Theme = Theme,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                if(!string.IsNullOrEmpty(item.ImagePath))
                {
                    button.ImagePath = item.ImagePath;
                    button.ImageAlign = ContentAlignment.MiddleLeft;
                    button.TextImageRelation = TextImageRelation.ImageBeforeText;
                }
                if (_orientation == ScrollOrientation.VerticalScroll)
                {
                    button.Size = new Size(DrawingRect.Width-padding, _itemSize);
                }
                else
                {
                    button.Size = new Size(_itemSize, DrawingRect.Height-padding);
                }
                button.Click += Button_Click;
                _buttonItems.Add(button);
                Controls.Add(button);
            }

            // Update selected button
            if (_selectedIndex >= 0 && _selectedIndex < _buttonItems.Count)
            {
                _buttonItems[_selectedIndex].IsSelected = true;
            }

            UpdateButtonPositions();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var button = sender as BeepButton;
            if (button != null)
            {
                int index = _buttonItems.IndexOf(button);
                if (index >= 0)
                {
                    SelectedIndex = index;
                }
            }
        }

        private void UpdateButtonPositions()
        {
            int startIndex = (int)Math.Floor(_scrollOffset);
            float offsetFraction = _scrollOffset - startIndex;
            int endIndex = Math.Min(startIndex + _visibleItemsCount + 1, _listItems.Count);

            for (int i = 0; i < _buttonItems.Count; i++)
            {
                var button = _buttonItems[i];
                if (i < startIndex || i >= endIndex)
                {
                    button.Visible = false;
                    continue;
                }

                button.Visible = true;
                float position;
                if (_orientation == ScrollOrientation.VerticalScroll)
                {
                    position = (i - _scrollOffset) * _itemSize;
                    button.Location = new Point(padding, (int)position);
                }
                else
                {
                    position = (i - _scrollOffset) * _itemSize;
                    button.Location = new Point((int)position, padding);
                }
            }
        }
        #endregion

        #region Layout and Drawing
        private void UpdateScrollBounds()
        {
            UpdateDrawingRect();
            if (_orientation == ScrollOrientation.VerticalScroll)
            {
                _visibleItemsCount = DrawingRect.Height / _itemSize;
            }
            else
            {
                _visibleItemsCount = DrawingRect.Width / _itemSize;
            }

            // Clamp scroll offset
            _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, _listItems.Count - _visibleItemsCount));
            UpdateButtonPositions();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateButtonItems(); // Recompute button sizes on resize
            UpdateScrollBounds();
            Invalidate();
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Enable anti-aliasing for smoother rendering
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Fill the background
            using (SolidBrush backgroundBrush = new SolidBrush(_currentTheme.ButtonBackColor))
            {
                graphics.FillRectangle(backgroundBrush, rectangle);
            }

            // Draw border (optional)
            if (BorderThickness > 0)
            {
                using (Pen borderPen = new Pen(_currentTheme.BorderColor, BorderThickness))
                {
                    graphics.DrawRectangle(borderPen, rectangle);
                }
            }

            // The BeepButton controls handle their own rendering
            // We don't need to draw items manually since UpdateButtonPositions manages visibility and positioning
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Background and border are handled by Draw, buttons handle their own rendering
        }
        #endregion

        #region Mouse and Scroll Interaction
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                // Start dragging
                _isDragging = true;
                _dragStartPoint = e.Location;
                _dragStartOffset = _scrollOffset;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDragging)
            {
                float delta;
                if (_orientation == ScrollOrientation.VerticalScroll)
                {
                    delta = (e.Y - _dragStartPoint.Y) / (float)_itemSize;
                }
                else
                {
                    delta = (e.X - _dragStartPoint.X) / (float)_itemSize;
                }
                _scrollOffset = _dragStartOffset - delta;
                _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, _listItems.Count - _visibleItemsCount));
                UpdateButtonPositions();
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = false;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            float delta = (e.Delta / 120f) * ScrollSpeed; // Adjust scroll speed
            _scrollOffset -= delta;
            _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, _listItems.Count - _visibleItemsCount));
            UpdateButtonPositions();
            Invalidate();
        }
        #endregion

        #region Value Management
        public override void SetValue(object value)
        {
            if (value is SimpleItem item)
            {
                SelectedItem = item;
            }
            else if (value != null)
            {
                var item1 = ListItems.FirstOrDefault(i => i.Value?.ToString() == value.ToString());
                if (item1 != null)
                {
                    SelectedItem = item1;
                }
            }
        }

        public override object GetValue()
        {
            return SelectedItem;
        }
        #endregion

        #region Theme and Disposal
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor = _currentTheme.ButtonBackColor;
            ParentBackColor = _currentTheme.ButtonBackColor;
            foreach (var button in _buttonItems)
            {
                button.Theme = Theme;
            }
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var button in _buttonItems)
                {
                    button.Click -= Button_Click;
                    Controls.Remove(button);
                    button.Dispose();
                }
                _buttonItems.Clear();
            }
            base.Dispose(disposing);
        }
        #endregion
    }

}