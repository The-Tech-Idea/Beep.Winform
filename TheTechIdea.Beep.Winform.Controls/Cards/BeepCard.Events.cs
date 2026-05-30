using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepCard
    {
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _interactionManager?.NotifyMouseEnter();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _interactionManager?.NotifyMouseLeave();
            if (_hoveredArea != null)
            {
                _hoveredArea = null;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _interactionManager?.NotifyMouseDown(e.Button, e.Location);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _interactionManager?.NotifyMouseUp(e.Button, e.Location);
        }

        // Override mouse move to track hover state
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _interactionManager?.NotifyMouseMove(e.Location);

            string newHoveredArea = null;
            Cursor desiredCursor = Cursors.Default;

            if (_showSelectionCheckbox && !_selectionRect.IsEmpty && _selectionRect.Contains(e.Location))
            {
                newHoveredArea = "SelectionCheckbox";
                desiredCursor = Cursors.Hand;
            }
            else if (!string.IsNullOrWhiteSpace(_contextMenuIcon) && !_contextMenuRect.IsEmpty && _contextMenuRect.Contains(e.Location))
            {
                newHoveredArea = "ContextMenu";
                desiredCursor = Cursors.Hand;
            }
            else if (_isCollapsible && !_collapseRect.IsEmpty && _collapseRect.Contains(e.Location))
            {
                newHoveredArea = "CollapseChevron";
                desiredCursor = Cursors.Hand;
            }
            else if (_layoutContext != null)
            {
                // Check which area is hovered
                if (_layoutContext.ButtonRect.Contains(e.Location))
                {
                    newHoveredArea = "Button";
                    desiredCursor = Cursors.Hand;
                }
                else if (_layoutContext.SecondaryButtonRect.Contains(e.Location))
                {
                    newHoveredArea = "SecondaryButton";
                    desiredCursor = Cursors.Hand;
                }
                else if (_layoutContext.ImageRect.Contains(e.Location))
                {
                    newHoveredArea = "Image";
                    desiredCursor = Cursors.Hand;
                }
                else if (_layoutContext.HeaderRect.Contains(e.Location))
                {
                    newHoveredArea = "Header";
                    desiredCursor = Cursors.Hand;
                }
                else if (_layoutContext.ParagraphRect.Contains(e.Location))
                {
                    newHoveredArea = "Paragraph";
                }
            }

            Cursor = desiredCursor;

            // Trigger repaint if hover state changed
            if (newHoveredArea != _hoveredArea)
            {
                _hoveredArea = newHoveredArea;
                Invalidate();
            }
        }

        /// <summary>
        /// Handles dialog keys (Enter, Space, Tab) for better dialog integration
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (!Enabled) return base.ProcessDialogKey(keyData);

            switch (keyData)
            {
                case Keys.Enter:
                case Keys.Space:
                    if (Focused || TabStop)
                    {
                        // Trigger primary button click if available
                        if (_layoutContext.ShowButton && !_layoutContext.ButtonRect.IsEmpty)
                        {
                            ButtonClicked?.Invoke(this, new BeepEventDataArgs("ButtonClicked", this));
                            return true;
                        }
                    }
                    break;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Redraws when focus is gained to show focus indicator
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _interactionManager?.NotifyFocusChanged(true);
            Invalidate(); // Redraw to show focus indicator
        }

        /// <summary>
        /// Redraws when focus is lost to remove focus indicator
        /// </summary>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _interactionManager?.NotifyFocusChanged(false);
            Invalidate(); // Redraw to remove focus indicator
        }

        /// <summary>
        /// Invalidates layout cache when control is resized
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // CRITICAL: Check timer is not null (design-time safety)
            if (_expandCollapseTimer != null && _isExpanded && !_expandCollapseTimer.Enabled)
            {
                _rememberedExpandedHeight = Height;
            }
            // Force full layout recalculation and repaint on resize
            InvalidateLayoutCache();
            Invalidate();
        }

        private void ToggleExpandedState()
        {
            IsExpanded = !IsExpanded;
        }

        private int GetCollapsedHeight()
        {
            int minCollapsed = Scale(72) + Math.Max(0, Scale(_accentBarHeight));
            if (_layoutContext != null && !_layoutContext.HeaderRect.IsEmpty)
            {
                int headerBased = (_layoutContext.HeaderRect.Bottom - DrawingRect.Top) + Scale(12);
                return Math.Max(minCollapsed, headerBased);
            }

            return minCollapsed;
        }

        private void StartExpandCollapseAnimation(bool expanding)
        {
            // CRITICAL: Check timer exists (design-time safety)
            if (_expandCollapseTimer == null) return;
            
            _expandAnimationStart = DateTime.UtcNow;
            _expandAnimationStartHeight = Height;
            _expandAnimationTargetHeight = expanding
                ? Math.Max(GetCollapsedHeight(), _rememberedExpandedHeight)
                : GetCollapsedHeight();

            if (expanding && _rememberedExpandedHeight < _expandAnimationStartHeight)
            {
                _rememberedExpandedHeight = _expandAnimationStartHeight;
                _expandAnimationTargetHeight = _rememberedExpandedHeight;
            }

            if (!_expandCollapseTimer.Enabled)
            {
                _expandCollapseTimer.Start();
            }
        }

        private void ExpandCollapseTimer_Tick(object sender, EventArgs e)
        {
            const double durationMs = 180d;
            double elapsed = (DateTime.UtcNow - _expandAnimationStart).TotalMilliseconds;
            double t = Math.Max(0d, Math.Min(1d, elapsed / durationMs));
            double eased = 1d - Math.Pow(1d - t, 3d);

            int newHeight = (int)Math.Round(_expandAnimationStartHeight + ((_expandAnimationTargetHeight - _expandAnimationStartHeight) * eased));
            if (newHeight != Height)
            {
                Height = newHeight;
            }

            InvalidateLayoutCache();
            Invalidate();

            if (t >= 1d)
            {
                _expandCollapseTimer.Stop();
                Height = _expandAnimationTargetHeight;
                InvalidateLayoutCache();
                Invalidate();
            }
        }

        private void LoadingTimer_Tick(object sender, EventArgs e)
        {
            _loadingShimmerPhase += 0.03f;
            if (_loadingShimmerPhase > 1f)
            {
                _loadingShimmerPhase -= 1f;
            }

            Invalidate();
        }

    }
}
