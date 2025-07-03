using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Breadcrumps")]
    [Category("Beep Controls")]
    [Description("Enhanced breadcrumb navigation with hit area support and advanced styling")]
    public class BeepBreadcrumps : BeepControl
    {
        #region Fields
        private BeepButton button;
        private BeepLabel label;
        private BeepImage image;

        private BindingList<SimpleItem> _items = new BindingList<SimpleItem>();
        private Dictionary<string, BreadcrumbState> _itemStates = new Dictionary<string, BreadcrumbState>();
        private string _hoveredItemName;
        private SimpleItem _selectedItem;
        private int _selectedIndex = -1;

        private int _itemSpacing = 8;
        private int _separatorWidth = 20;
        private string _separatorText = ">";
        private BreadcrumbStyle _style = BreadcrumbStyle.Modern;
        private bool _showIcons = true;
        private bool _showHomeIcon = true;
        #endregion

        #region Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data")]
        [Description("The breadcrumb items collection")]
        public BindingList<SimpleItem> Items
        {
            get => _items;
            set
            {
                if (_items != null)
                    _items.ListChanged -= Items_ListChanged;

                _items = value ?? new BindingList<SimpleItem>();
                _items.ListChanged += Items_ListChanged;

                InitializeItems();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text font for breadcrumb items")]
        public Font TextFont
        {
            get => Font;
            set
            {
                Font = value;
                UseThemeFont = false;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Spacing between breadcrumb items")]
        public int ItemSpacing
        {
            get => _itemSpacing;
            set
            {
                _itemSpacing = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text used as separator between items")]
        public string SeparatorText
        {
            get => _separatorText;
            set
            {
                _separatorText = value ?? ">";
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Visual style of the breadcrumb")]
        public BreadcrumbStyle Style
        {
            get => _style;
            set
            {
                _style = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show icons for breadcrumb items")]
        public bool ShowIcons
        {
            get => _showIcons;
            set
            {
                _showIcons = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show home icon as first item")]
        public bool ShowHomeIcon
        {
            get => _showHomeIcon;
            set
            {
                _showHomeIcon = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnCrumbClicked(new CrumbClickedEventArgs(_selectedIndex, _selectedItem.Name,_selectedItem.Text));
                Invalidate();
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0 && value < _items.Count)
                {
                    _selectedIndex = value;
                    SelectedItem = _items[value];
                }
            }
        }
        #endregion

        #region Events
        [Category("Action")]
        [Description("Occurs when a breadcrumb item is clicked")]
        public event EventHandler<CrumbClickedEventArgs> CrumbClicked;

        protected virtual void OnCrumbClicked(CrumbClickedEventArgs e)
        {
            CrumbClicked?.Invoke(this, e);
        }
        #endregion

        #region Constructor
        public BeepBreadcrumps()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                    ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            BackColor = Color.White;
            ForeColor = Color.Black;
            Font = new Font("Segoe UI", 9);
            BorderRadius = 8;
            Height = 36;

            InitializeDrawingComponents();
            _items.ListChanged += Items_ListChanged;

            // Add some default items for design-time experience
            if (DesignMode)
            {
                _items.Add(new SimpleItem { Name = "Home", Text = "Home", ImagePath = "home.svg" });
                _items.Add(new SimpleItem { Name = "Documents", Text = "Documents", ImagePath = "folder.svg" });
                _items.Add(new SimpleItem { Name = "Current", Text = "Current", ImagePath = "file.svg" });
            }
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
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Theme = this.Theme,
                ApplyThemeOnImage = true,
                IsFrameless = true
            };

            label = new BeepLabel
            {
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = false,
                TextAlign = ContentAlignment.MiddleLeft,
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

        protected override Size DefaultSize => new Size(300, 36);
        #endregion

        #region Item Management
        private void Items_ListChanged(object sender, ListChangedEventArgs e)
        {
            InitializeItems();
        }

        private void InitializeItems()
        {
            _itemStates.Clear();

            foreach (var item in _items)
            {
                var state = new BreadcrumbState
                {
                    Item = item,
                    IsHovered = false,
                    IsSelected = false
                };
                _itemStates[item.Name] = state;
            }

            Invalidate();
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_items.Count == 0) return;

            ClearHitList();
            DrawBreadcrumbItems(g);
        }

        private void DrawBreadcrumbItems(Graphics g)
        {
            int x = DrawingRect.Left + 5;
            int y = DrawingRect.Top;
            int itemHeight = DrawingRect.Height;

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var state = _itemStates.ContainsKey(item.Name) ? _itemStates[item.Name] : null;
                bool isHovered = state?.IsHovered ?? false;
                bool isSelected = state?.IsSelected ?? false;
                bool isLast = i == _items.Count - 1;

                // Calculate item dimensions
                var itemRect = CalculateItemRect(g, item, x, y, itemHeight, isHovered);

                // Draw the breadcrumb item
                DrawBreadcrumbItem(g, item, itemRect, isHovered, isSelected, isLast);

                // Add hit area
                AddHitArea(
                    $"Breadcrumb_{i}",
                    itemRect,
                    button,
                    () => OnItemClicked(item, i)
                );

                // Move to next position
                x = itemRect.Right;

                // Draw separator if not last item
                if (!isLast)
                {
                    x += DrawSeparator(g, x, y, itemHeight);
                }
            }
        }

        private Rectangle CalculateItemRect(Graphics g, SimpleItem item, int x, int y, int height, bool isHovered)
        {
            string displayText = item.Text ?? item.Name ?? "";
            var textSize = TextRenderer.MeasureText(displayText, Font);

            int iconWidth = (_showIcons && !string.IsNullOrEmpty(item.ImagePath)) ? 20 : 0;
            int padding = 8;
            int width = textSize.Width + padding * 2 + iconWidth + (iconWidth > 0 ? 4 : 0);

            if (isHovered && _style == BreadcrumbStyle.Modern)
            {
                width += 4; // Extra padding for hover effect
                height -= 2; // Slight height reduction for modern effect
                y += 1;
            }

            return new Rectangle(x, y, width, height);
        }

        private void DrawBreadcrumbItem(Graphics g, SimpleItem item, Rectangle rect, bool isHovered, bool isSelected, bool isLast)
        {
            string displayText = item.Text ?? item.Name ?? "";

            // Configure the reusable button component
            button.Text = displayText;
            button.ImagePath = (_showIcons && !string.IsNullOrEmpty(item.ImagePath)) ? item.ImagePath : "";
            button.IsHovered = isHovered;
            button.IsSelected = isSelected;

            // Apply style-specific appearance
            switch (_style)
            {
                case BreadcrumbStyle.Classic:
                    DrawClassicStyle(g, button, rect, isHovered, isLast);
                    break;
                case BreadcrumbStyle.Modern:
                    DrawModernStyle(g, button, rect, isHovered, isLast);
                    break;
                case BreadcrumbStyle.Pill:
                    DrawPillStyle(g, button, rect, isHovered, isLast);
                    break;
                case BreadcrumbStyle.Flat:
                    DrawFlatStyle(g, button, rect, isHovered, isLast);
                    break;
            }

            // Draw the button content
            button.Draw(g, rect);
        }

        private void DrawClassicStyle(Graphics g, BeepButton btn, Rectangle rect, bool isHovered, bool isLast)
        {
            if (isHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(50, _currentTheme.ButtonHoverBackColor)))
                {
                    g.FillRectangle(brush, rect);
                }
                using (var pen = new Pen(_currentTheme.ButtonHoverBorderColor, 1))
                {
                    g.DrawRectangle(pen, rect);
                }
            }

            btn.BackColor = isHovered ? Color.FromArgb(30, _currentTheme.ButtonHoverBackColor) : Color.Transparent;
            btn.ForeColor = isLast ? _currentTheme.ButtonForeColor : _currentTheme.LinkColor;
        }

        private void DrawModernStyle(Graphics g, BeepButton btn, Rectangle rect, bool isHovered, bool isLast)
        {
            if (isHovered)
            {
                using (var path = GetRoundedRectPath(rect, 4))
                using (var brush = new SolidBrush(Color.FromArgb(40, _currentTheme.ButtonHoverBackColor)))
                {
                    g.FillPath(brush, path);
                }
            }

            btn.BackColor = Color.Transparent;
            btn.ForeColor = isLast ? _currentTheme.ButtonForeColor : _currentTheme.LinkColor;
            btn.IsRounded = true;
            btn.BorderRadius = 4;
        }

        private void DrawPillStyle(Graphics g, BeepButton btn, Rectangle rect, bool isHovered, bool isLast)
        {
            var pillRect = new Rectangle(rect.X, rect.Y + 4, rect.Width, rect.Height - 8);

            using (var path = GetRoundedRectPath(pillRect, pillRect.Height / 2))
            {
                if (isHovered)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(60, _currentTheme.ButtonHoverBackColor)))
                    {
                        g.FillPath(brush, path);
                    }
                }

                if (isLast)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(80, _currentTheme.ButtonBackColor)))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }

            btn.BackColor = Color.Transparent;
            btn.ForeColor = isLast ? _currentTheme.ButtonForeColor : _currentTheme.LinkColor;
        }

        private void DrawFlatStyle(Graphics g, BeepButton btn, Rectangle rect, bool isHovered, bool isLast)
        {
            if (isHovered)
            {
                var underlineRect = new Rectangle(rect.X, rect.Bottom - 2, rect.Width, 2);
                using (var brush = new SolidBrush(_currentTheme.LinkColor))
                {
                    g.FillRectangle(brush, underlineRect);
                }
            }

            btn.BackColor = Color.Transparent;
            btn.ForeColor = isLast ? _currentTheme.ButtonForeColor : _currentTheme.LinkColor;
        }

        private int DrawSeparator(Graphics g, int x, int y, int height)
        {
            // Configure the reusable label for separator
            label.Text = _separatorText;
            label.ForeColor = Color.FromArgb(128, _currentTheme.LabelForeColor);
            label.BackColor = Color.Transparent;

            var sepSize = TextRenderer.MeasureText(_separatorText, Font);
            var sepRect = new Rectangle(x + _itemSpacing, y, sepSize.Width, height);

            label.Draw(g, sepRect);

            return sepRect.Width + _itemSpacing;
        }
        #endregion

        #region Mouse Events
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            string previousHovered = _hoveredItemName;
            _hoveredItemName = null;

            // Check which item is being hovered via hit test
            if (HitTest(e.Location) && HitTestControl != null)
            {
                if (HitTestControl.Name.StartsWith("Breadcrumb_"))
                {
                    if (int.TryParse(HitTestControl.Name.Substring(11), out int itemIndex))
                    {
                        if (itemIndex < _items.Count)
                        {
                            _hoveredItemName = _items[itemIndex].Name;
                        }
                    }
                }
            }

            // Update hover states if changed
            if (_hoveredItemName != previousHovered)
            {
                UpdateHoverStates();
                Cursor = !string.IsNullOrEmpty(_hoveredItemName) ? Cursors.Hand : Cursors.Default;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoveredItemName = null;
            UpdateHoverStates();
            Cursor = Cursors.Default;
        }

        private void UpdateHoverStates()
        {
            foreach (var kvp in _itemStates)
            {
                kvp.Value.IsHovered = kvp.Key == _hoveredItemName;
            }
            Invalidate();
        }

        private void OnItemClicked(SimpleItem item, int index)
        {
            _selectedIndex = index;
            _selectedItem = item;

            // Update selection states
            foreach (var state in _itemStates.Values)
            {
                state.IsSelected = state.Item == item;
            }

            OnCrumbClicked(new CrumbClickedEventArgs(index, item.Name,item.Text));
            Invalidate();
        }
        #endregion

        #region Public Methods
        public void AddItem(string name, string text = null, string imagePath = null)
        {
            var item = new SimpleItem
            {
                Name = name,
                Text = text ?? name,
                ImagePath = imagePath
            };
            _items.Add(item);
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                var item = _items[index];
                _items.RemoveAt(index);
                _itemStates.Remove(item.Name);
            }
        }

        public void Clear()
        {
            _items.Clear();
            _itemStates.Clear();
            _selectedItem = null;
            _selectedIndex = -1;
            Invalidate();
        }

        public void NavigateToIndex(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                // Remove all items after the specified index
                for (int i = _items.Count - 1; i > index; i--)
                {
                    RemoveItem(i);
                }
                SelectedIndex = index;
            }
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

                if (UseThemeFont)
                {
                    Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                }

                // Apply theme to drawing components
                button.Theme = Theme;
                button.ApplyTheme();

                label.Theme = Theme;
                label.ApplyTheme();

                image.Theme = Theme;
                image.ApplyTheme();
            }

            Invalidate();
        }
        #endregion

        #region Cleanup
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _items.ListChanged -= Items_ListChanged;
            }
            base.Dispose(disposing);
        }
        #endregion
    }

  
}