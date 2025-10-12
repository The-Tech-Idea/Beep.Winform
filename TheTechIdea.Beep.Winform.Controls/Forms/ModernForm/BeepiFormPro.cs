using System;
using System.Drawing;
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
                    // Lazy load metrics based on current style and theme
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
        public BeepiFormPro()
        {
         

            InitializeComponent();
         
            // Enable double buffering and optimized painting
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw | 
                     ControlStyles.UserPaint , true);
            UpdateStyles();
            
            _layout = new BeepiFormProLayoutManager(this);
            _hits = new BeepiFormProHitAreaManager(this);
            _interact = new BeepiFormProInteractionManager(this, _hits);
            
            // Don't hardcode painter - ApplyFormStyle() will set the correct one based on FormStyle property
            //BackColor = Color.White; // we paint everything
            InitializeBuiltInRegions();

            ApplyFormStyle(); // This sets ActivePainter based on FormStyle (which can be set at design time)
            BackColor = FormPainterMetrics.DefaultFor(FormStyle, UseThemeColors ? CurrentTheme : null).BackgroundColor;
        }
       
        
        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            base.OnDpiChanged(e);
            
            // Update our DPI scale when monitor DPI changes
            UpdateDpiScale();
            
            // Force layout recalculation for custom chrome elements
            if(ActivePainter!=null)
                ActivePainter.CalculateLayoutAndHitAreas(this);
            Invalidate();
        }
      
        private void ApplyFormStyle()
        {
            switch (FormStyle)
            {
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
                case FormStyle.Windows11:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new Windows11FormPainter();
                    break;
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
            
            Invalidate();
        }

        /// <summary>
        /// Override DisplayRectangle to exclude the caption bar area.
        /// This ensures controls added to the form don't overlap the caption.
        /// </summary>
        public override Rectangle DisplayRectangle
        {
            get
            {
                var rect = base.DisplayRectangle;
                
                // If caption bar is shown and we're in a custom form style (not Classic), reduce the display area
                if (ShowCaptionBar )
                {
                    int captionHeight = Math.Max(ScaleDpi(CaptionHeight), (int)(Font.Height * 2.5f));
                    rect.Y += captionHeight;
                    rect.Height -= captionHeight;
                }
                
                return rect;
            }
        }
    }
}

