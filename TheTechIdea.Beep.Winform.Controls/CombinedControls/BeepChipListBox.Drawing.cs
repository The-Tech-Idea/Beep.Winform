using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.CombinedControls
{
    /// <summary>
    /// Custom drawing logic for BeepChipListBox
    /// </summary>
    public partial class BeepChipListBox
    {
        #region Drawing

        protected override void DrawContent(Graphics g)
        {
            // The content is handled by child controls (BeepListBox, BeepMultiChipGroup, BeepTextBox)
            // This method can be used to draw additional decorations if needed

            // Draw subtle shadow/glow effects around sections if desired
            if (_currentTheme != null && ShowShadow)
            {
                DrawSectionEffects(g);
            }
        }

        private void DrawSectionEffects(Graphics g)
        {
            // Optional: Draw subtle inner shadows or highlights between sections
            // This creates a more polished look

            if (!_showDivider || _dividerPanel == null || !_dividerPanel.Visible) return;

            var dividerRect = _dividerPanel.Bounds;

            // Draw a subtle gradient shadow above and below the divider
            using (var shadowBrush = new LinearGradientBrush(
                new Rectangle(dividerRect.X, dividerRect.Y - 4, dividerRect.Width, 4),
                Color.FromArgb(20, 0, 0, 0),
                Color.Transparent,
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(shadowBrush, dividerRect.X, dividerRect.Y - 4, dividerRect.Width, 4);
            }

            using (var shadowBrush = new LinearGradientBrush(
                new Rectangle(dividerRect.X, dividerRect.Bottom, dividerRect.Width, 4),
                Color.Transparent,
                Color.FromArgb(20, 0, 0, 0),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(shadowBrush, dividerRect.X, dividerRect.Bottom, dividerRect.Width, 4);
            }
        }

        #endregion

        #region Paint Override

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Additional custom painting can be done here
            // The base class handles the background and border
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            // Ensure child panels have transparent backgrounds that blend well
            if (_currentTheme != null)
            {
                // The panels use the theme's background color
                var bgColor = _currentTheme.PanelBackColor;
                
                if (_searchPanel != null)
                {
                    _searchPanel.BackColor = Color.Transparent;
                }
                if (_chipPanel != null)
                {
                    _chipPanel.BackColor = Color.Transparent;
                }
                if (_listPanel != null)
                {
                    _listPanel.BackColor = Color.Transparent;
                }
            }
        }

        #endregion

        #region Visual State

        /// <summary>
        /// Updates the visual state based on focus and hover
        /// </summary>
        private void UpdateVisualState()
        {
            // Update border color based on focus
            if (ContainsFocus)
            {
                if (_currentTheme != null)
                {
                    BorderColor = _currentTheme.PrimaryColor;
                }
            }
            else
            {
                if (_currentTheme != null)
                {
                    BorderColor = _currentTheme.BorderColor;
                }
            }

            Invalidate();
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            UpdateVisualState();
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            UpdateVisualState();
        }

        #endregion

        #region Animation Support

        // Future: Add animation support for smooth transitions
        // - Expand/collapse animations for sections
        // - Fade animations for search results
        // - Selection highlight animations

        #endregion
    }
}

