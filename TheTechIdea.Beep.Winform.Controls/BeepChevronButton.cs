using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Base;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Chevron Button")]
    [Description("A chevron-shaped button control with text, image, animation, and direction support.")]
    public class BeepChevronButton : BaseControl
    {
        private string _imagePath;
        private BeepImage beepImage;
        private Font _textFont = new Font("Arial", 10);
        private Point rippleCenter;
        private float rippleRadius = 0;
        private Timer clickAnimationTimer;
        private float clickAnimationProgress = 1f;
        private const int clickAnimationDuration = 300;
        private DateTime clickAnimationStartTime;
        private bool isAnimatingClick = false;

        public BeepChevronButton()
        {
            Width = 100;
            Height = 50;

            beepImage = new BeepImage
            {
                Dock = DockStyle.None,
                Margin = new Padding(0)
            };

            IsChild = true;
            IsShadowAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            IsCustomeBorder = true;
            IsFrameless=true;
            Padding = new Padding(5);
            ApplyTheme();

            beepImage.MouseHover += (s, e) => { IsHovered = true; Invalidate(); };
            beepImage.MouseLeave += (s, e) => { IsHovered = false; Invalidate(); };
            beepImage.Click += (s, e) => OnClick(e);
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool IsChecked { get; set; }

        [Browsable(true)]
        [Category("Appearance")]
        public ChevronDirection Direction { get; set; } = ChevronDirection.Forward;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                _textFont = value;
                Font = _textFont;
                UseThemeFont = false;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Path to the image displayed when checked.")]
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                if (beepImage != null)
                {
                    beepImage.ImagePath = value;
                    Invalidate();
                }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            IsChecked = !IsChecked;
            StartClickAnimation(PointToClient(MousePosition));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            base.OnPaint(e);
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            UpdateDrawingRect();
            Draw(g, DrawingRect);
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle scaledRect = rectangle;
            int arrowSize = Direction is ChevronDirection.Up or ChevronDirection.Down
                            ? scaledRect.Width / 2
                            : scaledRect.Height / 2;

            Point[] points;

            switch (Direction)
            {
                case ChevronDirection.Backward: // ←
                    points = new Point[]
                    {
                        new Point(scaledRect.Right, scaledRect.Top),
                        new Point(scaledRect.Left + arrowSize, scaledRect.Top),
                        new Point(scaledRect.Left, scaledRect.Top + scaledRect.Height / 2),
                        new Point(scaledRect.Left + arrowSize, scaledRect.Bottom),
                        new Point(scaledRect.Right, scaledRect.Bottom)
                    };
                    break;

                case ChevronDirection.Down: // ↓
                    points = new Point[]
                    {
                        new Point(scaledRect.Left, scaledRect.Top),
                        new Point(scaledRect.Left, scaledRect.Bottom - arrowSize),
                        new Point(scaledRect.Left + scaledRect.Width / 2, scaledRect.Bottom),
                        new Point(scaledRect.Right, scaledRect.Bottom - arrowSize),
                        new Point(scaledRect.Right, scaledRect.Top)
                    };
                    break;

                case ChevronDirection.Up: // ↑
                    points = new Point[]
                    {
                        new Point(scaledRect.Left, scaledRect.Bottom),
                        new Point(scaledRect.Left, scaledRect.Top + arrowSize),
                        new Point(scaledRect.Left + scaledRect.Width / 2, scaledRect.Top),
                        new Point(scaledRect.Right, scaledRect.Top + arrowSize),
                        new Point(scaledRect.Right, scaledRect.Bottom)
                    };
                    break;

                default: // Forward (→)
                    points = new Point[]
                    {
                        new Point(scaledRect.Left, scaledRect.Top),
                        new Point(scaledRect.Right - arrowSize, scaledRect.Top),
                        new Point(scaledRect.Right, scaledRect.Top + scaledRect.Height / 2),
                        new Point(scaledRect.Right - arrowSize, scaledRect.Bottom),
                        new Point(scaledRect.Left, scaledRect.Bottom)
                    };
                    break;
            }

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(points);

                Color fillColor = IsChecked
                    ? _currentTheme.ButtonPressedBackColor
                    : IsHovered
                        ? _currentTheme.ButtonHoverBackColor
                        : _currentTheme.ButtonBackColor;

                using (SolidBrush brush = new SolidBrush(fillColor))
                    graphics.FillPath(brush, path);

                using (Pen pen = new Pen(_currentTheme.ShadowColor, BorderThickness))
                    graphics.DrawPath(pen, path);

                // Ripple
                if (isAnimatingClick)
                {
                    float radius = rippleRadius * clickAnimationProgress;
                    using (GraphicsPath ripplePath = new GraphicsPath())
                    {
                        ripplePath.AddEllipse(rippleCenter.X - radius, rippleCenter.Y - radius, radius * 2, radius * 2);
                        using (Brush rippleBrush = new SolidBrush(Color.FromArgb((int)(60 * (1 - clickAnimationProgress)), Color.White)))
                        {
                            Region clip = new Region(path);
                            graphics.Clip = clip;
                            graphics.FillPath(rippleBrush, ripplePath);
                            graphics.ResetClip();
                        }
                    }
                }
            }

            // Draw text
            if (!string.IsNullOrEmpty(Text))
            {
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;
                Rectangle textRect = scaledRect;
                TextRenderer.DrawText(graphics, Text, Font, textRect, _currentTheme.PrimaryTextColor, flags);
            }

            // Draw image if needed
            if (!string.IsNullOrEmpty(ImagePath))
            {
                int imageSize = Math.Min(scaledRect.Height - 10, 16);
                beepImage.MaximumSize = new Size(imageSize, imageSize);
                beepImage.Size = beepImage.MaximumSize;
                beepImage.Location = new Point(
                    scaledRect.Left + 5,
                    scaledRect.Top + (scaledRect.Height - imageSize) / 2
                );
                beepImage.DrawImage(graphics, new Rectangle(beepImage.Location, beepImage.Size));
            }
        }

        private void StartClickAnimation(Point clickPoint)
        {
            rippleCenter = clickPoint;
            rippleRadius = Math.Max(Width, Height);
            clickAnimationProgress = 0f;
            clickAnimationStartTime = DateTime.Now;
            isAnimatingClick = true;

            if (clickAnimationTimer == null)
            {
                clickAnimationTimer = new Timer { Interval = 16 };
                clickAnimationTimer.Tick += (s, e) =>
                {
                    double elapsed = (DateTime.Now - clickAnimationStartTime).TotalMilliseconds;
                    clickAnimationProgress = (float)Math.Min(1, elapsed / clickAnimationDuration);
                    if (clickAnimationProgress >= 1f)
                    {
                        clickAnimationTimer.Stop();
                        isAnimatingClick = false;
                    }
                    Invalidate();
                };
            }

            clickAnimationTimer.Start();
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            BackColor = _currentTheme.ButtonBackColor;
            ForeColor = _currentTheme.ButtonForeColor;
            HoverBackColor = _currentTheme.ButtonHoverBackColor;
            HoverForeColor = _currentTheme.ButtonHoverForeColor;
            DisabledBackColor = _currentTheme.DisabledBackColor;
            DisabledForeColor = _currentTheme.DisabledForeColor;
            FocusBackColor = _currentTheme.ButtonSelectedBackColor;
            FocusForeColor = _currentTheme.ButtonSelectedForeColor;
            PressedBackColor = _currentTheme.ButtonPressedBackColor;
            PressedForeColor = _currentTheme.ButtonPressedForeColor;

            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                Font = _textFont;
            }

            beepImage.Theme = Theme;
            Invalidate();
        }
    }
}
