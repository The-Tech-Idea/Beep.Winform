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

            // Select the painter for this tooltip's layout variant.
            //
            // This used to hard-code BeepStyledToolTipPainter, so ToolTipPainterFactory had zero
            // call sites and PreviewToolTipPainter, TourToolTipPainter and GlassToolTipPainter —
            // roughly 700 lines between them — were never instantiated. Every variant rendered
            // through the default painter, which is why Preview, Tour and Glass all looked the
            // same as each other in a side-by-side render.
            //
            // An explicitly assigned Painter still wins, so a host can override the factory.
            // Fully qualified: the inherited `Painters` member (List<IFormPainter> on the form base)
            // shadows the ToolTips.Painters namespace here.
            _painter ??= TheTechIdea.Beep.Winform.Controls.ToolTips.Painters.ToolTipPainterFactory.GetPainter(config);

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

            // Position against the anchor RECTANGLE, not a point. A 1x1 anchor made every
            // *Start / *End alignment collapse onto the centred placement, and — because the
            // point is the control's centre — put the tooltip on top of the control it describes.
            var anchorRect = _config.AnchorRect.IsEmpty
                ? new Rectangle(position, new Size(1, 1))
                : _config.AnchorRect;

            var (placement, finalPosition, arrowOffset) = ToolTipPositioningHelpers.Resolve(
                anchorRect,
                Size,
                _config.Placement,
                TotalOffset,
                _config.ViewportPadding);

            // Clamp to the space that actually exists on the side we settled on, then re-resolve
            // if that changed our size — the placement was computed for the old height.
            var clamped = ClampToAvailableSpace(anchorRect, placement);
            if (clamped != Size)
            {
                Size = clamped;
                (placement, finalPosition, arrowOffset) = ToolTipPositioningHelpers.Resolve(
                    anchorRect, Size, _config.Placement, TotalOffset, _config.ViewportPadding);
            }

            _actualPlacement = placement;
            _config.ArrowOffset = arrowOffset;
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
        /// Re-measures the tooltip for content that arrived after it was shown, then repositions.
        /// <para>
        /// Async content changes the tooltip's size, so simply repainting would leave a skeleton-
        /// sized window around a full-size image, and a placement that was resolved for the old
        /// size. Both must be redone — which is why this goes through the same
        /// <see cref="ToolTipPositioningHelpers.Resolve"/> path as the initial show.
        /// </para>
        /// </summary>
        public void RefreshContentSize()
        {
            if (_config == null || IsDisposed || _painter == null) return;

            using (var g = CreateGraphics())
            {
                var contentSize = _painter.CalculateSize(g, _config);
                var minWidth = ToolTipStyleHelpers.GetRecommendedMinWidth(_config.Style);
                var maxWidth = _config.MaxSize?.Width > 0
                    ? _config.MaxSize.Value.Width
                    : ToolTipStyleHelpers.GetRecommendedMaxWidth(_config.Style);

                var screenBounds = ToolTipPositioningHelpers.GetScreenBounds(_config.Position);
                var responsiveSize = ToolTipPositioningHelpers.CalculateResponsiveSize(
                    contentSize, new Size(maxWidth, 0), new Size(minWidth, 40), screenBounds);

                if (responsiveSize != Size) Size = responsiveSize;
            }

            UpdatePosition(_config.AnchorRect.IsEmpty
                ? new Rectangle(_config.Position, new Size(1, 1))
                : _config.AnchorRect);

            Invalidate();
        }

        /// <summary>
        /// Reposition against a point (follow-cursor scenarios).
        /// </summary>
        public void UpdatePosition(Point newPosition)
            => UpdatePosition(new Rectangle(newPosition, new Size(1, 1)));

        /// <summary>
        /// Reposition against an anchor rectangle. Used by follow-cursor and by auto-update when
        /// the anchor moves.
        /// <para>
        /// Goes through the same <see cref="ToolTipPositioningHelpers.Resolve"/> call as the initial
        /// show. It previously used a second implementation
        /// (<c>AdjustPositionForPlacement</c> + <c>ConstrainToScreen</c>) whose offset maths
        /// differed from the one that chose the placement, so a repositioned tooltip could land
        /// somewhere the placement engine had never validated.
        /// </para>
        /// </summary>
        public void UpdatePosition(Rectangle anchorRect)
        {
            if (_config == null) return;

            var (placement, position, arrowOffset) = ToolTipPositioningHelpers.Resolve(
                anchorRect, Size, _config.Placement, TotalOffset, _config.ViewportPadding);

            _actualPlacement = placement;
            _config.ArrowOffset = arrowOffset;
            Location = position;
            Invalidate();
        }

        /// <summary>Raised when the user toggles the pin, so the manager can change its hide rules.</summary>
        internal event EventHandler PinnedChanged;

        /// <summary>
        /// Hit-tests the pin and close buttons using the same rectangles the painter drew them
        /// with, and acts on a hit.
        /// </summary>
        private bool HandleHeaderButtonClick(Point location)
        {
            var content = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            content.Inflate(-ContentInset, -ContentInset);

            if (_config.Pinnable)
            {
                var pin = TheTechIdea.Beep.Winform.Controls.ToolTips.Painters.ToolTipHeaderButtons.PinRect(content, _config);
                if (!pin.IsEmpty && pin.Contains(location))
                {
                    _config.IsPinned = !_config.IsPinned;
                    Invalidate();
                    PinnedChanged?.Invoke(this, EventArgs.Empty);
                    return true;
                }
            }

            if (_config.Closable)
            {
                var close = TheTechIdea.Beep.Winform.Controls.ToolTips.Painters.ToolTipHeaderButtons.CloseRect(content, _config);
                if (!close.IsEmpty && close.Contains(location))
                {
                    _config.IsPinned = false;
                    _config.OnClose?.Invoke(_config.Key);
                    Hide();
                    return true;
                }
            }

            return false;
        }

        /// <summary>Padding between the window edge and the content the painter lays out in.</summary>
        private const int ContentInset = 8;

        /// <summary>
        /// Clamps the tooltip to the space that exists on <paramref name="placement"/>'s side of
        /// the anchor.
        /// <para>
        /// Sizing previously clamped only against 80% of the whole screen, so a tooltip placed
        /// above an anchor near the top of the display could be sized far taller than the gap it
        /// had to live in. Content that exceeds the clamp is not silently lost: it is ellipsised by
        /// the painters today, and [plans/08] tracks making the body scrollable instead.
        /// </para>
        /// </summary>
        private Size ClampToAvailableSpace(Rectangle anchorRect, ToolTipPlacement placement)
        {
            var available = ToolTipPositioningHelpers.AvailableSpaceFor(
                anchorRect, placement, TotalOffset, _config.ViewportPadding);

            // A readable maximum, not a fraction of the monitor. Material caps around 320 and
            // GitHub's hover cards around 360; 80% of a 4K display is not a tooltip.
            int maxWidth = _config.MaxSize?.Width > 0
                ? _config.MaxSize.Value.Width
                : DefaultReadableMaxWidth;

            int width = Math.Min(Size.Width, Math.Min(maxWidth, Math.Max(1, available.Width)));
            int height = Size.Height;

            // Only shrink to the available height when there is a usable amount of room; if the
            // side is very tight the flip/shift stage has already done what it can, and forcing a
            // tiny height here would produce an unreadable sliver.
            if (available.Height >= MinUsableHeight && height > available.Height)
                height = available.Height;

            return new Size(Math.Max(1, width), Math.Max(1, height));
        }

        /// <summary>Readable default maximum width, DPI-scaled.</summary>
        private int DefaultReadableMaxWidth => (int)(360 * (DeviceDpi / 96f));

        /// <summary>Below this, clamping to the available height would produce an unreadable sliver.</summary>
        private int MinUsableHeight => (int)(64 * (DeviceDpi / 96f));

        /// <summary>
        /// The full gap between anchor and tooltip: the configured offset plus the arrow, since the
        /// arrow occupies that space. Placement is validated and applied with the same value.
        /// </summary>
        private int TotalOffset
        {
            get
            {
                int offset = _config.Offset > 0
                    ? _config.Offset
                    : ToolTipStyleHelpers.GetRecommendedOffset(_config.Style);
                int arrow = _config.ShowArrow && _config.ArrowStyle != ToolTipArrowStyle.Hidden
                    ? (_config.ArrowSize > 0 ? _config.ArrowSize : ToolTipStyleHelpers.GetRecommendedArrowSize(_config.Style))
                    : 0;
                return offset + arrow;
            }
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

            // Header buttons first — they sit above any painter-specific content.
            if (HandleHeaderButtonClick(e.Location)) return;

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
