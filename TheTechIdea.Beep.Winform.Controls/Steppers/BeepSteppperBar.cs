using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;
using TheTechIdea.Beep.Winform.Controls.ToolTips;
 

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum StepState
    {
        Pending,
        Active,
        Completed,
        Error,
        Warning
    }

    public enum StepDisplayMode
    {
        StepNumber,
        CheckImage,
        SvgIcon
    }

    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Stepper Bar")]
    [Description("An interactive step-by-step progress indicator optimized for business workflows with animations and multiple display modes.")]
    public partial class BeepStepperBar : BaseControl
    {
        #region Private Fields
        private Orientation orientation = Orientation.Horizontal;
        private int selectedIndex = -1;
        private int currentStep = 0;
        private Size buttonSize = new Size(32, 32);
        private List<Rectangle> buttonBounds = new();
        private readonly Dictionary<int, Image> stepImages = new();
        private readonly Dictionary<int, string> stepLabels = new();
        private readonly Dictionary<int, StepState> stepStates = new();

        // Animation support
        private Timer animationTimer;
        private float animationProgress = 1f;
        private const int animationDuration = 250; // milliseconds
        private DateTime animationStartTime;
        private int animatingToIndex = -1;

        // Business-specific features
        private int stepCount = 4;
        private bool autoProgressSteps = true;
        private bool allowStepNavigation = true;
        private bool showConnectorLines = true;
        private bool highlightActiveStep = true;
        private int connectorLineWidth = 2;
        private Color completedStepColor = Color.FromArgb(34, 197, 94);  // Green
        private Color activeStepColor = Color.FromArgb(59, 130, 246);    // Blue  
        private Color pendingStepColor = Color.FromArgb(156, 163, 175);  // Gray
        private Color errorStepColor = Color.FromArgb(239, 68, 68);      // Red
        private Color warningStepColor = Color.FromArgb(245, 158, 11);   // Orange
        
        private bool _isApplyingTheme = false;
        private Font _textFont;

        // Tooltip support
        private bool _autoGenerateTooltips = true;
        private readonly Dictionary<int, string> _stepTooltips = new Dictionary<int, string>();
        private readonly Dictionary<int, ToolTipConfig> _stepTooltipConfigs = new Dictionary<int, ToolTipConfig>();
        private int _hoveredStepIndex = -1;
        private string _currentTooltipKey = null;
        #endregion

        #region Events
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        public event EventHandler<StepChangedEventArgs> StepChanged;
        public event EventHandler<StepValidatingEventArgs> StepValidating;
        public event EventHandler StepCompleted;
        public event EventHandler AllStepsCompleted;
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(typeof(Orientation), "Horizontal")]
        [Description("Layout direction - Horizontal or Vertical")]
        public Orientation Orientation
        {
            get => orientation;
            set { orientation = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Size of individual step circles")]
        public Size ButtonSize
        {
            get => buttonSize;
            set 
            { 
                buttonSize = StepperAccessibilityHelpers.GetAccessibleStepButtonSize(value);
                Invalidate(); 
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("Collection of steps to display")]
        public BindingList<SimpleItem> ListItems { get; set; } = new();

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("check.svg")]
        [Description("Image file to display for completed steps when using CheckImage mode")]
        public string CheckImage { get; set; } = "check.svg";

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(StepDisplayMode.StepNumber)]
        [Description("How to display step content: StepNumber, CheckImage, or SvgIcon")]
        public StepDisplayMode DisplayMode { get; set; } = StepDisplayMode.StepNumber;

        [Browsable(true)]
        [Category("Business Logic")]
        [DefaultValue(4)]
        [Description("Number of steps in the process")]
        public int StepCount
        {
            get => stepCount;
            set
            {
                stepCount = Math.Max(1, value);
                InitializeSteps();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Business Logic")]
        [DefaultValue(0)]
        [Description("Currently active step (0-based index)")]
        public int CurrentStep
        {
            get => currentStep;
            set
            {
                if (value >= 0 && value < stepCount)
                {
                    var oldStep = currentStep;
                    currentStep = value;
                    
                    // Update step states
                    UpdateStepStates();
                    
                    // Trigger animation (respect reduced motion)
                    if (!StepperAccessibilityHelpers.ShouldDisableAnimations(highlightActiveStep))
                    {
                        StartStepAnimation(value);
                    }
                    else
                    {
                        selectedIndex = value;
                    }
                    
                    // Update accessibility
                    ApplyAccessibilitySettings();
                    
                    // Update tooltips
                    UpdateStepperTooltip();
                    
                    // Fire events
                    StepChanged?.Invoke(this, new StepChangedEventArgs(oldStep, currentStep));
                    
                    Invalidate();
                }
            }
        }

        [Browsable(false)]
        [Description("Currently selected step index (read-only)")]
        public int SelectedIndex => selectedIndex;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Whether steps automatically progress when completed")]
        public bool AutoProgressSteps
        {
            get => autoProgressSteps;
            set => autoProgressSteps = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Whether users can click to navigate between steps")]
        public bool AllowStepNavigation
        {
            get => allowStepNavigation;
            set => allowStepNavigation = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Whether to show connector lines between steps")]
        public bool ShowConnectorLines
        {
            get => showConnectorLines;
            set
            {
                showConnectorLines = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Whether to highlight the active step with animation")]
        public bool HighlightActiveStep
        {
            get => highlightActiveStep && !StepperAccessibilityHelpers.IsReducedMotionEnabled();
            set
            {
                if (value && StepperAccessibilityHelpers.IsReducedMotionEnabled())
                {
                    highlightActiveStep = false; // Cannot enable if reduced motion is on
                }
                else
                {
                    highlightActiveStep = value;
                }
            }
        }

        // Color properties for step states
        [Browsable(true)]
        [Category("Colors")]
        [Description("Color for completed steps")]
        public Color CompletedStepColor
        {
            get => completedStepColor;
            set
            {
                completedStepColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Colors")]
        [Description("Color for the active/current step")]
        public Color ActiveStepColor
        {
            get => activeStepColor;
            set
            {
                activeStepColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Colors")]
        [Description("Color for pending/future steps")]
        public Color PendingStepColor
        {
            get => pendingStepColor;
            set
            {
                pendingStepColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Accessibility")]
        [Description("Accessible name for screen readers")]
        public string AccessibleName { get; set; }

        [Browsable(true)]
        [Category("Accessibility")]
        [Description("Accessible description for screen readers")]
        public string AccessibleDescription { get; set; }

        [Browsable(true)]
        [Category("Accessibility")]
        [Description("Accessible role for screen readers")]
        [DefaultValue(AccessibleRole.List)]
        public AccessibleRole AccessibleRole { get; set; } = AccessibleRole.List;
        #endregion

        #region Constructor
        public BeepStepperBar()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            
            ListItems.ListChanged += (s, e) => {
                SyncListItemsWithSteps();
                ApplyAccessibilitySettings();
                UpdateAllStepTooltips();
                Invalidate();
            };

            InitializeAnimation();
            InitializeSteps();
            ApplyAccessibilitySettings();
            UpdateAllStepTooltips();
        }

        private void InitializeAnimation()
        {
            animationTimer = new Timer { Interval = 16 }; // 60 FPS
            animationTimer.Tick += (s, e) =>
            {
                // Respect reduced motion preferences
                if (StepperAccessibilityHelpers.IsReducedMotionEnabled())
                {
                    animationTimer.Stop();
                    selectedIndex = animatingToIndex;
                    animationProgress = 1f;
                    Invalidate();
                    return;
                }
                
                var elapsed = (DateTime.Now - animationStartTime).TotalMilliseconds;
                animationProgress = (float)Math.Min(1, elapsed / animationDuration);
                
                if (animationProgress >= 1f)
                {
                    animationTimer.Stop();
                    selectedIndex = animatingToIndex;
                }
                
                Invalidate();
            };
        }

        private void InitializeSteps()
        {
            // Clear existing data
            stepLabels.Clear();
            stepStates.Clear();
            
            // Initialize default labels and states
            for (int i = 0; i < stepCount; i++)
            {
                stepLabels[i] = $"Step {i + 1}";
                stepStates[i] = i == currentStep ? StepState.Active : 
                               i < currentStep ? StepState.Completed : StepState.Pending;
            }
            
            SyncStepsWithListItems();
        }
        #endregion

        #region Step Management
        /// <summary>
        /// Sets the label for a specific step
        /// </summary>
        public void SetStepLabel(int stepIndex, string label)
        {
            if (stepIndex >= 0 && stepIndex < stepCount)
            {
                stepLabels[stepIndex] = label ?? $"Step {stepIndex + 1}";
                
                // Update corresponding ListItem if it exists
                if (stepIndex < ListItems.Count)
                {
                    ListItems[stepIndex].Name = stepLabels[stepIndex];
                }
                
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the label for a specific step
        /// </summary>
        public string GetStepLabel(int stepIndex)
        {
            return stepLabels.TryGetValue(stepIndex, out string label) ? label : $"Step {stepIndex + 1}";
        }

        /// <summary>
        /// Sets the state for a specific step
        /// </summary>
        public void SetStepState(int stepIndex, StepState state)
        {
            if (stepIndex >= 0 && stepIndex < stepCount)
            {
                stepStates[stepIndex] = state;
                
                // Update corresponding ListItem if it exists
                if (stepIndex < ListItems.Count)
                {
                    ListItems[stepIndex].IsChecked = state == StepState.Completed;
                }
                
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the state for a specific step
        /// </summary>
        public StepState GetStepState(int stepIndex)
        {
            return stepStates.TryGetValue(stepIndex, out StepState state) ? state : StepState.Pending;
        }

        /// <summary>
        /// Moves to the next step
        /// </summary>
        public bool NextStep()
        {
            if (currentStep < stepCount - 1)
            {
                // Validate current step before moving
                var args = new StepValidatingEventArgs(currentStep, currentStep + 1);
                StepValidating?.Invoke(this, args);
                
                if (!args.Cancel)
                {
                    // Mark current step as completed
                    SetStepState(currentStep, StepState.Completed);
                    StepCompleted?.Invoke(this, EventArgs.Empty);
                    
                    CurrentStep = currentStep + 1;
                    
                    // Check if all steps are completed
                    if (currentStep == stepCount - 1)
                    {
                        AllStepsCompleted?.Invoke(this, EventArgs.Empty);
                    }
                    
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Moves to the previous step
        /// </summary>
        public bool PreviousStep()
        {
            if (currentStep > 0)
            {
                CurrentStep = currentStep - 1;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Completes the current step and moves to next
        /// </summary>
        public void CompleteCurrentStep()
        {
            SetStepState(currentStep, StepState.Completed);
            if (autoProgressSteps)
            {
                NextStep();
            }
        }

        /// <summary>
        /// Marks a step as having an error
        /// </summary>
        public void SetStepError(int stepIndex, bool hasError = true)
        {
            if (stepIndex >= 0 && stepIndex < stepCount)
            {
                SetStepState(stepIndex, hasError ? StepState.Error : StepState.Pending);
            }
        }

        /// <summary>
        /// Configure stepper for task workflow
        /// </summary>
        public void ConfigureForTaskWorkflow()
        {
            StepCount = 4;
            SetStepLabel(0, "Created");
            SetStepLabel(1, "In Progress");
            SetStepLabel(2, "Review");
            SetStepLabel(3, "Completed");
            CurrentStep = 0;
            AllowStepNavigation = true;
            ShowConnectorLines = true;
        }

        /// <summary>
        /// Configure stepper for order tracking
        /// </summary>
        public void ConfigureForOrderTracking()
        {
            StepCount = 5;
            SetStepLabel(0, "Order Placed");
            SetStepLabel(1, "Processing");
            SetStepLabel(2, "Shipped");
            SetStepLabel(3, "Out for Delivery");
            SetStepLabel(4, "Delivered");
            CurrentStep = 0;
            AllowStepNavigation = false; // Order tracking is typically read-only
            DisplayMode = StepDisplayMode.CheckImage;
        }
        #endregion

        #region Synchronization
        private void SyncStepsWithListItems()
        {
            // Clear and rebuild ListItems based on steps
            ListItems.Clear();
            
            for (int i = 0; i < stepCount; i++)
            {
                var item = new SimpleItem
                {
                    ID = i,
                    Name = GetStepLabel(i),
                    Text = GetStepLabel(i),
                    IsChecked = GetStepState(i) == StepState.Completed,
                    IsSelected = i == currentStep
                };
                
                ListItems.Add(item);
            }
        }

        private void SyncListItemsWithSteps()
        {
            // Update stepCount based on ListItems
            if (ListItems.Count > 0 && ListItems.Count != stepCount)
            {
                stepCount = ListItems.Count;
                
                // Sync labels and states
                for (int i = 0; i < ListItems.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ListItems[i].Name))
                        stepLabels[i] = ListItems[i].Name;
                        
                    stepStates[i] = ListItems[i].IsChecked ? StepState.Completed :
                                   ListItems[i].IsSelected ? StepState.Active : StepState.Pending;
                }
            }
        }

        private void UpdateStepStates()
        {
            for (int i = 0; i < stepCount; i++)
            {
                StepState newState = i == currentStep ? StepState.Active :
                                   i < currentStep ? StepState.Completed : StepState.Pending;
                
                // Preserve error states
                if (GetStepState(i) != StepState.Error)
                {
                    SetStepState(i, newState);
                }
            }
        }
        #endregion

        #region Animation
        private void StartStepAnimation(int targetIndex)
        {
            // Respect reduced motion preferences
            if (StepperAccessibilityHelpers.IsReducedMotionEnabled())
            {
                selectedIndex = targetIndex;
                animationProgress = 1f;
                return;
            }
            
            if (targetIndex != animatingToIndex)
            {
                animatingToIndex = targetIndex;
                animationStartTime = DateTime.Now;
                animationProgress = 0f;
                animationTimer.Start();
            }
        }
        #endregion

        #region Painting
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (stepCount == 0) return;
            
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            buttonBounds.Clear();
            DrawSteps(e.Graphics);
        }

        private void DrawSteps(Graphics graphics)
        {
            int spacing = 20;
            int stepTotalSize = orientation == Orientation.Horizontal ? buttonSize.Width : buttonSize.Height;
            int totalLength = (stepTotalSize + spacing) * stepCount - spacing;

            Point startPoint = orientation == Orientation.Horizontal
                ? new Point((Width - totalLength) / 2, (Height - buttonSize.Height) / 2)
                : new Point((Width - buttonSize.Width) / 2, (Height - totalLength) / 2);

            for (int i = 0; i < stepCount; i++)
            {
                int x = orientation == Orientation.Horizontal
                    ? startPoint.X + i * (buttonSize.Width + spacing)
                    : startPoint.X;

                int y = orientation == Orientation.Horizontal
                    ? startPoint.Y
                    : startPoint.Y + i * (buttonSize.Height + spacing);

                Rectangle rect = new Rectangle(x, y, buttonSize.Width, buttonSize.Height);
                buttonBounds.Add(rect);

                // Draw connector line
                if (showConnectorLines && i > 0)
                {
                    DrawConnectorLine(graphics, i, rect);
                }

                // Draw step
                DrawStep(graphics, i, rect);
                
                // Draw step label
                DrawStepLabel(graphics, i, rect);
            }
        }

        private void DrawConnectorLine(Graphics graphics, int stepIndex, Rectangle rect)
        {
            StepState currentStepState = GetStepState(stepIndex - 1);
            
            // Use theme helpers for connector line color
            Color lineColor = StepperThemeHelpers.GetConnectorLineColor(
                _currentTheme, 
                UseThemeColors, 
                currentStepState,
                currentStepState == StepState.Completed ? completedStepColor : pendingStepColor);
            
            Point p1, p2;
            
            if (orientation == Orientation.Horizontal)
            {
                p1 = new Point(buttonBounds[stepIndex - 1].Right, buttonBounds[stepIndex - 1].Top + buttonSize.Height / 2);
                p2 = new Point(rect.Left, rect.Top + buttonSize.Height / 2);
            }
            else
            {
                p1 = new Point(buttonBounds[stepIndex - 1].Left + buttonSize.Width / 2, buttonBounds[stepIndex - 1].Bottom);
                p2 = new Point(rect.Left + buttonSize.Width / 2, rect.Top);
            }

            int accessibleLineWidth = StepperAccessibilityHelpers.GetAccessibleConnectorLineWidth(connectorLineWidth);
            using (var pen = new Pen(lineColor, accessibleLineWidth))
            {
                graphics.DrawLine(pen, p1, p2);
            }
        }

        private void DrawStep(Graphics graphics, int stepIndex, Rectangle rect)
        {
            StepState state = GetStepState(stepIndex);
            
            // Animate highlight for active step
            float scale = 1f;
            if (stepIndex == currentStep && highlightActiveStep && stepIndex == animatingToIndex && animationProgress < 1f)
            {
                scale = 1f + 0.15f * (float)Math.Sin(animationProgress * Math.PI);
            }

            Rectangle inflated = Rectangle.Inflate(rect, 
                (int)(rect.Width * (scale - 1) / 2), 
                (int)(rect.Height * (scale - 1) / 2));
            
            // Get step color based on state using theme helpers
            Color fillColor = state switch
            {
                StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(_currentTheme, UseThemeColors, completedStepColor),
                StepState.Active => StepperThemeHelpers.GetStepActiveColor(_currentTheme, UseThemeColors, activeStepColor),
                StepState.Error => StepperThemeHelpers.GetStepErrorColor(_currentTheme, UseThemeColors, errorStepColor),
                StepState.Warning => StepperThemeHelpers.GetStepWarningColor(_currentTheme, UseThemeColors, warningStepColor),
                _ => StepperThemeHelpers.GetStepPendingColor(_currentTheme, UseThemeColors, pendingStepColor)
            };

            // Draw step circle
            using (var fillBrush = new SolidBrush(fillColor))
            {
                graphics.FillEllipse(fillBrush, inflated);
            }

            // Draw border for active step using theme helpers
            if (stepIndex == currentStep)
            {
                Color borderColor = StepperThemeHelpers.GetStepBorderColor(_currentTheme, UseThemeColors, state, Color.White);
                int borderWidth = StepperAccessibilityHelpers.GetAccessibleBorderWidth(2);
                using (var borderPen = new Pen(borderColor, borderWidth))
                {
                    graphics.DrawEllipse(borderPen, inflated);
                }
            }

            // Draw content based on display mode
            DrawStepContent(graphics, stepIndex, state, inflated);
        }

        private void DrawStepContent(Graphics graphics, int stepIndex, StepState state, Rectangle rect)
        {
            switch (DisplayMode)
            {
                case StepDisplayMode.CheckImage:
                    if (state == StepState.Completed && !string.IsNullOrWhiteSpace(CheckImage))
                    {
                        if (!stepImages.ContainsKey(stepIndex))
                        {
                            try 
                            { 
                                stepImages[stepIndex] = ImageLoader.LoadImageFromResource(CheckImage); 
                            } 
                            catch 
                            { 
                                // Fallback to drawing a checkmark
                                DrawCheckmark(graphics, rect);
                                return;
                            }
                        }
                        
                        if (stepImages[stepIndex] != null)
                        {
                            var imageRect = new Rectangle(rect.X + 4, rect.Y + 4, rect.Width - 8, rect.Height - 8);
                            graphics.DrawImage(stepImages[stepIndex], imageRect);
                        }
                        else
                        {
                            DrawCheckmark(graphics, rect);
                        }
                    }
                    else if (state != StepState.Completed)
                    {
                        DrawStepNumber(graphics, stepIndex, rect);
                    }
                    break;
                    
                case StepDisplayMode.StepNumber:
                default:
                    if (state == StepState.Completed)
                    {
                        DrawCheckmark(graphics, rect);
                    }
                    else
                    {
                        DrawStepNumber(graphics, stepIndex, rect);
                    }
                    break;
            }
        }

        private void DrawStepNumber(Graphics graphics, int stepIndex, Rectangle rect)
        {
            string text = (stepIndex + 1).ToString();
            
            // Use font helpers for step number font
            Font font = StepperFontHelpers.GetStepNumberFont(this, ControlStyle, _textFont, this);
            
            var textSize = TextUtils.MeasureText(graphics, text, font);
            var textX = rect.Left + (rect.Width - textSize.Width) / 2;
            var textY = rect.Top + (rect.Height - textSize.Height) / 2;
            
            // Use theme helpers for step number text color
            StepState state = GetStepState(stepIndex);
            Color textColor = StepperThemeHelpers.GetStepTextColor(_currentTheme, UseThemeColors, state, Color.White);
            
            // Get step fill color for contrast calculation
            Color stepFillColor = state switch
            {
                StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(_currentTheme, UseThemeColors, completedStepColor),
                StepState.Active => StepperThemeHelpers.GetStepActiveColor(_currentTheme, UseThemeColors, activeStepColor),
                StepState.Error => StepperThemeHelpers.GetStepErrorColor(_currentTheme, UseThemeColors, errorStepColor),
                StepState.Warning => StepperThemeHelpers.GetStepWarningColor(_currentTheme, UseThemeColors, warningStepColor),
                _ => StepperThemeHelpers.GetStepPendingColor(_currentTheme, UseThemeColors, pendingStepColor)
            };
            
            // Adjust text color for contrast if needed
            if (StepperAccessibilityHelpers.IsHighContrastMode())
            {
                var (_, _, _, _, _, highContrastTextColor, _) = StepperAccessibilityHelpers.GetHighContrastColors();
                textColor = highContrastTextColor;
            }
            else
            {
                textColor = StepperAccessibilityHelpers.AdjustForContrast(textColor, stepFillColor);
            }
            
            using (var textBrush = new SolidBrush(textColor))
            {
                graphics.DrawString(text, font, textBrush, textX, textY);
            }
        }

        private void DrawCheckmark(Graphics graphics, Rectangle rect)
        {
            // Use icon helpers for checkmark (fallback if icon path fails)
            // This method is kept for backward compatibility but delegates to icon helpers
            StepState state = StepState.Completed;
            Color iconColor = StepperIconHelpers.GetIconColor(_currentTheme, UseThemeColors, state, Color.White);
            StepperIconHelpers.PaintCheckmarkIcon(graphics, rect, iconColor, 1f);
        }

        private void DrawStepLabel(Graphics graphics, int stepIndex, Rectangle rect)
        {
            string label = GetStepLabel(stepIndex);
            if (string.IsNullOrEmpty(label)) return;
            
            // Use font helpers for step label font
            StepState state = GetStepState(stepIndex);
            Font font = StepperFontHelpers.GetStepLabelFont(this, ControlStyle, state, _textFont);
            
            var textSize = TextUtils.MeasureText(graphics, label, font);
            
            // Use theme helpers for step label color
            Color textColor = StepperThemeHelpers.GetStepLabelColor(_currentTheme, UseThemeColors, state);
            
            // Ensure WCAG contrast compliance
            Color stepFillColor = state switch
            {
                StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(_currentTheme, UseThemeColors, completedStepColor),
                StepState.Active => StepperThemeHelpers.GetStepActiveColor(_currentTheme, UseThemeColors, activeStepColor),
                StepState.Error => StepperThemeHelpers.GetStepErrorColor(_currentTheme, UseThemeColors, errorStepColor),
                StepState.Warning => StepperThemeHelpers.GetStepWarningColor(_currentTheme, UseThemeColors, warningStepColor),
                _ => StepperThemeHelpers.GetStepPendingColor(_currentTheme, UseThemeColors, pendingStepColor)
            };
            
            // Adjust text color for contrast if needed
            if (StepperAccessibilityHelpers.IsHighContrastMode())
            {
                var (_, _, _, _, _, highContrastTextColor, _) = StepperAccessibilityHelpers.GetHighContrastColors();
                textColor = highContrastTextColor;
            }
            else
            {
                textColor = StepperAccessibilityHelpers.AdjustForContrast(textColor, BackColor);
            }
            
            float textX, textY;
            
            if (orientation == Orientation.Horizontal)
            {
                textX = rect.Left + (rect.Width - textSize.Width) / 2;
                textY = rect.Bottom + 5;
            }
            else
            {
                textX = rect.Right + 10;
                textY = rect.Top + (rect.Height - textSize.Height) / 2;
            }
            
            using (var textBrush = new SolidBrush(textColor))
            {
                graphics.DrawString(label, font, textBrush, textX, textY);
            }
        }
        #endregion

        #region Mouse Handling
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            // Detect hovered step
            int hoveredIndex = -1;
            for (int i = 0; i < buttonBounds.Count; i++)
            {
                if (buttonBounds[i].Contains(e.Location))
                {
                    hoveredIndex = i;
                    break;
                }
            }
            
            // Update tooltip for hovered step
            if (hoveredIndex != _hoveredStepIndex)
            {
                _hoveredStepIndex = hoveredIndex;
                UpdateTooltipForHoveredStep();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            
            // Hide tooltip when mouse leaves
            _hoveredStepIndex = -1;
            if (!string.IsNullOrEmpty(_currentTooltipKey))
            {
                _ = ToolTipManager.Instance.HideTooltipAsync(_currentTooltipKey);
                _currentTooltipKey = null;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            if (!allowStepNavigation) return;
            
            for (int i = 0; i < buttonBounds.Count; i++)
            {
                if (buttonBounds[i].Contains(e.Location))
                {
                    NavigateToStep(i);
                    ShowStepNotification(i);
                    break;
                }
            }
        }

        private void NavigateToStep(int stepIndex)
        {
            if (stepIndex >= 0 && stepIndex < stepCount)
            {
                // Validate navigation
                var args = new StepValidatingEventArgs(currentStep, stepIndex);
                StepValidating?.Invoke(this, args);
                
                if (!args.Cancel)
                {
                    CurrentStep = stepIndex;
                    
                    // Fire legacy event for backward compatibility
                    if (stepIndex < ListItems.Count)
                    {
                        SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(ListItems[stepIndex]));
                    }
                }
            }
        }
        #endregion

        #region Legacy Support
        /// <summary>
        /// Update current step (legacy method for TasksView compatibility)
        /// </summary>
        public void UpdateCurrentStep(int index)
        {
            CurrentStep = index;
        }

        /// <summary>
        /// Update checked state (legacy method)
        /// </summary>
        public void UpdateCheckedState(SimpleItem item)
        {
            int index = ListItems.IndexOf(item);
            if (index >= 0 && index < stepCount)
            {
                SetStepState(index, item.IsChecked ? StepState.Completed : StepState.Pending);
            }
        }
        #endregion

        #region Theme Application
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            if (_isApplyingTheme) return;
            
            _isApplyingTheme = true;
            try
            {
                // Use theme helpers for centralized color management
                if (_currentTheme != null && UseThemeColors)
                {
                    StepperThemeHelpers.ApplyThemeColors(this, _currentTheme, UseThemeColors);
                }
                
                // Set _textFont from theme stepper properties
                _textFont?.Dispose();
                _textFont = (_currentTheme?.StepperItemFont != null)
                    ? BeepFontManager.ToFont(_currentTheme.StepperItemFont)
                    : null;
                
                // Apply font theme
                StepperFontHelpers.ApplyFontTheme(this, ControlStyle, _textFont);
                
                // Apply accessibility adjustments (high contrast, reduced motion)
                ApplyAccessibilityAdjustments();
            }
            finally
            {
                _isApplyingTheme = false;
            }
            
            Invalidate();
        }
        #endregion

        #region Accessibility
        /// <summary>
        /// Apply accessibility settings (ARIA attributes)
        /// </summary>
        public void ApplyAccessibilitySettings()
        {
            StepperAccessibilityHelpers.ApplyAccessibilitySettings(this, AccessibleName, AccessibleDescription);
        }

        /// <summary>
        /// Apply accessibility adjustments (high contrast, reduced motion)
        /// </summary>
        public void ApplyAccessibilityAdjustments()
        {
            if (StepperAccessibilityHelpers.IsHighContrastMode())
            {
                StepperAccessibilityHelpers.ApplyHighContrastAdjustments(this, _currentTheme, UseThemeColors);
            }
        }

        /// <summary>
        /// Override MinimumSize to enforce accessible minimum size
        /// </summary>
        public override Size MinimumSize
        {
            get
            {
                var baseSize = base.MinimumSize;
                return StepperAccessibilityHelpers.GetAccessibleMinimumSize(baseSize);
            }
            set
            {
                var accessibleSize = StepperAccessibilityHelpers.GetAccessibleMinimumSize(value);
                base.MinimumSize = accessibleSize;
            }
        }
        #endregion

        #region Tooltip Integration

        /// <summary>
        /// Gets or sets whether tooltips are automatically generated for stepper steps
        /// When true, tooltips are generated from step labels and states
        /// </summary>
        [Browsable(true)]
        [Category("Tooltip")]
        [Description("Automatically generate tooltips for stepper steps based on their labels and states")]
        [DefaultValue(true)]
        public bool AutoGenerateTooltips
        {
            get => _autoGenerateTooltips;
            set
            {
                if (_autoGenerateTooltips != value)
                {
                    _autoGenerateTooltips = value;
                    UpdateAllStepTooltips();
                }
            }
        }

        /// <summary>
        /// Set a custom tooltip for a specific step
        /// </summary>
        /// <param name="stepIndex">The index of the step (0-based)</param>
        /// <param name="tooltipText">The tooltip text to display</param>
        public void SetStepTooltip(int stepIndex, string tooltipText)
        {
            if (stepIndex < 0 || stepIndex >= stepCount)
                return;

            if (string.IsNullOrEmpty(tooltipText))
            {
                _stepTooltips.Remove(stepIndex);
            }
            else
            {
                _stepTooltips[stepIndex] = tooltipText;
            }

            UpdateStepTooltip(stepIndex);
        }

        /// <summary>
        /// Get the tooltip text for a specific step
        /// </summary>
        public string GetStepTooltip(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= stepCount)
                return string.Empty;

            if (_stepTooltips.TryGetValue(stepIndex, out var customTooltip))
            {
                return customTooltip;
            }

            if (_autoGenerateTooltips)
            {
                return GenerateStepTooltip(stepIndex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Remove tooltip for a specific step
        /// </summary>
        public void RemoveStepTooltip(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= stepCount)
                return;

            _stepTooltips.Remove(stepIndex);
            UpdateStepTooltip(stepIndex);
        }

        /// <summary>
        /// Generate automatic tooltip text for a step
        /// </summary>
        private string GenerateStepTooltip(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= stepCount)
                return string.Empty;

            string stepLabel = GetStepLabel(stepIndex);
            StepState state = GetStepState(stepIndex);
            bool isCurrentStep = stepIndex == currentStep;
            bool isClickable = allowStepNavigation;

            string stateText = state switch
            {
                StepState.Completed => "Completed",
                StepState.Active => "Active",
                StepState.Error => "Error",
                StepState.Warning => "Warning",
                _ => "Pending"
            };

            string tooltip = $"Step {stepIndex + 1} of {stepCount}";
            
            if (!string.IsNullOrEmpty(stepLabel))
            {
                tooltip += $": {stepLabel}";
            }

            tooltip += $", {stateText}";

            if (isCurrentStep)
            {
                tooltip += " (Current)";
            }

            if (isClickable)
            {
                tooltip += ". Click to navigate";
            }

            return tooltip;
        }

        /// <summary>
        /// Create tooltip configuration for a step
        /// </summary>
        private ToolTipConfig CreateStepTooltipConfig(int stepIndex)
        {
            string tooltipText = GetStepTooltip(stepIndex);
            if (string.IsNullOrEmpty(tooltipText))
                return null;

            StepState state = GetStepState(stepIndex);
            ToolTipType tooltipType = state switch
            {
                StepState.Error => ToolTipType.Error,
                StepState.Warning => ToolTipType.Warning,
                StepState.Completed => ToolTipType.Success,
                StepState.Active => ToolTipType.Info,
                _ => ToolTipType.Default
            };

            return new ToolTipConfig
            {
                Text = tooltipText,
                Title = $"Step {stepIndex + 1}",
                Type = tooltipType,
                UseBeepThemeColors = UseThemeColors,
                ControlStyle = ControlStyle,
                ShowArrow = true,
                ShowShadow = true,
                Duration = 3000,
                Placement = ToolTipPlacement.Top
            };
        }

        /// <summary>
        /// Update tooltip for a specific step
        /// </summary>
        private void UpdateStepTooltip(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= stepCount)
                return;

            var config = CreateStepTooltipConfig(stepIndex);
            if (config != null)
            {
                _stepTooltipConfigs[stepIndex] = config;
            }
            else
            {
                _stepTooltipConfigs.Remove(stepIndex);
            }
        }

        /// <summary>
        /// Update tooltips for all steps
        /// </summary>
        private void UpdateAllStepTooltips()
        {
            _stepTooltipConfigs.Clear();
            for (int i = 0; i < stepCount; i++)
            {
                UpdateStepTooltip(i);
            }
        }

        /// <summary>
        /// Update tooltip for the hovered step
        /// </summary>
        private async void UpdateTooltipForHoveredStep()
        {
            // Hide existing tooltip first
            if (!string.IsNullOrEmpty(_currentTooltipKey))
            {
                await ToolTipManager.Instance.HideTooltipAsync(_currentTooltipKey);
                _currentTooltipKey = null;
            }

            if (_hoveredStepIndex >= 0 && _hoveredStepIndex < stepCount)
            {
                if (_stepTooltipConfigs.TryGetValue(_hoveredStepIndex, out var config))
                {
                    // Calculate position for the step button
                    if (_hoveredStepIndex < buttonBounds.Count)
                    {
                        Rectangle stepBounds = buttonBounds[_hoveredStepIndex];
                        Point tooltipPosition = new Point(
                            stepBounds.Left + stepBounds.Width / 2,
                            stepBounds.Top
                        );

                        // Convert to screen coordinates
                        tooltipPosition = PointToScreen(tooltipPosition);
                        config.Position = tooltipPosition;

                        // Show tooltip and store the key
                        _currentTooltipKey = await ToolTipManager.Instance.ShowTooltipAsync(config);
                    }
                }
            }
        }

        /// <summary>
        /// Update tooltip for the main stepper control
        /// </summary>
        private void UpdateStepperTooltip()
        {
            if (!EnableTooltip)
                return;

            string tooltipText = GenerateStepperTooltip();
            if (!string.IsNullOrEmpty(tooltipText))
            {
                TooltipText = tooltipText;
                TooltipTitle = "Stepper";
                TooltipType = ToolTipType.Info;
                UpdateTooltip();
            }
        }

        /// <summary>
        /// Generate tooltip text for the main stepper control
        /// </summary>
        private string GenerateStepperTooltip()
        {
            string currentStepLabel = GetStepLabel(currentStep);
            int percentage = stepCount > 0 ? (int)((float)(currentStep + 1) / stepCount * 100) : 0;

            string tooltip = $"Step {currentStep + 1} of {stepCount} ({percentage}%)";
            
            if (!string.IsNullOrEmpty(currentStepLabel))
            {
                tooltip += $": {currentStepLabel}";
            }

            return tooltip;
        }

        /// <summary>
        /// Show a notification when a step is clicked
        /// </summary>
        private void ShowStepNotification(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= stepCount)
                return;

            string stepLabel = GetStepLabel(stepIndex);
            StepState state = GetStepState(stepIndex);

            ToolTipType notificationType = state switch
            {
                StepState.Error => ToolTipType.Error,
                StepState.Warning => ToolTipType.Warning,
                StepState.Completed => ToolTipType.Success,
                _ => ToolTipType.Info
            };

            string message = $"Navigated to step {stepIndex + 1}";
            if (!string.IsNullOrEmpty(stepLabel))
            {
                message += $": {stepLabel}";
            }

            ShowNotification(message, notificationType, 2000);
        }

        /// <summary>
        /// Set tooltip for the stepper control itself
        /// </summary>
        public void SetStepperTooltip(string text, string title = null, ToolTipType type = ToolTipType.Default)
        {
            TooltipText = text;
            TooltipTitle = title ?? "Stepper";
            TooltipType = type;
            UpdateTooltip();
        }

        /// <summary>
        /// Show a stepper notification
        /// </summary>
        public void ShowStepperNotification(string message, ToolTipType type = ToolTipType.Info, int duration = 2000)
        {
            ShowNotification(message, type, duration);
        }
        #endregion

        #region Disposal
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textFont?.Dispose();
                animationTimer?.Stop();
                animationTimer?.Dispose();
                
                foreach (var image in stepImages.Values)
                {
                    image?.Dispose();
                }
                stepImages.Clear();
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    #region Event Args
    public class StepChangedEventArgs : EventArgs
    {
        public int OldStepIndex { get; }
        public int NewStepIndex { get; }

        public StepChangedEventArgs(int oldStepIndex, int newStepIndex)
        {
            OldStepIndex = oldStepIndex;
            NewStepIndex = newStepIndex;
        }
    }

    public class StepValidatingEventArgs : EventArgs
    {
        public int FromStep { get; }
        public int ToStep { get; }
        public bool Cancel { get; set; }

        public StepValidatingEventArgs(int fromStep, int toStep)
        {
            FromStep = fromStep;
            ToStep = toStep;
            Cancel = false;
        }
    }
    #endregion
}
