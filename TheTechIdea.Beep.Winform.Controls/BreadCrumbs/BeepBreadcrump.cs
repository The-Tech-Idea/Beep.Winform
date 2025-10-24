using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BreadCrumbs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Breadcrumps")]
    [Category("Beep Controls")]
    [Description("Enhanced breadcrumb navigation with hit area support and advanced styling")]
    public class BeepBreadcrump : BaseControl
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

        private IBreadcrumbPainter _painter; // strategy painter per Style
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
        private Font _textFont = new Font("Segoe UI", 9);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text font for breadcrumb items")]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                _textFont = value ?? new Font("Segoe UI", 9);
                SafeApplyFont(_textFont);
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
        [Description("Visual Style of the breadcrumb")]
        public BreadcrumbStyle BreadcrumbStyle
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;
                    InitializePainter();
                    Invalidate();
                }
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
                if (_selectedItem != null)
                {
                    OnCrumbClicked(new CrumbClickedEventArgs(_selectedIndex, _selectedItem.Name, _selectedItem.Text));
                }
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
        public BeepBreadcrump()
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
            InitializePainter();

            // Add some default items for design-time experience
            if (DesignMode)
            {
                _items.Add(new SimpleItem { Name = "Home", Text = "Home", ImagePath = "home.svg" });
                _items.Add(new SimpleItem { Name = "Documents", Text = "Documents", ImagePath = "folder.svg" });
                _items.Add(new SimpleItem { Name = "Current", Text = "Current", ImagePath = "file.svg" });
            }
        }

        private void InitializePainter()
        {
            // Select painter based on ProgressBarStyle
            switch (_style)
            {
                case BreadcrumbStyle.Classic:
                    _painter = new ClassicBreadcrumbPainter();
                    break;
                case BreadcrumbStyle.Modern:
                    _painter = new ModernBreadcrumbPainter();
                    break;
                case BreadcrumbStyle.Pill:
                    _painter = new PillBreadcrumbPainter();
                    break;
                case BreadcrumbStyle.Flat:
                default:
                    _painter = new FlatBreadcrumbPainter();
                    break;
            }

            // Initialize with current theme and font
            _painter?.Initialize(this, _currentTheme, _textFont ?? Font, _showIcons);
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
                if (item == null) continue;
                var key = item.Name ?? Guid.NewGuid().ToString();
                var state = new BreadcrumbState
                {
                    Item = item,
                    IsHovered = false,
                    IsSelected = false
                };
                _itemStates[key] = state;
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
            base.DrawContent(g);
            if (UseThemeColors && _currentTheme != null)
            {
                BackColor = _currentTheme.SideMenuBackColor;
                g.Clear(BackColor);
            }
            else
            {
                // Paint background based on selected Style
                BeepStyling.PaintStyleBackground(g, DrawingRect, ControlStyle);
            }
            if (_items.Count == 0) return;

            ClearHitList();
            DrawBreadcrumbItems(g);
        }

        private void DrawBreadcrumbItems(Graphics g)
        {
            if (_painter == null)
            {
                InitializePainter();
            }
            _painter?.Initialize(this, _currentTheme, _textFont ?? Font, _showIcons);

            int x = DrawingRect.Left + 5;
            int y = DrawingRect.Top;
            int itemHeight = DrawingRect.Height;

            Color sepColor = _currentTheme != null
                ? Color.FromArgb(128, _currentTheme.LabelForeColor)
                : Color.FromArgb(128, ForeColor);

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (item == null) continue;
                var key = item.Name ?? string.Empty;
                var state = _itemStates.ContainsKey(key) ? _itemStates[key] : null;
                bool isHovered = state?.IsHovered ?? false;
                bool isSelected = state?.IsSelected ?? false;
                bool isLast = i == _items.Count - 1;

                // Calculate item dimensions
                var itemRect = _painter.CalculateItemRect(g, item, x, y, itemHeight, isHovered);

                // Draw the breadcrumb item
                _painter.DrawItem(g, button, item, itemRect, isHovered, isSelected, isLast);

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
                    x += _painter.DrawSeparator(g, label, x, y, itemHeight, _separatorText, _textFont ?? Font, sepColor, _itemSpacing);
                }
            }
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
                            _hoveredItemName = _items[itemIndex]?.Name;
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

            OnCrumbClicked(new CrumbClickedEventArgs(index, item?.Name, item?.Text));
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
                if (item != null && !string.IsNullOrEmpty(item.Name))
                {
                    _itemStates.Remove(item.Name);
                }
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
                    _textFont?.Dispose();
                    _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                }

                // Apply theme to drawing components
                button.Theme = Theme;
                button.ApplyTheme();

                label.Theme = Theme;
                label.ApplyTheme();

                image.Theme = Theme;
                image.ApplyTheme();
            }

            InitializePainter();
            Invalidate();
        }
        #endregion

        #region Cleanup
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_items != null)
                {
                    _items.ListChanged -= Items_ListChanged;
                }
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    
}
