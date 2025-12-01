using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Steppers.Helpers;
using TheTechIdea.Beep.Winform.Controls.ToolTips;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
   
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Stepper Breadcrumb")]
    [Description("A breadcrumb-Style stepper control that draws chevron-shaped steps directly, with clickable steps and optional orientation.")]
    public class BeepStepperBreadCrumb : BaseControl
    {
        private Orientation orientation = Orientation.Horizontal;
        private int selectedIndex = -1;
        private readonly List<GraphicsPath> chevronPaths = new List<GraphicsPath>(); // For precise click detection

        // Animation
        private System.Windows.Forms.Timer animationTimer;
        private int animFrame;
        private const int animFramesTotal = 10;
        private int animIndex = -1;
        private Color animStart, animEnd;

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
            Padding = new Padding(5);
            animationTimer = new System.Windows.Forms.Timer { Interval = 25 };
            animationTimer.Tick += AnimationTimer_Tick;
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
            float t = animFrame / (float)animFramesTotal;
            if (t > 1f) t = 1f;
            int r = (int)(animStart.R + (animEnd.R - animStart.R) * t);
            int g = (int)(animStart.G + (animEnd.G - animStart.G) * t);
            int b = (int)(animStart.B + (animEnd.B - animStart.B) * t);
            animatedColors[animIndex] = Color.FromArgb(r, g, b);
            Invalidate();

            if (animFrame >= animFramesTotal)
            {
                animationTimer.Stop();
                animIndex = -1;
                animatedColors.Remove(animIndex);
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
    animatedColors.Clear();

    if (ListItems == null || ListItems.Count == 0)
        return;

    int count       = ListItems.Count;
    int totalLen    = orientation == Orientation.Horizontal 
                         ? DrawingRect.Width  
                         : DrawingRect.Height;
    int crossLen    = orientation == Orientation.Horizontal 
                         ? DrawingRect.Height 
                         : DrawingRect.Width;
    int stepLen     = totalLen / Math.Max(1, count);

    // Depth of each arrowhead. Tune (/3 or /4) if you want shallower points.
    int arrowSize   = crossLen / 4; 

    for (int i = 0; i < count; i++)
    {
        int x = orientation == Orientation.Horizontal
                    ? DrawingRect.Left + (i * stepLen)
                    : DrawingRect.Left;
        int y = orientation == Orientation.Horizontal
                    ? DrawingRect.Top
                    : DrawingRect.Top + (i * stepLen) ;

            Point[] pts;

        if (orientation == Orientation.Horizontal)
        {
            if (i == 0)
            {
                // ───► first: straight left, arrow on right
                pts = new[]
                {
                    new Point(x,                  y),
                    new Point(x + stepLen - arrowSize, y),
                    new Point(x + stepLen,        y + crossLen/2),
                    new Point(x + stepLen - arrowSize, y + crossLen),
                    new Point(x,                  y + crossLen),
                    new Point(x,                  y)

                };
            }
            else if (i == count - 1)
            {
                // ◄─── last: arrow on left, straight right
                pts = new[]
                {
                    new Point(x,                  y),
                    new Point(x + stepLen,        y),
                    new Point(x + stepLen,        y + crossLen),
                    new Point(x,                  y + crossLen),
                    new Point(x + arrowSize,      (y+ crossLen)- crossLen/2),
                    new Point(x ,      y ),
                };
            }
            else
            {
                // ◄───► middle: arrow on both sides
                pts = new[]
                {
                    new Point(x,                  y),
                    new Point(x + stepLen - arrowSize, y),
                     new Point(x + stepLen,        y + crossLen/2),
                    new Point(x + stepLen - arrowSize, y + crossLen),
                    new Point(x,                  y + crossLen),
                     new Point(x + arrowSize,      (y+ crossLen)- crossLen/2),
                    new Point(x ,      y ),

                };
            }
        }
        else
        {
            // Vertical: mirror the same pattern top/bottom
            if (i == 0)
            {
                // straight top, arrow at bottom
                pts = new[]
                {
                    new Point(x,                  y),
                    new Point(x + crossLen,       y),
                    new Point(x + crossLen/2,     y + stepLen),
                    new Point(x,                  y + stepLen)
                };
            }
            else if (i == count - 1)
            {
                // arrow at top, straight bottom
                pts = new[]
                {
                    new Point(x + crossLen/2,     y),
                    new Point(x + crossLen,       y + stepLen - arrowSize),
                    new Point(x + crossLen,       y + stepLen),
                    new Point(x,                  y + stepLen)
                };
            }
            else
            {
                // middle: arrow on both top & bottom
                pts = new[]
                {
                    new Point(x + crossLen/2,     y),               // top tip
                    new Point(x + crossLen,       y + arrowSize),   // right‑top slant
                    new Point(x + crossLen,       y + stepLen - arrowSize), // right‑bottom
                    new Point(x + crossLen/2,     y + stepLen),     // bottom tip
                    new Point(x,                  y + stepLen - arrowSize), // left‑bottom
                    new Point(x,                  y + arrowSize)     // left‑top
                };
            }
        }

        // hit‑test
        var path = new GraphicsPath();
        path.AddPolygon(pts);
        chevronPaths.Add(path);

        // fill - use theme helpers for consistent color management
        Color fill;
        if (animatedColors.ContainsKey(i))
        {
            fill = animatedColors[i];
        }
        else
        {
            StepState state = i < selectedIndex ? StepState.Completed :
                             i == selectedIndex ? StepState.Active : StepState.Pending;
            
            fill = state switch
            {
                StepState.Completed => StepperThemeHelpers.GetStepCompletedColor(_currentTheme, UseThemeColors),
                StepState.Active => StepperThemeHelpers.GetStepActiveColor(_currentTheme, UseThemeColors),
                _ => StepperThemeHelpers.GetStepPendingColor(_currentTheme, UseThemeColors)
            };
        }

        using (var br = new SolidBrush(fill))
            g.FillPolygon(br, pts);

        int accessibleBorderWidth = StepperAccessibilityHelpers.GetAccessibleBorderWidth(BorderThickness);
        using (var pen = new Pen(_currentTheme.ShadowColor, accessibleBorderWidth))
            g.DrawPolygon(pen, pts);

               if(orientation == Orientation.Horizontal)
                {
                    // Get item info
                    var headerText = ListItems[i].Name ?? "";
                    var subText = ListItems[i].Text ?? "";

                    // Fonts - use font helpers
                    StepState state = i == selectedIndex ? StepState.Active : StepState.Pending;
                    Font headerFont = StepperFontHelpers.GetStepLabelFont(this, ControlStyle, state);
                    Font subFont = StepperFontHelpers.GetStepTextFont(this, ControlStyle);

                    // Measure both
                    var headerSize = TextUtils.MeasureText(g,headerText, headerFont);
                    var subSize = TextUtils.MeasureText(g,subText, subFont);

                    // Total vertical space
                    float totalTextHeight = headerSize.Height + subSize.Height;

                    // Starting Y so both lines are vertically centered as a block
                    float startY = y + (crossLen - totalTextHeight) / 2;

                    // Horizontal X center
                    float headerX = x + (stepLen - headerSize.Width) / 2;
                    float subX = x + (stepLen - subSize.Width) / 2;

                    // Colors - use theme helpers for text color (state already declared above)
                    Color foreColor = StepperThemeHelpers.GetStepLabelColor(_currentTheme, UseThemeColors, state);

                    // Ensure WCAG contrast compliance
                    if (StepperAccessibilityHelpers.IsHighContrastMode())
                    {
                        var (_, _, _, _, _, highContrastTextColor, _) = StepperAccessibilityHelpers.GetHighContrastColors();
                        foreColor = highContrastTextColor;
                    }
                    else
                    {
                        foreColor = StepperAccessibilityHelpers.AdjustForContrast(foreColor, BackColor);
                    }

                    // Draw header
                    using (var brush = new SolidBrush(foreColor))
                    {
                        g.DrawString(headerText, headerFont, brush, headerX, startY);
                        g.DrawString(subText, subFont, brush, subX, startY + headerSize.Height);
                    }

                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            // Detect hovered step
            int hoveredIndex = -1;
            for (int i = 0; i < chevronPaths.Count; i++)
            {
                using (Region region = new Region(chevronPaths[i]))
                {
                    if (region.IsVisible(e.Location))
                    {
                        hoveredIndex = i;
                        break;
                    }
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
            IsPressed = false;
            IsSelected = false;
            for (int i = 0; i < chevronPaths.Count; i++)
            {
                using (Region region = new Region(chevronPaths[i]))
                {
                    if (region.IsVisible(e.Location))
                    {
                        OnStepClicked(i);
                        ShowStepNotification(i);
                        break;
                    }
                }
            }
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

            // Apply font theme
            StepperFontHelpers.ApplyFontTheme(this, ControlStyle);
            
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