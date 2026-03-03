using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.ToolTips;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
   
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Stepper Breadcrumb")]
    [Description("A breadcrumb-Style stepper control that draws chevron-shaped steps directly, with clickable steps and optional orientation.")]
    public partial class BeepStepperBreadCrumb : BaseControl
    {
        private Orientation orientation = Orientation.Horizontal;
        private int selectedIndex = -1;
        private readonly List<GraphicsPath> chevronPaths = new List<GraphicsPath>(); // For precise click detection
        private readonly List<Rectangle> chevronBounds = new List<Rectangle>();

        // Animation
        private System.Windows.Forms.Timer animationTimer;
        private int animFrame;
        private const int animFramesTotal = 10;
        private int animIndex = -1;
        private Color animStart, animEnd;

        private Font _textFont;

        // Tooltip support
        private bool _autoGenerateTooltips = true;
        private readonly Dictionary<int, string> _stepTooltips = new Dictionary<int, string>();
        private readonly Dictionary<int, ToolTipConfig> _stepTooltipConfigs = new Dictionary<int, ToolTipConfig>();
        private int _hoveredStepIndex = -1;
        private string _currentTooltipKey = null;
       

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Which way middle chevrons should point.")]
        public ChevronDirection Direction { get; set; } = ChevronDirection.Forward;

        private string _defaultImagePathForStepButtons = "check.svg";
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Default image path for checked steps. If not set, no image is displayed.")]
        public string CheckImage
        {
            get => _defaultImagePathForStepButtons;
            set
            {
                _defaultImagePathForStepButtons = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Controls whether the stepper is laid out horizontally or vertically.")]
        public Orientation Orientation
        {
            get => orientation;
            set
            {
                orientation = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The steps to display.")]
        public BindingList<SimpleItem> ListItems { get; set; } = new BindingList<SimpleItem>();

        [Browsable(false)]
        public int SelectedIndex => selectedIndex;

        [Browsable(false)]
        public SimpleItem SelectedItem => selectedIndex >= 0 && selectedIndex < ListItems.Count ? ListItems[selectedIndex] : null;

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

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        public BeepStepperBreadCrumb()
        {
            ListItems.ListChanged += (s, e) => 
            {
                ApplyAccessibilitySettings();
                UpdateAllStepTooltips();
                Invalidate();
            };
            SizeChanged += (s, e) => Invalidate();
            DoubleBuffered = true;
            Padding = new Padding(DpiScalingHelper.ScaleValue(5, this));
            animationTimer = new System.Windows.Forms.Timer { Interval = 25 };
            animationTimer.Tick += AnimationTimer_Tick;
            InitializePainter();
            ApplyAccessibilitySettings();
            UpdateAllStepTooltips();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Respect reduced motion preferences
            if (StepperAccessibilityHelpers.IsReducedMotionEnabled())
            {
                animationTimer.Stop();
                if (animIndex >= 0 && animIndex < ListItems.Count)
                {
                    animatedColors[animIndex] = animEnd;
                }
                animIndex = -1;
                Invalidate();
                return;
            }

            if (animIndex < 0 || animIndex >= ListItems.Count)
            {
                animationTimer.Stop();
                return;
            }

            animFrame++;
            float t = StepperAnimationEasing.CubicEaseOut(animFrame / (float)animFramesTotal);
            int r = (int)(animStart.R + (animEnd.R - animStart.R) * t);
            int g = (int)(animStart.G + (animEnd.G - animStart.G) * t);
            int b = (int)(animStart.B + (animEnd.B - animStart.B) * t);
            animatedColors[animIndex] = Color.FromArgb(r, g, b);
            Invalidate();

            if (animFrame >= animFramesTotal)
            {
                int completedIndex = animIndex;
                animationTimer.Stop();
                animIndex = -1;
                animatedColors.Remove(completedIndex);
            }
        }

        private Dictionary<int, Color> animatedColors = new Dictionary<int, Color>();

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            base.OnPaint(e);
        }
    protected override void DrawContent(Graphics g)
{
    base.DrawContent(g);
    UpdateDrawingRect();
    chevronPaths.Clear();
    chevronBounds.Clear();
    animatedColors.Clear();

    if (TryPaintWithRegisteredPainter(g))
    {
        return;
    }

    if (ListItems == null || ListItems.Count == 0)
        return;

    DrawLegacyChevronLayout(g);
}

        private void OnStepClicked(int index)
        {
            if (index >= 0 && index < ListItems.Count)
            {
                animIndex = index;
                animFrame = 0;
                
                // Use theme helpers for animation colors
                StepState currentState = selectedIndex == index ? StepState.Active : StepState.Pending;
                StepState targetState = StepState.Active;
                
                animStart = currentState == StepState.Active 
                    ? StepperThemeHelpers.GetStepActiveColor(_currentTheme, UseThemeColors)
                    : StepperThemeHelpers.GetStepPendingColor(_currentTheme, UseThemeColors);
                animEnd = StepperThemeHelpers.GetStepActiveColor(_currentTheme, UseThemeColors);
                
                animationTimer.Start();
                selectedIndex = index;
                _focusedStepIndex = index;
                SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(SelectedItem));
                UpdateStepperTooltip();
                Invalidate();
            }
        }

   

        public void UpdateCheckedState(SimpleItem item)
        {
            int index = ListItems.IndexOf(item);
            if (index >= 0 && index < ListItems.Count)
            {
                UpdateCurrentStep(index);
            }
        }

        public void UpdateCurrentStep(int index)
        {
            if (index >= 0 && index < ListItems.Count)
            {
                ListItems[index].IsChecked = true;
                SetAllStepsBefore(index);
                Invalidate();
            }
        }

        private void SetAllStepsBefore(int index)
        {
            for (int i = 0; i < ListItems.Count; i++)
            {
                ListItems[i].IsChecked = i <= index;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textFont = null;
            }
            base.Dispose(disposing);
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            if (IsChild && Parent != null)
            {
                BackColor = Parent.BackColor;
                ParentBackColor = Parent.BackColor;
            }
            
            // Use theme helpers for background color
            if (_currentTheme != null && UseThemeColors)
            {
                BackColor = StepperThemeHelpers.GetStepBackgroundColor(_currentTheme, UseThemeColors, BackColor);
            }
            else
            {
                BackColor = _currentTheme?.CardBackColor ?? BackColor;
            }
            
            ForeColor = _currentTheme?.CardTextForeColor ?? ForeColor;
            HoverBackColor = _currentTheme?.ButtonHoverBackColor ?? HoverBackColor;
            HoverForeColor = _currentTheme?.ButtonHoverForeColor ?? HoverForeColor;
            DisabledBackColor = _currentTheme?.DisabledBackColor ?? DisabledBackColor;
            DisabledForeColor = _currentTheme?.DisabledForeColor ?? DisabledForeColor;
            FocusBackColor = _currentTheme?.ButtonSelectedBackColor ?? FocusBackColor;
            FocusForeColor = _currentTheme?.ButtonSelectedForeColor ?? FocusForeColor;
            PressedBackColor = _currentTheme?.ButtonPressedBackColor ?? PressedBackColor;
            PressedForeColor = _currentTheme?.ButtonPressedForeColor ?? PressedForeColor;

            _textFont = (_currentTheme?.StepperItemFont != null)
                ? BeepThemesManager.ToFont(_currentTheme.StepperItemFont)
                : BeepThemesManager.ToFont(_currentTheme?.BodyMedium);
            InitializePainter();
            
            // Apply accessibility adjustments (high contrast, reduced motion)
            ApplyAccessibilityAdjustments();

            Invalidate();  // Trigger repaint
        }

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

        public bool UseThemeColors { get; private set; }

        /// <summary>
        /// Set a custom tooltip for a specific step
        /// </summary>
        /// <param name="stepIndex">The index of the step (0-based)</param>
        /// <param name="tooltipText">The tooltip text to display</param>
        public void SetStepTooltip(int stepIndex, string tooltipText)
        {
            if (stepIndex < 0 || stepIndex >= ListItems.Count)
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
            if (stepIndex < 0 || stepIndex >= ListItems.Count)
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
            if (stepIndex < 0 || stepIndex >= ListItems.Count)
                return;

            _stepTooltips.Remove(stepIndex);
            UpdateStepTooltip(stepIndex);
        }

        /// <summary>
        /// Generate automatic tooltip text for a step
        /// </summary>
        private string GenerateStepTooltip(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= ListItems.Count)
                return string.Empty;

            var item = ListItems[stepIndex];
            string stepLabel = item?.Name ?? item?.Text ?? "";
            StepState state = stepIndex < selectedIndex ? StepState.Completed :
                             stepIndex == selectedIndex ? StepState.Active : StepState.Pending;
            bool isCurrentStep = stepIndex == selectedIndex;

            string stateText = state switch
            {
                StepState.Completed => "Completed",
                StepState.Active => "Active",
                StepState.Error => "Error",
                StepState.Warning => "Warning",
                _ => "Pending"
            };

            string tooltip = $"Step {stepIndex + 1} of {ListItems.Count}";
            
            if (!string.IsNullOrEmpty(stepLabel))
            {
                tooltip += $": {stepLabel}";
            }

            tooltip += $", {stateText}";

            if (isCurrentStep)
            {
                tooltip += " (Current)";
            }

            tooltip += ". Click to navigate";

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

            StepState state = stepIndex < selectedIndex ? StepState.Completed :
                             stepIndex == selectedIndex ? StepState.Active : StepState.Pending;
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
            if (stepIndex < 0 || stepIndex >= ListItems.Count)
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
            for (int i = 0; i < ListItems.Count; i++)
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

            if (_hoveredStepIndex >= 0 && _hoveredStepIndex < ListItems.Count)
            {
                if (_stepTooltipConfigs.TryGetValue(_hoveredStepIndex, out var config))
                {
                    // Calculate position for the chevron step
                    if (_hoveredStepIndex < chevronPaths.Count)
                    {
                        using (Region region = new Region(chevronPaths[_hoveredStepIndex]))
                        {
                            RectangleF bounds = region.GetBounds(Graphics.FromHwnd(Handle));
                            Point tooltipPosition = new Point(
                                (int)(bounds.Left + bounds.Width / 2),
                                (int)bounds.Top
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
            if (ListItems == null || ListItems.Count == 0)
                return "Stepper (no steps)";

            int percentage = (int)((float)(selectedIndex + 1) / ListItems.Count * 100);
            var selectedItem = SelectedItem;
            string currentStepLabel = selectedItem?.Name ?? selectedItem?.Text ?? "";

            string tooltip = $"Step {selectedIndex + 1} of {ListItems.Count} ({percentage}%)";
            
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
            if (stepIndex < 0 || stepIndex >= ListItems.Count)
                return;

            var item = ListItems[stepIndex];
            string stepLabel = item?.Name ?? item?.Text ?? "";
            StepState state = stepIndex < selectedIndex ? StepState.Completed :
                             stepIndex == selectedIndex ? StepState.Active : StepState.Pending;

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
    }
}