using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
 

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Dock")]
    [Category("Beep Controls")]
    [Description("Enhanced docking control with smooth animations and hit area support")]
    public class BeepDock : BeepControl
    {
        #region Fields
        private BeepButton button;
        private BeepLabel label;
        private BeepImage image;

        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        private SimpleItem _selectedItem;
        private int _selectedIndex = -1;
        private string _hoveredItemName;

        private Dictionary<string, DockItemState> _itemStates = new Dictionary<string, DockItemState>();
        private Timer animationTimer;

        private int _itemSize = 50;
        private int _dockHeight = 60;
        private int _hoverOffset = 15;
        private int _dockCornerRadius = 15;
        private int _spacing = 10;
        private float _maxScale = 1.4f;
        private float _clickScaleFactor = 1.1f;
        private float _animationSpeed = 0.15f;
        private DockPosition _position = DockPosition.Bottom;
        private DockOrientation _orientation = DockOrientation.Horizontal;
        #endregion

        #region Properties
        [Category("Beep Dock Appearance")]
        [Description("Size of dock items")]
        public int ItemSize
        {
            get => _itemSize;
            set
            {
                _itemSize = value;
                UpdateLayout();
                Invalidate();
            }
        }

        [Category("Beep Dock Appearance")]
        [Description("Height of the dock container")]
        public int DockHeight
        {
            get => _dockHeight;
            set
            {
                _dockHeight = value;
                UpdateLayout();
                Invalidate();
            }
        }

        [Category("Beep Dock Appearance")]
        [Description("Spacing between dock items")]
        public int Spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;
                UpdateLayout();
                Invalidate();
            }
        }

        [Category("Beep Dock Behavior")]
        [Description("Maximum scale factor for hovered items")]
        public float MaxScale
        {
            get => _maxScale;
            set
            {
                _maxScale = value;
                Invalidate();
            }
        }

        [Category("Beep Dock Behavior")]
        [Description("Position of the dock")]
        public DockPosition Position
        {
            get => _position;
            set
            {
                _position = value;
                UpdateLayout();
                Invalidate();
            }
        }

        [Category("Beep Dock Behavior")]
        [Description("Orientation of the dock")]
        public DockOrientation Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                UpdateLayout();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("Collection of dock items")]
        public BindingList<SimpleItem> ListItems
        {
            get => items;
            set
            {
                if (value != null && value != items)
                {
                    items.ListChanged -= Items_ListChanged;
                    items = value;
                    items.ListChanged += Items_ListChanged;
                    InitializeItems();
                }
            }
        }

        [Browsable(false)]
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnSelectedItemChanged(_selectedItem);
                Invalidate();
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0 && value < items.Count)
                {
                    _selectedIndex = value;
                    SelectedItem = items[_selectedIndex];
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }
        #endregion

        #region Constructor
        public BeepDock()
        {
            DoubleBuffered = true;
            IsChild = true;
            IsFrameless = true;
            ShowAllBorders = false;
            IsBorderAffectedByTheme = false;

            // Initialize drawing components
            InitializeDrawingComponents();

            items.ListChanged += Items_ListChanged;

            animationTimer = new Timer { Interval = 16 };
            animationTimer.Tick += AnimateScaling;
            animationTimer.Start();

            UpdateLayout();
        }

        private void InitializeDrawingComponents()
        {
            button = new BeepButton
            {
                IsChild = true,
                ShowAllBorders = false,
                ShowShadow = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageAboveText,
                Theme = this.Theme,
                ApplyThemeOnImage = true
            };

            label = new BeepLabel
            {
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Theme = this.Theme
            };

            image = new BeepImage
            {
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = false,
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                Theme = this.Theme
            };
        }
        #endregion

        #region Item Management
        private void Items_ListChanged(object sender, ListChangedEventArgs e)
        {
            InitializeItems();
        }

        private void InitializeItems()
        {
            _itemStates.Clear();
            ClearHitList();

            foreach (var item in items)
            {
                var state = new DockItemState
                {
                    Item = item,
                    CurrentScale = 1.0f,
                    TargetScale = 1.0f,
                    IsHovered = false,
                    IsSelected = false
                };
                _itemStates[item.Name] = state;
            }

            UpdateLayout();
            Invalidate();
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (items.Count == 0) return;

            // Calculate dock dimensions
            var dockRect = CalculateDockRect();

            // Draw dock background
            DrawDockBackground(g, dockRect);

            // Draw dock items
            DrawDockItems(g);
        }

        private Rectangle CalculateDockRect()
        {
            int totalSize = CalculateTotalSize();

            return _orientation == DockOrientation.Horizontal
                ? new Rectangle((Width - totalSize) / 2, GetDockY(), totalSize, _dockHeight)
                : new Rectangle(GetDockX(), (Height - totalSize) / 2, _dockHeight, totalSize);
        }

        private int CalculateTotalSize()
        {
            int total = _spacing;
            foreach (var state in _itemStates.Values)
            {
                int itemSize = (int)(_itemSize * state.CurrentScale);
                total += itemSize + _spacing;
            }
            return total;
        }

        private int GetDockY()
        {
            return _position switch
            {
                DockPosition.Top => 0,
                DockPosition.Bottom => Height - _dockHeight,
                DockPosition.Center => (Height - _dockHeight) / 2,
                _ => Height - _dockHeight
            };
        }

        private int GetDockX()
        {
            return _position switch
            {
                DockPosition.Left => 0,
                DockPosition.Right => Width - _dockHeight,
                DockPosition.Center => (Width - _dockHeight) / 2,
                _ => 0
            };
        }

        private void DrawDockBackground(Graphics g, Rectangle dockRect)
        {
            using (var path = GetRoundedRectPath(dockRect, _dockCornerRadius))
            using (var brush = new SolidBrush(Color.FromArgb(200, _currentTheme.ButtonBackColor)))
            {
                g.FillPath(brush, path);

                // Add subtle border
                using (var pen = new Pen(Color.FromArgb(100, _currentTheme.BorderColor), 1))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        private void DrawDockItems(Graphics g)
        {
            ClearHitList();

            int currentPos = _spacing;
            int index = 0;

            foreach (var kvp in _itemStates)
            {
                var itemName = kvp.Key;
                var state = kvp.Value;
                var item = state.Item;

                // Configure the reusable image component
                image.ImagePath = item.ImagePath;
                image.IsHovered = state.IsHovered;

                // Calculate item dimensions
                int scaledSize = (int)(_itemSize * state.CurrentScale);
                var itemRect = CalculateItemRect(currentPos, scaledSize, state.IsHovered);

                // Draw the item
                image.Draw(g, itemRect);

                // Add hit area
                AddHitArea(
                    $"DockItem_{index}",
                    itemRect,
                    image,
                    () => OnItemClicked(item)
                );

                // Update position
                currentPos += scaledSize + _spacing;
                index++;
            }
        }

        private Rectangle CalculateItemRect(int position, int size, bool isHovered)
        {
            int hoverOffset = isHovered ? _hoverOffset : 0;

            if (_orientation == DockOrientation.Horizontal)
            {
                int x = (Width - CalculateTotalSize()) / 2 + position;
                int y = GetDockY() + (_dockHeight - size) / 2 - hoverOffset;
                return new Rectangle(x, y, size, size);
            }
            else
            {
                int x = GetDockX() + (_dockHeight - size) / 2 - hoverOffset;
                int y = (Height - CalculateTotalSize()) / 2 + position;
                return new Rectangle(x, y, size, size);
            }
        }
        #endregion

        #region Animation
        private void AnimateScaling(object sender, EventArgs e)
        {
            bool needsRedraw = false;

            foreach (var state in _itemStates.Values)
            {
                if (Math.Abs(state.TargetScale - state.CurrentScale) > 0.01f)
                {
                    state.CurrentScale = Lerp(state.CurrentScale, state.TargetScale, _animationSpeed);
                    needsRedraw = true;
                }
            }

            if (needsRedraw)
                Invalidate();
        }

        private float Lerp(float start, float end, float amount)
        {
            return start + (end - start) * amount;
        }

        private void ApplySpringEffect(string hoveredItemName)
        {
            foreach (var kvp in _itemStates)
            {
                var itemName = kvp.Key;
                var state = kvp.Value;

                if (itemName == hoveredItemName)
                {
                    state.TargetScale = _maxScale;
                    state.IsHovered = true;
                }
                else if (state.Item == SelectedItem)
                {
                    state.TargetScale = _clickScaleFactor;
                    state.IsHovered = false;
                }
                else
                {
                    state.TargetScale = 1.0f;
                    state.IsHovered = false;
                }
            }
        }
        #endregion

        #region Mouse Events
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Let the hit area system handle hover detection
            string previousHovered = _hoveredItemName;
            _hoveredItemName = null;

            // Check which item is being hovered via hit test
            if (HitTest(e.Location) && HitTestControl != null)
            {
                // Extract item name from hit area name
                if (HitTestControl.Name.StartsWith("DockItem_"))
                {
                    if (int.TryParse(HitTestControl.Name.Substring(9), out int itemIndex))
                    {
                        if (itemIndex < items.Count)
                        {
                            _hoveredItemName = items[itemIndex].Name;
                        }
                    }
                }
            }

            // Apply spring effect if hover state changed
            if (_hoveredItemName != previousHovered)
            {
                ApplySpringEffect(_hoveredItemName);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoveredItemName = null;
            ApplySpringEffect(null);
        }

        private void OnItemClicked(SimpleItem item)
        {
            SelectedItem = item;
            _selectedIndex = items.IndexOf(item);

            // Update selection state
            foreach (var state in _itemStates.Values)
            {
                state.IsSelected = state.Item == item;
            }

            ApplySpringEffect(_hoveredItemName);
        }
        #endregion

        #region Layout
        private void UpdateLayout()
        {
            if (_orientation == DockOrientation.Horizontal)
            {
                Height = _dockHeight + _hoverOffset + 20; // Extra space for hover effect
                MinimumSize = new Size(_itemSize * 3, Height);
            }
            else
            {
                Width = _dockHeight + _hoverOffset + 20;
                MinimumSize = new Size(Width, _itemSize * 3);
            }

            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }
        #endregion

        #region Theme
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme != null)
            {
                BackColor = _currentTheme.PanelBackColor;
                ForeColor = _currentTheme.LabelForeColor;

                // Apply theme to drawing components
                button.Theme = Theme;
                button.ApplyTheme();

                label.Theme = Theme;
                label.ApplyTheme();

                image.Theme = Theme;
                image.ApplyTheme();
            }

            IsChild = true;
            if (Parent != null) BackColor = Parent.BackColor;
            IsFrameless = true;
            ShowAllBorders = false;
            IsBorderAffectedByTheme = false;

            Invalidate();
        }
        #endregion

        #region Cleanup
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                animationTimer?.Stop();
                animationTimer?.Dispose();
                items.ListChanged -= Items_ListChanged;
            }
            base.Dispose(disposing);
        }
        #endregion
    }

 
}