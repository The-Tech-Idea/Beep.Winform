using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    /// <summary>
    /// Grid title header panel - displays title and action buttons
    /// Modern React-style component with BeepControl HitArea integration
    /// </summary>
    [ToolboxItem(false)]
    public class GridTitleHeaderPanel : BeepControl
    {
        #region Fields
        private BeepGrid _parentGrid;
        private BeepLabel _titleLabel;
        private List<BeepButton> _actionButtons;
        private string _titleText = "Data Grid";
        private bool _showActionButtons = true;
        private int _buttonSpacing = 8;
        private int _buttonSize = 24;
        #endregion

        #region Events
        public event EventHandler<string> ActionButtonClicked;
        public event EventHandler TitleChanged;
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("Data Grid")]
        public string TitleText
        {
            get => _titleText;
            set
            {
                if (_titleText != value)
                {
                    _titleText = value;
                    if (_titleLabel != null)
                        _titleLabel.Text = value;
                    TitleChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool ShowActionButtons
        {
            get => _showActionButtons;
            set
            {
                if (_showActionButtons != value)
                {
                    _showActionButtons = value;
                    UpdateActionButtonsVisibility();
                    SetupHitAreas(); // Re-setup hit areas when visibility changes
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(24)]
        public int ButtonSize
        {
            get => _buttonSize;
            set
            {
                if (_buttonSize != value && value > 16)
                {
                    _buttonSize = value;
                   // RecreateActionButtons();
                }
            }
        }

        [Browsable(false)]
        public BeepGrid ParentGrid => _parentGrid;
        #endregion

        #region Constructor
        public GridTitleHeaderPanel(BeepGrid parentGrid = null)
        {
            _parentGrid = parentGrid;
            _actionButtons = new List<BeepButton>();
            
            Height = 40;
            Dock = DockStyle.Top;
            
            InitializeComponents();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);

            // Subscribe to HitDetected event from BeepControl
            HitDetected += OnHitDetected;
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            // Create title label
            _titleLabel = new BeepLabel
            {
                Text = _titleText,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Theme = Theme,
                IsChild = true,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold)
            };

            CreateActionButtons();
            Controls.Add(_titleLabel);
            
            // Setup initial hit areas
            SetupHitAreas();
        }

        private void CreateActionButtons()
        {
            // Clear existing buttons
            foreach (var button in _actionButtons)
            {
                Controls.Remove(button);
                button.Dispose();
            }
            _actionButtons.Clear();

            if (!_showActionButtons) return;

            var buttonConfigs = new[]
            {
                new { Name = "Print", Icon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.072-printer.svg", Tooltip = "Print Grid" },
                new { Name = "Export", Icon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.083-share.svg", Tooltip = "Export Data" },
                new { Name = "Refresh", Icon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.071-refresh.svg", Tooltip = "Refresh Data" },
                new { Name = "Settings", Icon = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.077-settings.svg", Tooltip = "Grid Settings" }
            };

            foreach (var config in buttonConfigs)
            {
                var button = new BeepButton
                {
                    ImagePath = config.Icon,
                    HideText = true,
                    IsFrameless = true,
                    Size = new Size(_buttonSize, _buttonSize),
                    Theme = Theme,
                    IsChild = true,
                    MaxImageSize = new Size(_buttonSize - 4, _buttonSize - 4),
                    Name = config.Name,
                    ToolTipText = config.Tooltip,
                    Visible = _showActionButtons
                };

                _actionButtons.Add(button);
                Controls.Add(button);
            }
            
            ArrangeComponents();
        }

        private void UpdateActionButtonsVisibility()
        {
            foreach (var button in _actionButtons)
            {
                button.Visible = _showActionButtons;
            }
            ArrangeComponents();
        }
        #endregion

        #region Layout Management
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ArrangeComponents();
            SetupHitAreas(); // Re-setup hit areas on resize
        }

        private void ArrangeComponents()
        {
            if (_titleLabel == null) return;

            const int padding = 12;
            int rightOffset = Width - padding;

            // Position action buttons from right to left
            if (_showActionButtons)
            {
                for (int i = _actionButtons.Count - 1; i >= 0; i--)
                {
                    var button = _actionButtons[i];
                    if (button.Visible)
                    {
                        rightOffset -= _buttonSize;
                        button.Location = new Point(rightOffset, (Height - _buttonSize) / 2);
                        rightOffset -= _buttonSpacing;
                    }
                }
            }

            // Position title label
            int titleWidth = rightOffset - padding - padding;
            _titleLabel.Bounds = new Rectangle(padding, 0, Math.Max(titleWidth, 0), Height);
        }
        #endregion

        #region BeepControl HitArea Integration
        
        /// <summary>
        /// Setup HitAreas using BeepControl's built-in system
        /// This replaces manual mouse click handling
        /// </summary>
        private void SetupHitAreas()
        {
            // Clear existing hit areas using BeepControl method
            ClearHitList();

            if (!_showActionButtons) return;

            // Add HitArea for each visible action button using BeepControl AddHitArea
            foreach (var button in _actionButtons)
            {
                if (button.Visible)
                {
                    // Use BeepControl's AddHitArea method with button component and action
                    AddHitArea(
                        name: $"ActionButton_{button.Name}",
                        component: button,
                        hitAction: () => OnActionButtonClicked(button.Name)
                    );
                }
            }
        }

        /// <summary>
        /// Handle HitDetected events from BeepControl
        /// This automatically handles all mouse interactions
        /// </summary>
        private void OnHitDetected(object sender, ControlHitTestArgs e)
        {
            // BeepControl automatically handles the hit detection and calls the hitAction
            // No additional code needed here - the action is already executed via the hitAction delegate
            
            // Optional: Add logging or additional processing
            if (e.HitTest?.Name?.StartsWith("ActionButton_") == true)
            {
                string buttonName = e.HitTest.Name.Replace("ActionButton_", "");
                // Could add logging, analytics, or other cross-cutting concerns here
            }
        }

        #endregion

        #region Event Handling - Simplified with HitArea

        /// <summary>
        /// Handle action button clicks - called via HitArea actions
        /// No more manual coordinate calculations needed!
        /// </summary>
        private void OnActionButtonClicked(string actionName)
        {
            ActionButtonClicked?.Invoke(this, actionName);
        }

        // NOTE: Manual mouse event handling is no longer needed
        // BeepControl's HitArea system handles all mouse interactions automatically
        // The old OnMouseClick override has been removed

        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            // Draw separator line at bottom
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.DrawLine(pen, 0, Height - 1, Width, Height - 1);
            }
        }
        #endregion

        #region Public Methods
        public void SetParentGrid(BeepGrid parentGrid)
        {
            _parentGrid = parentGrid;
        }

        public void UpdateButtonVisibility(string buttonName, bool visible)
        {
            var button = _actionButtons.FirstOrDefault(b => b.Name == buttonName);
            if (button != null)
            {
                button.Visible = visible;
                ArrangeComponents();
                SetupHitAreas(); // Re-setup hit areas when individual button visibility changes
            }
        }

        public void SetButtonEnabled(string buttonName, bool enabled)
        {
            var button = _actionButtons.FirstOrDefault(b => b.Name == buttonName);
            if (button != null)
            {
                button.Enabled = enabled;
            }
        }
        #endregion

        #region Theme Support
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            _titleLabel?.ApplyTheme(_currentTheme);
            foreach (var button in _actionButtons)
            {
                button.ApplyTheme(_currentTheme);
            }
            
            // Re-setup hit areas to ensure theme changes are reflected
            SetupHitAreas();
            
            Invalidate();
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unsubscribe from BeepControl events
                HitDetected -= OnHitDetected;
                
                _titleLabel?.Dispose();
                foreach (var button in _actionButtons)
                {
                    button?.Dispose();
                }
                _actionButtons?.Clear();
                
                // BeepControl automatically cleans up HitAreas in its Dispose
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
