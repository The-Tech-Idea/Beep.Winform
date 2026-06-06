using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers;
using TheTechIdea.Beep.Winform.Controls.ToolTips.Painters;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    public partial class CustomToolTip
    {
        #region Public Methods - Lifecycle

        /// <summary>
        /// Apply tooltip configuration and prepare for display
        /// Enhanced with ToolTipStyleHelpers and ToolTipLayoutHelpers
        /// </summary>
        public void ApplyConfig(ToolTipConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            // Initialize painter if not already set
            _painter ??= new Painters.BeepStyledToolTipPainter();

            // Apply theme - use _currentTheme if set (from ApplyTheme()), otherwise use BeepThemesManager
            if (_currentTheme != null)
            {
                _theme = _currentTheme;
            }
            else if (_config.UseBeepThemeColors && BeepThemesManager.CurrentTheme != null)
            {
                _theme = BeepThemesManager.CurrentTheme;
            }

            // Apply theme colors to config
            if (_config.UseBeepThemeColors && _theme != null)
            {
                ToolTipThemeHelpers.ApplyThemeColors(_config, _theme, true);
            }

            // Apply accessibility enhancements (high contrast, contrast ratios)
            ApplyAccessibilityEnhancements();

            // Update accessibility properties with tooltip content
            UpdateAccessibilityProperties();

            // Calculate tooltip size with responsive sizing using helpers
            using (var g = CreateGraphics())
            {
                var contentSize = _painter.CalculateSize(g, _config);
                
                // Use ToolTipStyleHelpers for recommended sizes
                var minWidth = ToolTipStyleHelpers.GetRecommendedMinWidth(_config.Style);
                var maxWidth = _config.MaxSize?.Width > 0 
                    ? _config.MaxSize.Value.Width 
                    : ToolTipStyleHelpers.GetRecommendedMaxWidth(_config.Style);
                
                // Apply responsive sizing based on screen size
                var screenBounds = ToolTipPositioningHelpers.GetScreenBounds(_config.Position);
                var minSize = new Size(minWidth, 40); // Minimum readable size
                var maxSize = new Size(maxWidth, 0); // 0 means use default
                
                var responsiveSize = ToolTipPositioningHelpers.CalculateResponsiveSize(
                    contentSize, maxSize, minSize, screenBounds);
                
                Size = responsiveSize;
            }

            // Apply custom colors if specified
            if (_config.BackColor.HasValue)
            {
                BackColor = _config.BackColor.Value;
            }

            if (_config.ForeColor.HasValue)
            {
                ForeColor = _config.ForeColor.Value;
            }

            Invalidate();
        }

        /// <summary>
        /// Show tooltip at specified position with animation
        /// </summary>
        public async Task ShowAsync(Point position, CancellationToken cancellationToken = default)
        {
            if (_config == null)
            {
                throw new InvalidOperationException("Must call ApplyConfig before showing tooltip");
            }

            // Use smart positioning with collision detection
            var targetRect = new Rectangle(position, new Size(1, 1));
            var (placement, finalPosition) = ToolTipPositioningHelpers.FindBestPlacement(
                targetRect, Size, _config.Placement, _config.Offset);
            _actualPlacement = placement;

            Location = finalPosition;

            // Show and animate (respect reduced motion preference)
            Show();

            // Check if animations should be disabled for accessibility
            var shouldAnimate = _config.Animation != ToolTipAnimation.None && 
                               !ToolTipAccessibilityHelpers.ShouldDisableAnimations(_config.Animation);

            if (shouldAnimate)
            {
                await AnimateInAsync();
            }
            else
            {
                Opacity = 1.0;
            }
        }

        /// <summary>
        /// Hide tooltip with animation
        /// </summary>
        public async Task HideAsync()
        {
            // Check if animations should be disabled for accessibility
            var shouldAnimate = _config?.Animation != ToolTipAnimation.None && 
                               !ToolTipAccessibilityHelpers.ShouldDisableAnimations(_config.Animation);

            if (shouldAnimate)
            {
                await AnimateOutAsync();
            }
            else
            {
                Opacity = 0;
            }

            Hide();
        }

        /// <summary>
        /// Update tooltip position (for follow-cursor scenarios)
        /// </summary>
        public void UpdatePosition(Point newPosition)
        {
            var adjustedPosition = AdjustPositionForPlacement(newPosition, _actualPlacement);
            var constrainedPosition = ConstrainToScreen(adjustedPosition);
            Location = constrainedPosition;
        }

        #endregion

        #region Accessibility Methods

        /// <summary>
        /// Set accessibility properties for screen readers
        /// </summary>
        private void SetAccessibilityProperties()
        {
            // Set accessible role
            AccessibleRole = AccessibleRole.ToolTip;
            AccessibleName = "Tooltip";
            AccessibleDescription = "Additional information tooltip";
        }

        /// <summary>
        /// Update accessibility properties with tooltip content
        /// </summary>
        private void UpdateAccessibilityProperties()
        {
            if (_config == null) return;

            // Build accessible description from tooltip content
            var description = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(_config.Title))
            {
                description.Append(_config.Title);
                if (!string.IsNullOrEmpty(_config.Text))
                {
                    description.Append(". ");
                }
            }

            if (!string.IsNullOrEmpty(_config.Text))
            {
                description.Append(_config.Text);
            }

            if (description.Length > 0)
            {
                AccessibleDescription = description.ToString();
                AccessibleName = !string.IsNullOrEmpty(_config.Title) 
                    ? _config.Title 
                    : "Tooltip";
            }

            // Set tooltip type for screen readers
            var typeDescription = _config.Type switch
            {
                ToolTipType.Success => "Success message",
                ToolTipType.Warning => "Warning message",
                ToolTipType.Error => "Error message",
                ToolTipType.Info => "Information",
                ToolTipType.Help => "Help information",
                _ => "Tooltip"
            };

            if (!string.IsNullOrEmpty(AccessibleDescription))
            {
                AccessibleDescription = $"{typeDescription}: {AccessibleDescription}";
            }
            else
            {
                AccessibleDescription = typeDescription;
            }
        }

        /// <summary>
        /// Apply accessibility enhancements (high contrast, contrast ratios, etc.)
        /// </summary>
        private void ApplyAccessibilityEnhancements()
        {
            if (_config == null) return;

            // Check high contrast mode
            if (ToolTipAccessibilityHelpers.IsHighContrastMode())
            {
                var (backColor, foreColor, borderColor) = ToolTipAccessibilityHelpers.GetHighContrastColors();
                
                // Override colors with high contrast system colors
                if (!_config.BackColor.HasValue)
                    _config.BackColor = backColor;
                if (!_config.ForeColor.HasValue)
                    _config.ForeColor = foreColor;
                if (!_config.BorderColor.HasValue)
                    _config.BorderColor = borderColor;
            }
            else
            {
                // Ensure contrast ratios meet WCAG AA standards
                var backColor = _config.BackColor ?? BackColor;
                var foreColor = _config.ForeColor ?? ForeColor;
                var borderColor = _config.BorderColor ?? Color.Gray;

                var accessibleColors = ToolTipAccessibilityHelpers.GetAccessibleColors(
                    backColor, foreColor, borderColor);

                if (!_config.BackColor.HasValue)
                    _config.BackColor = accessibleColors.backColor;
                if (!_config.ForeColor.HasValue)
                    _config.ForeColor = accessibleColors.foreColor;
                if (!_config.BorderColor.HasValue)
                    _config.BorderColor = accessibleColors.borderColor;
            }
        }

        #endregion

        #region Mouse handling (C7)

        // C7: Detect clicks on the tour tooltip's "Skip" / "Next →" / "Done"
        // visual button areas. The tour painter renders these as text, not
        // real Button controls (so they pick up the tooltip's anti-aliased
        // paint). We hit-test the same rectangles the painter used and
        // invoke the callbacks the tour wired in ToolTipConfig.
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (_config == null) return;
            if (!(_painter is TourToolTipPainter)) return;
            if (_config.LayoutVariant != ToolTipLayoutVariant.Tour) return;

            // Layout constants from TourToolTipPainter — kept in sync
            // (the painter is the source of truth; if those constants
            // change, update these too).
            const int PaddingH   = 14;
            const int PaddingV   = 12;
            const int BtnH       = 26;

            int btnY = ClientSize.Height - BtnH - PaddingV;
            int margin = PaddingH;

            // "Skip" hit area — left side, full button height
            if (_config.ShowNavigationButtons)
            {
                var skipRect = new Rectangle(margin - 4, btnY, 60, BtnH);
                if (skipRect.Contains(e.Location))
                {
                    _config.OnSecondaryClick?.Invoke();
                    return;
                }
            }

            // "Next →" / "Done" hit area — right side, sized to text
            if (_config.ShowNavigationButtons && _config.CurrentStep <= _config.TotalSteps)
            {
                bool isLast   = _config.CurrentStep == _config.TotalSteps;
                string txt    = isLast ? "Done" : "Next →";
                int approxW   = (int)(txt.Length * 7.0) + 16;   // matches the painter's rough sizing
                var btnRect   = new Rectangle(ClientSize.Width - margin - approxW, btnY, approxW, BtnH);
                if (btnRect.Contains(e.Location))
                {
                    _config.OnPrimaryClick?.Invoke();
                    return;
                }
            }
        }

        #endregion
    }
}
