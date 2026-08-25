using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Containers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Containers.Models;
using TheTechIdea.Beep.Winform.Controls.Containers.Painters;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;
using TheTechIdea.Beep.Icons;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Containers")]
    [Description("A panel with a title and optional line below the title. Supports different shapes using regions.")]
    [DisplayName("Beep Panel")]
    // DISABLED: Designer attribute causes controls to be non-interactive when Design.Server assembly isn't properly loaded
    // [Designer("TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.BeepPanelDesigner, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    public partial class BeepPanel : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.Panel;
        private string _titleText = "Panel Title";
        private bool _showTitle = false;
        private bool _showTitleLine = false;
        private bool _titleLineFullWidth = true;
        private Color _titleLineColor = Color.Gray;
        private int _titleLineThickness = 2;
        private PanelTitleStyle _titleStyle = PanelTitleStyle.GroupBox;
        private int _titleGap = 8;

        private int _titleBottomY = 0;
        private ContentAlignment _titleAlignment = ContentAlignment.TopLeft;
        private PanelShape _panelShape = PanelShape.RoundedRectangle;
        private GraphicsPath _customShapePath;
        private CornerRadius _cornerRadius = new CornerRadius(8);
        private bool _cornerRadiusExplicit = false;
        private Region _panelRegion;
        private bool _isUpdatingRegion = false;
        private BeepPanelState _panelState = new BeepPanelState();
        private BeepPanelLayoutContext _panelLayoutContext = new BeepPanelLayoutContext();
        private PanelColorConfig _colorProfile = new PanelColorConfig();
        private bool _showTitleIcon;
        private int _titleIconSize = 16;
        private int _titleIconGap = 6;
        private bool _titleIconTintWithForeColor = true;

        private bool _isDisposing = false;
        private bool InDesignMode => LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode || (Site?.DesignMode ?? false);

        #region "Public Properties"
        // BeepPanel's TextFont is the TITLE font — a deliberate specialization of
        // BaseControl's body TextFont. The `new` keyword documents the shadowing.
        private new Font _textFont = SystemFonts.MessageBoxFont;
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Font TextFont
        {
            get => _textFont ?? Font;
            set
            {
                _textFont = value;
                UseThemeFont = false;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Panel color profile for runtime color overrides.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PanelColorConfig ColorProfile
        {
            get => _colorProfile;
            set
            {
                _colorProfile = value ?? new PanelColorConfig();
                ApplyColorProfile();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Title Bottom Location Y")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TitleBottomY
        {
            get => _titleBottomY;
            set { _titleBottomY = value; }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The text displayed as the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Icon key/path for the panel header title area.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue("")]
        public string IconPath
        {
            get => IconKey ?? string.Empty;
            set
            {
                string normalized = value ?? string.Empty;
                if (!string.Equals(IconKey ?? string.Empty, normalized, System.StringComparison.Ordinal))
                {
                    IconKey = normalized;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines whether to render an icon near the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        public bool ShowTitleIcon
        {
            get => _showTitleIcon;
            set
            {
                _showTitleIcon = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Header icon size in pixels before DPI scaling.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(16)]
        public int TitleIconSize
        {
            get => _titleIconSize;
            set
            {
                _titleIconSize = Math.Max(12, value);
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Spacing between header icon and title text before DPI scaling.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(6)]
        public int TitleIconGap
        {
            get => _titleIconGap;
            set
            {
                _titleIconGap = Math.Max(2, value);
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Tint header icon with title foreground color.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(true)]
        public bool TitleIconTintWithForeColor
        {
            get => _titleIconTintWithForeColor;
            set
            {
                _titleIconTintWithForeColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Config a line below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitleLine
        {
            get => _showTitleLine;
            set
            {
                if (_showTitleLine == value) return;
                _showTitleLine = value;
                // Line thickness contributes to headerHeight / ContentBounds top inset.
                if (IsHandleCreated)
                {
                    UpdateDrawingRect();
                    RefreshPanelLayoutContext(DrawingRect);
                    PerformLayout();
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the line below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TitleLineColor
        {
            get => _titleLineColor;
            set { _titleLineColor = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Thickness of the line below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TitleLineThickness
        {
            get => _titleLineThickness;
            set
            {
                _titleLineThickness = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines if the title is shown.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitle
        {
            get => _showTitle;
            set
            {
                if (_showTitle == value) return;
                _showTitle = value;
                // Refresh layout context so ContentBounds / _titleBottomY are recalculated
                // before PerformLayout repositions child controls.
                if (IsHandleCreated)
                {
                    UpdateDrawingRect();
                    RefreshPanelLayoutContext(DrawingRect);
                    PerformLayout();
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Alignment of the title text within the panel.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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
        [Description("Draw the title line with full width or just below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitleLineinFullWidth
        {
            get => _titleLineFullWidth;
            set
            {
                _titleLineFullWidth = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The style of the title display (GroupBox, Above, Below, Left, Right, Overlay).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(PanelTitleStyle.GroupBox)]
        public PanelTitleStyle TitleStyle
        {
            get => _titleStyle;
            set
            {
                if (_titleStyle != value)
                {
                    _titleStyle = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Gap between title text and border in GroupBox style (in pixels).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(8)]
        public int TitleGap
        {
            get => _titleGap;
            set
            {
                if (_titleGap != value)
                {
                    _titleGap = Math.Max(4, value);
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The shape style of the panel (Rectangle, RoundedRectangle, Ellipse, Custom).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(PanelShape.RoundedRectangle)]
        public PanelShape PanelShape
        {
            get => _panelShape;
            set
            {
                if (_panelShape != value)
                {
                    _panelShape = value;
                    // Auto-set IsRounded when using RoundedRectangle shape
                    if (value == PanelShape.RoundedRectangle && !IsRounded)
                    {
                        IsRounded = true;
                    }
                    UpdatePanelRegion();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Custom GraphicsPath for Custom shape mode. Set PanelShape to Custom to use this.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public GraphicsPath CustomShapePath
        {
            get => _customShapePath;
            set
            {
                if (_customShapePath != value)
                {
                    _customShapePath?.Dispose();
                    _customShapePath = value;
                    if (_panelShape == PanelShape.Custom)
                    {
                        UpdatePanelRegion();
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Per-corner radius, CSS <c>border-radius</c> style. When never set, each corner
        /// follows <see cref="BorderRadius"/> (the theme/uniform value). Setting any corner
        /// makes this the authoritative shape radius for the clip region.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Per-corner radius (CSS border-radius style). Leave unset to follow BorderRadius; set any corner to override the clip shape.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = value ?? new CornerRadius(0);
                _cornerRadiusExplicit = true;
                if (_panelShape == PanelShape.RoundedRectangle)
                {
                    UpdatePanelRegion();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Sets all four corner radii to the same value (and keeps the border radius aligned).
        /// </summary>
        public void SetCornerRadius(int all)
        {
            _cornerRadius.SetAll(all);
            _cornerRadiusExplicit = true;
            BorderRadius = all;
            UpdatePanelRegion();
            Invalidate();
        }

        /// <summary>
        /// Sets each corner independently, CSS order: top-left, top-right, bottom-right, bottom-left.
        /// </summary>
        public void SetCornerRadius(int topLeft, int topRight, int bottomRight, int bottomLeft)
        {
            _cornerRadius.TopLeft = topLeft;
            _cornerRadius.TopRight = topRight;
            _cornerRadius.BottomRight = bottomRight;
            _cornerRadius.BottomLeft = bottomLeft;
            _cornerRadiusExplicit = true;
            UpdatePanelRegion();
            Invalidate();
        }

        private CornerRadius EffectiveCornerRadius
            => _cornerRadiusExplicit ? _cornerRadius : new CornerRadius(BorderRadius);

        /// <summary>
        /// Builds a superellipse (squircle) path — the iOS/SwiftUI squircle shape.
        /// </summary>
        private static GraphicsPath CreateSquirclePath(Rectangle bounds, double n = 4.0)
        {
            var path = new GraphicsPath();
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            double w = bounds.Width / 2.0;
            double h = bounds.Height / 2.0;
            double cx = bounds.Left + w;
            double cy = bounds.Top + h;
            const int segments = 72;
            var points = new PointF[segments];
            double power = 2.0 / n;
            for (int i = 0; i < segments; i++)
            {
                double angle = 2.0 * Math.PI * i / segments;
                double cos = Math.Cos(angle);
                double sin = Math.Sin(angle);
                points[i] = new PointF(
                    (float)(cx + Math.Sign(cos) * Math.Pow(Math.Abs(cos), power) * w),
                    (float)(cy + Math.Sign(sin) * Math.Pow(Math.Abs(sin), power) * h));
            }
            path.AddPolygon(points);
            path.CloseFigure();
            return path;
        }

        #region Phase 1: Modern UX Properties

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Enable collapsible panel with expand/collapse functionality")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        public bool Collapsible { get; set; } = false;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Current collapsed state of the panel")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        public bool IsCollapsed
        {
            get => _isCollapsed;
            set
            {
                if (_isCollapsed != value)
                {
                    _isCollapsed = value;
                    if (Collapsible)
                    {
                        if (value)
                            Collapse(animate: true);
                        else
                            Expand(animate: true);
                    }
                }
            }
        }
        private bool _isCollapsed = false;

        [Browsable(true)]
        [Category("Animation")]
        [Description("Duration of collapse/expand animation in milliseconds")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(300)]
        public int CollapseAnimationDuration { get; set; } = 300;

        #endregion

        /// <summary>
        /// Override to prevent BaseControl from clearing the graphics surface over child controls.
        /// As a container control, we let Windows Forms handle child control painting naturally.
        /// FIX: Allow clearing for container controls so background paints correctly
        /// </summary>
        protected override bool AllowBaseControlClear => true;

        /// <summary>
        /// Override CreateParams to add WS_CLIPCHILDREN window style
        /// This ensures children paint in their own regions and parent doesn't paint over them
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // WS_EX_COMPOSITED - Double buffer entire window
                cp.Style |= 0x02000000;     // WS_CLIPCHILDREN - Exclude child areas from parent painting
                return cp;
            }
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                var rect = base.DisplayRectangle;
                var content = _panelLayoutContext?.ContentBounds ?? Rectangle.Empty;
                if (content.IsEmpty)
                {
                    return rect;
                }

                return Rectangle.Intersect(rect, content);
            }
        }
        

        /// <summary>
        /// Updates the panel region based on the current shape.
        /// This clips child controls to the panel's shape.
        /// </summary>
        private void UpdatePanelRegion()
        {
            if (IsDisposed || !IsHandleCreated || _isUpdatingRegion) return;
            
            try
            {
                _isUpdatingRegion = true;
                
                _panelRegion?.Dispose();
                _panelRegion = null;
                
                Rectangle bounds = ClientRectangle;
                if (bounds.Width <= 0 || bounds.Height <= 0) return;
                
                GraphicsPath path = null;
                try
                {
                    switch (_panelShape)
                    {
                        case PanelShape.Rectangle:
                            _panelRegion = new Region(bounds);
                            break;
                        case PanelShape.RoundedRectangle:
                            path = GraphicsExtensions.CreateRoundedRectanglePath(bounds, EffectiveCornerRadius);
                            if (path != null)
                            {
                                _panelRegion = new Region(path);
                            }
                            else
                            {
                                _panelRegion = new Region(bounds);
                            }
                            break;
                        case PanelShape.Ellipse:
                            path = new GraphicsPath();
                            path.AddEllipse(bounds);
                            _panelRegion = new Region(path);
                            break;
                        case PanelShape.Custom:
                            if (_customShapePath != null)
                            {
                                _panelRegion = new Region(_customShapePath);
                            }
                            else
                            {
                                _panelRegion = new Region(bounds);
                            }
                            break;
                        case PanelShape.Pill:
                            path = GraphicsExtensions.CreatePill(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                            _panelRegion = new Region(path);
                            break;
                        case PanelShape.Squircle:
                            path = CreateSquirclePath(bounds);
                            _panelRegion = new Region(path);
                            break;
                    }
                    
                    // Apply region to control to clip child controls
                    if (_panelRegion != null)
                    {
                        Region = _panelRegion.Clone();
                    }
                }
                finally
                {
                    path?.Dispose();
                    _isUpdatingRegion = false;
                }
            }
            catch (Exception ex)
            {
                _isUpdatingRegion = false;
                BeepLog.FallbackOnce("beeppanel.region", this, "update panel region", ex);
            }
        }

        private void ApplyColorProfile()
        {
            if (_colorProfile == null)
            {
                return;
            }

            if (_colorProfile.BackgroundColor != Color.Empty)
            {
                BackColor = _colorProfile.BackgroundColor;
            }
            if (_colorProfile.ForegroundColor != Color.Empty)
            {
                ForeColor = _colorProfile.ForegroundColor;
            }
            if (_colorProfile.BorderColor != Color.Empty)
            {
                BorderColor = _colorProfile.BorderColor;
            }
            if (_colorProfile.TitleLineColor != Color.Empty)
            {
                _titleLineColor = _colorProfile.TitleLineColor;
            }
        }

        private void RefreshPanelLayoutContext(Rectangle bounds)
        {
            string headerIconInput = GetHeaderIconInput();
            string resolvedIconPath = ResolveIconPath(headerIconInput);
            bool hasHeaderIconInput = !string.IsNullOrWhiteSpace(headerIconInput);
            _panelState = new BeepPanelState
            {
                TitleText = _titleText ?? string.Empty,
                ShowTitle = _showTitle,
                ShowTitleLine = _showTitleLine,
                ShowTitleLineFullWidth = _titleLineFullWidth,
                TitleAlignment = _titleAlignment,
                TitleStyle = _titleStyle,
                PanelShape = _panelShape,
                TitleGap = _titleGap,
                TitleLineThickness = _titleLineThickness,
                BorderThickness = BorderThickness,
                BorderRadius = BorderRadius,
                Padding = Padding,
                UseThemeColors = UseThemeColors,
                IsEnabled = Enabled,
                ShowTitleIcon = _showTitleIcon || hasHeaderIconInput,
                IconPath = headerIconInput,
                ResolvedIconPath = resolvedIconPath,
                TitleIconSize = _titleIconSize,
                TitleIconGap = _titleIconGap,
                TitleIconTintWithForeColor = _titleIconTintWithForeColor
            };

            _textFont = BeepPanelFontHelpers.GetTitleFont(this, _currentTheme);
            _panelLayoutContext = BeepPanelLayoutHelper.BuildLayout(this, bounds, _panelState, _textFont);
            _titleBottomY = _panelLayoutContext.HeaderBounds.Bottom;
        }

        private static string ResolveIconPath(string iconPath)
        {
            if (string.IsNullOrWhiteSpace(iconPath))
            {
                return string.Empty;
            }

            string input = iconPath.Trim();
            if (input.Contains("\\") || input.Contains("/") || input.StartsWith("<svg", System.StringComparison.OrdinalIgnoreCase))
            {
                return input;
            }

            var svgUiProperty = typeof(SvgsUI).GetProperty(input);
            if (svgUiProperty?.PropertyType == typeof(string))
            {
                var svgUiValue = svgUiProperty.GetValue(null) as string;
                if (!string.IsNullOrWhiteSpace(svgUiValue))
                {
                    return svgUiValue;
                }
            }

            var svgProperty = typeof(Svgs).GetProperty(input);
            if (svgProperty?.PropertyType == typeof(string))
            {
                var svgValue = svgProperty.GetValue(null) as string;
                if (!string.IsNullOrWhiteSpace(svgValue))
                {
                    return svgValue;
                }
            }

            var catalogPath = IconCatalog.GetPathByKey(input);
            if (!string.IsNullOrWhiteSpace(catalogPath))
            {
                return catalogPath;
            }

            return input;
        }

        private string GetHeaderIconInput()
        {
            if (!string.IsNullOrWhiteSpace(IconKey))
            {
                return IconKey;
            }

            if (!string.IsNullOrWhiteSpace(LeadingImagePath))
            {
                return LeadingImagePath;
            }

            return string.Empty;
        }

        #endregion

        #region "Constructor"
        public BeepPanel() : base()
        {
            // Prevent designer issues by not subscribing to events or theme managers in design mode
            if (InDesignMode)
            {
                this.Size = new Size(400, 300);
                this.BackColor = TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.Control);
                this.ForeColor = TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.ControlText);
                return;
            }

            // Runtime initialization
            CanBeFocused = false;
            CanBeSelected = false;
            CanBePressed = false;
            CanBeHovered = false;

            // Container control styles - SIMPLIFIED for proper child control painting
            // Use ONLY the essential styles that standard containers use
            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            
            // FIX: Disable external buffered graphics for containers
            // This was preventing child controls from painting
            UseExternalBufferedGraphics = false;
            
            DoubleBuffered = true;
            UpdateStyles();
            this.Size = new Size(400, 300);
            
            // Set default control style if not set
            if (ControlStyle == BeepControlStyle.None)
            {
                ControlStyle = BeepControlStyle.Material3;
            }

            ApplyColorProfile();
            
            // Initialize panel region after size is set
            if (IsHandleCreated)
            {
                UpdatePanelRegion();
            }

            try { ApplyTheme(); }
            catch (Exception ex)
            {
                BeepLog.FallbackOnce("beeppanel.theme.ctor", this, "apply theme in constructor", ex);
                this.BackColor = TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.Control);
                this.ForeColor = TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.ControlText);
            }
        }
        #endregion

        #region Phase 1: Collapsible Implementation

        public event EventHandler Collapsing;
        public event EventHandler Collapsed;
        public event EventHandler Expanding;
        public event EventHandler Expanded;

        private int _expandedHeight = 0;
        private int _collapsedHeight = 40; // Height when collapsed (just header)

        public void Collapse(bool animate = true)
        {
            if (_isCollapsed || !Collapsible) return;

            Collapsing?.Invoke(this, EventArgs.Empty);

            _expandedHeight = Height;

            if (animate && CollapseAnimationDuration > 0)
            {
                AnimateCollapse();
            }
            else
            {
                Height = _collapsedHeight;
                HideContent();
            }

            _isCollapsed = true;
            Collapsed?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        public void Expand(bool animate = true)
        {
            if (!_isCollapsed || !Collapsible) return;

            Expanding?.Invoke(this, EventArgs.Empty);

            if (animate && CollapseAnimationDuration > 0)
            {
                AnimateExpand();
            }
            else
            {
                Height = _expandedHeight > 0 ? _expandedHeight : 300;
                ShowContent();
            }

            _isCollapsed = false;
            Expanded?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        public void Toggle(bool animate = true)
        {
            if (_isCollapsed)
                Expand(animate);
            else
                Collapse(animate);
        }

        #endregion

        #region Accessibility

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new BeepPanelAccessibleObject(this);
        }

        private class BeepPanelAccessibleObject : ControlAccessibleObject
        {
            private readonly BeepPanel _owner;

            public BeepPanelAccessibleObject(BeepPanel owner) : base(owner)
            {
                _owner = owner;
            }

            public override string Name => _owner._titleText;
            public override string Description => _owner._isCollapsed ? "Collapsed panel" : "Expandable panel";
            public override AccessibleRole Role => AccessibleRole.Grouping;
            public override AccessibleStates State
            {
                get
                {
                    var state = base.State;
                    if (_owner._isCollapsed)
                        state |= AccessibleStates.Collapsed;
                    else
                        state |= AccessibleStates.Expanded;
                    return state;
                }
            }
        }

        #endregion

        private void AnimateCollapse()
        {
            int startHeight = Height;
            int endHeight = Math.Max(_collapsedHeight, MinimumSize.Height);
            int steps = Math.Max(1, CollapseAnimationDuration / 16);
            int currentStep = 0;

            Timer animTimer = new Timer { Interval = 16 };
            animTimer.Tick += (s, e) =>
            {
                if (IsDisposed || !IsHandleCreated)
                {
                    animTimer.Stop();
                    animTimer.Dispose();
                    return;
                }

                currentStep++;
                float progress = (float)currentStep / steps;
                progress = EaseInOutCubic(Math.Min(1f, progress));

                int newHeight = Math.Max(endHeight, (int)(startHeight + (endHeight - startHeight) * progress));
                SuspendLayout();
                Height = newHeight;
                ResumeLayout(false);

                if (currentStep >= steps)
                {
                    Height = endHeight;
                    HideContent();
                    animTimer.Stop();
                    animTimer.Dispose();
                }
            };
            animTimer.Start();
        }

        private void AnimateExpand()
        {
            ShowContent();
            int startHeight = Height;
            int endHeight = _expandedHeight > 0 ? _expandedHeight : 300;
            int steps = Math.Max(1, CollapseAnimationDuration / 16);
            int currentStep = 0;

            Timer animTimer = new Timer { Interval = 16 };
            animTimer.Tick += (s, e) =>
            {
                if (IsDisposed || !IsHandleCreated)
                {
                    animTimer.Stop();
                    animTimer.Dispose();
                    return;
                }

                currentStep++;
                float progress = (float)currentStep / steps;
                progress = EaseInOutCubic(Math.Min(1f, progress));

                int newHeight = Math.Max(MinimumSize.Height, (int)(startHeight + (endHeight - startHeight) * progress));
                SuspendLayout();
                Height = newHeight;
                ResumeLayout(false);

                if (currentStep >= steps)
                {
                    Height = endHeight;
                    animTimer.Stop();
                    animTimer.Dispose();
                }
            };
            animTimer.Start();
        }

        private float EaseInOutCubic(float t)
        {
            return t < 0.5f ? 4 * t * t * t : 1 - (float)Math.Pow(-2 * t + 2, 3) / 2;
        }

        private void HideContent()
        {
            foreach (var child in Controls.Cast<Control>().Where(c => !c.IsDisposed).ToArray())
            {
                child.Visible = false;
            }
        }

        private void ShowContent()
        {
            foreach (var child in Controls.Cast<Control>().Where(c => !c.IsDisposed).ToArray())
            {
                child.Visible = true;
            }
        }

        // Override Dispose to properly clean up
        protected override void Dispose(bool disposing)
        {
            _isDisposing = true;
            if (disposing)
            {
                // Dispose custom shape path
                _customShapePath?.Dispose();
                _customShapePath = null;

                // Dispose panel region
                _panelRegion?.Dispose();
                _panelRegion = null;

                // Note: _textFont is intentionally not disposed here. It may be a
                // shared system font, a theme-cached font, or a user-assigned font —
                // the owner of the font is responsible for its disposal.
            }

            base.Dispose(disposing);
        }


     

        public override void ApplyTheme()
        {
            if (InDesignMode) return; // keep designer stable

            this.SuspendLayout();
            try
            {
                base.ApplyTheme();
                if (_currentTheme == null) return;

                BackColor = _currentTheme.PanelBackColor;
                ForeColor = _currentTheme.PrimaryTextColor;
                BorderColor = _currentTheme.BorderColor;

                if (UseGradientBackground)
                {
                    GradientStartColor = _currentTheme.PanelGradiantStartColor != Color.Empty ? _currentTheme.PanelGradiantStartColor : _currentTheme.GradientStartColor;
                    GradientEndColor = _currentTheme.PanelGradiantEndColor != Color.Empty ? _currentTheme.PanelGradiantEndColor : _currentTheme.GradientEndColor;
                    GradientDirection = _currentTheme.GradientDirection;
                }

                if (!string.IsNullOrEmpty(_titleText) && _showTitle)
                {
                    _titleLineColor = _currentTheme.CardTitleForeColor != Color.Empty ? _currentTheme.CardTitleForeColor : _currentTheme.PrimaryTextColor;
                }

                if (UseThemeFont)
                {
                    _textFont = BeepPanelFontHelpers.GetTitleFont(this, _currentTheme);
                }
                if (_currentTheme.BorderRadius > 0)
                {
                    IsRounded = true;
                    BorderRadius = _currentTheme.BorderRadius;
                }
                if (!_cornerRadiusExplicit)
                {
                    _cornerRadius.SetAll(BorderRadius);
                }
                if (_currentTheme.BorderSize > 0) { BorderThickness = _currentTheme.BorderSize; }

                ShadowColor = _currentTheme.ShadowColor;
                ShadowOpacity = _currentTheme.ShadowOpacity;
                ApplyColorProfile();
                
                // Update region when theme changes (only if shape requires it)
                if (IsHandleCreated && (_panelShape == PanelShape.RoundedRectangle || _panelShape == PanelShape.Ellipse))
                {
                    UpdatePanelRegion();
                }
            }
            catch (Exception ex)
            {
                BeepLog.FallbackOnce("beeppanel.theme", this, "apply theme", ex);
                this.BackColor = TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.Control); ForeColor = TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(SystemColors.ControlText);
            }
            finally { this.ResumeLayout(false); }

            Invalidate();
        }


        protected override void DrawContent(Graphics g)
        {
            if (_isDisposing || IsDisposed || !IsHandleCreated) return;
            base.DrawContent(g);
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // Always refresh layout so _titleBottomY and ContentBounds are current,
                // even when ShowTitle is false (otherwise child controls keep the old offset).
                UpdateDrawingRect();
                RefreshPanelLayoutContext(DrawingRect);

                // Draw title if shown - this follows the control style
                if (_showTitle && !string.IsNullOrEmpty(_titleText))
                {
                    DrawTitle(g, DrawingRect);
                }
                
                // Note: Child controls are painted automatically by Windows Forms
                // after DrawContent completes, so they appear on top of the title
            }
            catch (Exception ex) { BeepLog.FallbackOnce("beeppanel.drawContent", this, "draw panel content", ex); }
        }
        
      
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Only update region if shape is not Rectangle (Rectangle doesn't need region)
            if (IsHandleCreated && !IsDisposed && _panelShape != PanelShape.Rectangle)
            {
                UpdatePanelRegion();
            }
        }
        
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // Only update region if shape is not Rectangle
            if (!IsDisposed && _panelShape != PanelShape.Rectangle)
            {
                UpdatePanelRegion();
            }
        }

        private void DrawTitle(Graphics g, Rectangle rectangle)
        {
            if (_isDisposing || IsDisposed) return;
            if (string.IsNullOrEmpty(_titleText) || _textFont == null) return;

            try
            {
                Color textColor = ForeColor;
                if (UseThemeColors && _currentTheme != null)
                {
                    textColor = _currentTheme.PrimaryTextColor;
                }
                RefreshPanelLayoutContext(rectangle);
                BeepPanelPainter.DrawTitleOverlay(
                    g,
                    _panelState,
                    _panelLayoutContext,
                    _textFont,
                    textColor,
                    BackColor,
                    _titleLineColor);
            }
            catch (Exception ex) { BeepLog.FallbackOnce("beeppanel.drawTitle", this, "draw panel title", ex); }
        }


        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            if (_isDisposing || IsDisposed ) return;
            try
            {
                RefreshPanelLayoutContext(rectangle);
                DrawTitle(graphics, rectangle);
            }
            catch (Exception ex) { BeepLog.FallbackOnce("beeppanel.draw", this, "draw panel", ex); }
        }
    }
}

