using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
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
        const int startyoffset = 0;
        private string _titleText = "Panel Title";
        private bool _showTitle = true;
        private bool _showTitleLine = true;
        private bool _titleLineFullWidth = true;
        private Color _titleLineColor = Color.Gray;
        private int _titleLineThickness = 2;
        private PanelTitleStyle _titleStyle = PanelTitleStyle.GroupBox;
        private int _titleGap = 8; // Gap between title text and border in GroupBox style

        private int _titleBottomY = startyoffset;
        private ContentAlignment _titleAlignment = ContentAlignment.TopLeft;
        private PanelShape _panelShape = PanelShape.RoundedRectangle;
        private GraphicsPath _customShapePath;
        private Region _panelRegion;
        private bool _isUpdatingRegion = false; // Prevent recursive region updates

        int padding = 2; // Adjusted padding for top, left, etc.

        // Track disposing to prevent paint/draw during removal
        private bool _isDisposing = false;
        private bool InDesignMode => LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode || (Site?.DesignMode ?? false);

        #region "Scrolling"
        private bool _enableScrolling = false;
        private int _scrollOffset = 0;
        private int _scrollSpeed = 1;
        private Timer _scrollTimer;
        private int _scrollInterval = 10;
        private int _scrollDirection = 1;
        private VScrollBar _verticalScrollBar;
        private HScrollBar _horizontalScrollBar;
        #endregion

        #region "Public Properties"
        private Font _textFont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
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
        [Description("Config a line below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitleLine
        {
            get => _showTitleLine;
            set { _showTitleLine = value; Invalidate(); }
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
            set { _titleLineThickness = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines if the title is shown.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitle
        {
            get => _showTitle;
            set { _showTitle = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Alignment of the title text within the panel.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContentAlignment TitleAlignment
        {
            get => _titleAlignment;
            set { _titleAlignment = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Draw the title line with full width or just below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitleLineinFullWidth
        {
            get => _titleLineFullWidth;
            set { _titleLineFullWidth = value; Invalidate(); }
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
        [DefaultValue(PanelShape.Rectangle)]
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

        #region Phase 1: Modern UX Properties

        [Browsable(true)]
        [Category("Scrolling")]
        [Description("Enable automatic scrollbars when content overflows")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        public bool AutoScroll
        {
            get => _enableScrolling;
            set
            {
                if (_enableScrolling != value)
                {
                    _enableScrolling = value;
                    if (value)
                        InitializeScrollBars();
                    else
                        RemoveScrollBars();
                    Invalidate();
                }
            }
        }

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
        [Category("Appearance")]
        [Description("Material Design elevation level (0-5), adds shadow depth")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(0)]
        public int Elevation { get; set; } = 0;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Elevation level when hovering (0-5)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(0)]
        public int ElevationOnHover { get; set; } = 0;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Keep header visible when scrolling content")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(false)]
        public bool StickyHeader { get; set; } = false;

        [Browsable(true)]
        [Category("State")]
        [Description("Current state of the panel (Normal, Loading, Empty, Error)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(PanelState.Normal)]
        public PanelState State { get; set; } = PanelState.Normal;

        [Browsable(true)]
        [Category("State")]
        [Description("Message to display when panel is in Empty state")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string EmptyStateMessage { get; set; } = "No content to display";

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
        

        /// <summary>
        /// Gets the shape path based on PanelShape using PathPainters
        /// </summary>
        private GraphicsPath GetShapePath(Rectangle bounds)
        {
            switch (_panelShape)
            {
                case PanelShape.Rectangle:
                    return PathPainterHelpers.CreateRoundedRectangle(bounds, 0);
                    
                case PanelShape.RoundedRectangle:
                    return PathPainterHelpers.CreateRoundedRectangle(bounds, BorderRadius);
                    
                case PanelShape.Ellipse:
                    var ellipsePath = new GraphicsPath();
                    ellipsePath.AddEllipse(bounds);
                    return ellipsePath;
                    
                case PanelShape.Custom:
                    if (_customShapePath != null)
                    {
                        return (GraphicsPath)_customShapePath.Clone();
                    }
                    return PathPainterHelpers.CreateRoundedRectangle(bounds, 0);
                    
                default:
                    return PathPainterHelpers.CreateRoundedRectangle(bounds, 0);
            }
        }
        
      
        
        /// <summary>
        /// Paints the border using BorderPainters, with custom path for GroupBox style
        /// </summary>
        private void PaintBorder(Graphics g, GraphicsPath basePath, Rectangle bounds)
        {
            if (basePath == null || basePath.PointCount == 0) return;
            
            try
            {
                GraphicsPath borderPath = basePath;
                
                // For GroupBox style, create a border path with gap for title
                if (_titleStyle == PanelTitleStyle.GroupBox && _showTitle && !string.IsNullOrEmpty(_titleText))
                {
                    borderPath = CreateGroupBoxBorderPath(bounds);
                }
                
                if (borderPath == null) return;
                
                var borderPainter = BorderPainterFactory.CreatePainter(ControlStyle);
                if (borderPainter != null)
                {
                    var state = ControlState.Normal;
                    if (IsHovered) state = ControlState.Hovered;
                    if (IsFocused) state = ControlState.Focused;
                    if (IsPressed) state = ControlState.Pressed;
                    
                    borderPainter.Paint(g, borderPath, IsFocused, ControlStyle, _currentTheme, UseThemeColors, state);
                }
                else
                {
                    // Fallback to simple border
                    Color borderColor = BorderColor;
                    if (UseThemeColors && _currentTheme != null)
                    {
                        borderColor = _currentTheme.BorderColor;
                    }
                    
                    using (var pen = new Pen(borderColor, BorderThickness))
                    {
                        g.DrawPath(pen, borderPath);
                    }
                }
                
                // Dispose if we created a new path
                if (borderPath != basePath)
                {
                    borderPath?.Dispose();
                }
            }
            catch { }
        }
        
        /// <summary>
        /// Creates a border path with gap for GroupBox title
        /// </summary>
        private GraphicsPath CreateGroupBoxBorderPath(Rectangle bounds)
        {
            if (string.IsNullOrEmpty(_titleText) || _textFont == null)
            {
                return GetShapePath(bounds);
            }
            
            var titleSize = TextRenderer.MeasureText(_titleText, _textFont);
            int titleX = 0;
            
            // Calculate title position
            switch (_titleAlignment)
            {
                case ContentAlignment.TopLeft:
                    titleX = bounds.Left + _titleGap;
                    break;
                case ContentAlignment.TopCenter:
                    titleX = bounds.Left + (bounds.Width - titleSize.Width) / 2;
                    break;
                case ContentAlignment.TopRight:
                    titleX = bounds.Right - titleSize.Width - _titleGap;
                    break;
            }
            
            int titleY = bounds.Top;
            int titleWidth = titleSize.Width + (_titleGap * 2);
            int borderThickness = BorderThickness;
            
            GraphicsPath path = new GraphicsPath();
            
            // Create border path with gap for title
            if (_panelShape == PanelShape.RoundedRectangle)
            {
                // For rounded rectangle, create path with gap
                int radius = BorderRadius;
                int diameter = radius * 2;
                
                // Top border - left segment (before title)
                if (titleX - _titleGap > bounds.Left + radius)
                {
                    // Left arc
                    path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
                    // Top line before gap
                    path.AddLine(bounds.Left + radius, bounds.Top, titleX - _titleGap, bounds.Top);
                }
                else
                {
                    // Start from left arc
                    path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
                }
                
                // Top border - right segment (after title)
                if (titleX + titleSize.Width + _titleGap < bounds.Right - radius)
                {
                    // Top line after gap
                    path.AddLine(titleX + titleSize.Width + _titleGap, bounds.Top, bounds.Right - radius, bounds.Top);
                    // Top-right arc
                    path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
                }
                else
                {
                    // Just the arc
                    path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
                }
                
                // Right border
                path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
                
                // Bottom border
                path.AddLine(bounds.Right - radius, bounds.Bottom, bounds.Left + radius, bounds.Bottom);
                
                // Left border
                path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
            }
            else
            {
                // For rectangle or other shapes, create simple border with gap
                // Top border - left segment
                if (titleX - _titleGap > bounds.Left)
                {
                    path.AddLine(bounds.Left, bounds.Top, titleX - _titleGap, bounds.Top);
                }
                
                // Top border - right segment
                if (titleX + titleSize.Width + _titleGap < bounds.Right)
                {
                    path.AddLine(titleX + titleSize.Width + _titleGap, bounds.Top, bounds.Right, bounds.Top);
                }
                
                // Right border
                path.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
                
                // Bottom border
                path.AddLine(bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);
                
                // Left border
                path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Top);
                path.CloseFigure();
            }
            
            return path;
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
                            path = GraphicsExtensions.GetRoundedRectPath(bounds, BorderRadius);
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
            catch 
            {
                _isUpdatingRegion = false;
            }
        }

        #endregion

        #region "Constructor"
        public BeepPanel() : base()
        {
            // Prevent designer issues by not subscribing to events or theme managers in design mode
            if (InDesignMode)
            {
                this.Size = new Size(400, 300);
                this.BackColor = SystemColors.Control;
                this.ForeColor = SystemColors.ControlText;
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
            
            // Initialize panel region after size is set
            if (IsHandleCreated)
            {
                UpdatePanelRegion();
            }

            try { ApplyTheme(); }
            catch
            {
                this.BackColor = SystemColors.Control;
                this.ForeColor = SystemColors.ControlText;
            }
        }
        #endregion

        //protected override void OnHandleDestroyed(EventArgs e)
        //{
        //    _isDisposing = true;
        //    base.OnHandleDestroyed(e);
        //}

        //// IMPORTANT: Avoid BaseControl's parent-change badge registration for this container
        //protected override void OnParentChanged(EventArgs e)
        //{
        //    // Intentionally do NOT call base.OnParentChanged to skip RegisterBadgeDrawer logic for BeepPanel
        //    try
        //    {
        //        if (IsChild && Parent != null)
        //        {
        //            BackColor = Parent.BackColor;
        //        }
                
        //        // Force refresh when parent changes (e.g., added to BeepiFormPro)
        //        if (Visible && !IsDisposed)
        //        {
        //            BeginInvoke(new Action(() =>
        //            {
        //                if (!IsDisposed && Visible)
        //                {
        //                    Invalidate(true);
        //                    Refresh();
        //                    // Ensure all child controls are properly refreshed
        //                    foreach (Control control in Controls)
        //                    {
        //                        if (!control.IsDisposed && control.Visible)
        //                        {
        //                            control.Invalidate(true);
        //                            control.Refresh();
        //                        }
        //                    }
        //                }
        //            }));
        //        }
        //    }
        //    catch { /* design-time safe */ }
        //}

        //protected override void OnVisibleChanged(EventArgs e)
        //{
        //    base.OnVisibleChanged(e);
        //    if (Visible && !IsDisposed)
        //    {
        //        // Force invalidation of all child controls when panel becomes visible
        //        Invalidate(true);
        //        Refresh();
        //        foreach (Control control in Controls)
        //        {
        //            if (!control.IsDisposed && control.Visible)
        //            {
        //                control.Invalidate(true);
        //                control.Refresh();
        //            }
        //        }
        //    }
        //}

        //protected override void OnHandleCreated(EventArgs e)
        //{
        //    base.OnHandleCreated(e);
        //    // Ensure proper rendering when handle is created
        //    if (!IsDisposed && Visible)
        //    {
        //        BeginInvoke(new Action(() =>
        //        {
        //            if (!IsDisposed && Visible)
        //            {
        //                Invalidate(true);
        //                Refresh();
        //            }
        //        }));
        //    }
        //}

        #region Phase 1: Auto-Scroll Implementation

        private void InitializeScrollBars()
        {
            if (_verticalScrollBar != null || _horizontalScrollBar != null)
                return; // Already initialized

            // Create modern thin vertical scrollbar
            _verticalScrollBar = new VScrollBar
            {
                Dock = DockStyle.Right,
                Width = 12,
                Visible = false,
                TabStop = false
            };
            _verticalScrollBar.Scroll += OnVerticalScroll;
            _verticalScrollBar.MouseWheel += OnScrollBarMouseWheel;
            Controls.Add(_verticalScrollBar);
            _verticalScrollBar.BringToFront();

            // Create modern thin horizontal scrollbar
            _horizontalScrollBar = new HScrollBar
            {
                Dock = DockStyle.Bottom,
                Height = 12,
                Visible = false,
                TabStop = false
            };
            _horizontalScrollBar.Scroll += OnHorizontalScroll;
            Controls.Add(_horizontalScrollBar);
            _horizontalScrollBar.BringToFront();

            // Initial update
            UpdateScrollBars();
        }

        private void RemoveScrollBars()
        {
            if (_verticalScrollBar != null)
            {
                _verticalScrollBar.Scroll -= OnVerticalScroll;
                _verticalScrollBar.MouseWheel -= OnScrollBarMouseWheel;
                Controls.Remove(_verticalScrollBar);
                _verticalScrollBar.Dispose();
                _verticalScrollBar = null;
            }

            if (_horizontalScrollBar != null)
            {
                _horizontalScrollBar.Scroll -= OnHorizontalScroll;
                Controls.Remove(_horizontalScrollBar);
                _horizontalScrollBar.Dispose();
                _horizontalScrollBar = null;
            }

            _scrollOffset = 0;
        }

        private void UpdateScrollBars()
        {
            if (!AutoScroll || _verticalScrollBar == null || _horizontalScrollBar == null)
                return;

            // Calculate content size
            int contentWidth = 0;
            int contentHeight = 0;

            foreach (Control child in Controls)
            {
                if (child == _verticalScrollBar || child == _horizontalScrollBar)
                    continue;

                contentWidth = Math.Max(contentWidth, child.Right + Padding.Right);
                contentHeight = Math.Max(contentHeight, child.Bottom + Padding.Bottom);
            }

            // Add title height offset
            contentHeight += _titleBottomY;

            // Determine visible area
            int visibleWidth = ClientSize.Width - (ShowVerticalScroll() ? _verticalScrollBar.Width : 0);
            int visibleHeight = ClientSize.Height - (ShowHorizontalScroll() ? _horizontalScrollBar.Height : 0) - _titleBottomY;

            // Update vertical scrollbar
            bool needVerticalScroll = contentHeight > visibleHeight;
            _verticalScrollBar.Visible = needVerticalScroll;
            if (needVerticalScroll)
            {
                _verticalScrollBar.Maximum = contentHeight - visibleHeight + _verticalScrollBar.LargeChange - 1;
                _verticalScrollBar.LargeChange = visibleHeight;
                _verticalScrollBar.SmallChange = 20;
            }

            // Update horizontal scrollbar
            bool needHorizontalScroll = contentWidth > visibleWidth;
            _horizontalScrollBar.Visible = needHorizontalScroll;
            if (needHorizontalScroll)
            {
                _horizontalScrollBar.Maximum = contentWidth - visibleWidth + _horizontalScrollBar.LargeChange - 1;
                _horizontalScrollBar.LargeChange = visibleWidth;
                _horizontalScrollBar.SmallChange = 20;
            }
        }

        private bool ShowVerticalScroll() => _verticalScrollBar?.Visible ?? false;
        private bool ShowHorizontalScroll() => _horizontalScrollBar?.Visible ?? false;

        private void OnVerticalScroll(object sender, ScrollEventArgs e)
        {
            _scrollOffset = e.NewValue;
            UpdateChildControlPositions();
            Invalidate();
        }

        private void OnHorizontalScroll(object sender, ScrollEventArgs e)
        {
            // Horizontal scroll offset
            UpdateChildControlPositions();
            Invalidate();
        }

        private void OnScrollBarMouseWheel(object sender, MouseEventArgs e)
        {
            // Mouse wheel scrolling
            if (_verticalScrollBar != null && _verticalScrollBar.Visible)
            {
                int newValue = _verticalScrollBar.Value - (e.Delta / 120) * _verticalScrollBar.SmallChange;
                newValue = Math.Max(_verticalScrollBar.Minimum, Math.Min(_verticalScrollBar.Maximum - _verticalScrollBar.LargeChange + 1, newValue));
                _verticalScrollBar.Value = newValue;
            }
        }

        private void UpdateChildControlPositions()
        {
            if (!AutoScroll) return;

            int scrollOffset = _verticalScrollBar?.Value ?? 0;
            int horizontalOffset = _horizontalScrollBar?.Value ?? 0;

            foreach (Control child in Controls)
            {
                if (child == _verticalScrollBar || child == _horizontalScrollBar)
                    continue;

                // Adjust child position based on scroll
                // Note: This is a simple implementation - might need AutoScrollPosition for better handling
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (AutoScroll && _verticalScrollBar != null && _verticalScrollBar.Visible)
            {
                int newValue = _verticalScrollBar.Value - (e.Delta / 120) * _verticalScrollBar.SmallChange;
                newValue = Math.Max(_verticalScrollBar.Minimum, Math.Min(_verticalScrollBar.Maximum - _verticalScrollBar.LargeChange + 1, newValue));
                _verticalScrollBar.Value = newValue;
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (AutoScroll)
                UpdateScrollBars();
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            if (AutoScroll)
                UpdateScrollBars();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (AutoScroll)
                UpdateScrollBars();
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

        private void AnimateCollapse()
        {
            int startHeight = Height;
            int endHeight = _collapsedHeight;
            int steps = CollapseAnimationDuration / 16; // 60 FPS
            int currentStep = 0;

            Timer animTimer = new Timer { Interval = 16 };
            animTimer.Tick += (s, e) =>
            {
                currentStep++;
                float progress = (float)currentStep / steps;
                progress = EaseInOutCubic(progress);

                int newHeight = (int)(startHeight + (endHeight - startHeight) * progress);
                Height = newHeight;

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
            int steps = CollapseAnimationDuration / 16;
            int currentStep = 0;

            Timer animTimer = new Timer { Interval = 16 };
            animTimer.Tick += (s, e) =>
            {
                currentStep++;
                float progress = (float)currentStep / steps;
                progress = EaseInOutCubic(progress);

                int newHeight = (int)(startHeight + (endHeight - startHeight) * progress);
                Height = newHeight;

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
            foreach (Control child in Controls)
            {
                if (child != _verticalScrollBar && child != _horizontalScrollBar)
                {
                    child.Visible = false;
                }
            }
        }

        private void ShowContent()
        {
            foreach (Control child in Controls)
            {
                if (child != _verticalScrollBar && child != _horizontalScrollBar)
                {
                    child.Visible = true;
                }
            }
        }

        #endregion

        // Override Dispose to properly clean up
        protected override void Dispose(bool disposing)
        {
            _isDisposing = true;
            if (disposing)
            {
                try
                {
                    if (_scrollTimer != null)
                    {
                        try { _scrollTimer.Stop(); } catch { }
                        _scrollTimer.Dispose();
                        _scrollTimer = null;
                    }

                    if (_verticalScrollBar != null) { try { _verticalScrollBar.Dispose(); } catch { } _verticalScrollBar = null; }
                    if (_horizontalScrollBar != null) { try { _horizontalScrollBar.Dispose(); } catch { } _horizontalScrollBar = null; }

                    // Dispose custom shape path
                    _customShapePath?.Dispose();
                    _customShapePath = null;
                    
                    // Dispose panel region
                    _panelRegion?.Dispose();
                    _panelRegion = null;

                    // Do not dispose font explicitly at design-time; just release reference
                    _textFont = null;
                }
                catch { }
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
                    try
                    {
                        if (_currentTheme.TitleSmall != null)
                            _textFont = BeepThemesManager.ToFont(_currentTheme.TitleSmall);
                        else if (_currentTheme.CardHeaderStyle != null)
                            _textFont = BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle);
                        else
                            _textFont = new Font(_currentTheme.FontFamily, _currentTheme.FontSizeBlockHeader, FontStyle.Regular);
                    }
                    catch { _textFont = new Font("Arial", 10); }
                }
                if (_currentTheme.BorderRadius > 0)
                {
                    IsRounded = true;
                    BorderRadius = _currentTheme.BorderRadius;
                }
                if (_currentTheme.BorderSize > 0) { BorderThickness = _currentTheme.BorderSize; }

                ShadowColor = _currentTheme.ShadowColor;
                ShadowOpacity = _currentTheme.ShadowOpacity;
                
                // Update region when theme changes (only if shape requires it)
                if (IsHandleCreated && (_panelShape == PanelShape.RoundedRectangle || _panelShape == PanelShape.Ellipse))
                {
                    UpdatePanelRegion();
                }
            }
            catch
            {
                BackColor = SystemColors.Control; ForeColor = SystemColors.ControlText; _titleLineColor = SystemColors.ControlDark;
            }
            finally { this.ResumeLayout(false); }

            Invalidate();
        }


        /// <summary>
        /// Calculates the title area rectangle that needs to be painted.
        /// This excludes the area where child controls will be rendered.
        /// </summary>
        private Rectangle GetTitleArea()
        {
            if (!_showTitle || string.IsNullOrEmpty(_titleText) || _textFont == null)
                return Rectangle.Empty;

            UpdateDrawingRect();
            var titleSize = TextRenderer.MeasureText(_titleText, _textFont);
            int titleHeight = titleSize.Height + padding;
            
            if (_showTitleLine)
            {
                titleHeight += _titleLineThickness + padding;
            }
            
            // Return the title area at the top of the control
            return new Rectangle(
                DrawingRect.Left,
                DrawingRect.Top,
                DrawingRect.Width,
                titleHeight + padding * 2
            );
        }

        protected override void DrawContent(Graphics g)
        {
            if (_isDisposing || IsDisposed || !IsHandleCreated) return;
              base.DrawContent(g);
            // Draw GroupBox-style title text (border is already drawn in OnPaintBackground)
           /*  if (_titleStyle == PanelTitleStyle.GroupBox && _showTitle && !string.IsNullOrEmpty(_titleText))
            {
                DrawGroupBoxTitle(e.Graphics, DrawingRect);
            } */
            try
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                
                // Draw title if shown - this follows the control style
                if (_showTitle && !string.IsNullOrEmpty(_titleText))
                {
                    UpdateDrawingRect();
                    DrawTitle(g, DrawingRect);
                }
                
                // Note: Child controls are painted automatically by Windows Forms
                // after DrawContent completes, so they appear on top of the title
            }
            catch { }
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
                // GroupBox style is handled separately in DrawGroupBoxBorder
                if (_titleStyle == PanelTitleStyle.GroupBox)
                {
                    DrawGroupBoxTitle(g, rectangle);
                    return;
                }

                var titleSize = System.Windows.Forms.TextRenderer.MeasureText(_titleText, _textFont);
                float textTop = 0;
                float textLeft = 0;
                Rectangle titleRect = Rectangle.Empty;

                // Calculate position based on title style
                switch (_titleStyle)
                {
                    case PanelTitleStyle.Above:
                        textTop = this.ContentRect.Top + padding;
                        switch (_titleAlignment)
                        {
                            case ContentAlignment.TopLeft:
                                textLeft = DrawingRect.Left + padding;
                                break;
                            case ContentAlignment.TopCenter:
                                textLeft = DrawingRect.Left + (DrawingRect.Width - titleSize.Width) / 2f;
                                break;
                            case ContentAlignment.TopRight:
                                textLeft = DrawingRect.Right - titleSize.Width - padding;
                                break;
                        }
                        break;

                    case PanelTitleStyle.Below:
                        textTop = DrawingRect.Bottom - titleSize.Height - padding;
                        switch (_titleAlignment)
                        {
                            case ContentAlignment.TopLeft:
                                textLeft = DrawingRect.Left + padding;
                                break;
                            case ContentAlignment.TopCenter:
                                textLeft = DrawingRect.Left + (DrawingRect.Width - titleSize.Width) / 2f;
                                break;
                            case ContentAlignment.TopRight:
                                textLeft = DrawingRect.Right - titleSize.Width - padding;
                                break;
                        }
                        break;

                    case PanelTitleStyle.Left:
                        textTop = DrawingRect.Top + (DrawingRect.Height - titleSize.Width) / 2f;
                        textLeft = DrawingRect.Left + padding;
                        // Rotate text for vertical display
                        titleRect = new Rectangle((int)textLeft, (int)textTop, titleSize.Height, titleSize.Width);
                        break;

                    case PanelTitleStyle.Right:
                        textTop = DrawingRect.Top + (DrawingRect.Height - titleSize.Width) / 2f;
                        textLeft = DrawingRect.Right - titleSize.Height - padding;
                        titleRect = new Rectangle((int)textLeft, (int)textTop, titleSize.Height, titleSize.Width);
                        break;

                    case PanelTitleStyle.Overlay:
                        textTop = DrawingRect.Top + padding;
                        switch (_titleAlignment)
                        {
                            case ContentAlignment.TopLeft:
                                textLeft = DrawingRect.Left + padding;
                                break;
                            case ContentAlignment.TopCenter:
                                textLeft = DrawingRect.Left + (DrawingRect.Width - titleSize.Width) / 2f;
                                break;
                            case ContentAlignment.TopRight:
                                textLeft = DrawingRect.Right - titleSize.Width - padding;
                                break;
                        }
                        // Draw background for overlay
                        using (var brush = new SolidBrush(Color.FromArgb(200, BackColor)))
                        {
                            g.FillRectangle(brush, (int)textLeft - 4, (int)textTop - 2, titleSize.Width + 8, titleSize.Height + 4);
                        }
                        break;
                }

                // Use theme-aware color based on ControlStyle
                Color textColor = ForeColor;
                if (UseThemeColors && _currentTheme != null)
                {
                    textColor = _currentTheme.PrimaryTextColor;
                }

                // Draw text
                if (titleRect.IsEmpty)
                {
                    TextRenderer.DrawText(g, _titleText, _textFont, new Point((int)textLeft, (int)textTop), textColor);
                }
                else
                {
                    // Rotated text for Left/Right styles
                    var state = g.Save();
                    g.TranslateTransform(titleRect.Left + titleRect.Width / 2f, titleRect.Top + titleRect.Height / 2f);
                    g.RotateTransform(_titleStyle == PanelTitleStyle.Left ? -90 : 90);
                    TextRenderer.DrawText(g, _titleText, _textFont, new Point(-titleSize.Width / 2, -titleSize.Height / 2), textColor);
                    g.Restore(state);
                }

                // Draw title line for Above style
                if (_showTitleLine && _titleStyle == PanelTitleStyle.Above)
                {
                    float textBottomY = textTop + titleSize.Height;
                    int lineY = (int)(textBottomY + 2);
                    int lineStartX = ShowTitleLineinFullWidth ? (DrawingRect.Left + BorderThickness) : (int)textLeft;
                    int lineEndX = ShowTitleLineinFullWidth ? (DrawingRect.Right - BorderThickness) : (int)(textLeft + titleSize.Width);

                    using (var pen = new Pen(_titleLineColor, _titleLineThickness))
                    {
                        g.DrawLine(pen, lineStartX, lineY, lineEndX, lineY);
                    }

                    _titleBottomY = lineY + _titleLineThickness + padding;
                }
                else
                {
                    _titleBottomY = (int)(textTop + titleSize.Height + padding);
                }
            }
            catch { }
        }


        /// <summary>
        /// Draws GroupBox-style title text (border is drawn in OnPaintBackground using BorderPainters)
        /// </summary>
        private void DrawGroupBoxTitle(Graphics g, Rectangle rectangle)
        {
            if (string.IsNullOrEmpty(_titleText) || _textFont == null) return;

            try
            {
                var titleSize = TextRenderer.MeasureText(_titleText, _textFont);
                int titleY = DrawingRect.Top;
                int titleX = 0;

                // Calculate title position based on alignment
                switch (_titleAlignment)
                {
                    case ContentAlignment.TopLeft:
                        titleX = DrawingRect.Left + _titleGap;
                        break;
                    case ContentAlignment.TopCenter:
                        titleX = DrawingRect.Left + (DrawingRect.Width - titleSize.Width) / 2;
                        break;
                    case ContentAlignment.TopRight:
                        titleX = DrawingRect.Right - titleSize.Width - _titleGap;
                        break;
                }

                // Use theme-aware color
                Color textColor = ForeColor;
                if (UseThemeColors && _currentTheme != null)
                {
                    textColor = _currentTheme.PrimaryTextColor;
                }

                // Draw text with background to break the border
                Rectangle textRect = new Rectangle(titleX - 4, titleY - titleSize.Height / 2, titleSize.Width + 8, titleSize.Height);
                
                // Fill background to create gap in border
                using (var brush = new SolidBrush(BackColor))
                {
                    g.FillRectangle(brush, textRect);
                }

                // Draw the title text
                TextRenderer.DrawText(g, _titleText, _textFont, new Point(titleX, titleY - titleSize.Height / 2), textColor);
            }
            catch { }
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            if (_isDisposing || IsDisposed ) return;
            try
            {
                DrawTitle(graphics, rectangle);
                // var children = Controls.Cast<Control>().ToArray();
                // foreach (Control ctrl in children)
                // {
                //     if (ctrl is IBeepUIComponent comp && !ctrl.IsDisposed)
                //     {
                //         try { comp.Draw(graphics, rectangle); } catch { }
                //     }
                // }
            }
            catch { }
        }
    }
}

