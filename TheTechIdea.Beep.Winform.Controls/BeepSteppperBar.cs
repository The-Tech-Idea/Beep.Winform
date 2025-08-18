using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Models;
using Timer = System.Windows.Forms.Timer;

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
    public class BeepStepperBar : BeepControl
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
            set { buttonSize = value; Invalidate(); }
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
                    
                    // Trigger animation
                    StartStepAnimation(value);
                    
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
            get => highlightActiveStep;
            set => highlightActiveStep = value;
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
                Invalidate();
            };

            InitializeAnimation();
            InitializeSteps();
        }

        private void InitializeAnimation()
        {
            animationTimer = new Timer { Interval = 16 }; // 60 FPS
            animationTimer.Tick += (s, e) =>
            {
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
                    Id = i,
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
            Color lineColor = currentStepState == StepState.Completed ? completedStepColor : pendingStepColor;
            
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

            using (var pen = new Pen(lineColor, connectorLineWidth))
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
            
            // Get step color based on state
            Color fillColor = state switch
            {
                StepState.Completed => completedStepColor,
                StepState.Active => activeStepColor,
                StepState.Error => errorStepColor,
                StepState.Warning => warningStepColor,
                _ => pendingStepColor
            };

            // Draw step circle
            using (var fillBrush = new SolidBrush(fillColor))
            {
                graphics.FillEllipse(fillBrush, inflated);
            }

            // Draw border for active step
            if (stepIndex == currentStep)
            {
                using (var borderPen = new Pen(Color.White, 2))
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
            var font = this.Font ?? new Font("Segoe UI", 10, FontStyle.Bold);
            var textSize = graphics.MeasureString(text, font);
            var textX = rect.Left + (rect.Width - textSize.Width) / 2;
            var textY = rect.Top + (rect.Height - textSize.Height) / 2;
            
            using (var textBrush = new SolidBrush(Color.White))
            {
                graphics.DrawString(text, font, textBrush, textX, textY);
            }
        }

        private void DrawCheckmark(Graphics graphics, Rectangle rect)
        {
            // Draw a simple checkmark
            using (var pen = new Pen(Color.White, 2))
            {
                var centerX = rect.Left + rect.Width / 2;
                var centerY = rect.Top + rect.Height / 2;
                var size = Math.Min(rect.Width, rect.Height) / 3;
                
                graphics.DrawLines(pen, new Point[]
                {
                    new Point(centerX - size/2, centerY),
                    new Point(centerX - size/6, centerY + size/2),
                    new Point(centerX + size/2, centerY - size/2)
                });
            }
        }

        private void DrawStepLabel(Graphics graphics, int stepIndex, Rectangle rect)
        {
            string label = GetStepLabel(stepIndex);
            if (string.IsNullOrEmpty(label)) return;
            
            var font = new Font(this.Font?.FontFamily ?? FontFamily.GenericSansSerif, 9, FontStyle.Regular);
            var textSize = graphics.MeasureString(label, font);
            
            Color textColor = GetStepState(stepIndex) == StepState.Active ? 
                (_currentTheme?.CardTitleForeColor ?? Color.Black) :
                (_currentTheme?.CardSubTitleForeColor ?? Color.Gray);
            
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
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            if (!allowStepNavigation) return;
            
            for (int i = 0; i < buttonBounds.Count; i++)
            {
                if (buttonBounds[i].Contains(e.Location))
                {
                    NavigateToStep(i);
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
            
            if (_currentTheme != null)
            {
                // Update colors based on theme
                completedStepColor = _currentTheme.ButtonPressedBackColor;
                activeStepColor = _currentTheme.ButtonBackColor;
                pendingStepColor = _currentTheme.DisabledBackColor;
            }
            
            Invalidate();
        }
        #endregion

        #region Disposal
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
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
