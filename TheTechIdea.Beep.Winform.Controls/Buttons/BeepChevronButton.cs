using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Buttons
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Chevron Button")]
    [Description("A chevron-shaped button control with text, image, animation, and direction support.")]
    public class BeepChevronButton : BaseControl
    {
        private const int ChevronPaddingToken = 6;
        private const int ChevronImageInsetToken = 5;
        private const int ChevronDefaultImageSizeToken = 16;
        private const int ChevronMinHitSizeToken = 48;

        private string _imagePath;
        private Font _textFont = new Font("Arial", 10);
        private bool _isChecked;
        private ChevronDirection _direction = ChevronDirection.Forward;
        private Point rippleCenter;
        private float rippleRadius = 0;
        private Timer clickAnimationTimer;
        private float clickAnimationProgress = 1f;
        private const int clickAnimationDuration = 300;
        private DateTime clickAnimationStartTime;
        private bool isAnimatingClick = false;
        private bool _showKeyboardFocus;
        
        // Layout rectangle for image hit testing
        private Rectangle imageRect;

        public BeepChevronButton()
        {
            Width = 100;
            Height = 50;

            IsChild = true;
            IsShadowAffectedByTheme = false;
            IsRoundedAffectedByTheme = false;
            IsCustomeBorder = true;
            IsFrameless = true;
            Padding = new Padding(5);
            AccessibleRole = AccessibleRole.PushButton;
            ApplyTheme();
            UpdateChevronRegion();
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value)
                {
                    return;
                }

                _isChecked = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool IsCheckable { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ShowRipple { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ReducedMotion { get; set; } = false;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("When true, arrow keys can rotate chevron direction while focused.")]
        public bool AllowArrowDirectionCycle { get; set; } = false;

        [Browsable(true)]
        [Category("Appearance")]
        public ChevronDirection Direction
        {
            get => _direction;
            set
            {
                if (_direction == value)
                {
                    return;
                }

                _direction = value;
                UpdateChevronRegion();
                Invalidate();
            }
        }

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
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Accessibility")]
        [DefaultValue(true)]
        [Description("Draw focus ring only when focus came from keyboard navigation.")]
        public bool FocusVisibleOnly { get; set; } = true;

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (IsCheckable)
            {
                IsChecked = !IsChecked;
                AccessibleRole = AccessibleRole.CheckButton;
            }
            else
            {
                AccessibleRole = AccessibleRole.PushButton;
            }

            if (ShowRipple && Enabled)
            {
                StartClickAnimation(PointToClient(MousePosition));
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            UpdateDrawingRect();
            ClearHitList();
            Draw(e.Graphics, DrawingRect);
        }

        protected override void DrawContent(Graphics g)
        {
           // base.DrawContent(g);
         
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle scaledRect = rectangle;

            using (GraphicsPath path = CreateChevronPath(scaledRect))
            {
                Color fillColor = IsChecked
                    ? _currentTheme.ButtonPressedBackColor
                    : IsHovered
                        ? _currentTheme.ButtonHoverBackColor
                        : _currentTheme.ButtonBackColor;

                if (!Enabled)
                {
                    fillColor = _currentTheme.DisabledBackColor;
                }

                var fillBrush = PaintersFactory.GetSolidBrush(fillColor);
                float borderWidth = Math.Max(1f, BorderThickness);
                var borderPen = PaintersFactory.GetPen(_currentTheme.ShadowColor, borderWidth);
                graphics.FillPath(fillBrush, path);
                graphics.DrawPath(borderPen, path);

                DrawStateOverlay(graphics, path);
                DrawFocusRing(graphics, path);

                // Ripple
                if (isAnimatingClick && ShowRipple)
                {
                    float radius = rippleRadius * clickAnimationProgress;
                    using (GraphicsPath ripplePath = new GraphicsPath())
                    {
                        ripplePath.AddEllipse(rippleCenter.X - radius, rippleCenter.Y - radius, radius * 2, radius * 2);
                        var rippleBrush = PaintersFactory.GetSolidBrush(Color.FromArgb((int)(60 * (1 - clickAnimationProgress)), Color.White));
                        using var clip = new Region(path);
                        graphics.Clip = clip;
                        graphics.FillPath(rippleBrush, ripplePath);
                        graphics.ResetClip();
                    }
                }
            }

            // Draw text - measure to prevent clipping
            if (!string.IsNullOrEmpty(Text))
            {
                Rectangle contentRect = GetContentRect(scaledRect);
                var textSize = TextRenderer.MeasureText(graphics, Text, TextFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);
                Rectangle textRect = new Rectangle(
                    contentRect.Left,
                    contentRect.Top,
                    Math.Min(textSize.Width, contentRect.Width),
                    Math.Min(textSize.Height, contentRect.Height)
                );
                textRect.X = contentRect.Left + (contentRect.Width - textRect.Width) / 2;
                textRect.Y = contentRect.Top + (contentRect.Height - textRect.Height) / 2;
                
                TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;
                var textColor = Enabled ? _currentTheme.PrimaryTextColor : _currentTheme.DisabledForeColor;
                TextRenderer.DrawText(graphics, Text, TextFont, textRect, textColor, flags);
            }

            // Draw image if needed - use StyledImagePainter directly
            if (!string.IsNullOrEmpty(ImagePath))
            {
                int imageInset = DpiScalingHelper.ScaleValue(ChevronImageInsetToken, this);
                int defaultImageSize = DpiScalingHelper.ScaleValue(ChevronDefaultImageSizeToken, this);
                int imageSize = Math.Min(scaledRect.Height - (imageInset * 2), defaultImageSize);
                imageRect = new Rectangle(
                    scaledRect.Left + imageInset,
                    scaledRect.Top + (scaledRect.Height - imageSize) / 2,
                    imageSize,
                    imageSize
                );
                
                try
                {
                    StyledImagePainter.Paint(graphics, imageRect, ImagePath);
                    AddHitArea("Image", imageRect, null, () => OnImageClick());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error drawing image: {ex.Message}");
                }
            }
        }

        protected virtual void OnImageClick()
        {
            OnClick(EventArgs.Empty);
        }

        private void StartClickAnimation(Point clickPoint)
        {
            if (!ShowRipple)
            {
                return;
            }

            rippleCenter = clickPoint;
            rippleRadius = Math.Max(Width, Height);
            clickAnimationProgress = 0f;
            clickAnimationStartTime = DateTime.Now;
            isAnimatingClick = true;

            if (clickAnimationTimer == null)
            {
                clickAnimationTimer = new Timer { Interval = ReducedMotion ? 33 : 16 };
                clickAnimationTimer.Tick += (s, e) =>
                {
                    double elapsed = (DateTime.Now - clickAnimationStartTime).TotalMilliseconds;
                    int duration = ReducedMotion ? clickAnimationDuration / 2 : clickAnimationDuration;
                    clickAnimationProgress = (float)Math.Min(1, elapsed / duration);
                    if (clickAnimationProgress >= 1f)
                    {
                        clickAnimationTimer.Stop();
                        isAnimatingClick = false;
                    }
                    Invalidate();
                };
            }

            clickAnimationTimer.Interval = ReducedMotion ? 33 : 16;

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
                _textFont = BeepThemesManager.ToFont(_currentTheme.ButtonFont);
                Font = _textFont;
            }

            EnsureMinimumAccessibleSize();

            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            EnsureMinimumAccessibleSize();
            UpdateChevronRegion();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (!string.IsNullOrEmpty(ImagePath) && imageRect.Contains(e.Location))
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _showKeyboardFocus = false;
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (!FocusVisibleOnly)
            {
                _showKeyboardFocus = true;
            }

            Invalidate();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            _showKeyboardFocus = false;
            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _showKeyboardFocus = true;

            if (!Enabled)
            {
                return;
            }

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                OnClick(EventArgs.Empty);
                e.Handled = true;
                return;
            }

            if (!AllowArrowDirectionCycle)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Right:
                    Direction = ChevronDirection.Forward;
                    e.Handled = true;
                    break;
                case Keys.Left:
                    Direction = ChevronDirection.Backward;
                    e.Handled = true;
                    break;
                case Keys.Up:
                    Direction = ChevronDirection.Up;
                    e.Handled = true;
                    break;
                case Keys.Down:
                    Direction = ChevronDirection.Down;
                    e.Handled = true;
                    break;
            }

            Invalidate();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (AllowArrowDirectionCycle)
            {
                Keys key = keyData & Keys.KeyCode;
                if (key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down)
                {
                    return true;
                }
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Cursor = Cursors.Default;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                clickAnimationTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        private GraphicsPath CreateChevronPath(Rectangle bounds)
        {
            int arrowSize = Direction is ChevronDirection.Up or ChevronDirection.Down
                ? Math.Max(4, bounds.Width / 2)
                : Math.Max(4, bounds.Height / 2);

            Point[] points;
            switch (Direction)
            {
                case ChevronDirection.Backward:
                    points = new[]
                    {
                        new Point(bounds.Right, bounds.Top),
                        new Point(bounds.Left + arrowSize, bounds.Top),
                        new Point(bounds.Left, bounds.Top + bounds.Height / 2),
                        new Point(bounds.Left + arrowSize, bounds.Bottom),
                        new Point(bounds.Right, bounds.Bottom)
                    };
                    break;
                case ChevronDirection.Down:
                    points = new[]
                    {
                        new Point(bounds.Left, bounds.Top),
                        new Point(bounds.Left, bounds.Bottom - arrowSize),
                        new Point(bounds.Left + bounds.Width / 2, bounds.Bottom),
                        new Point(bounds.Right, bounds.Bottom - arrowSize),
                        new Point(bounds.Right, bounds.Top)
                    };
                    break;
                case ChevronDirection.Up:
                    points = new[]
                    {
                        new Point(bounds.Left, bounds.Bottom),
                        new Point(bounds.Left, bounds.Top + arrowSize),
                        new Point(bounds.Left + bounds.Width / 2, bounds.Top),
                        new Point(bounds.Right, bounds.Top + arrowSize),
                        new Point(bounds.Right, bounds.Bottom)
                    };
                    break;
                default:
                    points = new[]
                    {
                        new Point(bounds.Left, bounds.Top),
                        new Point(bounds.Right - arrowSize, bounds.Top),
                        new Point(bounds.Right, bounds.Top + bounds.Height / 2),
                        new Point(bounds.Right - arrowSize, bounds.Bottom),
                        new Point(bounds.Left, bounds.Bottom)
                    };
                    break;
            }

            var path = new GraphicsPath();
            path.AddPolygon(points);
            return path;
        }

        private Rectangle GetContentRect(Rectangle bounds)
        {
            int padding = DpiScalingHelper.ScaleValue(ChevronPaddingToken, this);
            Rectangle content = Rectangle.Inflate(bounds, -padding, -padding);
            if (content.Width <= 0 || content.Height <= 0)
            {
                return bounds;
            }

            return content;
        }

        private void DrawStateOverlay(Graphics graphics, GraphicsPath path)
        {
            if (!Enabled)
            {
                return;
            }

            int alpha = 0;
            if (IsPressed)
            {
                alpha = 28;
            }
            else if (Focused)
            {
                alpha = 24;
            }
            else if (IsHovered)
            {
                alpha = 20;
            }

            if (alpha <= 0)
            {
                return;
            }

            Color overlayColor = Color.FromArgb(alpha, _currentTheme.PrimaryTextColor);
            var overlayBrush = PaintersFactory.GetSolidBrush(overlayColor);
            graphics.FillPath(overlayBrush, path);
        }

        private void DrawFocusRing(Graphics graphics, GraphicsPath path)
        {
            if (!Focused)
            {
                return;
            }

            if (FocusVisibleOnly && !_showKeyboardFocus)
            {
                return;
            }

            using var ringPath = path.CreateInsetPath(2f);
            if (ringPath == null || ringPath.PointCount <= 2)
            {
                return;
            }

            Color focusColor = _currentTheme?.ButtonSelectedForeColor ?? Color.DodgerBlue;
            var pen = PaintersFactory.GetPen(Color.FromArgb(220, focusColor), 2f);
            graphics.DrawPath(pen, ringPath);
        }

        private void EnsureMinimumAccessibleSize()
        {
            int min = DpiScalingHelper.ScaleValue(ChevronMinHitSizeToken, this);
            if (Width < min)
            {
                Width = min;
            }

            if (Height < min)
            {
                Height = min;
            }
        }

        private void UpdateChevronRegion()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            Rectangle regionRect = ClientRectangle;
            if (regionRect.Width <= 0 || regionRect.Height <= 0)
            {
                return;
            }

            using var path = CreateChevronPath(regionRect);
            Region?.Dispose();
            Region = new Region(path);
        }
    }
}
