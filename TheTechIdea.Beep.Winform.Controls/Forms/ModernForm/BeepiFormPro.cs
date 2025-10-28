using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro : Form
    {

        private FormPainterMetrics _formpaintermaterics;
        // Metrics used for layout and painting; can be set externally or lazy-loaded
        /// <summary>
        ///     i dont want to be serialized and persisted with the form
        /// </summary>
        /// 
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public FormPainterMetrics FormPainterMetrics
        {
            get
            {
                if (_formpaintermaterics == null)
                {
                    // Lazy load metrics based on current Style and theme
                    _formpaintermaterics = FormPainterMetrics.DefaultFor(FormStyle, UseThemeColors ? CurrentTheme : null);
                }
                return _formpaintermaterics;
            }
            set
            {
                _formpaintermaterics = value;
                Invalidate(); // Redraw with new metrics
            }
        }
        private readonly SolidBrush _bgBrush; // Cache brush
        private DateTime _lastInvalidate = DateTime.MinValue;
        private void DebouncedInvalidate(Rectangle? rect = null, bool invalidateChildren = false)
        {
            if ((DateTime.Now - _lastInvalidate).TotalMilliseconds < 16) // 60 FPS max
                return;
            _lastInvalidate = DateTime.Now;
            if (rect.HasValue)
                Invalidate(rect.Value, invalidateChildren);
            else
                Invalidate(invalidateChildren);
        }
        public BeepiFormPro()
        {

            AutoScaleMode = AutoScaleMode.Font;
            this.DoubleBuffered = true;
            // Enable double buffering and optimized painting
            SetStyle(
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                      ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();

            _layout = new BeepiFormProLayoutManager(this);
            _hits = new BeepiFormProHitAreaManager(this);
            _interact = new BeepiFormProInteractionManager(this, _hits);

            // Don't hardcode painter - ApplyFormStyle() will set the correct one based on FormStyle property
            //BackColor = Color.White; // we paint everything
            InitializeBuiltInRegions();
            InitializeComponent();

            // Update window region when handle is created
          
            // Always hook events for design-time and runtime refresh
            this.Resize += (s, e) => { UpdateWindowRegion(); DebouncedInvalidate(); };
            this.Scroll += (s, e) => { UpdateWindowRegion(); DebouncedInvalidate(); };
            this.HandleCreated += (s, e) => { UpdateWindowRegion(); DebouncedInvalidate(); };

            ApplyFormStyle(); // This sets ActivePainter based on FormStyle (which can be set at design time)
            BackColor = FormPainterMetrics.DefaultFor(FormStyle, UseThemeColors ? CurrentTheme : null).BackgroundColor; _bgBrush = new SolidBrush(BackColor); // Initialize in constructor
            FormBorderStyle = FormBorderStyle.None;
            //// Design-time: hook child control events for auto-refresh
            //if (DesignMode || (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime))
            //{
            //    this.ControlAdded += (s, e) => HookChildEvents(e.Control);
            //    foreach (Control c in Controls) HookChildEvents(c);
            //}
        }

        private void HookChildEvents(Control ctrl)
        {
            ctrl.Move += (s, e) => { if (DesignMode) this.Invalidate(); };
            ctrl.Resize += (s, e) => { if (DesignMode) this.Invalidate(); };
            ctrl.VisibleChanged += (s, e) => { if (DesignMode) this.Invalidate(); };
        }

        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            base.OnDpiChanged(e);

            // Update our DPI scale when monitor DPI changes
            // UpdateDpiScale();

            // Force layout recalculation for custom chrome elements
            if (ActivePainter != null)
                ActivePainter.CalculateLayoutAndHitAreas(this);
            
            // Update window region for new DPI
            UpdateWindowRegion();
            
            Invalidate();
        }
        
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            
            // CRITICAL: Ensure window region is set when form is first shown
            // This prevents rectangular corners from showing through on initial display
            UpdateWindowRegion();
        }

        private void ApplyFormStyle()
        {
            switch (FormStyle)
            {
                case FormStyle.Terminal:
                    FormBorderStyle = FormBorderStyle.Sizable;
                    ActivePainter = new TerminalFormPainter();
                    break;
                case FormStyle.Modern:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new ModernFormPainter();
                    break;
                case FormStyle.Minimal:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new MinimalFormPainter();
                    break;

                case FormStyle.MacOS:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new MacOSFormPainter();
                    break;
                case FormStyle.Fluent:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new FluentFormPainter();
                    break;
                case FormStyle.Material:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new MaterialFormPainter();
                    break;
                case FormStyle.Cartoon:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new CartoonFormPainter();
                    break;
                case FormStyle.ChatBubble:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new ChatBubbleFormPainter();
                    break;
                case FormStyle.Glass:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new GlassFormPainter();
                    break;
                case FormStyle.Metro:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new MetroFormPainter();
                    break;
                case FormStyle.Metro2:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new Metro2FormPainter();
                    break;
                case FormStyle.GNOME:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new GNOMEFormPainter();
                    break;
                case FormStyle.NeoMorphism:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new NeoMorphismFormPainter();
                    break;
                case FormStyle.Glassmorphism:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new GlassmorphismFormPainter();
                    break;
                case FormStyle.iOS:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new iOSFormPainter();
                    break;
                // Windows11 REMOVED - use regular WinForms for native Windows look
                case FormStyle.Nordic:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new NordicFormPainter();
                    break;
                case FormStyle.Paper:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new PaperFormPainter();
                    break;
                case FormStyle.Ubuntu:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new UbuntuFormPainter();
                    break;
                case FormStyle.KDE:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new KDEFormPainter();
                    break;
                case FormStyle.ArcLinux:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new ArcLinuxFormPainter();
                    break;
                case FormStyle.Dracula:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new DraculaFormPainter();
                    break;
                case FormStyle.Solarized:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new SolarizedFormPainter();
                    break;
                case FormStyle.OneDark:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new OneDarkFormPainter();
                    break;
                case FormStyle.GruvBox:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new GruvBoxFormPainter();
                    break;
                case FormStyle.Nord:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new NordFormPainter();
                    break;
                case FormStyle.Tokyo:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new TokyoFormPainter();
                    break;
                case FormStyle.Brutalist:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new BrutalistFormPainter();
                    break;
                case FormStyle.Retro:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new RetroFormPainter();
                    break;
                case FormStyle.Cyberpunk:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new CyberpunkFormPainter();
                    break;
                case FormStyle.Neon:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new NeonFormPainter();
                    break;
                case FormStyle.Holographic:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new HolographicFormPainter();
                    break;
                case FormStyle.Custom:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new CustomFormPainter();
                    break;
                default:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new MinimalFormPainter();
                    break;
            }

            // Force layout recalculation to reposition child controls based on new DisplayRectangle
            if (!DesignMode)
            {
                PerformLayout();
            }
            BackColor = FormPainterMetrics.DefaultFor(FormStyle, UseThemeColors ? CurrentTheme : null).BackgroundColor;
           
            // CRITICAL: Update window region to match new Style's corner radius
            UpdateWindowRegion();
            
            Invalidate();
        }

        /// <summary>
        /// Override DisplayRectangle to exclude the caption bar area and borders.
        /// This ensures controls added to the form don't overlap the caption or borders.
        /// </summary>
        public override Rectangle DisplayRectangle
        {
            get
            {
                var rect = base.DisplayRectangle;

                // Get border width from metrics
                int borderWidth = 0;
                if (ActivePainter != null)
                {
                    var metrics = FormPainterMetrics.DefaultFor(FormStyle, UseThemeColors ? CurrentTheme : null);
                    borderWidth = metrics?.BorderWidth ?? 1;
                }

                // Shrink by border width on all sides (left, top, right, bottom)
                rect.X += borderWidth;
                rect.Y += borderWidth;
                rect.Width -= borderWidth * 2;
                rect.Height -= borderWidth * 2;

                // If caption bar is shown, reduce the display area further
                if (ShowCaptionBar)
                {
                    int captionHeight = Math.Max(CaptionHeight, (int)(Font.Height * 2.5f));
                    rect.Y += captionHeight;
                    rect.Height -= captionHeight;
                }

                return rect;
            }
        }

        /// <summary>
        /// Gets the GraphicsPath for the form's border shape.
        /// This path is used by painters to draw borders with the correct shape (rounded, etc.).
        /// IMPORTANT: The path is INSET by half the border width because DrawPath centers the pen
        /// on the path line. This ensures the entire border is visible inside the client area.
        /// Uses GraphicsExtensions methods exclusively - NO rectangles!
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public GraphicsPath BorderShape
        {
            get
            {
                // Always create fresh path based on current size and painter
                if (ActivePainter != null)
                {
                    // Get the corner radius from the active painter
                    var radius = ActivePainter.GetCornerRadius(this);
                    
                    // Get border width from metrics
                    var metrics = FormPainterMetrics.DefaultFor(FormStyle, UseThemeColors ? CurrentTheme : null);
                    int borderWidth = metrics?.BorderWidth ?? 1;

                    // Create the outer path using GraphicsExtensions
                    GraphicsPath outerPath;
                    if (radius.TopLeft == radius.TopRight &&
                        radius.TopLeft == radius.BottomLeft &&
                        radius.TopLeft == radius.BottomRight)
                    {
                        // Uniform radius - use simple method
                        outerPath = GraphicsExtensions.CreateRoundedRectanglePath(ClientRectangle, radius.TopLeft);
                    }
                    else
                    {
                        // Different radii per corner - use full method
                        outerPath = GraphicsExtensions.CreateRoundedRectanglePath(ClientRectangle,
                            radius.TopLeft, radius.TopRight, radius.BottomLeft, radius.BottomRight);
                    }
                    
                    // CRITICAL: Inset the path by HALF the border width using GraphicsExtensions
                    // This ensures the pen draws centered and the entire border is visible
                    float inset = borderWidth / 2f;
                    var insetPath = outerPath.CreateInsetPath(inset, radius.TopLeft);
                    
                    outerPath.Dispose(); // Clean up the temporary outer path
                    return insetPath;
                }
                else
                {
                    // Fallback: create simple path and inset it
                    var outerPath = GraphicsExtensions.CreateRoundedRectanglePath(ClientRectangle, 0);
                    var insetPath = outerPath.CreateInsetPath(0.5f, 0); // Inset by 0.5 for 1px border
                    outerPath.Dispose();
                    return insetPath;
                }
            }
        }
    }
}

