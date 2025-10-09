using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro : Form
    {
        public BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;
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

                    _formpaintermaterics = FormPainterMetrics.DefaultFor(FormStyle, CurrentTheme);
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
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            UpdateStyles();
            BackColor = Color.White; // we paint everything
            UpdateDpiScale();
            _layout = new BeepiFormProLayoutManager(this);
            _hits = new BeepiFormProHitAreaManager(this);
            _interact = new BeepiFormProInteractionManager(this, _hits);
            ActivePainter = new MinimalFormPainter();
            InitializeBuiltInRegions();
            ApplyFormStyle();
        }

        private void ApplyFormStyle()
        {
            switch (FormStyle)
            {
                case FormStyle.Modern:
                case FormStyle.Minimal:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new MinimalFormPainter();
                    break;
                case FormStyle.Classic:
                    FormBorderStyle = FormBorderStyle.Sizable;
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
                default:
                    FormBorderStyle = FormBorderStyle.None;
                    ActivePainter = new MinimalFormPainter();
                    break;
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
                
                // If caption bar is shown and we're in a custom form style, reduce the display area
                if (ShowCaptionBar && (FormStyle == FormStyle.Modern || FormStyle == FormStyle.Minimal || FormStyle == FormStyle.Material || FormStyle == FormStyle.Fluent || FormStyle == FormStyle.MacOS))
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

