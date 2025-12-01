using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Ratings.Painters;
using TheTechIdea.Beep.Winform.Controls.Ratings;
using TheTechIdea.Beep.Winform.Controls.Ratings.Helpers;
using TheTechIdea.Beep.Winform.Controls.ToolTips;
using TheTechIdea.Beep.Winform.Controls.Base;




namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep Star Rating")]
    [Category("Beep Controls")]
    [Description("A modern star rating control with animations, hover effects, and business application features.")]
    public class BeepStarRating : BaseControl
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
        private bool _autoGenerateTooltip = false;
        private bool _isDragging = false;
        private int _keyboardFocusedStar = -1; // For keyboard navigation

        // Painter pattern
        private RatingStyle _ratingStyle = RatingStyle.ClassicStar;
        private IRatingPainter _painter;
        private bool _isApplyingTheme = false; // Prevent re-entrancy during theme application

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

                    // Update accessibility attributes
                    RatingAccessibilityHelpers.ApplyAccessibilitySettings(this);

                    // Update tooltip if auto-generate is enabled
                    if (_autoGenerateTooltip)
                    {
                        UpdateRatingTooltip();
                    }

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
            set
            {
                // Respect reduced motion preference
                _enableAnimations = value && !RatingAccessibilityHelpers.ShouldDisableAnimations(false);
                Invalidate();
            }
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
                // Respect reduced motion preference
                bool newValue = value && !RatingAccessibilityHelpers.ShouldDisableGlowEffects(false);
                if (_useGlowEffect != newValue)
                {
                    _useGlowEffect = newValue;
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
            set
            {
                _customTooltipText = value ?? "";
                if (_autoGenerateTooltip && string.IsNullOrEmpty(_customTooltipText))
                {
                    UpdateRatingTooltip();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Automatically generate tooltip text based on current rating state.")]
        [DefaultValue(false)]
        public bool AutoGenerateTooltip
        {
            get => _autoGenerateTooltip;
            set
            {
                if (_autoGenerateTooltip != value)
                {
                    _autoGenerateTooltip = value;
                    if (_autoGenerateTooltip)
                    {
                        UpdateRatingTooltip();
                    }
                }
            }
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
                    if (_autoGenerateTooltip)
                    {
                        UpdateRatingTooltip();
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
                    if (_autoGenerateTooltip)
                    {
                        UpdateRatingTooltip();
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

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Visual style of the rating control.")]
        [DefaultValue(RatingStyle.ClassicStar)]
        public RatingStyle RatingStyle
        {
            get => _ratingStyle;
            set
            {
                if (_ratingStyle != value)
                {
                    _ratingStyle = value;
                    UpdatePainter();
                    Invalidate();
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

            // Initialize painter
            UpdatePainter();

            // Enable keyboard focus and tab navigation
            SetStyle(ControlStyles.Selectable, true);
            TabStop = true;

            // Apply accessibility settings
            RatingAccessibilityHelpers.ApplyAccessibilitySettings(this);

            // Apply theme initially
            ApplyTheme();

            // Update tooltip if auto-generate is enabled
            if (_autoGenerateTooltip)
            {
                UpdateRatingTooltip();
            }
        }

        private void UpdatePainter()
        {
            _painter?.Dispose();
            _painter = RatingPainterFactory.CreatePainter(_ratingStyle);
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
        /// Called automatically when AutoGenerateTooltip is enabled and rating changes
        /// </summary>
        private void UpdateRatingTooltip()
        {
            if (!EnableTooltip)
                return;

            // If auto-generate is enabled and no explicit tooltip text is set
            if (_autoGenerateTooltip && string.IsNullOrEmpty(_customTooltipText))
            {
                GenerateRatingTooltip();
            }
            // If explicit tooltip text is set, use it
            else if (!string.IsNullOrEmpty(_customTooltipText))
            {
                TooltipText = _customTooltipText;
                UpdateTooltip();
            }
            // Legacy behavior: use old UpdateTooltip logic
            else if (_showTooltip)
            {
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Generate tooltip text based on current rating state
        /// </summary>
        private void GenerateRatingTooltip()
        {
            if (!EnableTooltip)
                return;

            string context = !string.IsNullOrEmpty(_ratingContext) ? _ratingContext : "Rating";
            string tooltipText = "";
            string tooltipTitle = context;
            ToolTipType tooltipType = ToolTipType.Info;

            if (_selectedRating == 0)
            {
                tooltipText = "Not rated. Click a star to set your rating.";
                tooltipType = ToolTipType.Info;
            }
            else
            {
                if (_allowHalfStars && _preciseRating > 0)
                {
                    tooltipText = $"{_preciseRating:F1} out of {_starCount} stars";
                }
                else
                {
                    tooltipText = $"{_selectedRating} out of {_starCount} stars";
                }

                // Add rating count if enabled
                if (_showRatingCount && _ratingCount > 0)
                {
                    tooltipText += $"\n{_ratingCount} {((_ratingCount == 1) ? "rating" : "ratings")} total";
                }

                // Add average if enabled
                if (_showAverage && _averageRating > 0)
                {
                    tooltipText += $"\nAverage: {_averageRating:F1} stars";
                }

                // Set tooltip type based on rating
                if (_selectedRating >= _starCount * 0.8) // 80% or higher
                {
                    tooltipType = ToolTipType.Success;
                }
                else if (_selectedRating >= _starCount * 0.5) // 50% or higher
                {
                    tooltipType = ToolTipType.Info;
                }
                else // Below 50%
                {
                    tooltipType = ToolTipType.Warning;
                }
            }

            // Update tooltip properties
            TooltipText = tooltipText;
            TooltipTitle = tooltipTitle;
            TooltipType = tooltipType;
            UpdateTooltip();
        }

        /// <summary>
        /// Legacy tooltip update method (for backward compatibility)
        /// </summary>
        private void UpdateTooltip()
        {
            if (!_showTooltip && !_autoGenerateTooltip) return;

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
            
            if (!_autoGenerateTooltip)
            {
                this.TooltipText = tooltip;
            }
        }

        /// <summary>
        /// Set rating tooltip with custom text, title, and type
        /// </summary>
        public void SetRatingTooltip(string text, string title = null, ToolTipType type = ToolTipType.Info)
        {
            TooltipText = text;
            if (!string.IsNullOrEmpty(title))
            {
                TooltipTitle = title;
            }
            TooltipType = type;
            _customTooltipText = text; // Store custom text to prevent auto-generation
            UpdateTooltip();
        }

        /// <summary>
        /// Show rating notification (success/info) when rating is submitted
        /// </summary>
        public void ShowRatingNotification(bool showOnSubmit = true, bool showOnChange = false)
        {
            if (_selectedRating == 0)
                return;

            if (showOnSubmit && _autoSubmit)
            {
                // Show success notification when rating is submitted
                if (_selectedRating >= _starCount * 0.8)
                {
                    ShowSuccess($"Rating submitted: {_selectedRating} stars", 2000);
                }
                else
                {
                    ShowInfo($"Rating submitted: {_selectedRating} stars", 2000);
                }
            }
            else if (showOnChange && _selectedRating > 0)
            {
                // Show info notification when rating changes
                ShowInfo($"Rating: {_selectedRating} out of {_starCount} stars", 1500);
            }
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
            if (_currentTheme == null || _painter == null)
                return;

            // Ensure DrawingRect is valid
            if (rectangle.Width <= 0 || rectangle.Height <= 0)
                return;

            // Create painter context
            var context = new RatingPainterContext
            {
                Graphics = graphics,
                Bounds = rectangle,
                StarCount = _starCount,
                SelectedRating = _selectedRating,
                PreciseRating = _preciseRating,
                HoveredStar = _hoveredStar,
                AllowHalfStars = _allowHalfStars,
                ReadOnly = _readOnly,
                StarSize = _starSize,
                Spacing = _spacing,
                FilledStarColor = _filledStarColor,
                EmptyStarColor = _emptyStarColor,
                HoverStarColor = _hoverStarColor,
                StarBorderColor = _starBorderColor,
                StarBorderThickness = _starBorderThickness,
                EnableAnimations = _enableAnimations,
                UseGlowEffect = _useGlowEffect,
                StarScale = _starScale,
                GlowIntensity = _glowIntensity,
                ShowLabels = _showLabels,
                RatingLabels = _ratingLabels,
                LabelFont = _labelFont,
                LabelColor = _labelColor,
                ShowRatingCount = _showRatingCount,
                RatingCount = _ratingCount,
                ShowAverage = _showAverage,
                AverageRating = _averageRating,
                RatingContext = _ratingContext,
                Theme = _currentTheme,
                UseThemeColors = UseThemeColors,
                ControlStyle = ControlStyle,
                RatingStyle = _ratingStyle
            };

            // Delegate to painter
            _painter.Paint(context);
        }

        #endregion

        #region Interaction
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_readOnly || _painter == null) return;

            // Create context for hit testing
            var context = CreatePainterContext();

            // Determine which star is being hovered using painter's hit test
            int newHoveredStar = -1;
            for (int i = 0; i < _starCount; i++)
            {
                Rectangle starRect = _painter.GetHitTestRect(context, i);
                if (starRect.Contains(e.Location))
                {
                    newHoveredStar = i;
                    break;
                }
            }

            // Handle drag-to-rate functionality
            if (_isDragging && e.Button == MouseButtons.Left && newHoveredStar >= 0)
            {
                int newRating = newHoveredStar + 1;
                
                // Handle half-star logic during drag
                if (_allowHalfStars)
                {
                    Rectangle starRect = _painter.GetHitTestRect(context, newHoveredStar);
                    float relativeX = (e.X - starRect.X) / (float)starRect.Width;
                    if (relativeX < 0.5f)
                    {
                        _preciseRating = newHoveredStar + 0.5f;
                    }
                    else
                    {
                        _preciseRating = newRating;
                    }
                    _selectedRating = (int)Math.Ceiling(_preciseRating);
                }
                else
                {
                    SelectedRating = newRating;
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

            if (_readOnly || _painter == null) return;

            // Create context for hit testing
            var context = CreatePainterContext();

            // Determine which star was clicked using painter's hit test
            for (int i = 0; i < _starCount; i++)
            {
                Rectangle starRect = _painter.GetHitTestRect(context, i);

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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_readOnly || _painter == null) return;

            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_readOnly || _painter == null) return;

            if (e.Button == MouseButtons.Left && _isDragging)
            {
                _isDragging = false;
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (_readOnly) return;

            // Double-click to clear rating
            if (e.Button == MouseButtons.Left)
            {
                ClearRating();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (_readOnly) return;

            // Keyboard navigation
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Down:
                    // Move to previous star
                    if (_keyboardFocusedStar < 0)
                        _keyboardFocusedStar = _selectedRating > 0 ? _selectedRating - 1 : 0;
                    else if (_keyboardFocusedStar > 0)
                        _keyboardFocusedStar--;
                    else
                        _keyboardFocusedStar = 0;
                    
                    SelectedRating = _keyboardFocusedStar + 1;
                    e.Handled = true;
                    break;

                case Keys.Right:
                case Keys.Up:
                    // Move to next star
                    if (_keyboardFocusedStar < 0)
                        _keyboardFocusedStar = _selectedRating > 0 ? _selectedRating - 1 : 0;
                    else if (_keyboardFocusedStar < _starCount - 1)
                        _keyboardFocusedStar++;
                    else
                        _keyboardFocusedStar = _starCount - 1;
                    
                    SelectedRating = _keyboardFocusedStar + 1;
                    e.Handled = true;
                    break;

                case Keys.Home:
                    // Move to first star
                    _keyboardFocusedStar = 0;
                    SelectedRating = 1;
                    e.Handled = true;
                    break;

                case Keys.End:
                    // Move to last star
                    _keyboardFocusedStar = _starCount - 1;
                    SelectedRating = _starCount;
                    e.Handled = true;
                    break;

                case Keys.Enter:
                case Keys.Space:
                    // Confirm current rating (if not already set)
                    if (_keyboardFocusedStar >= 0)
                    {
                        SelectedRating = _keyboardFocusedStar + 1;
                        if (_autoSubmit)
                        {
                            RatingSubmitted?.Invoke(this, new RatingSubmittedEventArgs(_selectedRating, _ratingContext));
                        }
                    }
                    e.Handled = true;
                    break;

                case Keys.Escape:
                    // Clear rating
                    ClearRating();
                    _keyboardFocusedStar = -1;
                    e.Handled = true;
                    break;

                case Keys.Delete:
                case Keys.Back:
                    // Clear rating
                    ClearRating();
                    _keyboardFocusedStar = -1;
                    e.Handled = true;
                    break;
            }

            if (e.Handled)
            {
                Invalidate();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            
            // Initialize keyboard focus to current rating
            if (_keyboardFocusedStar < 0)
            {
                _keyboardFocusedStar = _selectedRating > 0 ? _selectedRating - 1 : 0;
            }
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            
            // Reset keyboard focus
            _keyboardFocusedStar = -1;
            Invalidate();
        }

        private RatingPainterContext CreatePainterContext()
        {
            return new RatingPainterContext
            {
                Graphics = null, // Not needed for hit testing
                Bounds = DrawingRect,
                StarCount = _starCount,
                SelectedRating = _selectedRating,
                PreciseRating = _preciseRating,
                HoveredStar = _hoveredStar,
                AllowHalfStars = _allowHalfStars,
                ReadOnly = _readOnly,
                StarSize = _starSize,
                Spacing = _spacing,
                FilledStarColor = _filledStarColor,
                EmptyStarColor = _emptyStarColor,
                HoverStarColor = _hoverStarColor,
                StarBorderColor = _starBorderColor,
                StarBorderThickness = _starBorderThickness,
                EnableAnimations = _enableAnimations,
                UseGlowEffect = _useGlowEffect,
                StarScale = _starScale,
                GlowIntensity = _glowIntensity,
                ShowLabels = _showLabels,
                RatingLabels = _ratingLabels,
                LabelFont = _labelFont,
                LabelColor = _labelColor,
                ShowRatingCount = _showRatingCount,
                RatingCount = _ratingCount,
                ShowAverage = _showAverage,
                AverageRating = _averageRating,
                RatingContext = _ratingContext,
                Theme = _currentTheme,
                UseThemeColors = UseThemeColors,
                ControlStyle = ControlStyle,
                RatingStyle = _ratingStyle
            };
        }
        #endregion

        #region Theme
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_isApplyingTheme) return;

            _isApplyingTheme = true;
            try
            {
                if (_currentTheme == null)
                    return;

                // Use theme helpers for centralized color management
                if (UseThemeColors)
                {
                    RatingThemeHelpers.ApplyThemeColors(this, _currentTheme, UseThemeColors);
                }

                // Apply high contrast adjustments if needed
                RatingAccessibilityHelpers.ApplyHighContrastAdjustments(this, _currentTheme, UseThemeColors);

                // Apply background color
                BackColor = _currentTheme.BackColor;

                // Apply font theme using font helpers
                RatingFontHelpers.ApplyFontTheme(this, ControlStyle);

                // Respect reduced motion for animations and glow effects
                if (RatingAccessibilityHelpers.ShouldDisableAnimations(_enableAnimations))
                {
                    _enableAnimations = false;
                }
                if (RatingAccessibilityHelpers.ShouldDisableGlowEffects(_useGlowEffect))
                {
                    _useGlowEffect = false;
                }

                // Apply accessible sizing if in high contrast mode
                if (RatingAccessibilityHelpers.IsHighContrastMode())
                {
                    _starSize = RatingAccessibilityHelpers.GetAccessibleStarSize(_starSize);
                    _spacing = RatingAccessibilityHelpers.GetAccessibleSpacing(_spacing);
                    _starBorderThickness = RatingAccessibilityHelpers.GetAccessibleBorderWidth(_starBorderThickness);
                }
            }
            finally
            {
                _isApplyingTheme = false;
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
                _painter?.Dispose();
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