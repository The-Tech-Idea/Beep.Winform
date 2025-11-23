using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel.Design;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Icons;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters;
using TheTechIdea.Beep.Winform.Controls.BottomNavBars.Helpers;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars
{
    /// <summary>
    /// Simple BottomBar navigation control.
    /// Works on a list of SimpleItem (icons + text) and offers a centered floating CTA and selection animation.
    /// </summary>
    [ToolboxItem(true)]
    [Designer(typeof(ControlDesigner))]
    public class BottomBar : BaseControl
    {
        private BindingList<SimpleItem> _items = new BindingList<SimpleItem>();
        private SimpleItem? _selectedItem;
        private readonly ImagePainter _imagePainter = new ImagePainter();
        private Timer? _tickerTimer;
        private double _tickerMs;
        private Timer? _selectionTimer;
        private float _indicatorX;
        private float _indicatorTargetX;
        private float _indicatorStartX;
        private float _indicatorWidth;
        private float _indicatorTargetWidth;
        private float _indicatorStartWidth;
        private int _animationDuration = 240; // ms
        private DateTime _animationStart;
        private IBottomBarPainter? _bottomBarPainter;
        private BottomBarHitTestHelper _bbHitTestHelper;
        private BeepBottomBarLayoutHelper _layoutHelper = new BeepBottomBarLayoutHelper();
        private BottomBarStyle _style = BottomBarStyle.Classic;

        /// <summary>
        /// Initializes a new instance of the <see cref="BottomBar"/> control.
        /// </summary>
        public BottomBar()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            Height = 72;
            Dock = DockStyle.Bottom;
            BackColor = Color.White;
            _items.ListChanged += Items_ListChanged;
            InitializeAnimationTimer();
            _bbHitTestHelper = new BottomBarHitTestHelper(this);
            _bbHitTestHelper.ItemClicked += BottomBarHit_ItemClicked;
            InitializePainterFromStyle(_style);
            TabStop = true; // enable keyboard focus
            this.AccessibleRole = AccessibleRole.MenuBar;
            this.AccessibleName = "Bottom Navigation";
            _tickerTimer = new Timer { Interval = 50 };
            _tickerTimer.Tick += (s, e) => { _tickerMs += _tickerTimer.Interval; Invalidate(); };
            _tickerTimer.Start();
        }

        private void BottomBarHit_ItemClicked(object? sender, ItemClickEventArgs e)
        {
            if (e?.Item != null)
            {
                SelectedItem = e.Item;
                ItemClicked?.Invoke(e.Item);
            }
        }

        private void InitializePainterFromStyle(BottomBarStyle style)
        {
            _bottomBarPainter?.Dispose();
            switch (style)
            {
                case BottomBarStyle.Classic:
                    _bottomBarPainter = new ClassicBottomBarPainter(); break;
                case BottomBarStyle.FloatingCTA:
                    _bottomBarPainter = new FloatingCTABottomBarPainter(); break;
                case BottomBarStyle.Bubble:
                    _bottomBarPainter = new BubbleBottomBarPainter(); break;
                case BottomBarStyle.Pill:
                    _bottomBarPainter = new PillBottomBarPainter(); break;
                case BottomBarStyle.Diamond:
                    _bottomBarPainter = new DiamondBottomBarPainter(); break;
                case BottomBarStyle.NotionMinimal:
                    _bottomBarPainter = new NotionMinimalBottomBarPainter(); break;
                default:
                    _bottomBarPainter = new ClassicBottomBarPainter(); break;
            }
        }

        private void InitializeAnimationTimer()
        {
            _selectionTimer = new Timer { Interval = 15 };
            _selectionTimer.Tick += SelectionTimer_Tick;
        }

        private void SelectionTimer_Tick(object? s, EventArgs e)
        {
            double elapsed = (DateTime.Now - _animationStart).TotalMilliseconds;
            double progress = Math.Min(1.0, elapsed / _animationDuration);
            double eased = 1 - Math.Pow(1 - progress, 3);
            // interpolate from start to target using eased progress
            _indicatorX = _indicatorStartX + (float)(_indicatorTargetX - _indicatorStartX) * (float)eased;
            _indicatorWidth = _indicatorStartWidth + (float)(_indicatorTargetWidth - _indicatorStartWidth) * (float)eased;
            Invalidate();
            if (progress >= 1.0)
            {
                _indicatorX = _indicatorTargetX;
                _indicatorWidth = _indicatorTargetWidth;
                _selectionTimer?.Stop();
            }
        }

        #region Properties
        [Browsable(true)]
        [Category("Data")]
        public new BindingList<SimpleItem> Items
        {
            get => _items;
            set
            {
                if (_items != null)
                    _items.ListChanged -= Items_ListChanged;
                _items = value ?? new BindingList<SimpleItem>();
                _items.ListChanged += Items_ListChanged;
                Invalidate();
            }
        }

        /// <summary>
        /// Visual style used to determine the painter for the BottomBar.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        public BottomBarStyle BarStyle { get => _style; set { _style = value; InitializePainterFromStyle(_style); Invalidate(); } }

        /// <summary>
        /// The currently selected item in the BottomBar.
        /// </summary>
        [Browsable(false)]
        public SimpleItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    StartIndicatorAnimationToSelected();
                    OnSelectedItemChanged();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(72)]
        public int BarHeight
        {
            get => Height;
            set
            {
                Height = Math.Max(48, value);
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public Color AccentColor { get; set; } = Color.FromArgb(96, 80, 255);

        [Browsable(true)]
        [Category("Behavior")]
        public new int AnimationDuration
        {
            get => _animationDuration;
            set => _animationDuration = Math.Max(80, value);
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(-1)]
        public int CTAIndex { get; set; } = -1; // If set, this index is treated as CTA (centered)

        [Browsable(true)]
        [Category("Data")]
        public string DefaultItemImagePath { get; set; } = Svgs.Menu;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ShowCTAShadow { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(1.6f)]
        public float CTAWidthFactor { get => _layoutHelper.CtaWidthFactor; set => _layoutHelper.CtaWidthFactor = Math.Max(1.0f, value); }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(1.25f)]
        public float SelectedWidthFactor { get => _layoutHelper.SelectedWidthFactor; set => _layoutHelper.SelectedWidthFactor = Math.Max(1.0f, value); }
        #endregion

        #region Events
        public event Action<SimpleItem>? ItemClicked;
        public event EventHandler<SelectedItemChangedEventArgs>? SelectedItemChanged;
        #endregion

        #region Overrides
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Create painter context
            var rect = ClientRectangle;
            rect.Inflate(-8, -8);
            if (Items == null || Items.Count == 0) return;

            var ctx = new BottomBarPainterContext
            {
                Graphics = g,
                Bounds = rect,
                Items = Items.ToList(),
                SelectedIndex = Items.IndexOf(SelectedItem!),
                HoverIndex = _bbHitTestHelper?.HoveredIndex ?? -1,
                HitTest = _hitTest,
                ImagePainter = _imagePainter,
                DefaultImagePath = DefaultItemImagePath,
                CTAIndex = CTAIndex,
                AccentColor = AccentColor
            };
            ctx.OnItemClicked = (idx, btn) =>
            {
                if (idx >= 0 && idx < Items.Count)
                {
                    SelectedItem = Items[idx];
                    ItemClicked?.Invoke(Items[idx]);
                }
            };
            ctx.LayoutHelper = _layoutHelper;
            // precompute layout with selected item included for reflow
            _layoutHelper.CtaWidthFactor = 1.6f; // default; can be exposed as property
            _layoutHelper.SelectedWidthFactor = 1.25f; // default expansion for selected pill
            _layoutHelper.EnsureLayout(ctx.Bounds, ctx.Items, ctx.CTAIndex, ctx.SelectedIndex);
            // derive an animation phase for pulsing/hover effects -> 0..1
            double seconds = _tickerMs / 1000.0;
            ctx.AnimationPhase = (float)((Math.Sin(seconds * 2 * Math.PI * 0.9) + 1.0) / 2.0);
            // Set the current theme on the ImagePainter so it can recolor icons where applicable
            ctx.ImagePainter.CurrentTheme = _currentTheme;
            ctx.ImagePainter.ApplyThemeOnImage = true;

            _bottomBarPainter?.CalculateLayout(ctx);
            // Ensure hit helper is updated with computed rectangles
            _bbHitTestHelper?.UpdateItems(ctx.Items, new System.Collections.Generic.List<Rectangle>(ctx.LayoutHelper.GetItemRectangles()));
            // allow painter to register additional or expanded hit areas (CTA, pill, etc.)
            _bottomBarPainter?.RegisterHitAreas(ctx);
            // Initialize indicator position on first layout
            if (_indicatorWidth <= 0)
            {
                var indicatorRect = ctx.LayoutHelper.GetIndicatorRect();
                _indicatorWidth = indicatorRect.Width;
                _indicatorX = indicatorRect.Left;
                _indicatorStartX = _indicatorX;
                _indicatorStartWidth = _indicatorWidth;
                _indicatorTargetX = _indicatorX;
                _indicatorTargetWidth = _indicatorWidth;
            }
            // ensure Animated indicator values are provided to painters
            ctx.AnimatedIndicatorX = _indicatorX;
            ctx.AnimatedIndicatorWidth = _indicatorWidth;
            _bottomBarPainter?.Paint(ctx);
        }

        /// <summary>
        /// Handles mouse click and routes to hit test helper.
        /// </summary>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            var rect = ClientRectangle;
            rect.Inflate(-8, -8);
            if (Items == null || Items.Count == 0) return;
            _bbHitTestHelper?.HandleMouseClick(e.Location, e.Button);
            var idx = _bbHitTestHelper?.FocusedIndex ?? -1;
            if (idx >= 0 && idx < Items.Count)
            {
                SelectedItem = Items[idx];
                ItemClicked?.Invoke(SelectedItem);
            }
        }

        /// <summary>
        /// Handles mouse down events to support hit testing and keyboard focus.
        /// </summary>
        /// <param name="e">Mouse event args</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _bbHitTestHelper?.HandleMouseDown(e.Location, e);
            // Ensure we can receive keyboard focus
            if (CanFocus) Focus();
        }

        /// <summary>
        /// Handles mouse up events and routes them to the hit test helper.
        /// </summary>
        /// <param name="e">Mouse event args</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _bbHitTestHelper?.HandleMouseUp(e.Location, e);
        }

        /// <summary>
        /// Handles mouse leave events to reset hover state in the hit test helper.
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _bbHitTestHelper?.HandleMouseLeave();
        }

        /// <summary>
        /// Determines if a given key is treated as an input key (so it will be processed by the control).
        /// </summary>
        /// <param name="keyData">Key data pressed</param>
        /// <returns>True if the key should be processed by the control</returns>
        protected override bool IsInputKey(Keys keyData)
        {
            // We want to handle arrow keys, home/end, space, enter
            if (keyData == Keys.Left || keyData == Keys.Right || keyData == Keys.Home || keyData == Keys.End || keyData == Keys.Space || keyData == Keys.Enter)
                return true;
            return base.IsInputKey(keyData);
        }

        /// <summary>
        /// Handles keyboard navigation and activation for the BottomBar.
        /// </summary>
        /// <param name="e">Key event args</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (Items == null || Items.Count == 0) return;
            int idx = Items.IndexOf(SelectedItem ?? Items[0]);
            switch (e.KeyCode)
            {
                case Keys.Left:
                    idx = idx <= 0 ? Items.Count - 1 : idx - 1;
                    SelectedItem = Items[idx];
                    break;
                case Keys.Right:
                    idx = (idx + 1) % Items.Count;
                    SelectedItem = Items[idx];
                    break;
                case Keys.Home:
                    SelectedItem = Items[0];
                    break;
                case Keys.End:
                    SelectedItem = Items[Items.Count - 1];
                    break;
                case Keys.Space:
                case Keys.Enter:
                    // Activate (fire ItemClicked)
                    ItemClicked?.Invoke(SelectedItem!);
                    break;
                default:
                    break;
            }
            Invalidate();
        }

        /// <summary>
        /// Cleans up resources used by the BottomBar.
        /// </summary>
        /// <param name="disposing">Whether disposing is in progress</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _selectionTimer?.Stop();
                _selectionTimer?.Dispose();
                _imagePainter?.Dispose();
                _bottomBarPainter?.Dispose();
                if (_bbHitTestHelper != null)
                {
                    _bbHitTestHelper.ItemClicked -= BottomBarHit_ItemClicked;
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Helpers
        private void Items_ListChanged(object? sender, ListChangedEventArgs e)
        {
            Invalidate();
            // Update hit areas with new items or count
            var bounds = ClientRectangle;
            bounds.Inflate(-8, -8);
            _layoutHelper.InvalidateLayout();
            _layoutHelper.EnsureLayout(bounds, Items.ToList(), CTAIndex, Items.IndexOf(SelectedItem!));
            _bbHitTestHelper?.UpdateItems(Items.ToList(), new System.Collections.Generic.List<Rectangle>(_layoutHelper.GetItemRectangles()));
        }

        private void DrawNavItem(Graphics g, SimpleItem item, Rectangle itemRect, int index)
        {
            int iconSize = 24;
            int textHeight = 12;
            int totalHeight = iconSize + textHeight + 4;
            int yOffset = itemRect.Top + (itemRect.Height - totalHeight) / 2;

            Rectangle iconRect = new Rectangle(itemRect.Left + (itemRect.Width - iconSize) / 2, yOffset, iconSize, iconSize);

            // ImagePainter render
            _imagePainter.ImagePath = GetIconPath(item);
            // Keep current ImagePainter theme settings; do not override here.
            _imagePainter.ApplyThemeOnImage = false;
            _imagePainter.ImageEmbededin = ImageEmbededin.Button;
            _imagePainter.DrawImage(g, iconRect);

            // Draw label
            using (var font = new Font("Segoe UI", 9f))
            using (var brush = new SolidBrush(Color.FromArgb(110, 110, 110)))
            {
                var textRect = new Rectangle(itemRect.Left + 4, iconRect.Bottom + 2, itemRect.Width - 8, textHeight);
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text, font, brush, textRect, sf);
            }
        }

        private void StartIndicatorAnimationToSelected()
        {
            if (SelectedItem == null) return;
            var idx = Items.IndexOf(SelectedItem);
            if (idx < 0) return;

            var rect = ClientRectangle;
            rect.Inflate(-8, -8);
            // Ensure layout computed (with selected for reflow)
            _layoutHelper.EnsureLayout(rect, Items.ToList(), CTAIndex, Items.IndexOf(SelectedItem!));
            var itemRects = _layoutHelper.GetItemRectangles();
            if (idx >= 0 && idx < itemRects.Count)
            {
                var itemRect = itemRects[idx];
                _indicatorTargetWidth = Math.Max(16, itemRect.Width - 16);
                float target = itemRect.Left + (itemRect.Width - _indicatorTargetWidth) / 2f;
                _indicatorTargetX = target;
                // set start values from current
                _indicatorStartX = _indicatorX;
                _indicatorStartWidth = _indicatorWidth;
            }
            _animationStart = DateTime.Now;
            if ((_selectionTimer?.Enabled ?? false) == false) _selectionTimer?.Start();
        }

        private void OnSelectedItemChanged()
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(SelectedItem!));
        }

        private string GetIconPath(SimpleItem item)
        {
            if (!string.IsNullOrEmpty(item?.ImagePath)) return item.ImagePath;
            var key = (item?.MenuID ?? item?.Name ?? item?.Text ?? string.Empty).ToLowerInvariant();
            if (key.Contains("home")) return Svgs.NavDashboard;
            if (key.Contains("search") || key.Contains("find")) return Svgs.Search;
            if (key.Contains("settings") || key.Contains("gear")) return Svgs.Settings;
            if (key.Contains("inbox") || key.Contains("mail")) return Svgs.Mail;
            return Svgs.Menu; // fallback
        }

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0) { path.AddRectangle(rect); return path; }
            int d = radius * 2;
            Rectangle arc = new Rectangle(rect.Location, new Size(d, d));
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - d;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - d;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
        #endregion
    }
}
