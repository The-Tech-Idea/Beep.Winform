using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Desktop.Common.Util;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Star Rating")]
    [Category("Beep Controls")]
    [Description("A modern star rating control with animations and hover effects.")]
    public class BeepStarRating : BeepControl
    {
        #region Fields and Properties
        // Core rating properties
        private int _starCount = 5;
        private int _selectedRating = 0;
        private int _starSize = 24;
        private int _spacing = 5;
        private int _hoveredStar = -1;

        // Star appearance 
        private Color _filledStarColor = Color.Gold;
        private Color _emptyStarColor = Color.Gray;
        private Color _starBorderColor = Color.Black;
        private float _starBorderThickness = 1f;
        private Color _hoverStarColor = Color.Orange;

        // Animation properties
        private bool _enableAnimations = true;
        private Timer _animationTimer;
        private float[] _starScale; // Array to track scale of each star for animation
        private float _animationSpeed = 0.15f;
        private bool _useGlowEffect = true;
        private float _glowIntensity = 0.4f;

        // Optional labels
        private bool _showLabels = false;
        private string[] _ratingLabels = { "Poor", "Fair", "Good", "Very Good", "Excellent" };
        private Font _labelFont;
        private Color _labelColor = Color.Black;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Number of stars in the rating control.")]
        [DefaultValue(5)]
        public int StarCount
        {
            get => _starCount;
            set
            {
                if (value > 0 && _starCount != value)
                {
                    _starCount = value;
                    InitializeAnimationArrays();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Currently selected rating.")]
        [DefaultValue(0)]
        public int SelectedRating
        {
            get => _selectedRating;
            set
            {
                if (value >= 0 && value <= _starCount && _selectedRating != value)
                {
                    int oldRating = _selectedRating;
                    _selectedRating = value;

                    if (_enableAnimations && IsHandleCreated)
                    {
                        // Animate the rating change
                        for (int i = 0; i < _starCount; i++)
                        {
                            // If star is newly selected, make it pop
                            if (i < value && (oldRating == 0 || i >= oldRating))
                            {
                                _starScale[i] = 1.5f; // Start big
                            }
                            // If star is newly deselected, make it shrink
                            else if (i >= value && i < oldRating)
                            {
                                _starScale[i] = 0.8f; // Start small
                            }
                        }

                        EnsureAnimationTimerRunning();
                    }

                    Invalidate();
                    RatingChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Size of each star.")]
        [DefaultValue(24)]
        public int StarSize
        {
            get => _starSize;
            set
            {
                if (value > 0 && _starSize != value)
                {
                    _starSize = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Spacing between stars.")]
        [DefaultValue(5)]
        public int Spacing
        {
            get => _spacing;
            set
            {
                if (value >= 0 && _spacing != value)
                {
                    _spacing = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the filled stars.")]
        public Color FilledStarColor
        {
            get => _filledStarColor;
            set
            {
                if (_filledStarColor != value)
                {
                    _filledStarColor = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the empty stars.")]
        public Color EmptyStarColor
        {
            get => _emptyStarColor;
            set
            {
                if (_emptyStarColor != value)
                {
                    _emptyStarColor = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the star when hovered.")]
        public Color HoverStarColor
        {
            get => _hoverStarColor;
            set
            {
                if (_hoverStarColor != value)
                {
                    _hoverStarColor = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the star border.")]
        public Color StarBorderColor
        {
            get => _starBorderColor;
            set
            {
                if (_starBorderColor != value)
                {
                    _starBorderColor = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Thickness of the star border.")]
        [DefaultValue(1.0f)]
        public float StarBorderThickness
        {
            get => _starBorderThickness;
            set
            {
                if (value > 0 && _starBorderThickness != value)
                {
                    _starBorderThickness = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Enable smooth animations when rating changes.")]
        [DefaultValue(true)]
        public bool EnableAnimations
        {
            get => _enableAnimations;
            set => _enableAnimations = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Enable glow effect on hovered and selected stars.")]
        [DefaultValue(true)]
        public bool UseGlowEffect
        {
            get => _useGlowEffect;
            set
            {
                if (_useGlowEffect != value)
                {
                    _useGlowEffect = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show text labels below the stars.")]
        [DefaultValue(false)]
        public bool ShowLabels
        {
            get => _showLabels;
            set
            {
                if (_showLabels != value)
                {
                    _showLabels = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text labels for each rating level.")]
        public string[] RatingLabels
        {
            get => _ratingLabels;
            set
            {
                _ratingLabels = value;
                if (_showLabels)
                {
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font for the rating labels.")]
        public Font LabelFont
        {
            get => _labelFont;
            set
            {
                if (_labelFont != value)
                {
                    _labelFont?.Dispose();
                    _labelFont = value;
                    if (_showLabels)
                    {
                        Invalidate();
                    }
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color for the rating labels.")]
        public Color LabelColor
        {
            get => _labelColor;
            set
            {
                if (_labelColor != value)
                {
                    _labelColor = value;
                    if (_showLabels)
                    {
                        Invalidate();
                    }
                }
            }
        }

        public event EventHandler RatingChanged;
        #endregion

        #region Constructor and Initialization
        public BeepStarRating()
        {
            DoubleBuffered = true;
            MinimumSize = new Size(120, 30);
            Cursor = Cursors.Hand;
            BoundProperty = "SelectedRating";
            BorderRadius = 0; // Default no rounded corners for this control

            // Initialize animation arrays
            InitializeAnimationArrays();

            // Initialize label font
            _labelFont = new Font("Segoe UI", 8, FontStyle.Regular);

            // Apply theme initially
            ApplyTheme();
        }

        private void InitializeAnimationArrays()
        {
            _starScale = new float[_starCount];
            for (int i = 0; i < _starCount; i++)
            {
                _starScale[i] = 1.0f; // Initialize all stars to normal scale
            }
        }

        private void EnsureAnimationTimerRunning()
        {
            if (_animationTimer == null)
            {
                _animationTimer = new Timer();
                _animationTimer.Interval = 16; // About 60fps
                _animationTimer.Tick += AnimationTimer_Tick;
            }

            if (!_animationTimer.Enabled)
                _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            bool needsRedraw = false;
            bool allDone = true;

            // Animate each star's scale
            for (int i = 0; i < _starCount; i++)
            {
                float targetScale = 1.0f; // Normal size

                if (_starScale[i] != targetScale)
                {
                    _starScale[i] = Lerp(_starScale[i], targetScale, _animationSpeed);

                    // Check if we're close enough to consider it done
                    if (Math.Abs(_starScale[i] - targetScale) > 0.01f)
                    {
                        allDone = false;
                    }

                    needsRedraw = true;
                }
            }

            // Redraw if needed
            if (needsRedraw)
                Invalidate();

            // Stop timer if all animations are complete
            if (allDone)
                _animationTimer.Stop();
        }

        // Helper function for smooth animation
        private float Lerp(float start, float end, float amount)
        {
            return start + (end - start) * amount;
        }
        #endregion

        #region Painting
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            Draw(g, DrawingRect);
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            if (_currentTheme == null)
                return;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Ensure DrawingRect is valid
            if (rectangle.Width <= 0 || rectangle.Height <= 0)
                return;

            // Calculate dynamic star size based on rectangle size
            int availableWidth = rectangle.Width - (Spacing * (_starCount - 1));
            int dynamicStarSize = Math.Min(availableWidth / _starCount, rectangle.Height - (_showLabels ? 25 : 10));
            int starSize = Math.Min(_starSize, dynamicStarSize);

            // Center stars within rectangle
            int startX = rectangle.Left + (rectangle.Width - (starSize * _starCount + Spacing * (_starCount - 1))) / 2;
            int startY = rectangle.Top + (rectangle.Height - (_showLabels ? starSize + 20 : starSize)) / 2;

            // Draw stars
            for (int i = 0; i < _starCount; i++)
            {
                // Determine star state and color
                Color fillColor;
                float scale = _enableAnimations ? _starScale[i] : 1.0f;

                if (i < _selectedRating)
                {
                    // Filled star
                    fillColor = _filledStarColor;
                }
                else if (i == _hoveredStar)
                {
                    // Hovered star
                    fillColor = _hoverStarColor;
                }
                else
                {
                    // Empty star
                    fillColor = _emptyStarColor;
                }

                // Calculate position accounting for scale
                int scaledSize = (int)(starSize * scale);
                int offsetX = (int)((starSize - scaledSize) / 2);
                int offsetY = (int)((starSize - scaledSize) / 2);

                // Draw the star with glow effect if enabled
                DrawModernStar(
                    graphics,
                    startX + i * (starSize + Spacing) + offsetX,
                    startY + offsetY,
                    scaledSize,
                    fillColor,
                    i < _selectedRating || i == _hoveredStar
                );
            }

            // Draw labels if enabled
            if (_showLabels && _selectedRating > 0 && _selectedRating <= _ratingLabels.Length)
            {
                string label = _ratingLabels[_selectedRating - 1];

                using (SolidBrush brush = new SolidBrush(_labelColor))
                {
                    SizeF textSize = graphics.MeasureString(label, _labelFont);
                    PointF textPos = new PointF(
                        rectangle.Left + (rectangle.Width - textSize.Width) / 2,
                        startY + starSize + 5);

                    graphics.DrawString(label, _labelFont, brush, textPos);
                }
            }
        }

        private void DrawModernStar(Graphics graphics, int x, int y, int size, Color fillColor, bool isActive)
        {
            // Calculate star points
            PointF[] starPoints = CalculateStarPoints(x + size / 2, y + size / 2, size / 2, size / 4, 5);

            // Draw glow effect if enabled and star is active
            if (_useGlowEffect && isActive)
            {
                // Create a slightly larger path for the glow
                PointF[] glowPoints = CalculateStarPoints(
                    x + size / 2,
                    y + size / 2,
                    size / 2 + 2,
                    size / 4 + 2,
                    5);

                using (GraphicsPath glowPath = new GraphicsPath())
                {
                    glowPath.AddPolygon(glowPoints);

                    // Create a PathGradientBrush for the glow effect
                    using (PathGradientBrush glowBrush = new PathGradientBrush(glowPath))
                    {
                        // Glow color (soft light based on the fill color)
                        Color glowColor = Color.FromArgb(
                            100,
                            Math.Min(255, fillColor.R + 40),
                            Math.Min(255, fillColor.G + 40),
                            Math.Min(255, fillColor.B + 40));

                        glowBrush.CenterColor = glowColor;
                        glowBrush.SurroundColors = new Color[] { Color.FromArgb(0, fillColor) };

                        graphics.FillPath(glowBrush, glowPath);
                    }
                }
            }

            // Draw the star fill with gradient for more depth
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(starPoints);

                // Create a gradient brush for more depth
                using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
                    new Rectangle(x, y, size, size),
                    Color.FromArgb(255, fillColor),
                    Color.FromArgb(200,
                        Math.Max(0, fillColor.R - 30),
                        Math.Max(0, fillColor.G - 30),
                        Math.Max(0, fillColor.B - 30)),
                    LinearGradientMode.ForwardDiagonal))
                {
                    graphics.FillPath(gradientBrush, path);
                }

                // Draw border with smoother pen
                using (Pen pen = new Pen(StarBorderColor, StarBorderThickness))
                {
                    pen.LineJoin = LineJoin.Round; // Smoother corners
                    graphics.DrawPath(pen, path);
                }
            }

            // Add a highlight spot for more dimension
            if (isActive)
            {
                int spotSize = size / 6;
                int spotX = x + size / 4;
                int spotY = y + size / 4;

                using (GraphicsPath spotPath = new GraphicsPath())
                {
                    spotPath.AddEllipse(spotX, spotY, spotSize, spotSize);

                    using (PathGradientBrush spotBrush = new PathGradientBrush(spotPath))
                    {
                        spotBrush.CenterColor = Color.FromArgb(70, 255, 255, 255);
                        spotBrush.SurroundColors = new Color[] { Color.FromArgb(0, 255, 255, 255) };

                        graphics.FillPath(spotBrush, spotPath);
                    }
                }
            }
        }

        private PointF[] CalculateStarPoints(float centerX, float centerY, float outerRadius, float innerRadius, int numPoints)
        {
            PointF[] points = new PointF[numPoints * 2];
            double angleStep = Math.PI / numPoints;
            double angle = -Math.PI / 2; // Start at the top

            for (int i = 0; i < points.Length; i++)
            {
                float radius = i % 2 == 0 ? outerRadius : innerRadius;
                points[i] = new PointF(
                    centerX + (float)(Math.Cos(angle) * radius),
                    centerY + (float)(Math.Sin(angle) * radius));
                angle += angleStep;
            }

            return points;
        }
        #endregion

        #region Interaction
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Calculate dynamic star size
            int availableWidth = DrawingRect.Width - (Spacing * (_starCount - 1));
            int dynamicStarSize = Math.Min(availableWidth / _starCount, DrawingRect.Height - (_showLabels ? 25 : 10));
            int starSize = Math.Min(_starSize, dynamicStarSize);

            // Calculate starting X position (same as in Draw)
            int startX = DrawingRect.Left + (DrawingRect.Width - (starSize * _starCount + Spacing * (_starCount - 1))) / 2;
            int startY = DrawingRect.Top + (DrawingRect.Height - (_showLabels ? starSize + 20 : starSize)) / 2;

            // Determine which star is being hovered
            int newHoveredStar = -1;
            for (int i = 0; i < _starCount; i++)
            {
                Rectangle starRect = new Rectangle(
                    startX + i * (starSize + Spacing),
                    startY,
                    starSize,
                    starSize);

                if (starRect.Contains(e.Location))
                {
                    newHoveredStar = i;
                    break;
                }
            }

            // If hover state changed, update and redraw
            if (newHoveredStar != _hoveredStar)
            {
                _hoveredStar = newHoveredStar;

                // Animate the hovered star
                if (_hoveredStar >= 0 && _enableAnimations)
                {
                    _starScale[_hoveredStar] = 1.2f;
                    EnsureAnimationTimerRunning();
                }

                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredStar != -1)
            {
                _hoveredStar = -1;

                // No need to modify scales, the animation timer will reset them
                if (_enableAnimations)
                    EnsureAnimationTimerRunning();

                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Calculate star dimensions same as in Draw method
            int availableWidth = DrawingRect.Width - (Spacing * (_starCount - 1));
            int dynamicStarSize = Math.Min(availableWidth / _starCount, DrawingRect.Height - (_showLabels ? 25 : 10));
            int starSize = Math.Min(_starSize, dynamicStarSize);

            int startX = DrawingRect.Left + (DrawingRect.Width - (starSize * _starCount + Spacing * (_starCount - 1))) / 2;
            int startY = DrawingRect.Top + (DrawingRect.Height - (_showLabels ? starSize + 20 : starSize)) / 2;

            // Determine which star was clicked
            for (int i = 0; i < _starCount; i++)
            {
                Rectangle starRect = new Rectangle(
                    startX + i * (starSize + Spacing),
                    startY,
                    starSize,
                    starSize);

                if (starRect.Contains(e.Location))
                {
                    // Toggle behavior: if clicking the currently selected star, clear rating
                    if (i + 1 == _selectedRating)
                        SelectedRating = 0;
                    else
                        SelectedRating = i + 1;

                    break;
                }
            }
        }
        #endregion

        #region Theme
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null)
                return;

            // Apply star rating specific theme properties
            FilledStarColor = _currentTheme.StarRatingFillColor != Color.Empty
                ? _currentTheme.StarRatingFillColor
                : _currentTheme.PrimaryColor;

            EmptyStarColor = _currentTheme.StarRatingBackColor != Color.Empty
                ? _currentTheme.StarRatingBackColor
                : Color.FromArgb(200, 200, 200);

            HoverStarColor = _currentTheme.StarRatingHoverForeColor != Color.Empty
                ? _currentTheme.StarRatingHoverForeColor
                : _currentTheme.AccentColor;

            StarBorderColor = _currentTheme.StarRatingBorderColor != Color.Empty
                ? _currentTheme.StarRatingBorderColor
                : Color.FromArgb(130, 130, 130);

            BackColor = _currentTheme.BackColor;

            LabelColor = _currentTheme.PrimaryTextColor;

            // Apply font if using theme fonts
            if (UseThemeFont)
            {
                if (_labelFont != null)
                {
                    _labelFont.Dispose();
                }

                _labelFont = FontListHelper.CreateFontFromTypography(_currentTheme.LabelFont);
            }

            Invalidate();
        }
        #endregion

        #region Value Setting and Getting
        public override void SetValue(object value)
        {
            if (value is int intValue)
            {
                SelectedRating = intValue;
            }
            else if (value is string strValue && int.TryParse(strValue, out int parsedValue))
            {
                SelectedRating = parsedValue;
            }
            else if (value != null)
            {
                // Try to convert any other value to int
                try
                {
                    SelectedRating = Convert.ToInt32(value);
                }
                catch
                {
                    // Ignore conversion errors
                }
            }
        }

        public override object GetValue()
        {
            return SelectedRating;
        }
        #endregion

        #region Disposal
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Dispose();
                _labelFont?.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}