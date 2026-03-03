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
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;

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
        private const string AccessibilityDescriptionPrefix = "BottomBar status:";
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
        private bool _isDisposed;

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
            _tickerTimer.Tick += TickerTimer_Tick;
            _tickerTimer.Start();
            UpdateAccessibilityMetadata();
        }

        private void TickerTimer_Tick(object? sender, EventArgs e)
        {
            if (_isDisposed || _tickerTimer == null || !Visible || !IsHandleCreated)
            {
                return;
            }

            _tickerMs += _tickerTimer.Interval;
            Invalidate();
        }

        private void BottomBarHit_ItemClicked(object? sender, ItemClickEventArgs e)
        {
            if (e?.Item != null)
            {
                ActivateIndex(e.Index, raiseClick: true);
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
                case BottomBarStyle.MovableNotch:
                    _bottomBarPainter = new MovableNotchBottomBarPainter(); break;
                case BottomBarStyle.OutlineFloatingCTA:
                    _bottomBarPainter = new OutlineFloatingCTABottomBarPainter(); break;
                case BottomBarStyle.SegmentedTrack:
                    _bottomBarPainter = new SegmentedTrackBottomBarPainter(); break;
                case BottomBarStyle.GlassAcrylic:
                    _bottomBarPainter = new GlassAcrylicBottomBarPainter(); break;
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
            float previousX = _indicatorX;
            float previousWidth = _indicatorWidth;
            double elapsed = (DateTime.Now - _animationStart).TotalMilliseconds;
            double progress = Math.Min(1.0, elapsed / _animationDuration);
            double eased = 1 - Math.Pow(1 - progress, 3);
            // interpolate from start to target using eased progress
            // optionally use a slightly snappier easing curve (ease out quint)
            var eased2 = 1 - Math.Pow(1 - progress, 4);
            _indicatorX = _indicatorStartX + (float)(_indicatorTargetX - _indicatorStartX) * (float)eased2;
            _indicatorWidth = _indicatorStartWidth + (float)(_indicatorTargetWidth - _indicatorStartWidth) * (float)eased2;
            InvalidateIndicatorRegion(previousX, previousWidth);
            InvalidateIndicatorRegion(_indicatorX, _indicatorWidth);
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
        public BottomBarStyle BarStyle
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;
                    InitializePainterFromStyle(_style);
                    SyncLayoutAndHitTest();
                }
            }
        }

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
                SyncLayoutAndHitTest();
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
        public float CTAWidthFactor
        {
            get => _layoutHelper.CtaWidthFactor;
            set
            {
                _layoutHelper.CtaWidthFactor = Math.Max(1.0f, value);
                SyncLayoutAndHitTest();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(1.25f)]
        public float SelectedWidthFactor
        {
            get => _layoutHelper.SelectedWidthFactor;
            set
            {
                _layoutHelper.SelectedWidthFactor = Math.Max(1.0f, value);
                SyncLayoutAndHitTest();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(1.05f)]
        public float FloatingCTANotchRadiusFactor { get; set; } = 1.05f;

        // Movable notch tuning
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(22f)]
        public float MovableNotchDepth { get; set; } = 22f;
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(1.15f)]
        public float MovableNotchWidthFactor { get; set; } = 1.15f;

        // Outline CTA tuning
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(4)]
        public int OutlineRingStrokeWidth { get; set; } = 4;
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(36)]
        public int OutlineHaloAlpha { get; set; } = 36;
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(12)]
        public int OutlineInnerAlpha { get; set; } = 12;
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(1.4f)]
        public float OutlineHaloScale { get; set; } = 1.4f;

        // Segmented track tuning
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(6)]
        public int SegmentedTrackHeight { get; set; } = 6;
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(40)]
        public int SegmentedIndicatorWidth { get; set; } = 40;

        // Glass Acrylic tuning
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(0.6f)]
        public float GlassAcrylicOpacity { get; set; } = 0.6f;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(8)]
        public int CTAShadowYOffset { get; set; } = 8;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool MovableNotchOutlineCTA { get; set; } = false;
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
                SelectedIndex = Items.IndexOf(SelectedItem),
                HoverIndex = _bbHitTestHelper?.HoveredIndex ?? -1,
                HitTest = _hitTest,
                ImagePainter = _imagePainter,
                DefaultImagePath = DefaultItemImagePath,
                CTAIndex = CTAIndex,
                AccentColor = AccentColor,
                BarBackColor = BackColor,
                BarForeColor = ForeColor,
                BarHoverBackColor = BackColor,
                BarHoverForeColor = ForeColor,
                BadgeBackColor = Color.FromArgb(220, AccentColor),
                BadgeForeColor = Color.White,
                OnAccentColor = Color.White
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
            // Populate theme-driven color tokens into painter context
            if (_currentTheme != null)
            {
                ctx.BarBackColor = _currentTheme.NavigationBackColor != Color.Empty ? _currentTheme.NavigationBackColor : _currentTheme.SurfaceColor;
                ctx.BarForeColor = _currentTheme.NavigationForeColor != Color.Empty ? _currentTheme.NavigationForeColor : _currentTheme.ForeColor;
                ctx.BarHoverBackColor = _currentTheme.NavigationHoverBackColor != Color.Empty ? _currentTheme.NavigationHoverBackColor : _currentTheme.PanelBackColor;
                ctx.BarHoverForeColor = _currentTheme.NavigationHoverForeColor != Color.Empty ? _currentTheme.NavigationHoverForeColor : _currentTheme.ForeColor;
                ctx.BadgeBackColor = _currentTheme.BadgeBackColor;
                ctx.BadgeForeColor = _currentTheme.BadgeForeColor;
                ctx.OnAccentColor = _currentTheme.OnPrimaryColor;
                // Derive navigation border and shadow colors from existing theme tokens
                ctx.NavigationBorderColor = _currentTheme.BorderColor != Color.Empty ? _currentTheme.BorderColor : _currentTheme.ActiveBorderColor;
                // Prefer NavigationHoverBackColor as the semantic base color for shadows when available
                var shadowBase = _currentTheme.NavigationHoverBackColor != Color.Empty ? _currentTheme.NavigationHoverBackColor : (_currentTheme.BorderColor != Color.Empty ? _currentTheme.BorderColor : Color.Black);
                // Use a slightly stronger alpha to better mimic a soft shadow; painters may use this directly or build layered shadows
                ctx.NavigationShadowColor = Color.FromArgb(100, shadowBase.R, shadowBase.G, shadowBase.B);
            }
            // precompute layout with selected item included for reflow
            _layoutHelper.CtaWidthFactor = CTAWidthFactor;
            _layoutHelper.SelectedWidthFactor = SelectedWidthFactor;
            _layoutHelper.EnsureLayout(ctx.Bounds, ctx.Items, ctx.CTAIndex, ctx.SelectedIndex);
            // Allow painters to read control properties (floating CTA notch etc.)
            if (_bottomBarPainter is TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters.FloatingCTABottomBarPainter fcPainter)
            {
                fcPainter.NotchRadiusFactor = FloatingCTANotchRadiusFactor;
            }
            if (_bottomBarPainter is TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters.MovableNotchBottomBarPainter mnPainter)
            {
                mnPainter.NotchDepth = MovableNotchDepth;
                mnPainter.NotchWidthFactor = MovableNotchWidthFactor;
                mnPainter.NotchRadiusFactor = FloatingCTANotchRadiusFactor;
                mnPainter.OutlineCTA = MovableNotchOutlineCTA;
                mnPainter.OutlineStroke = OutlineRingStrokeWidth;
            }
            if (_bottomBarPainter is TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters.OutlineFloatingCTABottomBarPainter ofcPainter)
            {
                ofcPainter.RingStrokeWidth = OutlineRingStrokeWidth;
                ofcPainter.HaloAlpha = OutlineHaloAlpha;
                ofcPainter.InnerAlpha = OutlineInnerAlpha;
                ofcPainter.HaloScale = OutlineHaloScale;
            }
            if (_bottomBarPainter is TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters.SegmentedTrackBottomBarPainter segPainter)
            {
                segPainter.TrackHeight = SegmentedTrackHeight;
                segPainter.IndicatorWidth = SegmentedIndicatorWidth;
            }
            if (_bottomBarPainter is TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters.GlassAcrylicBottomBarPainter gPainter)
            {
                gPainter.AcrylicOpacity = GlassAcrylicOpacity;
            }
            // derive an animation phase for pulsing/hover effects -> 0..1
            double seconds = _tickerMs / 1000.0;
            ctx.AnimationPhase = (float)((Math.Sin(seconds * 2 * Math.PI * 0.9) + 1.0) / 2.0);
            // Set the current theme on the ImagePainter so it can recolor icons where applicable
            ctx.ImagePainter.CurrentTheme = _currentTheme;
            ctx.ImagePainter.ApplyThemeOnImage = true;
            ctx.CTAShadowYOffset = CTAShadowYOffset;

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
            if (Items == null || Items.Count == 0) return;
            _bbHitTestHelper?.HandleMouseClick(e.Location, e.Button);
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _bbHitTestHelper?.HandleMouseMove(e.Location);
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
            if (idx < 0) idx = 0;
            bool handled = false;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    idx = idx <= 0 ? Items.Count - 1 : idx - 1;
                    ActivateIndex(idx, raiseClick: false);
                    handled = true;
                    break;
                case Keys.Right:
                    idx = (idx + 1) % Items.Count;
                    ActivateIndex(idx, raiseClick: false);
                    handled = true;
                    break;
                case Keys.Home:
                    ActivateIndex(0, raiseClick: false);
                    handled = true;
                    break;
                case Keys.End:
                    ActivateIndex(Items.Count - 1, raiseClick: false);
                    handled = true;
                    break;
                case Keys.Space:
                case Keys.Enter:
                    ActivateIndex(idx, raiseClick: true);
                    handled = true;
                    break;
                default:
                    break;
            }
            if (handled)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            Invalidate();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            UpdateAccessibilityMetadata();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (Items.Count > 0 && _bbHitTestHelper.FocusedIndex < 0)
            {
                var idx = Items.IndexOf(SelectedItem);
                _bbHitTestHelper.FocusedIndex = idx >= 0 ? idx : 0;
            }
            UpdateAccessibilityMetadata();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            UpdateAccessibilityMetadata();
        }

        protected override AccessibleObject CreateAccessibilityInstance()
            => new BottomBarAccessibleObject(this);

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SyncLayoutAndHitTest();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            SyncLayoutAndHitTest();
        }

        protected override void OnDpiScaleChanged(float oldScaleX, float oldScaleY, float newScaleX, float newScaleY)
        {
            base.OnDpiScaleChanged(oldScaleX, oldScaleY, newScaleX, newScaleY);
            int minBarHeight = DpiScalingHelper.ScaleValue(48, DpiScalingHelper.GetDpiScaleFactor(this));
            if (Height < minBarHeight)
            {
                Height = minBarHeight;
            }
            SyncLayoutAndHitTest();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (_tickerTimer == null)
            {
                return;
            }

            if (Visible && !_isDisposed)
            {
                _tickerTimer.Start();
            }
            else
            {
                _tickerTimer.Stop();
            }
        }

        /// <summary>
        /// Cleans up resources used by the BottomBar.
        /// </summary>
        /// <param name="disposing">Whether disposing is in progress</param>
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _isDisposed = true;
                if (_items != null)
                {
                    _items.ListChanged -= Items_ListChanged;
                }
                if (_selectionTimer != null)
                {
                    _selectionTimer.Stop();
                    _selectionTimer.Tick -= SelectionTimer_Tick;
                    _selectionTimer.Dispose();
                }
                if (_tickerTimer != null)
                {
                    _tickerTimer.Stop();
                    _tickerTimer.Tick -= TickerTimer_Tick;
                    _tickerTimer.Dispose();
                }
                _imagePainter?.Dispose();
                _bottomBarPainter?.Dispose();
                if (_bbHitTestHelper != null)
                {
                    _bbHitTestHelper.ItemClicked -= BottomBarHit_ItemClicked;
                    _bbHitTestHelper.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// Applies the current theme tokens to the BottomBar control and its painters
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            try
            {
                if (_currentTheme == null) return;
                // Set bar background/fore using Navigation tokens if available
                BackColor = _currentTheme.NavigationBackColor != Color.Empty ? _currentTheme.NavigationBackColor : _currentTheme.SurfaceColor;
                ForeColor = _currentTheme.NavigationForeColor != Color.Empty ? _currentTheme.NavigationForeColor : _currentTheme.ForeColor;
                // Accent color default from theme
                AccentColor = _currentTheme.AccentColor;
                // Badge colors
                foreach (var item in Items)
                {
                    // Do not override per-item custom badge colors if set
                    if (item.BadgeBackColor == Color.Empty) item.BadgeBackColor = _currentTheme.BadgeBackColor;
                    if (item.BadgeForeColor == Color.Empty) item.BadgeForeColor = _currentTheme.BadgeForeColor;
                }
                // Update ImagePainter's theme if available
                _imagePainter.CurrentTheme = _currentTheme;
                _imagePainter.ApplyThemeOnImage = true;
                Invalidate();
            }
            catch { }
        }
        #endregion

        #region Helpers
        private void Items_ListChanged(object? sender, ListChangedEventArgs e)
        {
            SyncLayoutAndHitTest();
        }

        private void SyncLayoutAndHitTest()
        {
            if (_isDisposed)
            {
                return;
            }

            _layoutHelper.InvalidateLayout();
            if (Items == null || Items.Count == 0)
            {
                Invalidate();
                return;
            }

            var bounds = ClientRectangle;
            bounds.Inflate(-8, -8);
            _layoutHelper.EnsureLayout(bounds, Items.ToList(), CTAIndex, Items.IndexOf(SelectedItem));
            _bbHitTestHelper?.UpdateItems(Items.ToList(), new System.Collections.Generic.List<Rectangle>(_layoutHelper.GetItemRectangles()));
            Invalidate();
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
            using (var brush = new SolidBrush(_currentTheme == null ? Color.FromArgb(110, 110, 110) : _currentTheme.NavigationForeColor))
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

        private void InvalidateIndicatorRegion(float indicatorX, float indicatorWidth)
        {
            if (indicatorWidth <= 0 || !_layoutHelper.GetIndicatorRect().IntersectsWith(ClientRectangle))
            {
                Invalidate();
                return;
            }

            var indicatorTemplate = _layoutHelper.GetIndicatorRect();
            var invalidRect = new Rectangle(
                (int)Math.Floor(indicatorX) - 8,
                indicatorTemplate.Top - 8,
                (int)Math.Ceiling(indicatorWidth) + 16,
                indicatorTemplate.Height + 16);
            Invalidate(invalidRect);
        }

        private void OnSelectedItemChanged()
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(SelectedItem!));
            UpdateAccessibilityMetadata();
        }

        private void ActivateIndex(int index, bool raiseClick)
        {
            if (index < 0 || index >= Items.Count)
            {
                return;
            }

            _bbHitTestHelper.FocusedIndex = index;
            SelectedItem = Items[index];
            if (raiseClick)
            {
                ItemClicked?.Invoke(Items[index]);
            }

            UpdateAccessibilityMetadata();
        }

        private int GetItemIndexAt(Point clientPoint)
        {
            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                if (rects[i].Contains(clientPoint))
                {
                    return i;
                }
            }

            return -1;
        }

        private void UpdateAccessibilityMetadata()
        {
            if (string.IsNullOrWhiteSpace(AccessibleName))
            {
                AccessibleName = "Bottom Navigation";
            }

            if (AccessibleRole == AccessibleRole.Default || AccessibleRole == AccessibleRole.None)
            {
                AccessibleRole = AccessibleRole.MenuBar;
            }

            int count = Items?.Count ?? 0;
            int selectedIndex = Items?.IndexOf(SelectedItem) ?? -1;
            int focusedIndex = _bbHitTestHelper?.FocusedIndex ?? -1;
            string selectedText = selectedIndex >= 0 && selectedIndex < count ? Items[selectedIndex]?.Text : null;
            string focusedText = focusedIndex >= 0 && focusedIndex < count ? Items[focusedIndex]?.Text : null;

            string status = $"{AccessibilityDescriptionPrefix} {count} items. " +
                            (Enabled ? "Control enabled." : "Control disabled.");
            if (!string.IsNullOrWhiteSpace(selectedText))
            {
                status += $" Selected: {selectedText}.";
            }
            if (!string.IsNullOrWhiteSpace(focusedText))
            {
                status += $" Focused: {focusedText}.";
            }

            if (string.IsNullOrWhiteSpace(AccessibleDescription) ||
                AccessibleDescription.StartsWith(AccessibilityDescriptionPrefix, StringComparison.Ordinal))
            {
                AccessibleDescription = status;
            }

            AccessibleDefaultActionDescription = "Select navigation item";

            if (IsHandleCreated)
            {
                AccessibilityNotifyClients(AccessibleEvents.DescriptionChange, -1);
                AccessibilityNotifyClients(AccessibleEvents.ValueChange, -1);
            }
        }

        private sealed class BottomBarAccessibleObject : ControlAccessibleObject
        {
            private readonly BottomBar _owner;

            public BottomBarAccessibleObject(BottomBar owner) : base(owner)
            {
                _owner = owner;
            }

            public override AccessibleRole Role => AccessibleRole.MenuBar;
            public override string Name => _owner.AccessibleName ?? "Bottom Navigation";
            public override string Description => _owner.AccessibleDescription;

            public override int GetChildCount() => _owner.Items?.Count ?? 0;

            public override AccessibleObject GetChild(int index)
            {
                if (_owner.Items == null || index < 0 || index >= _owner.Items.Count)
                {
                    return null;
                }

                return new BottomBarItemAccessibleObject(_owner, this, index);
            }

            public override AccessibleObject HitTest(int x, int y)
            {
                var clientPoint = _owner.PointToClient(new Point(x, y));
                int idx = _owner.GetItemIndexAt(clientPoint);
                return idx >= 0 ? GetChild(idx) : base.HitTest(x, y);
            }
        }

        private sealed class BottomBarItemAccessibleObject : AccessibleObject
        {
            private readonly BottomBar _owner;
            private readonly AccessibleObject _parent;
            private readonly int _index;

            public BottomBarItemAccessibleObject(BottomBar owner, AccessibleObject parent, int index)
            {
                _owner = owner;
                _parent = parent;
                _index = index;
            }

            public override AccessibleObject Parent => _parent;
            public override AccessibleRole Role => AccessibleRole.MenuItem;
            public override string Name => _owner.Items[_index]?.Text ?? $"Item {_index + 1}";

            public override string Description
            {
                get
                {
                    var item = _owner.Items[_index];
                    if (!string.IsNullOrWhiteSpace(item?.SubText))
                    {
                        return item.SubText;
                    }
                    if (!string.IsNullOrWhiteSpace(item?.BadgeText))
                    {
                        return $"Badge {item.BadgeText}";
                    }
                    return string.Empty;
                }
            }

            public override Rectangle Bounds
            {
                get
                {
                    var rects = _owner._layoutHelper.GetItemRectangles();
                    if (_index < 0 || _index >= rects.Count)
                    {
                        return Rectangle.Empty;
                    }
                    return _owner.RectangleToScreen(rects[_index]);
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    var states = AccessibleStates.Selectable | AccessibleStates.Focusable;
                    if (!_owner.Enabled)
                    {
                        states |= AccessibleStates.Unavailable;
                    }

                    if (_owner.Items[_index] == _owner.SelectedItem)
                    {
                        states |= AccessibleStates.Selected;
                    }

                    if (_owner._bbHitTestHelper?.FocusedIndex == _index)
                    {
                        states |= AccessibleStates.Focused;
                    }

                    return states;
                }
            }

            public override string DefaultAction => "Select";

            public override void DoDefaultAction()
            {
                _owner.ActivateIndex(_index, raiseClick: true);
            }
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
