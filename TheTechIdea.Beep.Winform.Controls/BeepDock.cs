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
    [Description("Docking control for Beep")]
    public class BeepDock : BeepControl
    {
        private List<BeepImage> dockItems = new List<BeepImage>();
        private BindingList<SimpleItem> items = new BindingList<SimpleItem>();
        private SimpleItem _selectedItem;
        private int _selectedIndex = -1;

        private BeepImage _hoveredItem = null;
        private BeepImage _clickedItem = null;
        private Dictionary<BeepImage, float> _targetScales = new Dictionary<BeepImage, float>();
        private Timer animationTimer;

        private int _itemSize = 50;
        private int _dockHeight = 40;
        private int _hoverOffset = 15;
        private int _dockCornerRadius = 15;

        [Category("Beep Dock Appearance")]
        public int ItemSize
        {
            get => _itemSize;
            set
            {
                _itemSize = value;
                foreach (var item in dockItems)
                {
                    item.BaseSize = value;
                    _targetScales[item] = 1.0f;
                }
                UpdateLayout();
            }
        }

        private int spacing = 10;
        private float maxScale = 1.4f;
        private float clickScaleFactor = 1.1f;
        private float animationSpeed = 0.15f;

        public BeepDock()
        {
            DoubleBuffered = true;
            IsChild=true;
            IsFrameless = true;
            ShowAllBorders = false;
            IsBorderAffectedByTheme = false;
            items.ListChanged += Items_ListChanged;
            InitializeItems();

            animationTimer = new Timer { Interval = 16 };
            animationTimer.Tick += AnimateScaling;
            animationTimer.Start();
        }

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
        public List<SimpleItem> SelectedItems
        {
            get
            {
                List<SimpleItem> selectedItems = new();
                foreach (var item in dockItems)
                {
                    if (_targetScales[item] == clickScaleFactor)
                    {
                        selectedItems.Add(item.Info);
                    }
                }
                return selectedItems;
            }
        }

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0 && value < dockItems.Count)
                {
                    _selectedIndex = value;
                    SelectedItem = dockItems[_selectedIndex].Info;
                }
            }
        }

        [Browsable(true)]
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

        private void Items_ListChanged(object sender, ListChangedEventArgs e)
        {
            InitializeItems();
        }

        private void InitializeItems()
        {
            dockItems.Clear();
            _targetScales.Clear();

            foreach (var simpleItem in items)
            {
                AddDockItem(simpleItem);
            }
            UpdateLayout();
            Invalidate();
        }

        private void AddDockItem(SimpleItem simpleItem)
        {
            BeepImage item = new BeepImage
            {
                Info = simpleItem,
                ImagePath = simpleItem.ImagePath,
                ScaleMode = ImageScaleMode.KeepAspectRatio,
                BaseSize = ItemSize,
                Size = new Size(ItemSize, ItemSize)
            };

            dockItems.Add(item);
            _targetScales[item] = 1.0f;
        }

        private void AnimateScaling(object sender, EventArgs e)
        {
            bool needsRedraw = false;
            foreach (var item in dockItems)
            {
                float targetScale = _targetScales[item];
                float currentScale = item.ScaleFactor;

                if (Math.Abs(targetScale - currentScale) > 0.01f)
                {
                    item.ScaleFactor = Lerp(currentScale, targetScale, animationSpeed);
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int totalWidth = spacing;
            foreach (var item in dockItems)
            {
                int newSize = (int)(item.BaseSize * item.ScaleFactor);
                totalWidth += newSize + spacing;
            }

            int dockWidth = Math.Max(totalWidth, _itemSize * 3);
            Rectangle dockRect = new Rectangle((Width - dockWidth) / 2, Height - _dockHeight, dockWidth, _dockHeight);

            using (GraphicsPath path = CreateRoundedRectangle(dockRect, _dockCornerRadius))
            using (Brush dockBrush = new SolidBrush(_currentTheme.ButtonBackColor))
            {
                e.Graphics.FillPath(dockBrush, path);
            }

            int currentX = (Width - totalWidth) / 2;
            foreach (var item in dockItems)
            {
                int newSize = (int)(item.BaseSize * item.ScaleFactor);
                int yOffset = (Height - _dockHeight) / 2;
                if (item == _hoveredItem) yOffset -= _hoverOffset;

                Rectangle drawRect = new Rectangle(currentX, yOffset, newSize, newSize);
                item.Draw(e.Graphics, drawRect, drawRect);
                currentX += newSize + spacing;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            BeepImage newHoveredItem = null;
            Point localMousePos = this.PointToClient(Cursor.Position);

            int currentX = (Width - dockItems.Count * (_itemSize + spacing)) / 2;
            foreach (var item in dockItems)
            {
                int newSize = (int)(item.BaseSize * item.ScaleFactor);
                Rectangle itemRect = new Rectangle(currentX, (Height - _dockHeight) / 2, newSize, newSize);

                if (itemRect.Contains(localMousePos))
                {
                    newHoveredItem = item;
                    break;
                }

                currentX += newSize + spacing;
            }

            if (newHoveredItem != _hoveredItem)
            {
                _hoveredItem = newHoveredItem;
                ApplySpringEffect();
            }
        }

        private void ApplySpringEffect()
        {
            foreach (var item in dockItems)
            {
                if (item == _hoveredItem)
                {
                    _targetScales[item] = maxScale;
                }
                else if (item.Info == SelectedItem)
                {
                    _targetScales[item] = clickScaleFactor;
                }
                else
                {
                    _targetScales[item] = 1.0f;
                }
            }
        }
        private void UpdateLayout()
        {
            // 🔹 The dock should have a fixed height smaller than the items
            Height = _dockHeight;

            // 🔹 Ensure minimum width to fit items
            int totalWidth = spacing;
            foreach (var item in dockItems)
            {
                int newSize = (int)(item.BaseSize * item.ScaleFactor);
                totalWidth += newSize + spacing;
            }
            Width = Math.Max(totalWidth, _itemSize * 3);

            Invalidate(); // 🔹 Redraw to reflect changes
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
        public override void ApplyTheme()
        {
          //  base.ApplyTheme();
          BackColor = _currentTheme.PanelBackColor;
            ForeColor = _currentTheme.LabelForeColor;
            IsChild = true;
            if(Parent!=null)         BackColor = Parent.BackColor;
            IsChild = true;
            IsFrameless = true;
            ShowAllBorders = false;
            IsBorderAffectedByTheme = false;
        }
    }
}
