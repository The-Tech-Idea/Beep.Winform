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
using TheTechIdea.Beep.Winform.Controls.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars
{
    /// <summary>
    /// Simple BottomBar navigation control.
    /// Works on a list of SimpleItem (icons + text) and offers a centered floating CTA and selection animation.
    /// </summary>
    [ToolboxItem(true)]
    [Designer(typeof(ControlDesigner))]
    public partial class BottomBar : BaseControl
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
        private LabelVisibilityPolicy _labelPolicy = LabelVisibilityPolicy.Always;
        private ToolTip? _toolTip;
        private string _lastTooltipText = "";
        private bool _isDisposed;

        /// <summary>Height of the visible bar band, excluding any CTA headroom above it.</summary>
        private int _barHeight = 72;

        /// <summary>
        /// Initializes a new instance of the <see cref="BottomBar"/> control.
        /// </summary>
        public BottomBar()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            Height = _barHeight;

            // Placement, not Dock. DockStyle.Bottom forces the bar to span the parent's full width,
            // and a docked control cannot be centred because docking owns its bounds - so the
            // floating centred pill the reference designs show was unreachable. ApplyPlacement runs
            // again once there is a parent to measure against.
            Placement = BottomBarPlacement.CenteredBottom;
            BackColor = Color.White;
            _items.ListChanged += Items_ListChanged;
            InitializeAnimationTimer();
            _bbHitTestHelper = new BottomBarHitTestHelper(this);
            _bbHitTestHelper.ItemClicked += BottomBarHit_ItemClicked;
            _bbHitTestHelper.PopupRequested += BottomBarHit_PopupRequested;
            _bbHitTestHelper.PopupClosed += BottomBarHit_PopupClosed;
            InitializePainterFromStyle(_style);

            // The painter decides the headroom, so the control can only be sized once one exists.
            ApplyBarHeight();
            UpdateTickerState();

            TabStop = true; // enable keyboard focus
            this.AccessibleRole = AccessibleRole.MenuBar;
            this.AccessibleName = "Bottom Navigation";
            _tickerTimer = new Timer { Interval = 50 };
            _tickerTimer.Tick += TickerTimer_Tick;

            // Started by UpdateTickerState, not here. This used to Start() unconditionally in the
            // constructor and never stop: a bar that was hidden, had no items, or used a style with
            // nothing to animate still invalidated the whole control twenty times a second for the
            // lifetime of the form.
            _toolTip = new ToolTip { InitialDelay = 400, ReshowDelay = 100, ShowAlways = true };
            UpdateAccessibilityMetadata();
        }

        private void TickerTimer_Tick(object? sender, EventArgs e)
        {
            if (!ShouldAnimate)
            {
                UpdateTickerState();
                return;
            }

            _tickerMs += _tickerTimer!.Interval;
            Invalidate();
        }

        /// <summary>Whether there is anything worth repainting on a timer right now.</summary>
        private bool ShouldAnimate =>
            !_isDisposed
            && _tickerTimer != null
            && Visible
            && IsHandleCreated
            && Items is { Count: > 0 }
            && _animateContinuously
            && (_bottomBarPainter?.WantsContinuousAnimation ?? false);

        private bool _animateContinuously = true;

        /// <summary>
        /// Whether styles with a breathing selection effect keep animating.
        /// </summary>
        /// <remarks>
        /// The motion was perpetual and had no off switch. A navigation bar that never stops moving is
        /// a distraction on a desktop and a battery cost on a laptop, and reduced-motion settings have
        /// nothing to turn off. The default keeps the existing look; setting this false stops the
        /// ticker outright.
        /// </remarks>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Whether styles with a breathing selection effect keep animating.")]
        public bool AnimateContinuously
        {
            get => _animateContinuously;
            set
            {
                if (_animateContinuously == value) return;
                _animateContinuously = value;
                UpdateTickerState();
                Invalidate();
            }
        }

        /// <summary>Runs the ticker only while something is actually animating.</summary>
        internal void UpdateTickerState()
        {
            if (_tickerTimer == null) return;

            bool run = ShouldAnimate;
            if (run == _tickerTimer.Enabled) return;

            if (run) _tickerTimer.Start();
            else _tickerTimer.Stop();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            UpdateTickerState();
        }

        private void BottomBarHit_ItemClicked(object? sender, ItemClickEventArgs e)
        {
            if (e?.Item != null)
            {
                ActivateIndex(e.Index, raiseClick: true);
            }
        }

        private void BottomBarHit_PopupRequested(object? sender, Helpers.PopupEventArgs e)
        {
            PopupRequested?.Invoke(this, e);
        }

        private void BottomBarHit_PopupClosed(object? sender, Helpers.PopupEventArgs e)
        {
            PopupClosed?.Invoke(this, e);
            Invalidate();
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
            var eased = 1 - Math.Pow(1 - progress, 4);
            _indicatorX = _indicatorStartX + (float)(_indicatorTargetX - _indicatorStartX) * (float)eased;
            _indicatorWidth = _indicatorStartWidth + (float)(_indicatorTargetWidth - _indicatorStartWidth) * (float)eased;
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

                // Rebuild the layout and hit areas, exactly as a change WITHIN the list does.
                //
                // This used to call Invalidate() alone. EnsureLayout skips recomputing when the bounds
                // have not changed and nothing marked it dirty, and the item list is not part of that
                // test - so the cached rectangles from the PREVIOUS list survived. Painters then walk
                // `for (i = 0; i < rects.Count; i++) context.Items[i]`, and a shorter list threw
                // ArgumentOutOfRangeException out of OnPaint, which has no catch: the ticker's
                // Invalidate re-raised it about twenty times a second. A longer list failed the other
                // way - the extra items were never painted and never hit-testable.
                SyncLayoutAndHitTest();
                ApplyPlacement();
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

                    // Styles differ in how far their CTA protrudes, so the control's height follows
                    // the style rather than staying at whatever the previous one needed.
                    ApplyBarHeight();

                    // ...and only some styles animate, so the ticker follows the style too.
                    UpdateTickerState();

                    // A style that protrudes registers its overhang with the parent; one that does
                    // not must clear a registration the previous style left behind.
                    UpdateCtaExternalDrawing();

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
            get => _barHeight;
            set
            {
                _barHeight = Math.Max(48, value);
                ApplyBarHeight();
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
        // [DefaultValue] must match the initialiser or the designer omits the property when it is
        // set to the value it claims is default - here it would drop ShowCTAShadow = false and the
        // shadow would come back on the next load.
        [DefaultValue(true)]
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
        [DefaultValue(1.0f)]
        [Description("Extra width for the selected cell. 1.0 keeps the equal-cell grid the reference designs use.")]
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

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(LabelVisibilityPolicy.Always)]
        public LabelVisibilityPolicy LabelPolicy
        {
            get => _labelPolicy;
            set
            {
                if (_labelPolicy != value)
                {
                    _labelPolicy = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(48)]
        public int MinItemTouchWidth
        {
            get => _layoutHelper.MinTouchTargetWidth;
            set
            {
                _layoutHelper.MinTouchTargetWidth = Math.Max(32, value);
                SyncLayoutAndHitTest();
            }
        }

        [Browsable(false)]
        public bool IsOverflow => _layoutHelper.IsOverflow;
        #endregion

        #region Events
        public event Action<SimpleItem>? ItemClicked;
        public event EventHandler<SelectedItemChangedEventArgs>? SelectedItemChanged;
        public event EventHandler<Helpers.PopupEventArgs>? PopupRequested;
        public event EventHandler<Helpers.PopupEventArgs>? PopupClosed;
        #endregion

        #region Overrides

        /// <summary>
        /// The band the painters draw the bar into, and the same rectangle the hit areas are built
        /// from.
        /// </summary>
        /// <remarks>
        /// <para>
        /// One method, three callers. This was computed independently in OnPaint, in
        /// SyncLayoutAndHitTest and in StartIndicatorAnimationToSelected - once via the PainterInset
        /// constant and twice as a literal 8. They agreed only because the numbers happened to match,
        /// and paint and hit-testing disagreeing means clicking one item and selecting another.
        /// </para>
        /// <para>
        /// The top is pulled down by whatever headroom the current style needs, so a CTA that
        /// protrudes above the bar has somewhere to go inside the control instead of being clipped
        /// by its top edge.
        /// </para>
        /// </remarks>
        private Rectangle GetPainterBounds()
        {
            var rect = ClientRectangle;
            rect.Inflate(-PainterInset, -PainterInset);

            int band = BandContentHeight;
            int overhang = CanDrawOutsideBounds ? 0 : CurrentTopOverhang(band);

            if (overhang > 0 && rect.Height > band)
            {
                rect.Y += rect.Height - band;
                rect.Height = band;
            }

            return rect;
        }

        /// <summary>The band's inner height - what BarHeight asks for, less the painter inset.</summary>
        private int BandContentHeight => Math.Max(1, _barHeight - PainterInset * 2);

        /// <summary>Headroom the current style needs above the bar band.</summary>
        internal int CurrentTopOverhang(int contentHeight)
            => _bottomBarPainter?.GetTopOverhang(contentHeight) ?? 0;

        /// <summary>
        /// Sizes the control to the band plus whatever headroom the current style needs.
        /// </summary>
        /// <remarks>
        /// BarHeight is the height of the visible bar. A style whose CTA protrudes above it makes the
        /// CONTROL taller so the shape has somewhere to go; the band keeps the height that was asked
        /// for. Taking the headroom out of the band instead was tried first and does not work - at any
        /// ordinary bar height the CTA wants more room than the band can spare, so the overhang was
        /// simply skipped and the shape stayed clipped.
        /// </remarks>
        private void ApplyBarHeight()
        {
            // On a provider parent the CTA is drawn outside the control, so no headroom is reserved
            // and the control is exactly the bar. Everywhere else the headroom is what stops the
            // shape being clipped.
            int overhang = CanDrawOutsideBounds ? 0 : CurrentTopOverhang(BandContentHeight);
            int desired = _barHeight + overhang;
            if (Height != desired) Height = desired;
        }


        /// <summary>
        /// Whether the parent can host drawing that falls outside this control's bounds.
        /// </summary>
        /// <remarks>
        /// Only BaseControl-derived containers and BeepiFormPro implement the provider, so this is
        /// false for a plain Panel or Form - and the fallback below matters, because the alternative
        /// there is not a clipped CTA but no CTA at all.
        /// </remarks>
        private bool CanDrawOutsideBounds => Parent is IExternalDrawingProvider;

        /// <summary>
        /// Registers the protruding part of the CTA to be drawn on the parent's own surface.
        /// </summary>
        /// <remarks>
        /// <para>
        /// WinForms has no real transparency. Reserving headroom inside the control and letting
        /// IsChild fill it with the parent's back colour works over a flat parent - which is what
        /// IsChild samples - but over a gradient, an image, or another control it is a flat rectangle
        /// of one sampled colour sitting where the background should show through.
        /// </para>
        /// <para>
        /// Drawing on the parent has no such limit, so it is the preferred path and the control keeps
        /// its band height. The reserved headroom remains the fallback for parents that cannot host
        /// external drawing.
        /// </para>
        /// </remarks>
        private void UpdateCtaExternalDrawing()
        {
            if (Parent is not IExternalDrawingProvider provider) return;

            provider.ClearChildExternalDrawing(this);

            if (CurrentTopOverhang(BandContentHeight) <= 0) return;

            provider.AddChildExternalDrawing(this, DrawCtaOnParent, DrawingLayer.AfterAll);
            try { Parent?.Invalidate(); } catch (Exception ex) { BeepLog.Fallback(this, "invalidate parent for CTA overhang", ex); }
        }

        /// <summary>Draws the bar onto the parent, clipped to the strip above this control.</summary>
        private void DrawCtaOnParent(Graphics parentGraphics, Rectangle childBounds)
        {
            int overhang = CurrentTopOverhang(BandContentHeight);
            if (overhang <= 0 || parentGraphics == null) return;

            var strip = new Rectangle(childBounds.Left, childBounds.Top - overhang, childBounds.Width, overhang);
            if (strip.Height <= 0 || strip.Width <= 0) return;

            var saved = parentGraphics.Save();
            try
            {
                // Only the part above the control lands; the rest is already painted on the control.
                parentGraphics.SetClip(strip);
                parentGraphics.TranslateTransform(childBounds.Left, childBounds.Top);
                PaintBar(parentGraphics);
            }
            catch (Exception ex)
            {
                BeepLog.Failure(this, "draw the CTA overhang on the parent", ex);
            }
            finally
            {
                parentGraphics.Restore(saved);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            PaintBar(e.Graphics);
        }

        /// <summary>
        /// Paints the whole bar into <paramref name="g"/>, in control-local coordinates.
        /// </summary>
        /// <remarks>
        /// Called twice for a style whose CTA protrudes, when the parent supports external drawing:
        /// once onto the control, and once onto the parent clipped to the strip above the control.
        /// The same painter draws both halves, so the two cannot drift apart the way a separate
        /// "draw just the CTA" routine in each painter would.
        /// </remarks>
        private void PaintBar(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Create painter context
            var rect = GetPainterBounds();
            if (Items == null || Items.Count == 0) return;

            var ctx = new BottomBarPainterContext
            {
                Graphics = g,
                Bounds = rect,
                Items = Items.ToList(),
                SelectedIndex = Items.IndexOf(SelectedItem),
                HoverIndex = _bbHitTestHelper?.HoveredIndex ?? -1,
                HitTest = _bbHitTestHelper?.ControlHitTest,
                BarHitTest = _bbHitTestHelper,
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
                var shadowBase = _currentTheme.NavigationHoverBackColor != Color.Empty ? _currentTheme.NavigationHoverBackColor : (_currentTheme.BorderColor != Color.Empty ? _currentTheme.BorderColor : _currentTheme.SurfaceColor);
                // Use a slightly stronger alpha to better mimic a soft shadow; painters may use this directly or build layered shadows
                ctx.NavigationShadowColor = Color.FromArgb(100, shadowBase.R, shadowBase.G, shadowBase.B);
            }
            // precompute layout with selected item included for reflow
            _layoutHelper.CtaWidthFactor = CTAWidthFactor;
            _layoutHelper.SelectedWidthFactor = SelectedWidthFactor;

            // The 74/24/12 grid is specified in logical pixels, so it needs the monitor scale to
            // survive on a scaled display. DeviceDpi is authoritative and updates on WM_DPICHANGED.
            _layoutHelper.DpiScale = DpiScalingHelper.GetDpiScaleFactor(this);
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
            ctx.LabelPolicy = _labelPolicy;
            ctx.HasChildrenPopup = _bbHitTestHelper?.PopupOpen ?? false;
            ctx.PopupParentIndex = _bbHitTestHelper?.PopupParentIndex ?? -1;

            _bottomBarPainter?.CalculateLayout(ctx);
            // Ensure hit helper is updated with computed rectangles
            _bbHitTestHelper?.UpdateItems(ctx.Items,
                new System.Collections.Generic.List<Rectangle>(ctx.LayoutHelper.GetItemRectangles()));
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
            UpdateTooltip(e.Location);
        }

        private void UpdateTooltip(Point location)
        {
            if (_toolTip == null || _bbHitTestHelper == null || Items == null || Items.Count == 0) return;
            if (_bbHitTestHelper.PopupOpen)
            {
                if (!string.IsNullOrEmpty(_lastTooltipText))
                {
                    _toolTip.Hide(this);
                    _lastTooltipText = "";
                }
                return;
            }

            int idx = _bbHitTestHelper.FindItemAt(location);
            if (idx < 0 || idx >= Items.Count)
            {
                if (!string.IsNullOrEmpty(_lastTooltipText))
                {
                    _toolTip.Hide(this);
                    _lastTooltipText = "";
                }
                return;
            }

            var item = Items[idx];
            if (item == null) return;

            bool showTooltip = _labelPolicy == LabelVisibilityPolicy.IconOnly ||
                               (!string.IsNullOrEmpty(item.SubText)) ||
                               (item.Children != null && item.Children.Count > 0);

            if (!showTooltip)
            {
                if (!string.IsNullOrEmpty(_lastTooltipText))
                {
                    _toolTip.Hide(this);
                    _lastTooltipText = "";
                }
                return;
            }

            string tooltipText = item.Text;
            if (!string.IsNullOrEmpty(item.SubText))
                tooltipText += Environment.NewLine + item.SubText;
            if (item.Children != null && item.Children.Count > 0)
                tooltipText += Environment.NewLine + $"({item.Children.Count} sub-items)";
            if (!string.IsNullOrEmpty(item.BadgeText))
                tooltipText += Environment.NewLine + $"Badge: {item.BadgeText}";

            if (tooltipText != _lastTooltipText)
            {
                _toolTip.Hide(this);
                _toolTip.SetToolTip(this, tooltipText);
                _lastTooltipText = tooltipText;
            }
        }

        /// <summary>
        /// Determines if a given key is treated as an input key (so it will be processed by the control).
        /// </summary>
        /// <param name="keyData">Key data pressed</param>
        /// <returns>True if the key should be processed by the control</returns>
        protected override bool IsInputKey(Keys keyData)
        {
            // We want to handle arrow keys, home/end, space, enter, escape
            if (keyData == Keys.Left || keyData == Keys.Right || keyData == Keys.Home || keyData == Keys.End || keyData == Keys.Space || keyData == Keys.Enter || keyData == Keys.Escape)
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
                    if (_bbHitTestHelper?.PopupOpen == true)
                    {
                        _bbHitTestHelper.ClosePopup();
                    }
                    else
                    {
                        ActivateIndex(idx, raiseClick: true);
                    }
                    handled = true;
                    break;
                case Keys.Escape:
                    if (_bbHitTestHelper?.PopupOpen == true)
                    {
                        _bbHitTestHelper.ClosePopup();
                    }
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
                DisposePlacement();
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
                    _bbHitTestHelper.PopupRequested -= BottomBarHit_PopupRequested;
                    _bbHitTestHelper.PopupClosed -= BottomBarHit_PopupClosed;
                    _bbHitTestHelper.Dispose();
                }
                if (_toolTip != null)
                {
                    _toolTip.RemoveAll();
                    _toolTip.Dispose();
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BottomBar.ApplyTheme error: {ex.Message}");
            }
        }
        #endregion

        #region Helpers
        public void CloseChildPopup()
        {
            _bbHitTestHelper?.ClosePopup();
        }

        private void Items_ListChanged(object? sender, ListChangedEventArgs e)
        {
            SyncLayoutAndHitTest();

            // The centred placement sizes the bar to its item count, so adding or removing an item
            // changes how wide the panel should be.
            ApplyPlacement();
        }

        private void SyncLayoutAndHitTest()
        {
            if (_isDisposed)
            {
                return;
            }

            _layoutHelper.InvalidateLayout();
            UpdateTickerState();
            if (Items == null || Items.Count == 0)
            {
                Invalidate();
                return;
            }

            var bounds = GetPainterBounds();
            _layoutHelper.EnsureLayout(bounds, Items.ToList(), CTAIndex, Items.IndexOf(SelectedItem));
            _bbHitTestHelper?.UpdateItems(Items.ToList(),
                new System.Collections.Generic.List<Rectangle>(_layoutHelper.GetItemRectangles()));
            Invalidate();
        }

        private void StartIndicatorAnimationToSelected()
        {
            if (SelectedItem == null) return;
            var idx = Items.IndexOf(SelectedItem);
            if (idx < 0) return;

            // The same band the painters and the hit areas use - see GetPainterBounds.
            var rect = GetPainterBounds();
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
            bool hasPopup = _bbHitTestHelper?.PopupOpen ?? false;
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
            if (hasPopup)
            {
                status += " Popup menu open.";
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

                    var item = _owner.Items[_index];
                    if (item != null && item.Children != null && item.Children.Count > 0)
                    {
                        states |= AccessibleStates.HasPopup;
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

        #endregion
    }
}
