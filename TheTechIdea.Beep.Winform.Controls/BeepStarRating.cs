using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;
 
 
 

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Star Rating")]
    [Category("Beep Controls")]
    [Description("A modern star rating control with animations, hover effects, and business application features.")]
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

        // Business application features
        private bool _readOnly = false;
        private bool _showTooltip = true;
        private string _ratingContext = "Overall Rating";
        private bool _autoSubmit = false;
        private bool _showRatingCount = false;
        private int _ratingCount = 0;
        private decimal _averageRating = 0m;
        private bool _showAverage = false;
        private string _customTooltipText = "";
        private bool _allowHalfStars = false;
        private float _preciseRating = 0f;

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
                    _preciseRating = value;

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
                    
                    // Auto submit if enabled
                    if (_autoSubmit && value > 0)
                    {
                        RatingSubmitted?.Invoke(this, new RatingSubmittedEventArgs(_selectedRating, _ratingContext));
                    }
                }
            }
        }

        /// <summary>
        /// Alias for SelectedRating to maintain compatibility with ProductsView
        /// </summary>
        [Browsable(false)]
        public int Rating
        {
            get => SelectedRating;
            set => SelectedRating = value;
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

        // Business Features
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Makes the rating control read-only (display only).")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                if (_readOnly != value)
                {
                    _readOnly = value;
                    Cursor = _readOnly ? Cursors.Default : Cursors.Hand;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Context description for this rating (e.g., 'Product Quality', 'Service Rating').")]
        public string RatingContext
        {
            get => _ratingContext;
            set => _ratingContext = value ?? "Overall Rating";
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Automatically submit rating when user selects a star.")]
        [DefaultValue(false)]
        public bool AutoSubmit
        {
            get => _autoSubmit;
            set => _autoSubmit = value;
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Show tooltip with rating information.")]
        [DefaultValue(true)]
        public bool ShowTooltip
        {
            get => _showTooltip;
            set => _showTooltip = value;
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Custom tooltip text. If empty, automatic tooltip is generated.")]
        public string CustomTooltipText
        {
            get => _customTooltipText;
            set => _customTooltipText = value ?? "";
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Show the total number of ratings.")]
        [DefaultValue(false)]
        public bool ShowRatingCount
        {
            get => _showRatingCount;
            set
            {
                if (_showRatingCount != value)
                {
                    _showRatingCount = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Total number of ratings (for display).")]
        [DefaultValue(0)]
        public int RatingCount
        {
            get => _ratingCount;
            set
            {
                if (_ratingCount != value)
                {
                    _ratingCount = value;
                    if (_showRatingCount)
                    {
                        Invalidate();
                    }
                }
            }
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Show average rating alongside stars.")]
        [DefaultValue(false)]
        public bool ShowAverage
        {
            get => _showAverage;
            set
            {
                if (_showAverage != value)
                {
                    _showAverage = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Average rating value to display.")]
        public decimal AverageRating
        {
            get => _averageRating;
            set
            {
                if (_averageRating != value)
                {
                    _averageRating = value;
                    if (_showAverage)
                    {
                        Invalidate();
                    }
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow half-star ratings for more precise feedback.")]
        [DefaultValue(false)]
        public bool AllowHalfStars
        {
            get => _allowHalfStars;
            set
            {
                if (_allowHalfStars != value)
                {
                    _allowHalfStars = value;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Precise rating value (supports half stars when AllowHalfStars is true).")]
        public float PreciseRating
        {
            get => _preciseRating;
            set
            {
                if (value >= 0 && value <= _starCount && _preciseRating != value)
                {
                    _preciseRating = value;
                    _selectedRating = (int)Math.Round(value);
                    Invalidate();
                    RatingChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        // Events
        public event EventHandler RatingChanged;
        public event EventHandler<RatingSubmittedEventArgs> RatingSubmitted;
        public event EventHandler<RatingHoverEventArgs> RatingHover;
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

        #region Business Methods
        /// <summary>
        /// Set rating with context information for business applications
        /// </summary>
        public void SetBusinessRating(int rating, string context, int totalRatings = 0, decimal averageRating = 0m)
        {
            RatingContext = context;
            RatingCount = totalRatings;
            AverageRating = averageRating;
            SelectedRating = rating;
            
            UpdateTooltip();
        }

        /// <summary>
        /// Configure for product review scenario
        /// </summary>
        public void ConfigureForProductReview()
        {
            StarCount = 5;
            ShowLabels = false;
            ShowRatingCount = true;
            ShowAverage = true;
            ShowTooltip = true;
            RatingContext = "Product Rating";
            EnableAnimations = true;
            UseGlowEffect = true;
        }

        /// <summary>
        /// Configure for service quality rating
        /// </summary>
        public void ConfigureForServiceRating()
        {
            StarCount = 5;
            ShowLabels = true;
            RatingLabels = new[] { "Poor", "Fair", "Good", "Very Good", "Excellent" };
            RatingContext = "Service Quality";
            AutoSubmit = true;
            EnableAnimations = true;
        }

        /// <summary>
        /// Configure for compact display in lists/grids
        /// </summary>
        public void ConfigureForCompactDisplay()
        {
            StarSize = 16;
            Spacing = 2;
            ShowLabels = false;
            ShowRatingCount = false;
            ShowAverage = false;
            EnableAnimations = false;
            UseGlowEffect = false;
            ReadOnly = true;
        }

        /// <summary>
        /// Configure for feedback collection
        /// </summary>
        public void ConfigureForFeedback(string feedbackContext)
        {
            RatingContext = feedbackContext;
            StarCount = 5;
            ShowLabels = true;
            ShowTooltip = true;
            AutoSubmit = false;
            EnableAnimations = true;
            UseGlowEffect = true;
        }

        /// <summary>
        /// Reset rating to unrated state
        /// </summary>
        public void ClearRating()
        {
            SelectedRating = 0;
            _preciseRating = 0f;
        }

        /// <summary>
        /// Update tooltip based on current state
        /// </summary>
        private void UpdateTooltip()
        {
            if (!_showTooltip) return;

            string tooltip = _customTooltipText;
            
            if (string.IsNullOrEmpty(tooltip))
            {
                if (_selectedRating == 0)
                {
                    tooltip = $"{_ratingContext}: Not rated";
                }
                else
                {
                    tooltip = $"{_ratingContext}: {_selectedRating}/{_starCount} stars";
                    
                    if (_showRatingCount && _ratingCount > 0)
                    {
                        tooltip += $" ({_ratingCount} {(_ratingCount == 1 ? "rating" : "ratings")})";
                    }
                    
                    if (_showAverage && _averageRating > 0)
                    {
                        tooltip += $"\nAverage: {_averageRating:F1} stars";
                    }
                }
            }
            
            this.ToolTipText = tooltip;
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

            // Calculate space for additional text
            int additionalTextHeight = 0;
            if (_showLabels) additionalTextHeight += 20;
            if (_showRatingCount || _showAverage) additionalTextHeight += 15;

            // Calculate dynamic star size based on rectangle size
            int availableWidth = rectangle.Width - (Spacing * (_starCount - 1));
            int availableHeight = rectangle.Height - additionalTextHeight;
            int dynamicStarSize = Math.Min(availableWidth / _starCount, availableHeight);
            int starSize = Math.Min(_starSize, dynamicStarSize);

            // Center stars within rectangle
            int startX = rectangle.Left + (rectangle.Width - (starSize * _starCount + Spacing * (_starCount - 1))) / 2;
            int startY = rectangle.Top + (rectangle.Height - (starSize + additionalTextHeight)) / 2;

            // Draw stars
            for (int i = 0; i < _starCount; i++)
            {
                // Determine star state and color
                Color fillColor;
                float scale = _enableAnimations ? _starScale[i] : 1.0f;
                bool isPartiallyFilled = _allowHalfStars && _preciseRating > i && _preciseRating < i + 1;
                bool isFilled = _selectedRating > i || (_allowHalfStars && _preciseRating > i);

                if (isFilled)
                {
                    // Filled star
                    fillColor = _filledStarColor;
                }
                else if (i == _hoveredStar && !_readOnly)
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
                    isFilled || i == _hoveredStar,
                    isPartiallyFilled ? (_preciseRating - i) : 1.0f
                );
            }

            // Draw labels if enabled
            if (_showLabels && _selectedRating > 0 && _selectedRating <= _ratingLabels.Length)
            {
                string label = _ratingLabels[_selectedRating - 1];
                using (SolidBrush brush = new SolidBrush(_labelColor))
                {
                    SizeF textSize = TextUtils.MeasureText(graphics,label, _labelFont);
                    PointF textPos = new PointF(
                        rectangle.Left + (rectangle.Width - textSize.Width) / 2,
                        startY + starSize + 5);

                    graphics.DrawString(label, _labelFont, brush, textPos);
                }
            }

            // Draw rating count and average
            if (_showRatingCount || _showAverage)
            {
                string info = "";
                if (_showRatingCount && _ratingCount > 0)
                {
                    info = $"({_ratingCount} {(_ratingCount == 1 ? "rating" : "ratings")})";
                }
                
                if (_showAverage && _averageRating > 0)
                {
                    if (!string.IsNullOrEmpty(info)) info += " ";
                    info += $"Avg: {_averageRating:F1}";
                }

                if (!string.IsNullOrEmpty(info))
                {
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, _labelColor)))
                    {
                        Font infoFont = new Font(_labelFont.FontFamily, 7, FontStyle.Regular);
                        SizeF textSize = TextUtils.MeasureText(graphics,info, infoFont);
                        PointF textPos = new PointF(
                            rectangle.Left + (rectangle.Width - textSize.Width) / 2,
                            rectangle.Bottom - textSize.Height - 2);

                        graphics.DrawString(info, infoFont, brush, textPos);
                        infoFont.Dispose();
                    }
                }
            }
        }

        private void DrawModernStar(Graphics graphics, int x, int y, int size, Color fillColor, bool isActive, float fillRatio = 1.0f)
        {
            // Calculate star points
            PointF[] starPoints = CalculateStarPoints(x + size / 2, y + size / 2, size / 2, size / 4, 5);

            // Draw glow effect if enabled and star is active
            if (_useGlowEffect && isActive && !_readOnly)
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

            // Draw the star fill
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddPolygon(starPoints);

                // Handle partial filling for half-stars
                if (fillRatio < 1.0f && fillRatio > 0f)
                {
                    // Create clipping region for partial fill
                    int clipWidth = (int)(size * fillRatio);
                    Rectangle clipRect = new Rectangle(x, y, clipWidth, size);
                    
                    graphics.SetClip(clipRect);
                    
                    // Draw filled portion
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
                    
                    graphics.ResetClip();
                    
                    // Draw empty portion
                    Rectangle emptyClipRect = new Rectangle(x + clipWidth, y, size - clipWidth, size);
                    graphics.SetClip(emptyClipRect);
                    
                    using (SolidBrush emptyBrush = new SolidBrush(_emptyStarColor))
                    {
                        graphics.FillPath(emptyBrush, path);
                    }
                    
                    graphics.ResetClip();
                }
                else
                {
                    // Draw full star
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
                }

                // Draw border with smoother pen
                using (Pen pen = new Pen(StarBorderColor, StarBorderThickness))
                {
                    pen.LineJoin = LineJoin.Round; // Smoother corners
                    graphics.DrawPath(pen, path);
                }
            }

            // Add a highlight spot for more dimension
            if (isActive && !_readOnly)
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

            if (_readOnly) return;

            // Calculate dynamic star size
            int additionalTextHeight = 0;
            if (_showLabels) additionalTextHeight += 20;
            if (_showRatingCount || _showAverage) additionalTextHeight += 15;

            int availableWidth = DrawingRect.Width - (Spacing * (_starCount - 1));
            int availableHeight = DrawingRect.Height - additionalTextHeight;
            int dynamicStarSize = Math.Min(availableWidth / _starCount, availableHeight);
            int starSize = Math.Min(_starSize, dynamicStarSize);

            // Calculate starting X position (same as in Draw)
            int startX = DrawingRect.Left + (DrawingRect.Width - (starSize * _starCount + Spacing * (_starCount - 1))) / 2;
            int startY = DrawingRect.Top + (DrawingRect.Height - (starSize + additionalTextHeight)) / 2;

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

                // Fire hover event
                if (_hoveredStar >= 0)
                {
                    RatingHover?.Invoke(this, new RatingHoverEventArgs(_hoveredStar + 1, _ratingContext));
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

            if (_readOnly) return;

            // Calculate star dimensions same as in Draw method
            int additionalTextHeight = 0;
            if (_showLabels) additionalTextHeight += 20;
            if (_showRatingCount || _showAverage) additionalTextHeight += 15;

            int availableWidth = DrawingRect.Width - (Spacing * (_starCount - 1));
            int availableHeight = DrawingRect.Height - additionalTextHeight;
            int dynamicStarSize = Math.Min(availableWidth / _starCount, availableHeight);
            int starSize = Math.Min(_starSize, dynamicStarSize);

            int startX = DrawingRect.Left + (DrawingRect.Width - (starSize * _starCount + Spacing * (_starCount - 1))) / 2;
            int startY = DrawingRect.Top + (DrawingRect.Height - (starSize + additionalTextHeight)) / 2;

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
                    int newRating = i + 1;
                    
                    // Handle half-star logic
                    if (_allowHalfStars)
                    {
                        float relativeX = (e.X - starRect.X) / (float)starRect.Width;
                        if (relativeX < 0.5f)
                        {
                            _preciseRating = i + 0.5f;
                        }
                        else
                        {
                            _preciseRating = newRating;
                        }
                        _selectedRating = (int)Math.Ceiling(_preciseRating);
                    }
                    else
                    {
                        // Toggle behavior: if clicking the currently selected star, clear rating
                        if (newRating == _selectedRating)
                            SelectedRating = 0;
                        else
                            SelectedRating = newRating;
                    }

                    UpdateTooltip();
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
            else if (value is float floatValue)
            {
                PreciseRating = floatValue;
            }
            else if (value is decimal decimalValue)
            {
                PreciseRating = (float)decimalValue;
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
            return _allowHalfStars ? (object)_preciseRating : _selectedRating;
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

    #region Event Args
    public class RatingSubmittedEventArgs : EventArgs
    {
        public int Rating { get; }
        public string Context { get; }

        public RatingSubmittedEventArgs(int rating, string context)
        {
            Rating = rating;
            Context = context;
        }
    }

    public class RatingHoverEventArgs : EventArgs
    {
        public int HoveredRating { get; }
        public string Context { get; }

        public RatingHoverEventArgs(int hoveredRating, string context)
        {
            HoveredRating = hoveredRating;
            Context = context;
        }
    }
    #endregion
}